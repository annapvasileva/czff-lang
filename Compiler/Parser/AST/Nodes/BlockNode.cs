using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis.Models;

namespace Compiler.Parser.AST.Nodes;

public class BlockNode : AstNode
{
    public IList<StatementNode> Statements { get; set; }
    public SymbolTable Scope { get; set; }

    public BlockNode(IList<StatementNode> statements)
    {
        Statements = statements;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}