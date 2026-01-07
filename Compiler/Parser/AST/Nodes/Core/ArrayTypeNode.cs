namespace Compiler.Parser.AST.Nodes.Core;

public class ArrayTypeNode : TypeAnnotationNode
{
    public TypeAnnotationNode ElementType { get; }

    // ссылка на class symbol?
    public ArrayTypeNode(TypeAnnotationNode elementType)
    {
        ElementType = elementType;
    }

    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
    
    public override string GetName => "[" + ElementType.GetName;
}