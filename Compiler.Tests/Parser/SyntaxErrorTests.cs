using Compiler.Lexer;
using Compiler.Parser;

namespace Compiler.Tests.Parser;

public class SyntaxErrorTests
{
    [Theory]
    [ClassData(typeof(SyntaxErrorTestsData))]
    public void SyntaxErrorTest(string source, string expectedErrorMessage)
    {
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        
        ParserException exception = Assert.Throws<ParserException>(() => parser.Parse());
        Assert.Equal(expectedErrorMessage, exception.Message);
    }
}