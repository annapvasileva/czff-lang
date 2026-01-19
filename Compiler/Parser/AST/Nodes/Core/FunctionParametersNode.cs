using System.Text.Json.Serialization;

namespace Compiler.Parser.AST.Nodes.Core;

public class FunctionParametersNode : AstNode
{
    public record Variable(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] TypeAnnotationNode Type);

    [JsonPropertyName("parameters")]
    public List<Variable> Parameters { get; }
    
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