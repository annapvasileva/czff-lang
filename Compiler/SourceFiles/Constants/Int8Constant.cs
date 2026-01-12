using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class Int8Constant : ConstantItem
{
    public Int8Constant(int data) : base(4, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}