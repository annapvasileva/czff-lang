namespace Compiler.Lexer;

public class Lexer
{
    private readonly string _source;

    private Cursor cursor = new Cursor();
    private char _currentChar;

    public Lexer(string source)
    {
        _source = source;
    }

    public Token GetNextToken()
    {
        SetCurrentChar();
        while (CanSkip())
        {
            SkipWhiteSpaces();
            SkipComment();
        }

        int startLine = cursor.Line;
        int startColumn = cursor.Column;
        if (_currentChar == '+') 
            return CreateNewToken(TokenType.Plus, "+");
        if (_currentChar == '=')
            return CreateNewToken(TokenType.Assign, "=");
        if (_currentChar == '(')
            return CreateNewToken(TokenType.LeftRoundBracket, "(");
        if (_currentChar == ')')
            return CreateNewToken(TokenType.RightRoundBracket, ")");
        if (_currentChar == '{')
            return CreateNewToken(TokenType.LeftCurlyBracket, "{");
        if (_currentChar == '}')
            return CreateNewToken(TokenType.RightCurlyBracket, "}");
        if (_currentChar == '\0')
            return CreateNewToken(TokenType.Eof, "\0");
        if (char.IsLetter(_currentChar) || _currentChar == '_')
        {
            string word = ReadWord();
            TokenType? tokenType = GetKeywordType(word);
            if (!tokenType.HasValue)
            {
                return CreateNewToken(TokenType.Identifier, word, startLine, startColumn);
            }

            return CreateNewToken(tokenType.Value, word, startLine, startColumn);
        }
        if (char.IsDigit(_currentChar))
        {
            string number = ReadNumber();
            return CreateNewToken(TokenType.IntegerLiteral, number, startLine, startColumn);
        }
        
        throw new Exception($"Unexpected character: {_currentChar} on position {startColumn+1}:{startColumn+1}");
    }
    
    public IEnumerable<Token> GetTokens()
    {
        yield return GetNextToken();
    }

    private void SetCurrentChar()
    {
        if (cursor.Position >= _source.Length)
        {
            _currentChar = '\0';
        }
        else
        {
            _currentChar = _source[cursor.Position];
        }
    }
    
    private char Peek()
    {
        if (cursor.Position < _source.Length - 1)
        {
            return _source[cursor.Position + 1];
        }

        return '\0';
    }

    private void SkipWhiteSpaces()
    {
        while (_currentChar == ' ' || _currentChar == '\t' || _currentChar == '\n')
        {
            if (_currentChar == ' ' || _currentChar == '\t')
            {
                cursor.MoveNext();
            }
            else
            {
                cursor.MoveNewLine();
            }

            SetCurrentChar();
        }
    }

    private void SkipComment()
    {
        if (_currentChar == '/')
        {
            while (_source[cursor.Position] != '\n')
            {
                cursor.MoveNext();
            }
            cursor.MoveNewLine();
            SetCurrentChar();
        }
        else
        {
            cursor.MoveNext();
            SetCurrentChar();
            char nextChar = Peek();
            while (!(_currentChar == '/' && nextChar == '=' || nextChar == '\0'))
            {
                if (_currentChar == '\n')
                {
                    cursor.MoveNewLine();
                }
                else
                {
                    cursor.MoveNext();
                }
                SetCurrentChar();
                nextChar = Peek();
            }

            cursor.MoveNext();
            cursor.MoveNext();
            SetCurrentChar();
        }
    }

    private bool CanSkip()
    {
        char nextChar = Peek();
        return _currentChar == '\n' ||
               _currentChar == '\t' ||
               _currentChar == ' ' ||
               _currentChar == '/' && nextChar == '/' ||
               _currentChar == '=' && nextChar == '/';
    }

    private string ReadWord()
    {
        string word = "";
        do
        {
            word += _currentChar;
            cursor.MoveNext();
            SetCurrentChar();
        } 
        while (char.IsDigit(_currentChar) || char.IsLetter(_currentChar) || _currentChar == '_');

        return word;
    }
    
    private string ReadNumber()
    {
        string number = "";
        do
        {
            number += _currentChar;
            cursor.MoveNext();
            SetCurrentChar();
        } 
        while (char.IsDigit(_currentChar));

        return number;
    }

    private TokenType? GetKeywordType(string word)
    {
        switch (word)
        {
            case "var":
                return TokenType.Var;
            case "func":
                return TokenType.Func;
            case "print":
                return TokenType.Print;
            case "integer":
                return TokenType.Integer;
        }

        return null;
    }

    private Token CreateNewToken(TokenType tokenType, string lexeme)
    {
        return new Token(tokenType, lexeme, cursor.Line + 1, cursor.Column + 1);
    }

    private Token CreateNewToken(TokenType tokenType, string lexeme, int line, int column)
    {
        return new Token(tokenType, lexeme, line, column);
    }
}
