using Compiler.Util;
using Compiler.Generation;
using Compiler.Serialization;
using Compiler.SourceFiles;

using Newtonsoft.Json;
using CommandLine;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler;

internal abstract class Options
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
        var expectedAst = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(){ },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "a",
                                new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new LiteralExpressionNode("3", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "res",
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("a"),
                                    new IdentifierExpressionNode("b"),
                                    BinaryOperatorType.Addition)),
                            new PrintStatementNode(new IdentifierExpressionNode("res"))
                        })
                )
            }));

        var generator = new Generator(generatorSettings); // instead of string argument there will be AST from Parser
        
        // Ball ball = generator.Generate(expectedAst);
        
        var serializer = new Serializer();
        // serializer.Serialize(ball, target);
    }
}

