using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Serialization.Handlers;

public class ConstantsHandler: Handler
{
    public override void Handle(Ball source, IList<byte> target)
    {
        ConstantPool pool = source.ConstantPool;
        byte[] length = ByteConverter.IntToU2(pool.Length);
        
        target.Add(length[1]);
        target.Add(length[0]);
        
        IList<ConstantItem> items = pool.GetConstants();
        
        foreach (var constant in items)
        {
            target.Add(constant.Tag);
            foreach (var v in constant.Data)
            {
                target.Add(v);
            }
        }
        
        Next?.Handle(source, target);
    }
}