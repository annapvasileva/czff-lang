using Compiler.SourceFiles;

namespace Compiler.Serialization.Handlers;

public class HeaderHandler: Handler
{
    public override void Handle(Ball source, IList<byte> target)
    {
        foreach (var v in source.Header.MagicalNumber)
        {
            target.Add(v);
        }
        
        foreach (var v in source.Header.Version)
        {
            target.Add(v);
        }
        
        target.Add(source.Header.Flags);

        Next?.Handle(source, target);
    }
}