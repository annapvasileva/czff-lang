using Compiler.Util;
using Compiler.Compiler;
using Compiler.Serializer;
using Compiler.SourceFiles;

using Newtonsoft.Json;
using CommandLine;

class Options
{
    [Value(0, Required = true, HelpText = "Path to file with code, that will be compiled into bytecode")]
    public string Source { get; set; } = "";

    [Option('t', "target", Required = false, HelpText = "Path to the source file", Default = null)]
    public string? Target { get; set; }
}

internal class Program
{
    public static void Main(string[] args)
    {
//------------------------------------------------------------------------------------------
//                                   Controller Part
//------------------------------------------------------------------------------------------

        string source = "";
        string? target = null;
        
        Parser.Default.ParseArguments<Options>(args)
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

        if (target == null)
        {
            target = AppContext.BaseDirectory;
        }
        
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

        var generator = new Generator(generatorSettings, ""); // instead of string argument there will be AST from Parser
        
        Ball ball = generator.Generate();;
        
        var serializer = new Serializer(ball, target);
        serializer.Serialize();
    }
}

