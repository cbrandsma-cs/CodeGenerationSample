using Microsoft.CodeAnalysis;

namespace Sample.Generators;

public static class DiagnosticsDescriptors
{
    public static readonly DiagnosticDescriptor ClassWithWrongNameMessage
        = new("ERR001",                                        // id
            "Wrong name",                                      // title
            "The class '{0}' must be contains 'Model' prefix", // message
            "Generator",                                       // category
            DiagnosticSeverity.Error,
            true);

    public static DiagnosticDescriptor ClassMissingInterfaceMessage =
        new("ERR002",
            "Service class missing interface",
            "The class '{0}' must have an interface named 'I{0}'",
            "Generator",
            DiagnosticSeverity.Error,
            true);
}