namespace Compiler.SemanticAnalysis;

public class SemanticException : Exception
{
    public SemanticException(string message)
        : base(message) { }
}