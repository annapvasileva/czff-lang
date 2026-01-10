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
        var funcParams = ParseFunctionParameters();
        Expect(TokenType.RightRoundBracket);
        var body = ParseBlock();
        if (body.Statements.Count == 0 || !(body.Statements.Last() is ReturnStatementNode))
        {
            throw new ParserException("Ожидался return в конце функции", CurrentToken.Line, CurrentToken.Start);
        }

        return new FunctionDeclarationNode(returnType, funcName, funcParams, body);
    }

    private TypeAnnotationNode ParseType()
    {
        if (IsBuiltInType(CurrentToken.Kind))
        {
            var token = MoveNext();
            return new SimpleTypeNode(token.Lexeme);
        }

        if (CurrentToken.Kind == TokenType.Array)
        { 
            MoveNext();
            Expect(TokenType.Less);
            if (!IsBuiltInType(CurrentToken.Kind))
            {
                throw new ParserException("BuiltIn type was expected", CurrentToken.Line, CurrentToken.Start);
            }

            string type = CurrentToken.Lexeme;
            MoveNext();
            Expect(TokenType.Greater);

            return new ArrayTypeNode(new SimpleTypeNode(type));
        }
        
        throw new ParserException("Type expected", CurrentToken.Line, CurrentToken.Start);
    }

    private FunctionParametersNode ParseFunctionParameters()
    {
        var parameters = new List<FunctionParametersNode.Variable>();
        if (CurrentToken.Kind != TokenType.RightRoundBracket)
        {
            bool flag = false;
            do
            {
                if (flag)
                    Expect(TokenType.Comma);
                flag = true;
                var paramType = ParseType();
                var paramName = Expect(TokenType.Identifier).Lexeme;
                parameters.Add(new FunctionParametersNode.Variable(paramName, paramType));
            } while (CurrentToken.Kind == TokenType.Comma);
        }

        return new FunctionParametersNode(parameters);
    }

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
            TokenType.Return => ParseReturn(),
            _ => ParseAssignment()
        };
    }

    private VariableDeclarationNode ParseVariableDeclaration()
    {
        Expect(TokenType.Var);
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

    private ReturnStatementNode ParseReturn()
    {
        Expect(TokenType.Return);
        ExpressionNode? expression = null;
        if (CurrentToken.Kind != TokenType.Semicolon)
        {
            expression = ParseExpression();
        }

        return new ReturnStatementNode(expression);
    }

    private StatementNode ParseAssignment()
    {
        int saveIndex = _currentTokenIndex;
        ExpressionNode left = ParseExpression();
        if (CurrentToken.Kind != TokenType.Assign)
        {
            _currentTokenIndex = saveIndex;
            return ParseExpressionStatement();
        }

        Expect(TokenType.Assign);
        ExpressionNode right = ParseExpression();

        if (left is IdentifierExpressionNode identifierExpressionNode)
        {
            return new IdentifierAssignmentStatementNode(identifierExpressionNode, right);
        }
        if (left is ArrayIndexExpressionNode arrayIndexExpressionNode)
        {
            return new ArrayAssignmentStatementNode(arrayIndexExpressionNode, right);
        }

        Token start = _tokens[_currentTokenIndex];
        throw new ParserException("Identifier or array index was expected", start.Line, start.Start);
    }

    private ExpressionStatementNode ParseExpressionStatement()
    {
        return new ExpressionStatementNode(ParseExpression());
    }
    
    private ExpressionNode ParseExpression()
    {
        return ParseLogicalOr();
    }

    private ExpressionNode ParseLogicalOr()
    {
        ExpressionNode expr = ParseLogicalAnd();
        while (CurrentToken.Kind == TokenType.LogicalOr)
        {
            MoveNext();
            ExpressionNode right = ParseLogicalAnd();
            expr = new BinaryExpressionNode(expr, right, BinaryOperatorType.LogicalOr);
        }

        return expr;
    }

    private ExpressionNode ParseLogicalAnd()
    {
        ExpressionNode expr = ParseEquality();
        while (CurrentToken.Kind == TokenType.LogicalAnd)
        {
            MoveNext();
            ExpressionNode right = ParseEquality();
            expr = new BinaryExpressionNode(expr, right, BinaryOperatorType.LogicalAnd);
        }

        return expr;
    }

    private ExpressionNode ParseEquality()
    {
        ExpressionNode expr = ParseComparison();
        while (true)
        {
            BinaryOperatorType? binaryOperatorType;
            switch (CurrentToken.Kind)
            {
                case TokenType.Equal:
                    binaryOperatorType = BinaryOperatorType.Equal;
                    break;
                case TokenType.NotEqual:
                    binaryOperatorType = BinaryOperatorType.NotEqual;
                    break;
                default:
                    binaryOperatorType = null;
                    break;
            }

            if (binaryOperatorType == null)
            {
                break;
            }

            MoveNext();
            ExpressionNode right = ParseComparison();
            expr = new BinaryExpressionNode(expr, right, binaryOperatorType.Value);
        }

        return expr;
    }

    private ExpressionNode ParseComparison()
    {
        ExpressionNode expr = ParseAdditive();
        while (true)
        {
            BinaryOperatorType? binaryOperatorType;
            switch (CurrentToken.Kind)
            {
                case TokenType.Less:
                    binaryOperatorType = BinaryOperatorType.Less;
                    break;
                case TokenType.LessEqual:
                    binaryOperatorType = BinaryOperatorType.LessOrEqual;
                    break;
                case TokenType.Greater:
                    binaryOperatorType = BinaryOperatorType.Greater;
                    break;
                case TokenType.GreaterEqual:
                    binaryOperatorType = BinaryOperatorType.GreaterOrEqual;
                    break;
                default:
                    binaryOperatorType = null;
                    break;
            }

            if (binaryOperatorType == null)
            {
                break;
            }

            MoveNext();
            ExpressionNode right = ParseComparison();
            expr = new BinaryExpressionNode(expr, right, binaryOperatorType.Value);
        }

        return expr;
    }

    private ExpressionNode ParseAdditive()
    {
        ExpressionNode expr = ParseMultiplicative();
        while (true)
        {
            BinaryOperatorType? binaryOperatorType;
            switch (CurrentToken.Kind)
            {
                case TokenType.Plus:
                    binaryOperatorType = BinaryOperatorType.Addition;
                    break;
                case TokenType.Minus:
                    binaryOperatorType = BinaryOperatorType.Subtraction;
                    break;
                default:
                    binaryOperatorType = null;
                    break;
            }

            if (binaryOperatorType == null)
            {
                break;
            }

            MoveNext();
            ExpressionNode right = ParseMultiplicative();
            expr = new BinaryExpressionNode(expr, right, binaryOperatorType.Value);
        }

        return expr;
    }

    private ExpressionNode ParseMultiplicative()
    {
        ExpressionNode expr = ParseUnary();
        while (true)
        {
            BinaryOperatorType? binaryOperatorType;
            switch (CurrentToken.Kind)
            {
                case TokenType.Multiply:
                    binaryOperatorType = BinaryOperatorType.Multiplication;
                    break;
                case TokenType.Divide:
                    binaryOperatorType = BinaryOperatorType.Division;
                    break;
                case TokenType.Modulo:
                    binaryOperatorType = BinaryOperatorType.Modulus;
                    break;
                default:
                    binaryOperatorType = null;
                    break;
            }

            if (binaryOperatorType == null)
            {
                break;
            }

            MoveNext();
            ExpressionNode right = ParseUnary();
            expr = new BinaryExpressionNode(expr, right, binaryOperatorType.Value);
        }

        return expr;
    }

    private ExpressionNode ParseUnary()
    {
        if (CurrentToken.Kind == TokenType.Minus || CurrentToken.Kind == TokenType.LogicalNegation)
        {
            var unaryOperatorType = UnaryOperatorType.Minus;
            if (CurrentToken.Kind == TokenType.LogicalNegation)
            {
                unaryOperatorType = UnaryOperatorType.Negation;
            }
            MoveNext();
            ExpressionNode expr = ParseUnary();
            return new UnaryExpressionNode(unaryOperatorType, expr);
        }

        return ParsePostfix();
    }

    private ExpressionNode ParsePostfix()
    {
        ExpressionNode expr = ParsePrimary();
        while(true)
        {
            if (CurrentToken.Kind == TokenType.LeftSquareBracket)
            {
                MoveNext();
                ExpressionNode index = ParseExpression();
                Expect(TokenType.RightSquareBracket);
                expr = new ArrayIndexExpressionNode(expr, index);
            }
            else if (CurrentToken.Kind == TokenType.LeftRoundBracket && expr is IdentifierExpressionNode identifierExpressionNode)
            {
                MoveNext();
                var args = new List<ExpressionNode>();
                if (CurrentToken.Kind != TokenType.RightRoundBracket)
                {
                    bool flag = false;
                    do
                    {
                        if (flag)
                            Expect(TokenType.Comma);
                        flag = true;
                        args.Add(ParseExpression());
                    } while (CurrentToken.Kind == TokenType.Comma);
                }
                Expect(TokenType.RightRoundBracket);
                expr = new FunctionCallExpressionNode(identifierExpressionNode.Name, args);
            }
            else
            {
                break;
            }
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

        if (CurrentToken.Kind == TokenType.BoolLiteral)
        {
            var boolLiteral = Expect(TokenType.BoolLiteral);
            return new LiteralExpressionNode(boolLiteral.Lexeme, LiteralType.BooleanLiteral);
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

        if (CurrentToken.Kind == TokenType.New)
        {
            return ParseNewExpression();
        }

        throw new ParserException("Ожидался expression", CurrentToken.Line, CurrentToken.Start);
    }

    private ExpressionNode ParseNewExpression()
    {
        Expect(TokenType.New);
        TypeAnnotationNode elementType = ParseType();
        Expect(TokenType.LeftRoundBracket);
        ExpressionNode arraySize = ParseExpression();
        Expect(TokenType.RightRoundBracket);
        Expect(TokenType.LeftSquareBracket);
        Expect(TokenType.RightSquareBracket);

        return new ArrayCreationExpressionNode(elementType, arraySize);
    }
    
    private bool IsBuiltInType(TokenType tokenType)
    {
        return tokenType == TokenType.Integer ||
               tokenType == TokenType.Void ||
               tokenType == TokenType.Bool;
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