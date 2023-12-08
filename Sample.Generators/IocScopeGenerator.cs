using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sample.Generators;

[Generator]
public class IocScopeGenerator: IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {

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
        // predicate, returns all Classes with [AddToScope] attribute

        return syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
               classDeclarationSyntax.AttributeLists.Count > 0 &&
               classDeclarationSyntax.AttributeLists.ToList()
                   .Any(al => al.Attributes
                       .Any(a => a.Name.ToString() == "AddToScope"));
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
        var scopes = new List<IocScope>();

        foreach (var classSyntax in classes)
        {
            // Converting the class to a semantic model to access much more meaningful data.
            var model = compilation.GetSemanticModel(classSyntax.SyntaxTree);

            // Parse to declared symbol, so you can access each part of code separately,
            // such as interfaces, methods, members, constructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax);

            var className = symbol.Name;
            var interfaceName = $"I{className}";
            
            var interfaces = ((ITypeSymbol)symbol).AllInterfaces;
            if (!interfaces.Any(x => x.Name == interfaceName))
            {
                var error = Diagnostic.Create(DiagnosticsDescriptors.ClassMissingInterfaceMessage,
                    classSyntax.Identifier.GetLocation(),
                    className);

                context.ReportDiagnostic(error);

                return;
            }

            var classNamespace = symbol.ContainingNamespace?.ToDisplayString();

            var classAssembly = symbol.ContainingAssembly?.Name;

            scopes.Add(new IocScope { 
                ClassName = className, 
                InterfaceName = $"I{className}",
                NamespaceName = classNamespace!
                });

        }

        var sourceCode = SourceGeneratorHelper.CreateIocScopes("WebApi", "WebApi", "WebApiScopes", scopes);
        context.AddSource("IoCServiceScope.g.cs", SourceText.From(sourceCode, Encoding.UTF8));
    }
}