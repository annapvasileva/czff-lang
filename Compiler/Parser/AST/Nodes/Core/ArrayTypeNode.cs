namespace Compiler.Parser.AST.Nodes.Core;

public class ArrayTypeNode : TypeAnnotationNode
{
    public TypeAnnotationNode ElementType { get; }

    // ссылка на class symbol?
    public ArrayTypeNode(ArrayTypeNode elementType)
    {
        ElementType = elementType;
    }

    public override void Accept(IVisitor visitor)
    {
        visitor.VisitArrayTypeNode(this);
    }
}