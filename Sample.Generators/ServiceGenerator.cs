using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sample.Generators
{
    [Generator]
    public class ServiceGenerator: IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
                "ServiceAttribute.g.cs", 
                SourceText.From(SourceGeneratorHelper.Attribute, Encoding.UTF8)));

            IncrementalValuesProvider<ClassDeclarationSyntax> enumDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                    transform: static (ctx, _) => GetTargetForGeneration(ctx));

            IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilationsAndEnums =
                context.CompilationProvider.Combine(enumDeclarations.Collect());

            context.RegisterSourceOutput(compilationsAndEnums,
                (spc, source) => Execute(source.Item1, source.Item2, spc));
        }

        public static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode)
        {
            // predicate, returns all Classes with [GenerateService] attribute

            return syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                   classDeclarationSyntax.AttributeLists.Count > 0 &&
                   classDeclarationSyntax.AttributeLists.ToList()
                       .Any(al => al.Attributes
                           .Any(a => a.Name.ToString() == "GenerateService"));
        }

        public static ClassDeclarationSyntax GetTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
            return classDeclarationSyntax;
        }

        public void Execute(Compilation compilation, 
            ImmutableArray<ClassDeclarationSyntax> classes, 
            SourceProductionContext context)
        {
            foreach (var classSyntax in classes)
            {
                // Converting the class to a semantic model to access much more meaningful data.
                var model = compilation.GetSemanticModel(classSyntax.SyntaxTree);

                // Parse to declared symbol, so you can access each part of code separately,
                // such as interfaces, methods, members, constructor parameters etc.
                var symbol = model.GetDeclaredSymbol(classSyntax);

                var className = symbol.Name;

                if (!className.Contains("Model"))
                {
                    var error = Diagnostic.Create(DiagnosticsDescriptors.ClassWithWrongNameMessage,
                        classSyntax.Identifier.GetLocation(),
                        className);

                    context.ReportDiagnostic(error);

                    return;
                }

                var classNamespace = symbol.ContainingNamespace?.ToDisplayString();

                var classAssembly = symbol.ContainingAssembly?.Name;

                // Get the template string
                var sourceCode = SourceGeneratorHelper.CreateService(classNamespace, classAssembly, className);

                context.AddSource(
                    $"{className}{"Service"}.g.cs",
                    SourceText.From(sourceCode, Encoding.UTF8)
                );
            }
        }

    }
}
