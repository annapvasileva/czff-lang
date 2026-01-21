namespace Compiler.Lexer;

public record Token(TokenType Kind, string Lexeme, int Line, int Start);
