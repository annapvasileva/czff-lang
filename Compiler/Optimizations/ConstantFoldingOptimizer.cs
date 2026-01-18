using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Optimizations;

public class ConstantFoldingOptimizer : INodeVisitor
{
    public void Visit(LiteralExpressionNode literalExpressionNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(IdentifierExpressionNode identifierExpressionNode)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

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
        throw new NotImplementedException();
    }

    public void Visit(FunctionDeclarationNode functionDeclarationNode)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void Visit(IdentifierAssignmentStatementNode assigmentStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ArrayAssignmentStatementNode assigmentStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BlockNode blockNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ArrayCreationExpressionNode arrayCreationExpressionNode)
    {
        throw new NotImplementedException();
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

    public void Visit(PrintStatementNode printStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ProgramNode programNode)
    {
        throw new NotImplementedException();
    }
}