namespace Compiler.Parser;

public class ParserException : Exception
{
    public int Line { get; }
    public int Column { get; }

    public ParserException(string message, int line, int column)
        : base($"{message} at line {line}, column {column}")
    {
        Line = line;
        Column = column;
    }
}