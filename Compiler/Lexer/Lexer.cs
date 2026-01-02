namespace Compiler.Lexer;

public class Lexer
{
    private readonly string _source;

    public Lexer(string source)
    {
        _source = source;
    }
    
    public IEnumerable<Token> GetTokens()
    {
        throw new NotImplementedException();
    }
}
