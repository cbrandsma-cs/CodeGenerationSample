// using System.Text;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.Text;
//
// namespace Sample.Generators
// {
//     [Generator]
//     public class FileGenerator22: IIncrementalGenerator
//     {
//         public void Initialize(IncrementalGeneratorInitializationContext context)
//         {
//             context.RegisterPostInitializationOutput(ctx =>
//             {
//                 var sourceText = $@"
// namespace SourceGeneratorInCSharp
// {{
//     public static class HelloWorld2
//     {{
//         public static void SayHello()
//         {{
//             Console.WriteLine(""Hello From Generator 123"");
//         }}
//     }}
// }}
// ";
//
//                 ctx.AddSource("ExampleGenerator.g", SourceText.From(sourceText, Encoding.UTF8));
//             });
//         }
//     }
// }