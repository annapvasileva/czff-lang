namespace Compiler.Lexer;

public class LexerException : Exception
{
    public int Line { get; }
    public int Column { get; }

    public LexerException(string message, int line, int column)
        : base($"{message} at line {line}, column {column}")
    {
        Line = line;
        Column = column;
    }
}