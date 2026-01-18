using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis.Models;

namespace Compiler.Optimizations;

public class DeadCodeEliminationOptimizer(SymbolTable scope) : INodeVisitor
{
    private SymbolTable _scope = scope;
    private bool _terminated = false;
    
    public void Visit(LiteralExpressionNode literalExpressionNode)
    { }

    public void Visit(IdentifierExpressionNode identifierExpressionNode)
    { }

    public void Visit(SimpleTypeNode simpleTypeNode)
    { }

    public void Visit(ArrayTypeNode arrayTypeNode)
    {
        arrayTypeNode.ElementType.Accept(this);
    }

    public void Visit(BinaryExpressionNode binaryExpressionNode)
    {
        binaryExpressionNode.LeftExpression.Accept(this);
        binaryExpressionNode.RightExpression.Accept(this);
    }

    public void Visit(UnaryExpressionNode unaryExpressionNode)
    {
        unaryExpressionNode.Expression.Accept(this);
    }

    public void Visit(FunctionCallExpressionNode functionCallExpressionNode)
    {
        foreach (ExpressionNode argument in functionCallExpressionNode.Arguments)
        {
            argument.Accept(this);
        }
    }

    public void Visit(VariableDeclarationNode variableDeclarationNode)
    {
        variableDeclarationNode.Type.Accept(this);
        if (variableDeclarationNode.Expression != null)
        {
            variableDeclarationNode.Expression.Accept(this);
        }
    }

    public void Visit(FunctionDeclarationNode functionDeclarationNode)
    {
        _scope = functionDeclarationNode.Body.Scope;
        functionDeclarationNode.ReturnType.Accept(this);
        functionDeclarationNode.Parameters.Accept(this);
        functionDeclarationNode.Body.Accept(this);
        
        _scope = _scope.Parent!;
    }

    public void Visit(ClassDeclarationNode classDeclarationNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionParametersNode functionParametersNode)
    {
        foreach (var parameter in functionParametersNode.Parameters)
        {
            parameter.Type.Accept(this);
        }
    }

    public void Visit(ExpressionStatementNode expressionStatementNode)
    {
        expressionStatementNode.Expression.Accept(this);
    }

    public void Visit(IdentifierAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Left.Accept(this);
        assigmentStatementNode.Right.Accept(this);
    }

    public void Visit(ArrayAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Left.Accept(this);
        assigmentStatementNode.Right.Accept(this);
    }

    public void Visit(BlockNode blockNode)
    {
        var newStatements = new List<StatementNode>();
        _terminated = false;
        foreach (var statement in blockNode.Statements)
        {
            if (_terminated)
            {
                break;
            }
            statement.Accept(this);
            newStatements.Add(statement);
        }

        blockNode.Statements = newStatements;
        _terminated = false;
    }

    public void Visit(ArrayCreationExpressionNode arrayCreationExpressionNode)
    {
        arrayCreationExpressionNode.ElementType.Accept(this);
        arrayCreationExpressionNode.Size.Accept(this);
    }

    public void Visit(ArrayIndexExpressionNode arrayIndexExpressionNode)
    {
        arrayIndexExpressionNode.Array.Accept(this);
        arrayIndexExpressionNode.Index.Accept(this);
    }

    public void Visit(MemberAccessNode memberAccessNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BreakStatementNode breakStatementNode)
    {
        _terminated = true;
    }

    public void Visit(ContinueStatementNode continueStatementNode)
    {
        _terminated = true;
    }

    public void Visit(ReturnStatementNode returnStatementNode)
    {
        _terminated = true;
    }

    public void Visit(IfStatementNode ifStatementNode)
    {
        _scope = ifStatementNode.IfBlock.Scope;
        ifStatementNode.Condition.Accept(this);
        ifStatementNode.IfBlock.Accept(this);
        _scope = _scope.Parent!;
        if (ifStatementNode.ElseBlock != null)
        {
            _scope = ifStatementNode.ElseBlock.Scope;
            ifStatementNode.ElseBlock.Accept(this);
            _scope = _scope.Parent!;
        }
    }

    public void Visit(ElifStatementNode elifStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileStatementNode whileStatementNode)
    {
        _scope = whileStatementNode.Body.Scope;
        whileStatementNode.Condition.Accept(this);
        whileStatementNode.Body.Accept(this);
        _scope = _scope.Parent!;
    }

    public void Visit(ForStatementNode forStatementNode)
    {
        _scope = forStatementNode.Body.Scope;
        forStatementNode.Init.Accept(this);
        forStatementNode.Condition.Accept(this);
        forStatementNode.Post.Accept(this);
        forStatementNode.Body.Accept(this);
        _scope = _scope.Parent!;
    }

    public void Visit(PrintStatementNode printStatementNode)
    {
        printStatementNode.Expression.Accept(this);
    }

    public void Visit(ProgramNode programNode)
    {
        foreach (var func in programNode.Functions)
        {
            func.Accept(this);
        }
    }
}