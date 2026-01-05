namespace Compiler.Parser.AST.Nodes.Core;

public class FunctionParametersNode : AstNode
{
    public record Variable
    {
        public string Name { get; }
        
        public TypeAnnotationNode Type { get; }
    }

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