namespace Compiler.Parser.AST.Nodes.Core;

public class IdentifierNode : AstNode
{
    public string Name { get; }
    
    public IdentifierNode(string name) => Name = name;
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitIdentifierNode(this);
    }
}