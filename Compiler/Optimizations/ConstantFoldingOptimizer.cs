using System.Data;
using System.Linq.Expressions;
using Compiler.Generation;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.Util.LiteralTranslation;

namespace Compiler.Optimizations;

public class ConstantFoldingOptimizer : INodeVisitor
{
    private ExpressionNode _newExpression;
    
    private static LiteralExpressionNode MakeNewLiteral(LiteralExpressionNode leftLiteral, LiteralExpressionNode rightLiteral, BinaryOperatorType type)
    {
        
        INativeType left = NativeTypeBuilder.Build(leftLiteral);
        INativeType right = NativeTypeBuilder.Build(rightLiteral);

        switch (type)
        {
            case BinaryOperatorType.Addition:
                left.Add(right);
                break;
            case BinaryOperatorType.Subtraction:
                left.Subtract(right);
                break;
            case BinaryOperatorType.Multiplication:
                left.Multiply(right);
                break;
            case BinaryOperatorType.Division:
                left.Divide(right);
                break;
            case BinaryOperatorType.Less:
                left.Less(right);
                break;
            case BinaryOperatorType.LessOrEqual:
                left.LessOrEqual(right);
                break;
            case BinaryOperatorType.Equal:
                left.Equal(right);
                break;
            case BinaryOperatorType.Greater:
                left.Greater(right);
                break;
            case BinaryOperatorType.GreaterOrEqual:
                left.GreaterOrEqual(right);
                break;
            case BinaryOperatorType.NotEqual:
                left.NotEqual(right);
                break;
            case BinaryOperatorType.LogicalOr:
                left.LogicalOr(right);
                break;
            case BinaryOperatorType.LogicalAnd:
                left.LogicalAnd(right);
                break;
            case BinaryOperatorType.Modulus:
                left.Mod(right);
                break;
            default:
                throw new GeneratorException("Binary operator not supported.");
        }

        return left.GetExpressionView();
    }
    
    private static LiteralExpressionNode MakeNewLiteral(LiteralExpressionNode literal,UnaryOperatorType type)
    {
        INativeType native = NativeTypeBuilder.Build(literal);
        
        switch (type)
        {
            case UnaryOperatorType.Minus:
                native.Minus();
                break;
            case UnaryOperatorType.Negation:
                native.Negative();
                break;
            default:
                throw new GeneratorException("Unary operator not supported.");
        }

        return native.GetExpressionView();
    }
    
    public void Visit(LiteralExpressionNode literalExpressionNode)
    {
        _newExpression = literalExpressionNode;
    }

    public void Visit(IdentifierExpressionNode identifierExpressionNode)
    {
        _newExpression = identifierExpressionNode;
    }

    public void Visit(SimpleTypeNode simpleTypeNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ArrayTypeNode arrayTypeNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BinaryExpressionNode binaryExpressionNode)
    {
        binaryExpressionNode.LeftExpression.Accept(this);
        binaryExpressionNode.LeftExpression = _newExpression;
        
        binaryExpressionNode.RightExpression.Accept(this);
        binaryExpressionNode.RightExpression = _newExpression;
        
        var left = binaryExpressionNode.LeftExpression;
        var right = binaryExpressionNode.RightExpression;

        if (left is not LiteralExpressionNode leftLiteral || right is not LiteralExpressionNode rightLiteral)
        {
            _newExpression = binaryExpressionNode;
            return;
        }

        _newExpression = MakeNewLiteral(leftLiteral, rightLiteral, binaryExpressionNode.BinaryOperatorType);
        
    }

    public void Visit(UnaryExpressionNode unaryExpressionNode)
    {
        unaryExpressionNode.Expression.Accept(this);
        unaryExpressionNode.Expression = _newExpression;
        
        if (unaryExpressionNode.Expression is not LiteralExpressionNode literal)
        {
            _newExpression = unaryExpressionNode;
            return;
        }
        
        _newExpression = MakeNewLiteral(literal, unaryExpressionNode.UnaryOperatorType);
    }

    public void Visit(FunctionCallExpressionNode functionCallExpressionNode)
    {
        var args = functionCallExpressionNode.Arguments;
        
        for (int i = 0; i < args.Count; i++)
        {
            args[i].Accept(this);
            args[i] = _newExpression;
        }
        
        _newExpression = functionCallExpressionNode;
    }

    public void Visit(VariableDeclarationNode variableDeclarationNode)
    {
        if (variableDeclarationNode.Expression == null)
        {
            return;
        }    
        
        variableDeclarationNode.Expression.Accept(this);
        variableDeclarationNode.Expression = _newExpression;
    }

    public void Visit(FunctionDeclarationNode functionDeclarationNode)
    {
        functionDeclarationNode.Body.Accept(this);
    }

    public void Visit(ClassDeclarationNode classDeclarationNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionParametersNode functionParametersNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ExpressionStatementNode expressionStatementNode)
    {
        expressionStatementNode.Expression.Accept(this);
        
    }

    public void Visit(IdentifierAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Right.Accept(this);

        assigmentStatementNode.Right = _newExpression;
    }

    public void Visit(ArrayAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Left.Accept(this);
        if (_newExpression is not ArrayIndexExpressionNode left)
        {
            throw new ConstraintException("Got not ArrayIndexExpression from Visiting ArrayIndexExpression");
        }

        assigmentStatementNode.Left = left;

        assigmentStatementNode.Right.Accept(this);
        assigmentStatementNode.Right = _newExpression;
    }

    public void Visit(BlockNode blockNode)
    {
        foreach (var statement in blockNode.Statements)
        {
            statement.Accept(this);
        }
    }

    public void Visit(ArrayCreationExpressionNode arrayCreationExpressionNode)
    {
        arrayCreationExpressionNode.Size.Accept(this);
        arrayCreationExpressionNode.Size = _newExpression;
    }

    public void Visit(ArrayIndexExpressionNode arrayIndexExpressionNode)
    {
        arrayIndexExpressionNode.Index.Accept(this);
        arrayIndexExpressionNode.Index = _newExpression;
    }

    public void Visit(MemberAccessNode memberAccessNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BreakStatementNode breakStatementNode)
    {
        return;
    }

    public void Visit(ContinueStatementNode continueStatementNode)
    {
        return;
    }

    public void Visit(ReturnStatementNode returnStatementNode)
    {
        if (returnStatementNode.Expression == null)
        {
            return;
        }
        
        returnStatementNode.Expression.Accept(this);
        returnStatementNode.Expression = _newExpression;
    }

    public void Visit(IfStatementNode ifStatementNode)
    {
        ifStatementNode.Condition.Accept(this);
        ifStatementNode.Condition = _newExpression;
        
        ifStatementNode.IfBlock.Accept(this);
        
        ifStatementNode.ElseBlock?.Accept(this);
    }

    public void Visit(ElifStatementNode elifStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileStatementNode whileStatementNode)
    {
        whileStatementNode.Condition.Accept(this);
        whileStatementNode.Condition = _newExpression;
        
        whileStatementNode.Body.Accept(this);
    }

    public void Visit(ForStatementNode forStatementNode)
    {
        forStatementNode.Init.Accept(this);
        
        forStatementNode.Condition.Accept(this);
        forStatementNode.Condition = _newExpression;
        
        forStatementNode.Body.Accept(this);
        
        forStatementNode.Post.Accept(this);
    }

    public void Visit(PrintStatementNode printStatementNode)
    {
        printStatementNode.Expression.Accept(this);
        printStatementNode.Expression =  _newExpression;
    }

    public void Visit(ProgramNode programNode)
    {
        foreach (var function in programNode.Functions)
        {
            function.Accept(this);
        }
    }
}