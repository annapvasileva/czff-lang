using Compiler.Parser.AST.Nodes.Core;

namespace Compiler.Parser.AST.Nodes.Statements;

public class FunctionDeclarationNode : DeclarationNode
{
    public TypeAnnotationNode ReturnType { get; }
    public string Name { get; }
    public IList<FunctionParameterNode> Parameters { get; }
    public BlockNode Body { get; }

    public FunctionDeclarationNode(TypeAnnotationNode returnType, string name,
        IList<FunctionParameterNode> parameters, BlockNode body)
    {
        ReturnType = returnType;
        Name = name;
        Parameters = parameters;
        Body = body;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}