using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class UIntConstant : ConstantItem
{
    public UIntConstant(int data) : base(3, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}