[AttributeUsage(AttributeTargets.Class)]
public class ComponentAttribute : Attribute
{
}

[Component]
public class TestLoggerComponent
{
    public void Log(string message)
    {
        Console.WriteLine($"LOG: {message}");
    }
}

// Main program that uses the generated code
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Application starting...");
        
        var logger = new TestLoggerComponent();
        
        // Register components with the generated registry
        Generated.ComponentRegistry.Register("main-logger", logger);
        
        // Access components using the generated registry
        var myLogger = Generated.ComponentRegistry.GetComponentName("main-logger");
        myLogger.Log("Using the generated component registry!");
        
        Generated.ComponentRegistry.ListAllComponents();
        
        Console.WriteLine("Application completed.");
    }
}
