namespace Compiler.Parser.AST;

public class AstTree
{
    public AstNode Root { get; private set; }

    public AstTree(AstNode root)
    {
        Root = root;
    }
}