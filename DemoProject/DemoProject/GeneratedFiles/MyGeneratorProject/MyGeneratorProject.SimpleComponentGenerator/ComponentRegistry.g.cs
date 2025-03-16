using System;
using System.Collections.Generic;

namespace Generated
{
    // Auto-generated component registry
    public static class ComponentRegistry
    {
        private static Dictionary<string, object> _components = new Dictionary<string, object>();

        public static void Register(string name, object component)
        {
            _components[name] = component;
            Console.WriteLine($"Component registered: {name}");
        }

        public static T Get<T>(string name) where T : class
        {
            if (_components.TryGetValue(name, out var component))
            {
                return component as T;
            }
            return null;
        }

        // Generated getter for TestLoggerComponent
        public static TestLoggerComponent GetComponentName(string name)
        {
            return Get<TestLoggerComponent>(name);
        }

        public static void ListAllComponents()
        {
            Console.WriteLine("=== Registered Components ===");
            foreach (var pair in _components)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value.GetType().Name}");
            }
        }
    }
}
