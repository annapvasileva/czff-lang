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
        throw new NotImplementedException();
    }

    public void Visit(BinaryExpressionNode binaryExpressionNode) { }

    public void Visit(UnaryExpressionNode unaryExpressionNode)
    {
        throw new NotImplementedException();
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
        SymbolTableManager.ExitScope();
    }

    public void Visit(ClassDeclarationNode classDeclarationNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionParameterNode functionParameterNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ExpressionStatementNode expressionStatementNode) { }

    public void Visit(AssignmentStatementNode assigmentStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BlockNode blockNode)
    {
        blockNode.Scope = SymbolTableManager.CurrentScope;
        foreach (var statement in blockNode.Statements)
        {
            statement.Accept(this);
        }
    }

    public void Visit(ArrayIndexExpressionNode arrayIndexExpressionNode)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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

    public void Visit(PrintStatementNode printStatementNode) { }

    public void Visit(ProgramNode programNode)
    {
        foreach (var funcDeclaration in programNode.Functions)
        {
            funcDeclaration.Accept(this);
        }
    }
}