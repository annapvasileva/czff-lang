using Compiler.Parser;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis.Models;

namespace Compiler.SemanticAnalysis;

public class SymbolTableBuilder : INodeVisitor
{
    public SymbolTableManager SymbolTableManager { get; }
    
    public SymbolTable SymbolTable => SymbolTableManager.CurrentScope;

    public SymbolTableBuilder()
    {
        SymbolTableManager = new SymbolTableManager();
    }
    
    public void Visit(LiteralExpressionNode literalExpressionNode) { }

    public void Visit(IdentifierExpressionNode identifierExpressionNode) { }

    public void Visit(SimpleTypeNode simpleTypeNode) { }

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
        throw new NotImplementedException();
    }

    public void Visit(VariableDeclarationNode variableDeclarationNode)
    {
        SymbolTableManager.DeclareVariable(variableDeclarationNode.Name, variableDeclarationNode.Type.GetName);
    }

    public void Visit(FunctionDeclarationNode functionDeclarationNode)
    {
        SymbolTableManager.DeclareFunction(functionDeclarationNode.Name, functionDeclarationNode.ReturnType.GetName);
        SymbolTableManager.EnterScope(true);
        functionDeclarationNode.Body.Accept(this);
        SymbolTableManager.SetFunctionLocalsLength(functionDeclarationNode.Name);
        SymbolTableManager.ExitScope();
    }

    public void Visit(ClassDeclarationNode classDeclarationNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionParametersNode functionParametersNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ExpressionStatementNode expressionStatementNode) { }

    public void Visit(ArrayAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Left.Accept(this);
        assigmentStatementNode.Right.Accept(this);
    }
    
    public void Visit(IdentifierAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Left.Accept(this);
        assigmentStatementNode.Right.Accept(this);
    }

    public void Visit(BlockNode blockNode)
    {
        blockNode.Scope = SymbolTableManager.CurrentScope;
        foreach (var statement in blockNode.Statements)
        {
            statement.Accept(this);
        }
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
        throw new NotImplementedException();
    }

    public void Visit(ContinueStatementNode continueStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ReturnStatementNode returnStatementNode)
    {
        if (returnStatementNode.Expression != null)
        {
            returnStatementNode.Expression.Accept(this);
        }
    }

    public void Visit(IfStatementNode ifStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ElifStatementNode elifStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileStatementNode whileStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ForStatementNode forStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(PrintStatementNode printStatementNode)
    {
        printStatementNode.Expression.Accept(this);
    }

    public void Visit(ProgramNode programNode)
    {
        foreach (var funcDeclaration in programNode.Functions)
        {
            funcDeclaration.Accept(this);
        }
    }
}