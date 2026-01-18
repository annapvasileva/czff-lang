using Compiler.CompilerPipeline;
using Compiler.Generation;
using Compiler.Lexer;
using Compiler.Optimizations;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;
using Compiler.Serialization;
using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Tests.TaskTests;

public class PipelineTests
{
    [Theory]
    [ClassData(typeof(PipelineTestsData))]
    public void FactorialTest(string sourceText)
    {
        var lexer = new CompilerLexer(sourceText);
        IEnumerable<Token> tokens = lexer.GetTokens();
        var parser = new CompilerParser(tokens.ToList());
        AstTree ast = parser.Parse();
        
        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
            new DeadCodeEliminationOptimizer(symbolTableBuilder.SymbolTable),
            new DeadCodeEliminationSecondStage(symbolTableBuilder.SymbolTable),
            new SymbolTableBuilder(),
        };
        Pipeline.Run(ast, pipelineUnits);

        SymbolTable scope = symbolTableBuilder.SymbolTable;
        
        var generator = new Generator(new CompilerSettings());
        
        Ball ball = generator.Generate(ast, scope);
        var serializer = new Serializer();
        var exception = Record.Exception(() => serializer.SerializeToArray(ball));
        Assert.Null(exception);
    }
}