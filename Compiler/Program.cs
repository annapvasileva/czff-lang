using System.Data;
using Compiler.Util;
using Compiler.Generation;
using Compiler.Serialization;
using Compiler.SourceFiles;

using Newtonsoft.Json;
using CommandLine;
using Compiler.Lexer;
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

    [Option('c', "config", Required = true, HelpText = "Path to config file")]
    public string PathToConfig { get; set; } = null!;

    [Option('t', "target", Required = true, HelpText = "Path to the source file")]
    public string Target { get; set; } = null!;

    [Option('m', "multiple-files", Required = false, Default = false, HelpText = "Search all" +
        " .szff files in the directory of the target. Option for code with dependencies from external source.")] 
    public bool MultipleFiles { get; set; }
}

internal abstract class Program
{
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
        
        if (!File.Exists(options.PathToConfig) || !File.Exists(options.Source) || !Directory.Exists(Path.GetDirectoryName(options.Target)))
        {
            throw new Exception("The specified file or directory doesn't exist");
        }
        
        string json = File.ReadAllText(options.PathToConfig);

        CompilerSettings? compilerSettings = JsonConvert.DeserializeObject<CompilerSettings>(json);
        if (compilerSettings == null) {
            Console.WriteLine("No generator settings found");
            return 1;
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

        var analyzer = new SymbolTableBuilder();
        ast.Root.Accept(analyzer);

        SymbolTable scope = analyzer.SymbolTable;
        
        var generator = new Generator(compilerSettings); // instead of string argument there will be AST from Parser
        
        Ball ball = generator.Generate(ast, scope);
        
        var serializer = new Serializer();
        serializer.Serialize(ball, options.Target);

        return 0;
    }
}

