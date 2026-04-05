using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddSingleton<IMyService, MyService>();  // Singleton
// builder.Services.AddScoped<IMyService, MyService>();    // Scoped
builder.Services.AddTransient<IMyService, MyService>();   // Transient

// Register controller for endpoint mapping
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
});

app.Run();
