using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser;

public interface INodeVisitor
{
    void VisitLiteralNode(LiteralNode literalNode);
    void VisitIdentifierNode(IdentifierNode identifierNode);
    void VisitSimpleTypeNode(SimpleTypeNode simpleTypeNode);
    void VisitArrayTypeNode(ArrayTypeNode arrayTypeNode);
    void VisitBinaryExpressionNode(BinaryExpressionNode binaryExpressionNode);
    void VisitUnaryExpressionNode(UnaryExpressionNode unaryExpressionNode);
    void VisitFunctionCallExpressionNode(FunctionCallExpressionNode functionCallExpressionNode);
    void VisitVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode);
    void VisitFunctionDeclarationNode(FunctionDeclarationNode functionDeclarationNode);
    void VisitClassDeclarationNode(ClassDeclarationNode classDeclarationNode);
    void VisitFunctionParameterNode(FunctionParameterNode functionParameterNode);
    void VisitExpressionStatementNode(ExpressionStatementNode expressionStatementNode);
    void VisitAssignmentStatementNode(AssignmentStatementNode assigmentStatementNode);
    void VisitBlockNode(BlockNode blockNode);
    void VisitArrayIndexExpressionNode(ArrayIndexExpressionNode arrayIndexExpressionNode);
    void VisitMemberAccessNode(MemberAccessNode memberAccessNode);
    void VisitBreakStatementNode(BreakStatementNode breakStatementNode);
    void VisitContinueStatementNode(ContinueStatementNode continueStatementNode);
    void VisitReturnStatementNode(ReturnStatementNode returnStatementNode);
    void VisitIfStatementNode(IfStatementNode ifStatementNode);
    void VisitElifStatementNode(ElifStatementNode elifStatementNode);
    void VisitWhileStatementNode(WhileStatementNode whileStatementNode);
    void VisitForStatementNode(ForStatementNode forStatementNode);
    void VisitPrintStatementNode(PrintStatementNode printStatementNode);
}