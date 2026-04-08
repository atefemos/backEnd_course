using System;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;

public class Person
{
    public string UserName { get; set; }
    public int UserAge { get; set; }
}

// Binary Serialization
class Program
{
    static void Main()
    {
        Person SamplePerson = new Person { UserName = "John Doe", UserAge = 30 };
        // Binary Serialization
        using (FileStream fs = new FileStream("person.dat", FileMode.Create))
        {
            BinaryWriter writer = new BinaryWriter(fs);
            writer.Write(SamplePerson.UserName);
            writer.Write(SamplePerson.UserAge);
        }
        Console.WriteLine("Binary serialization complete.");
    }
}

// XML Serialization
class Program
{
    static void Main()
    {
        Person SamplePerson = new Person { UserName = "Alice", UserAge = 30 };
        // XML Serialization
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Person));
        using (StreamWriter writer = new StreamWriter("person.xml"))
        {
            xmlSerializer.Serialize(writer, SamplePerson);
        }
        Console.WriteLine("XML serialization complete.");
    }
}

// JSON Serialization
class Program
{
    static void Main()
    {
        Person samplePerson = new Person { UserName = "Alice", UserAge = 30 };
        string jsonString = JsonSerializer.Serialize(samplePerson);

        File.WriteAllText("person.json", jsonString);

        Console.WriteLine("JSON serialization complete.");
    }
}