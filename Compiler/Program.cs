using Compiler.Util;
using Compiler.Generation;
using Compiler.Serialization;
using Compiler.SourceFiles;

using Newtonsoft.Json;
using CommandLine;
using Compiler.Lexer;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;

namespace Compiler;

internal class Options
{
    [Value(0, Required = true, HelpText = "Path to file with code, that will be compiled into bytecode")]
    public string Source { get; set; } = "";

    [Option('t', "target", Required = false, HelpText = "Path to the source file", Default = null)]
    public string? Target { get; set; }
}

internal abstract class Program
{
    public static void Main(string[] args)
    {
//------------------------------------------------------------------------------------------
//                                   Controller Part
//------------------------------------------------------------------------------------------

        string source = "";
        string? target = null;
        
        CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options =>
            {
                source = options.Source;
                target = options.Target;
            })
            .WithNotParsed(errors =>
            {
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
            });

        target ??= AppContext.BaseDirectory;
        
//------------------------------------------------------------------------------------------
//                                   Configuration Part
//------------------------------------------------------------------------------------------

        string projectRoot = Directory.GetParent( // project/bin/debug/net10/ <- is being started here. Need to do normal path later
            Directory.GetParent(Directory.GetParent(
                    Directory.GetParent(
                        AppDomain.CurrentDomain.BaseDirectory
                    )!.FullName
                )!.FullName
            )!.FullName
        )!.FullName;

        string configPath = Path.Combine(projectRoot, "GeneratorConfig.json");

        string json = File.ReadAllText(configPath);

        GeneratorSettings? generatorSettings = JsonConvert.DeserializeObject<GeneratorSettings>(json);
        if (generatorSettings == null) {
            Console.WriteLine("No generator settings found");
            return;
        }
        
//------------------------------------------------------------------------------------------
//                                   Pipeline Part
//                         Need to add Lexer, Parser, Analyzer
//------------------------------------------------------------------------------------------
        var lexer = new CompilerLexer(
            """
            =/
            Our first simple program on CZFF 
            /=
            func void Main() {
                var int a = 2;
                var int b = 3;
                var int res = a + b;
                print res;
            }
            """
            );

        IEnumerable<Token> tokens = lexer.GetTokens();
        
        var parser = new CompilerParser(tokens.ToList());
        AstTree ast = parser.Parse();

        var analyzer = new SymbolTableBuilder();
        ast.Root.Accept(analyzer);

        SymbolTable scope = analyzer.SymbolTable;
        
        var generator = new Generator(generatorSettings); // instead of string argument there will be AST from Parser
        
        Ball ball = generator.Generate(ast, scope);
        
        var serializer = new Serializer();
        serializer.Serialize(ball, target);
    }
}

