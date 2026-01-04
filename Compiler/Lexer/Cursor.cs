namespace Compiler.Lexer;

public class Cursor
{
    public int Line { get; private set; } = 0;
    public int Column { get; private set; } = 0;
    public int Position { get; private set; } = 0;
    
    public Cursor() { }

    public void MoveNext()
    {
        Column++;
        Position++;
    }

    public void MoveNewLine()
    {
        Line++;
        Column = 0;
        Position++;
    }
}