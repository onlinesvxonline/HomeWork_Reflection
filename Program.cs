using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class CustomNameAttribute : Attribute
{
    public string Name { get; }

    public CustomNameAttribute(string name)
    {
        Name = name;
    }
}

public class CustomObject
{
    [CustomName("CustomFieldName")]
    public int I = 7;

    public string ObjectToString()
    {
        string result = "";

        foreach (var field in GetType().GetFields())
        {
            CustomNameAttribute attribute = (CustomNameAttribute)Attribute.GetCustomAttribute(field, typeof(CustomNameAttribute));
            string propertyName = attribute != null ? attribute.Name : field.Name;

            result += $"{propertyName}:{field.GetValue(this)} ";
        }

        return result.Trim();
    }

    public void StringToObject(string data)
    {
        string[] pairs = data.Split(' ');

        foreach (string pair in pairs)
        {
            string[] parts = pair.Split(':');
            string propertyName = parts[0];
            string propertyValue = parts[1];

            foreach (var field in GetType().GetFields())
            {
                CustomNameAttribute attribute = (CustomNameAttribute)Attribute.GetCustomAttribute(field, typeof(CustomNameAttribute));
                if (attribute != null && attribute.Name == propertyName)
                {
                    field.SetValue(this, Convert.ChangeType(propertyValue, field.FieldType));
                }
            }
        }
    }
}

class Program
{
    static void Main()
    {
        CustomObject obj = new CustomObject();
        string data = obj.ObjectToString();
        Console.WriteLine(data);

        CustomObject newObj = new CustomObject();
        newObj.StringToObject(data);

        // Проверка, что значения были скопированы корректно
        Console.WriteLine($"Проверка: {newObj.I}");
    }
}