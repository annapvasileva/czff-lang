using Compiler.SourceFiles;

namespace Compiler.Serialization.Handlers;

public abstract class Handler
{
    protected Handler? Next;
    
    public abstract void Handle(Ball source, IList<byte> target);

    public Handler AddNextHandler(Handler next)
    {
        if (Next == null)
        {
            Next = next;
        }
        else
        {
            Next.AddNextHandler(next);
        }

        return this;
    }
}