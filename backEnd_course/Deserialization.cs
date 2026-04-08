using System;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;

public class Program
{
    public static void Main()
    // Serialize example for testing
    {
        var samplePerson = new SocketsHttpPlaintextStreamFilterContext {UserName = "Alice", UserAge = 30 };

        // Binary Serialization
        using (var fs = new FileStream("person.dat", FileMode.Open))
        using (var reader = new BinaryReader(fs))
        {
             deserializedPerson = new Person
            {
                UserName = reader.ReadString(),
                UserAge = reader.ReadInt32()
            };
        }

        Console.WriteLine($"Binary Deserialization - UserName: {deserializedPerson.UserName}, UserAge: {deserializedPerson.UserAge}");

    }
}

public class Program
{
    public static void Main()
    {
        var xmlData = "<Person><UserName>Bob</UserName><UserAge>30</UserAge></Person>";
        var serializer = new XmlSerializer(typeof(Person));

        using (var reader = new StringReader(xmlData))
        {
            var deserializedPerson = (Person)serializer.Deserialize(reader);
            Console.WriteLine($"XML Deserialization - UserName: {deserializedPerson.UserName}, UserAge: {deserializedPerson.UserAge}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        var jsonData = "{\"UserName\": \"Charlie\", \"UserAge\": 45}";
        var deserializedPerson = JsonSerializer.Deserialize<Person>(jsonData);
        
        Console.WriteLine($"JSON Deserialization - UserName: {deserializedPerson.UserName}, UserAge: {deserializedPerson.UserAge}");
    }
}

public class Program
{
    public static void Main()
    {
        try
        {
            var jsonData = "{\"UserName\": \"Dana\"}";
            var deserializedPerson = JsonSerializer.Deserialize<Person>(jsonData);

            if (string.IsNullOrEmpty(deserializedPerson.UserName))
                throw new Exception("UserName is required");

            Console.WriteLine("Data Integrity Verified");
            Console.WriteLine($"UserName: {deserializedPerson.UserName}, UserAge: {deserializedPerson.UserAge}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during deserialization: {ex.Message}");
        }
    }
}