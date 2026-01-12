using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class BoolConstant : ConstantItem
{
    public BoolConstant(int data) : base(12, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}