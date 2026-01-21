using Compiler.Parser;
using Compiler.Parser.AST;

namespace Compiler.CompilerPipeline;

public class Pipeline
{
    public static void Run(AstTree astTree, IList<INodeVisitor> units)
    {
        foreach (var nodeVisitor in units)
        {
            astTree.Root.Accept(nodeVisitor);
        }
    }
}