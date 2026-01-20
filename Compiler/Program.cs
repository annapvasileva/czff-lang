
using Compiler.Util;
using Compiler.Generation;
using Compiler.Serialization;
using Compiler.SourceFiles;

using Newtonsoft.Json;
using CommandLine;
using Compiler.CompilerPipeline;
using Compiler.Lexer;
using Compiler.Optimizations;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;

namespace Compiler;


internal class Options
{
    [Option('s', "source", Required = true, HelpText = "Path to file with code," +
                                                       " that will be compiled into bytecode")]
    public string Source { get; set; } = null!;
    
    [Option('t', "target", Required = true, HelpText = "Path to the source file")]
    public string Target { get; set; } = null!;

    [Option('f', "use-cf", Required=false, HelpText = "Do not use constant folding optimization")]
    public bool ConstantFolding { get; set; } = false;
    
    [Option('d', "use-dce", Required=false, HelpText = "Do not use dead code elimination optimization")]
    public bool DeadCodeElimination { get; set; } = false;
}

internal abstract class Program
{
    private static readonly byte[] Version = [0, 0, 5];

    public static void Main(string[] args)
    {
        //------------------------------------------------------------------------------------------
        //                                   Controller Part
        //------------------------------------------------------------------------------------------
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .MapResult(
                Run,
                _ => 1
            );
    }

    public static int Run(Options options)
    { 
        //------------------------------------------------------------------------------------------
        //                                   Configuration Part
        //------------------------------------------------------------------------------------------
        
        if (!File.Exists(options.Source) || !Directory.Exists(Path.GetDirectoryName(options.Target)))
        {
            throw new Exception("The specified file or directory doesn't exist");
        }
        
        string sourceText = File.ReadAllText(options.Source);
        
        //------------------------------------------------------------------------------------------
        //                                   Pipeline Part
        //                         Need to add Lexer, Parser, Analyzer
        //------------------------------------------------------------------------------------------
        
        var lexer = new CompilerLexer(sourceText);

        IEnumerable<Token> tokens = lexer.GetTokens();
        
        var parser = new CompilerParser(tokens.ToList());
        AstTree ast = parser.Parse();

        
        var symbolTableBuilder = new SymbolTableBuilder();
        var pipelineUnits = new List<INodeVisitor>()
        {
            symbolTableBuilder,
            new SemanticAnalyzer(symbolTableBuilder.SymbolTable),
        };
        if (options.ConstantFolding)
        {
            pipelineUnits.Add(new ConstantFoldingOptimizer());
        }

        if (options.DeadCodeElimination)
        {
            
            pipelineUnits.Add(new DeadCodeEliminationOptimizer(symbolTableBuilder.SymbolTable));
            pipelineUnits.Add(new DeadCodeEliminationSecondStage(symbolTableBuilder.SymbolTable));
            
            symbolTableBuilder = new SymbolTableBuilder();
            pipelineUnits.Add(symbolTableBuilder);
        }
        
        Pipeline.Run(ast, pipelineUnits);

        SymbolTable scope = symbolTableBuilder.SymbolTable;
        
        var generator = new Generator(Version);
        
        Ball ball = generator.Generate(ast, scope);
        
        var serializer = new Serializer();
        serializer.Serialize(ball, options.Target);

        return 0;
    }
}

