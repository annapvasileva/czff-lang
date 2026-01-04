using Compiler.Lexer;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser;

public class CompilerParser
{
    private IList<Token> _tokens;
    private int _currentTokenIndex;
    private Token CurrentToken => _currentTokenIndex < _tokens.Count ? _tokens[_currentTokenIndex] : _tokens.Last();

    public CompilerParser(IList<Token> tokens)
    {
        _tokens = tokens;
        _currentTokenIndex = 0;
    }

    public AstTree Parse()
    {
        var functionDeclarations = new List<FunctionDeclarationNode>();
        do
        {
            if (CurrentToken.Kind == TokenType.Func)
            {
                functionDeclarations.Add(ParseFunctionDeclaration());
            }
            else
            {
                throw new ParserException("Top-level statements can only include function declarations",
                    CurrentToken.Line, CurrentToken.Start);
            }
        } while (CurrentToken.Kind != TokenType.Eof);
        
        var programNode = new ProgramNode(functionDeclarations);
        var ast = new AstTree(programNode);

        return ast;
    }

    private FunctionDeclarationNode ParseFunctionDeclaration()
    {
        Expect(TokenType.Func);
        var returnType = ParseType();
        var funcName = Expect(TokenType.Identifier).Lexeme;
        Expect(TokenType.LeftRoundBracket);
        Expect(TokenType.RightRoundBracket);
        var body = ParseBlock();

        return new FunctionDeclarationNode(returnType, funcName, new List<FunctionParameterNode>(), body);
    }

    private TypeAnnotationNode ParseType()
    {
        if (IsBuiltInType(CurrentToken.Kind))
        {
            var token = MoveNext();
            return new SimpleTypeNode(token.Lexeme);
        }
        
        throw new ParserException("Type expected",
            CurrentToken.Line, CurrentToken.Start);
    }

    // private IdentifierExpressionNode ParseIdentifier()
    // {
    //     var token = Expect(TokenType.Identifier);
    //     return new IdentifierExpressionNode(token.Lexeme);
    // }

    private BlockNode ParseBlock()
    {
        Expect(TokenType.LeftCurlyBracket);
        var statements = new List<StatementNode>();
        while (CurrentToken.Kind != TokenType.RightCurlyBracket && CurrentToken.Kind != TokenType.Eof)
        {
            statements.Add(ParseStatement());
            Expect(TokenType.Semicolon);
        }
        Expect(TokenType.RightCurlyBracket);
        
        return new BlockNode(statements);
    }

    private StatementNode ParseStatement()
    {
        return CurrentToken.Kind switch
        {
            TokenType.Func => ParseFunctionDeclaration(),
            TokenType.Var => ParseVariableDeclaration(),
            TokenType.Print => ParsePrint(),
            _ => ParseExpressionStatement()
        };
    }

    private VariableDeclarationNode ParseVariableDeclaration()
    {
        Expect(TokenType.Var);
        // where check no void?
        var variableType = ParseType();
        var variableName = Expect(TokenType.Identifier).Lexeme;
        
        if (CurrentToken.Kind == TokenType.Assign)
        {
            Expect(TokenType.Assign);
            ExpressionNode expression = ParseExpression();
            
            return new VariableDeclarationNode(variableType, variableName, expression);
        }
        
        return new VariableDeclarationNode(variableType, variableName);
    }

    private PrintStatementNode ParsePrint()
    {
        Expect(TokenType.Print);
        return new PrintStatementNode(ParseExpression());
    }

    private ExpressionStatementNode ParseExpressionStatement()
    {
        return new ExpressionStatementNode(ParseExpression());
    }
    
    private ExpressionNode ParseExpression()
    {
        return ParseAdditive();
    }

    private ExpressionNode ParseAdditive()
    {
        ExpressionNode expr = ParsePrimary();
        while (CurrentToken.Kind == TokenType.Plus)
        {
            Expect(TokenType.Plus);
            ExpressionNode right = ParsePrimary();
            expr = new BinaryExpressionNode(expr, right, BinaryOperatorType.Addition);
        }

        return expr;
    }

    private ExpressionNode ParsePrimary()
    {
        if (CurrentToken.Kind == TokenType.IntegerLiteral)
        {
            var intLiteral = Expect(TokenType.IntegerLiteral);
            return new LiteralExpressionNode(intLiteral.Lexeme, LiteralType.IntegerLiteral);
        }

        if (CurrentToken.Kind == TokenType.Identifier)
        {
            var identifier = Expect(TokenType.Identifier);
            return new IdentifierExpressionNode(identifier.Lexeme);
        }

        if (CurrentToken.Kind == TokenType.LeftRoundBracket)
        {
            Expect(TokenType.LeftRoundBracket);
            ExpressionNode expr = ParseExpression();
            Expect(TokenType.RightRoundBracket);

            return expr;
        }

        throw new ParserException("Ожидался expression", CurrentToken.Line, CurrentToken.Start);
    }
    
    private bool IsBuiltInType(TokenType tokenType)
    {
        return tokenType == TokenType.Integer ||
               tokenType == TokenType.Void;
    }

    private Token MoveNext()
    {
        var token = CurrentToken;
        _currentTokenIndex += 1;
        return token;
    }
    
    private Token Expect(TokenType expectedType)
    {
        if (CurrentToken.Kind == expectedType)
        {
            return MoveNext();
        }
        
        throw new ParserException(
            $"Ожидался {expectedType}, но получен {CurrentToken.Kind}",
            CurrentToken.Line,
            CurrentToken.Start
        );
    }

    private Token Peek()
    {
        var position = _currentTokenIndex + 1;
        if (position < _tokens.Count)
        {
            return _tokens[position];
        }

        return _tokens.Last();
    }
}