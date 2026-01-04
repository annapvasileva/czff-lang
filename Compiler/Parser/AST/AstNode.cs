namespace Compiler.Parser.AST;

public abstract class AstNode
{
    public abstract void Accept(INodeVisitor visitor);
}