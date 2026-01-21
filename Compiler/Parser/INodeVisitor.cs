using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser;

public interface INodeVisitor
{
    void Visit(LiteralExpressionNode literalExpressionNode);
    void Visit(IdentifierExpressionNode identifierExpressionNode);
    void Visit(SimpleTypeNode simpleTypeNode);
    void Visit(ArrayTypeNode arrayTypeNode);
    void Visit(BinaryExpressionNode binaryExpressionNode);
    void Visit(UnaryExpressionNode unaryExpressionNode);
    void Visit(FunctionCallExpressionNode functionCallExpressionNode);
    void Visit(VariableDeclarationNode variableDeclarationNode);
    void Visit(FunctionDeclarationNode functionDeclarationNode);
    void Visit(ClassDeclarationNode classDeclarationNode);
    void Visit(FunctionParametersNode functionParametersNode);
    void Visit(ExpressionStatementNode expressionStatementNode);
    void Visit(IdentifierAssignmentStatementNode assigmentStatementNode);
    void Visit(ArrayAssignmentStatementNode assigmentStatementNode);
    void Visit(BlockNode blockNode);
    void Visit(ArrayCreationExpressionNode arrayCreationExpressionNode);
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
    void Visit(ProgramNode programNode);
}