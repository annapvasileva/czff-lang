using Compiler.Parser.AST.Nodes.Core;

namespace Compiler.Parser.AST.Nodes.Statements;

public class FunctionDeclarationNode : DeclarationNode
{
    public TypeAnnotationNode ReturnType { get; }
    public string Name { get; }
    public FunctionParametersNode Parameters { get; }
    public BlockNode Body { get; }
    public int LocalsLength { get; set;  }

    public FunctionDeclarationNode(TypeAnnotationNode returnType, string name,
        FunctionParametersNode parameters, BlockNode body)
    {
        ReturnType = returnType;
        Name = name;
        Parameters = parameters;
        Body = body;
        LocalsLength = 0;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}