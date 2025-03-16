using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace MyGeneratorProject
{
    [Generator]
    public class SimpleComponentGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Find classes with [Component] attribute
            IncrementalValuesProvider<ClassDeclarationSyntax> componentClasses = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsComponentClass(s),
                    transform: static (ctx, _) => GetComponentClass(ctx))
                .Where(static c => c != null);

            // Collect all component classes
            IncrementalValueProvider<ImmutableArray<ClassDeclarationSyntax>> collectedComponents = 
                componentClasses.Collect();

            // Generate code based on found components
            context.RegisterSourceOutput(collectedComponents, 
                (spc, components) => GenerateComponentCode(spc, components));
        }

        private static bool IsComponentClass(SyntaxNode node)
        {
            // Check if it's a class with [Component] attribute
            if (node is ClassDeclarationSyntax classDecl)
            {
                foreach (var attributeList in classDecl.AttributeLists)
                {
                    foreach (var attribute in attributeList.Attributes)
                    {
                        if (attribute.Name.ToString() == "Component")
                            return true;
                    }
                }
            }
            return false;
        }

        private static ClassDeclarationSyntax GetComponentClass(GeneratorSyntaxContext context)
        {
            return (ClassDeclarationSyntax)context.Node;
        }

        private void GenerateComponentCode(SourceProductionContext context, 
                                         ImmutableArray<ClassDeclarationSyntax> components)
        {
            if (components.IsDefaultOrEmpty)
                return;

            var sourceBuilder = new StringBuilder();
            
            sourceBuilder.AppendLine("using System;");
            sourceBuilder.AppendLine("using System.Collections.Generic;");
            sourceBuilder.AppendLine();
            sourceBuilder.AppendLine("namespace Generated");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("    // Auto-generated component registry");
            sourceBuilder.AppendLine("    public static class ComponentRegistry");
            sourceBuilder.AppendLine("    {");
            sourceBuilder.AppendLine("        private static Dictionary<string, object> _components = new Dictionary<string, object>();");
            sourceBuilder.AppendLine();
            
            // Registration method
            sourceBuilder.AppendLine("        public static void Register(string name, object component)");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine("            _components[name] = component;");
            sourceBuilder.AppendLine("            Console.WriteLine($\"Component registered: {name}\");");
            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine();
            
            // Generic getter
            sourceBuilder.AppendLine("        public static T Get<T>(string name) where T : class");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine("            if (_components.TryGetValue(name, out var component))");
            sourceBuilder.AppendLine("            {");
            sourceBuilder.AppendLine("                return component as T;");
            sourceBuilder.AppendLine("            }");
            sourceBuilder.AppendLine("            return null;");
            sourceBuilder.AppendLine("        }");
            sourceBuilder.AppendLine();
            
            // Generate typed getters for each component class
            foreach (var componentClass in components)
            {
                string className = componentClass.Identifier.Text;
                
                sourceBuilder.AppendLine($"        // Generated getter for {className}");
                sourceBuilder.AppendLine($"        public static {className} GetComponentName(string name)");
                sourceBuilder.AppendLine("        {");
                sourceBuilder.AppendLine($"            return Get<{className}>(name);");
                sourceBuilder.AppendLine("        }");
                sourceBuilder.AppendLine();
            }
            
            // List all components method
            sourceBuilder.AppendLine("        public static void ListAllComponents()");
            sourceBuilder.AppendLine("        {");
            sourceBuilder.AppendLine("            Console.WriteLine(\"=== Registered Components ===\");");
            sourceBuilder.AppendLine("            foreach (var pair in _components)");
            sourceBuilder.AppendLine("            {");
            sourceBuilder.AppendLine("                Console.WriteLine($\"{pair.Key}: {pair.Value.GetType().Name}\");");
            sourceBuilder.AppendLine("            }");
            sourceBuilder.AppendLine("        }");
            
            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");

            // Add the source
            context.AddSource("ComponentRegistry.g.cs", sourceBuilder.ToString());
        }
    }
}
