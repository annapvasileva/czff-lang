using Compiler.Lexer;
using Compiler.Parser;
using Compiler.SemanticAnalysis;

namespace Compiler.Tests.SemanticAnalysisTests;

public class SymbolTableBuilderErrorTests
{
    [Theory]
    [ClassData(typeof(SymbolTableBuilderErrorTestsData))]
    public void SyntaxErrorTest(string source, string expectedErrorMessage)
    {
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();
        var builder = new SymbolTableBuilder();
        
        SemanticException exception = Assert.Throws<SemanticException>(() => ast.Root.Accept(builder));
        Assert.Equal(expectedErrorMessage, exception.Message);
    }
}
