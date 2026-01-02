namespace Compiler.Parser.AST.Nodes.Core;

public class FunctionParameterNode : AstNode
{
    public string Name { get; }
    public TypeAnnotationNode Type { get; }

    public FunctionParameterNode(string name, TypeAnnotationNode type)
    {
        Name = name;
        Type = type;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.VisitFunctionParameterNode(this);
    }
}