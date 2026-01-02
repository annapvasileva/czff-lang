using Compiler.Parser.AST.Nodes.Core;

namespace Compiler.Parser.AST.Nodes.Statements;

public class FunctionDeclarationNode : DeclarationNode
{
    // где будет void?
    public TypeAnnotationNode ReturnType { get; }
    public IdentifierNode Identifier { get; }
    public IList<FunctionParameterNode> Parameters { get; }
    public BlockNode Body { get; }

    public FunctionDeclarationNode(TypeAnnotationNode returnType, IdentifierNode identifier,
        IList<FunctionParameterNode> parameters, BlockNode body)
    {
        ReturnType = returnType;
        Identifier = identifier;
        Parameters = parameters;
        Body = body;
    }
    
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitFunctionDeclarationNode(this);
    }
}