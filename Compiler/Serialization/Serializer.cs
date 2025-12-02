using Compiler.SourceFiles;

namespace Compiler.Serialization;

public class Serializer(Ball source, string target)
{
    public Ball Source { get; set; } = source;
    public string Target { get; set; } = target;

    public void Serialize()
    {
        throw new NotImplementedException();
    }
}