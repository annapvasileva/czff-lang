using Compiler.Parser;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SourceFiles;

namespace Compiler.Generation;

public class BallGeneratingVisitor(Ball target) : INodeVisitor
{
    private Ball _target = target;
    
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
        var func = new Function();
        ConstantPool constantPool = _target.ConstantPool;
        
        int idx = constantPool.GetIndex(new ConstantItem(5, functionDeclarationNode.Name));

        if (idx < 0)
        {
            constantPool.AddConstant(new ConstantItem(5, functionDeclarationNode.Name));
        }
        
        foreach (var param in functionDeclarationNode.Parameters)
        {
            param.Accept(this);
        }
        
        functionDeclarationNode.Accept(this);
        
        functionDeclarationNode.Body.Accept(this);
    }

    public void Visit(ClassDeclarationNode classDeclarationNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionParameterNode functionParameterNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ExpressionStatementNode expressionStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(AssignmentStatementNode assigmentStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BlockNode blockNode)
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
        foreach (var func in programNode.Functions)
        {
            func.Accept(this);
        }
        
        // There will be classes
    }
}