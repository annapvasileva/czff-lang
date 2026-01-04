using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Parser.AST.Nodes;

public class ProgramNode : AstNode
{
    public IList<FunctionDeclarationNode> Functions { get; }

    public ProgramNode(IList<FunctionDeclarationNode> functions)
    {
        Functions = functions;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}