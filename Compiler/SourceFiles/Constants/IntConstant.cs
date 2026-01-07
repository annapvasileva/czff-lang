using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class IntConstant : ConstantItem
{
    public IntConstant(int data) : base(4, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}