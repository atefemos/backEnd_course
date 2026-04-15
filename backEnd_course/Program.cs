using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using CustomNamespace; //replace with your namespace

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.All;
    logging.RequestBodyLogLimit = 4096; // Limit request body logging to 4KB
    logging.ResponseBodyLogLimit = 4096; // Limit response body logging to 4KB
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddSingleton<IMyService, MyService>();  // Singleton
// builder.Services.AddScoped<IMyService, MyService>();    // Scoped
builder.Services.AddTransient<IMyService, MyService>();   // Transient

// Register controller for endpoint mapping
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen();

// Configure to listen on HTTP only for simplicity
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5294); // HTTP only
});

var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpLogging();

// Custom middleware for logging request path and response status
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    await next.Invoke();
    Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
});

// Custom middleware for tracking request duration
app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    Console.WriteLine($"Start Time: {startTime}");
    await next.Invoke();
    var duration = DateTime.UtcNow - startTime;
    Console.WriteLine($"Response Time: {duration.TotalMilliseconds} ms");
});

// Custom middleware for logging service creation
app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    myService.LogCreation("First Middleware");
    await next();
});

app.Use(async (context, next) =>
{
    var myService = context.RequestServices.GetRequiredService<IMyService>();
    myService.LogCreation("Second Middleware");
    await next();
});

// Middleware to log security events if response status indicates an issue
app.Use(async (context, next) =>
{
    await next(); // Run the next middleware first

    if (context.Response.StatusCode >= 400)
    {
        Console.WriteLine($"Security Event: {context.Request.Path} - Status Code: {context.Response.StatusCode}");
    }
});

// Simulated HTTPS Enforcement Middleware
app.Use(async (context, next) =>
{
    // Check for a query parameter to simulate HTTPS enforcement (e.g., "?secure=true")
    if (context.Request.Query["secure"] != "true")
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Simulated HTTPS Required");
        return;
    }

    await next();
});

// Middleware for input validation
app.Use(async (context, next) =>
{
    var input = context.Request.Query["input"].ToString();
    if (!IsValidInput(input))
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Invalid Input");
        }
        return;
    }

    await next();
});

// Middleware for short-circuiting unauthorized access
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/unauthorized")
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized Access");
        }
        return; // Exit middleware pipeline early if unauthorized
    }
    await next();
});

// Middleware for simulated authentication and secure cookies
app.Use(async (context, next) =>
{
    // Simulate authentication with a query parameter (e.g., "?authenticated=true")
    var isAuthenticated = context.Request.Query["authenticated"] == "true";
    if (!isAuthenticated)
    {
        if (!context.Response.HasStarted)
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Access Denied");
        }
        return;
    }

    context.Response.Cookies.Append("SecureCookie", "SecureData", new CookieOptions
    {
        HttpOnly = true,
        Secure = true
    });

    await next();
});

// Middleware for asynchronous processing
app.Use(async (context, next) =>
{
    await Task.Delay(100); // Simulate async operation
    if (!context.Response.HasStarted)
    {
        await context.Response.WriteAsync("Processed Asynchronously\n");
    }
    await next();
});

// Helper method for input validation
static bool IsValidInput(string input)
{
    // Checks for any unsafe characters or patterns, including "<script>"
    return string.IsNullOrEmpty(input) || (input.All(char.IsLetterOrDigit) && !input.Contains("<script>"));
}

// Configure the HTTP request pipeline for development environment
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// app.MapControllers();

app.MapGet("/", (IMyService myService) =>
{
    myService.LogCreation("Root");
    return Results.Ok("Check the console for service creation logs.");
}).ExcludeFromDescription();

// Final Response Middleware
app.Run(async (context) =>
{
    if (!context.Response.HasStarted)
    {
        await context.Response.WriteAsync("Final Response from Application\n");
    }
});

app.Run();


// *************
// Integrating Swagger and Generating API Clients

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
        app.MapControllers();

        //app.Run();
        Task.Run(() => app.RunAsync());
        //await Task.Delay(3000);
        //await new ClientGenerator().GenerateClient();

        var httpClient = new HttpClient();
        var client = new CustomApiClient("http://localhost:5000", httpClient);
        var user = await client.GetUserAsync(1);
        Console.WriteLine($"Fetched User: {user}");
    }
}
