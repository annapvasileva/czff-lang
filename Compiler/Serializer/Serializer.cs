using Compiler.SourceFiles;

namespace Compiler.Serializer;

public class Serializer
{
    public Ball Source { get; set; }
    public string Target { get; set; }

    public Serializer(Ball source,  string target)
    {
        Source = source;
        Target = target;
    }

    public void Serialize()
    {
        throw new NotImplementedException();
    }
}