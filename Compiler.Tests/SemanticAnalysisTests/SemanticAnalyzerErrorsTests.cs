using Compiler.Lexer;
using Compiler.Parser;
using Compiler.SemanticAnalysis;

namespace Compiler.Tests.SemanticAnalysisTests;

public class SemanticAnalyzerErrorsTests
{
    [Theory]
    [ClassData(typeof(SemanticAnalyzerErrorsData))]
    public void SyntaxErrorTest(string source, string expectedErrorMessage)
    {
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();
        var builder = new SymbolTableBuilder();
        ast.Root.Accept(builder);
        var semanticAnalyzer = new SemanticAnalyzer(builder.SymbolTable);
        
        SemanticException exception = Assert.Throws<SemanticException>(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Equal(expectedErrorMessage, exception.Message);
    }
}