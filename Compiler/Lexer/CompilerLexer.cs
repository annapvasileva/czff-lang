namespace Compiler.Lexer;

public class CompilerLexer
{
    private readonly string _source;

    private Cursor cursor = new Cursor();
    private char _currentChar;

    public CompilerLexer(string source)
    {
        _source = source;
    }

    public Token GetNextToken()
    {
        SetCurrentChar();
        while (CanSkipWhitespaces() || CanSkipComments())
        {
            if (CanSkipWhitespaces())
                SkipWhiteSpaces();
            if (CanSkipComments())
                SkipComment();
        }

        int startLine = cursor.Line + 1;
        int startColumn = cursor.Column + 1;
        if (_currentChar == '+')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.Plus, "+", startLine, startColumn);
        }

        if (_currentChar == '*')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.Multiply, "*", startLine, startColumn);
        }

        if (_currentChar == '/')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.Divide, "/", startLine, startColumn);
        }

        if (_currentChar == '%')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.Modulo, "%", startLine, startColumn);
        }

        if (_currentChar == '<')
        {
            cursor.MoveNext();
            SetCurrentChar();
            if (_currentChar == '=')
            {
                cursor.MoveNext();
                return CreateNewToken(TokenType.LessEqual, "<=", startLine, startColumn);
            }
            return CreateNewToken(TokenType.Less, "<", startLine, startColumn);
        }

        if (_currentChar == '>')
        {
            cursor.MoveNext();
            SetCurrentChar();
            if (_currentChar == '=')
            {
                cursor.MoveNext();
                return CreateNewToken(TokenType.GreaterEqual, ">=", startLine, startColumn);
            }
            return CreateNewToken(TokenType.Greater, ">", startLine, startColumn);
        }

        if (_currentChar == '=')
        {
            cursor.MoveNext();
            SetCurrentChar();
            if (_currentChar == '=')
            {
                cursor.MoveNext();
                return CreateNewToken(TokenType.Equal, "==", startLine, startColumn);
            }
            return CreateNewToken(TokenType.Assign, "=", startLine, startColumn);
        }

        if (_currentChar == '!')
        {
            cursor.MoveNext();
            SetCurrentChar();
            if (_currentChar == '=')
            {
                cursor.MoveNext();
                return CreateNewToken(TokenType.NotEqual, "!=", startLine, startColumn);
            }
            return CreateNewToken(TokenType.LogicalNegation, "!", startLine, startColumn);
        }

        if (_currentChar == '|')
        {
            cursor.MoveNext();
            SetCurrentChar();
            if (_currentChar != '|')
            {
                throw new LexerException($"Unexpected character: {_currentChar}", cursor.Line + 1, cursor.Column + 1);
            }

            cursor.MoveNext();
            return CreateNewToken(TokenType.LogicalOr, "||", startLine, startColumn);
        }

        if (_currentChar == '&')
        {
            cursor.MoveNext();
            SetCurrentChar();
            if (_currentChar != '&')
            {
                throw new LexerException($"Unexpected character: {_currentChar}", cursor.Line + 1, cursor.Column + 1);
            }

            cursor.MoveNext();
            return CreateNewToken(TokenType.LogicalAnd, "&&", startLine, startColumn);
        }

        if (_currentChar == ',')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.Comma, ",", startLine, startColumn);
        }

        if (_currentChar == ';')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.Semicolon, ";", startLine, startColumn);
        }

        if (_currentChar == '(')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.LeftRoundBracket, "(", startLine, startColumn);
        }

        if (_currentChar == ')')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.RightRoundBracket, ")", startLine, startColumn);
        }

        if (_currentChar == '{')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.LeftCurlyBracket, "{", startLine, startColumn);
        }

        if (_currentChar == '}')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.RightCurlyBracket, "}", startLine, startColumn);
        }

        if (_currentChar == '[')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.LeftSquareBracket, "[", startLine, startColumn);
        }

        if (_currentChar == ']')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.RightSquareBracket, "]", startLine, startColumn);
        }

        if (_currentChar == '\0')
        {
            cursor.MoveNewLine();
            return CreateNewToken(TokenType.Eof, "\0");
        }

        if (char.IsLetter(_currentChar) || _currentChar == '_')
        {
            string word = ReadWord();
            if (word == "true")
            {
                return CreateNewToken(TokenType.BoolLiteral, "true", startLine, startColumn);
            }
            if (word == "false")
            {
                return CreateNewToken(TokenType.BoolLiteral, "false", startLine, startColumn);
            }

            TokenType? tokenType = GetKeywordType(word);
            if (!tokenType.HasValue)
            {
                return CreateNewToken(TokenType.Identifier, word, startLine, startColumn);
            }

            return CreateNewToken(tokenType.Value, word, startLine, startColumn);
        }
        if (char.IsDigit(_currentChar) || _currentChar == '-' && char.IsDigit(Peek()))
        {
            string number = ReadNumber();
            return CreateNewToken(TokenType.IntegerLiteral, number, startLine, startColumn);
        }
        else if (_currentChar == '-')
        {
            cursor.MoveNext();
            return CreateNewToken(TokenType.Minus, "-", startLine, startColumn);
        }
        
        throw new LexerException($"Unexpected character: {_currentChar}", startLine + 1, startColumn + 1);
    }
    
    public IEnumerable<Token> GetTokens()
    {
        Token token;
        do
        {
            token = GetNextToken();
            yield return token;
        } 
        while (token.Kind != TokenType.Eof);
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
        while (CanSkipWhitespaces())
        {
            if (_currentChar == ' ' || _currentChar == '\r' || _currentChar == '\t')
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
        int startLine = cursor.Line + 1;
        int startColumn = cursor.Column + 1;
        if (_currentChar == '/')
        {
            while (!(_currentChar == '\n' || _currentChar == '\0'))
            {
                cursor.MoveNext();
                SetCurrentChar();
            }

            if (_currentChar == '\n')
            {
                cursor.MoveNewLine();
                SetCurrentChar();
            }
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

            if (nextChar == '\0')
            {
                throw new LexerException($"Unclosed comment", startLine, startColumn);
            }

            cursor.MoveNext();
            cursor.MoveNext();
            SetCurrentChar();
        }
    }

    private bool CanSkipWhitespaces()
    {
        return _currentChar == '\n' ||
               _currentChar == '\t' ||
               _currentChar == '\r' ||
               _currentChar == ' ';
    }
    
    private bool CanSkipComments()
    {
        char nextChar = Peek();
        return _currentChar == '/' && nextChar == '/' ||
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
            case "int":
                return TokenType.Integer;
            case "int64":
                return TokenType.Integer64;
            case "int128":
                return TokenType.Integer128;
            case "bool":
                return TokenType.Bool;
            case "void":
                return TokenType.Void;
            case "array":
                return TokenType.Array;
            case "return":
                return TokenType.Return;
            case "new":
                return TokenType.New;
            case "if":
                return TokenType.If;
            case "else":
                return TokenType.Else;
            case "for":
                return TokenType.For;
            case "while":
                return TokenType.While;
            case "break":
                return TokenType.Break;
            case "continue":
                return TokenType.Continue;
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
