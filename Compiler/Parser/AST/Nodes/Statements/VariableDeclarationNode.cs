using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Parser.AST.Nodes.Statements;

public class VariableDeclarationNode : DeclarationNode
{
    public TypeAnnotationNode Type { get; }
    public IdentifierNode Identifier { get; }
    public ExpressionNode? Expression { get; }

    public VariableDeclarationNode(TypeAnnotationNode type, IdentifierNode identifier,
        ExpressionNode expression)
    {
        Type = type;
        Identifier = identifier;
        Expression = expression;
    }

    public override void Accept(IVisitor visitor)
    {
        visitor.VisitVariableDeclarationNode(this);
    }
}