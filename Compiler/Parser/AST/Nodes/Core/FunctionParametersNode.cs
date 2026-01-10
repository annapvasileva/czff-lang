namespace Compiler.Parser.AST.Nodes.Core;

public class FunctionParametersNode : AstNode
{
    public record Variable(string Name, TypeAnnotationNode Type);

    public List<Variable> Parameters;
    
    public FunctionParametersNode(List<Variable> parameters)
    {
        Parameters = parameters;
    }
    
    public FunctionParametersNode()
    {
        Parameters = new List<Variable>();
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}