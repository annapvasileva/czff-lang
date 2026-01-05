using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.SemanticAnalysis.Models;
using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Generation;

public class Generator
{
    private GeneratorSettings _generatorSettings;

    public Generator(GeneratorSettings generatorSettings)
    {
        _generatorSettings = generatorSettings;
    }
    
    public Ball Generate(AstTree target, SymbolTable symbolTable)
    {
        Header header = new Header(_generatorSettings.Version, 0);
        Ball ball = new Ball(header);
        
        var visitor = new BallGeneratingVisitor(ball, symbolTable);
        target.Root.Accept(visitor);
        
        return ball;
    }
}