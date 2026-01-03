using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser;

public interface INodeVisitor
{
    void Visit(LiteralNode literalNode);
    void Visit(IdentifierNode identifierNode);
    void Visit(SimpleTypeNode simpleTypeNode);
    void Visit(ArrayTypeNode arrayTypeNode);
    void Visit(BinaryExpressionNode binaryExpressionNode);
    void Visit(UnaryExpressionNode unaryExpressionNode);
    void Visit(FunctionCallExpressionNode functionCallExpressionNode);
    void Visit(VariableDeclarationNode variableDeclarationNode);
    void Visit(FunctionDeclarationNode functionDeclarationNode);
    void Visit(ClassDeclarationNode classDeclarationNode);
    void Visit(FunctionParameterNode functionParameterNode);
    void Visit(ExpressionStatementNode expressionStatementNode);
    void Visit(AssignmentStatementNode assigmentStatementNode);
    void Visit(BlockNode blockNode);
    void Visit(ArrayIndexExpressionNode arrayIndexExpressionNode);
    void Visit(MemberAccessNode memberAccessNode);
    void Visit(BreakStatementNode breakStatementNode);
    void Visit(ContinueStatementNode continueStatementNode);
    void Visit(ReturnStatementNode returnStatementNode);
    void Visit(IfStatementNode ifStatementNode);
    void Visit(ElifStatementNode elifStatementNode);
    void Visit(WhileStatementNode whileStatementNode);
    void Visit(ForStatementNode forStatementNode);
    void Visit(PrintStatementNode printStatementNode);
}