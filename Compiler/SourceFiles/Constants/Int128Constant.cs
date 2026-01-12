using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class Int128Constant : ConstantItem
{
    public Int128Constant(int data) : base(10, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}