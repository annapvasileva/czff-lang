using Compiler.Operations;
using Compiler.Serialization.Handlers;
using Compiler.SourceFiles;

namespace Compiler.Serialization;

public class Serializer
{
    private List<byte> _buffer;

    private IOperationVisitor visitor;

    private Handler _handlers;
    
    public Serializer()
    {
        _buffer = new List<byte>();
        visitor = new SerializingVisitor(_buffer);
        _handlers = new HeaderHandler()
            .AddNextHandler(new ConstantsHandler())
            .AddNextHandler(new ClassesHandler(visitor));
    }
    
    public void Serialize(Ball source, string target)
    {
        _buffer.Clear();
        _handlers.Handle(source, _buffer);
        
        File.WriteAllBytes(target, _buffer.ToArray());
    }
}