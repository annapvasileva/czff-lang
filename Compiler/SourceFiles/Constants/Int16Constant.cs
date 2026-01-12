using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class Int16Constant : ConstantItem
{
    public Int16Constant(int data) : base(5, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}