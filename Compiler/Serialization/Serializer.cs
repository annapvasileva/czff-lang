using Compiler.Serialization.Handlers;
using Compiler.SourceFiles;

namespace Compiler.Serialization;

public class Serializer(Ball source, string target)
{
    public Ball Source { get; set; } = source;
    public string Target { get; set; } = target;

    private Handler _handlers = new HeaderHandler()
        .AddNextHandler(new ConstantsHandler());
    
    public void Serialize()
    {
        var result = new List<byte>();
        _handlers.Handle(Source, result);
        
        File.WriteAllBytes(Target, result.ToArray());
    }
}