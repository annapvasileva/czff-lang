using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class UInt128Constant : ConstantItem
{
    public UInt128Constant(int data) : base(9, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}