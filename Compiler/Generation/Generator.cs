using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Generation;

public class Generator(GeneratorSettings generatorSettings, string target)
{
    private GeneratorSettings _generatorSettings = generatorSettings;
    public string Target { get; set; } = target;

    public Ball Generate()
    {
        throw new NotImplementedException();
    }
}