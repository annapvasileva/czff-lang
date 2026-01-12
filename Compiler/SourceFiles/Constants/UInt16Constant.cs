using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class UInt16Constant : ConstantItem
{
    public UInt16Constant(int data) : base(2, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}