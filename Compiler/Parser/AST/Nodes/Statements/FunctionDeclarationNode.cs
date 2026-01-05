using Compiler.Parser.AST.Nodes.Core;

namespace Compiler.Parser.AST.Nodes.Statements;

public class FunctionDeclarationNode : DeclarationNode
{
    public TypeAnnotationNode ReturnType { get; }
    public string Name { get; }
    public FunctionParametersNode Parameterses { get; }
    public BlockNode Body { get; }

    public FunctionDeclarationNode(TypeAnnotationNode returnType, string name,
        FunctionParametersNode parameterses, BlockNode body)
    {
        ReturnType = returnType;
        Name = name;
        Parameterses = parameterses;
        Body = body;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}