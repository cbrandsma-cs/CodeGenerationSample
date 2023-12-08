using Microsoft.CodeAnalysis;
using System;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Sample.Generators
{
    [Generator]
    public class FileGenerator2: IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext  context)
        {
            context.RegisterPostInitializationOutput(ctx =>
            {
                var sourceText = $@"
namespace SourceGeneratorInCSharp
{{
    public static class HelloWorld
    {{
        public static void SayHello()
        {{
            Console.WriteLine(""Hello From Generator"");
        }}
    }}
}}
";

                ctx.AddSource("ExampleGenerator.g", SourceText.From(sourceText, Encoding.UTF8));
            });
        }

    }
}
