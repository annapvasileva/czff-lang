using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class UInt8Constant : ConstantItem
{
    public UInt8Constant(int data) : base(1, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}