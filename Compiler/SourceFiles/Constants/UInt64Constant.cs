using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class UInt64Constant : ConstantItem
{
    public UInt64Constant(int data) : base(7, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}