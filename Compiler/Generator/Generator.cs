using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Compiler;

public class Generator
{
    private GeneratorSettings _generatorSettings;
    public string Target { get; set; }

    public Generator(GeneratorSettings generatorSettings, string target)
    {
        _generatorSettings = generatorSettings;
        Target = target;
    }

    public Ball Generate()
    {
        throw new NotImplementedException();
    }
}