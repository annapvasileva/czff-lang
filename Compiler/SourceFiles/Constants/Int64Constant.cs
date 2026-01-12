
using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class Int64Constant : ConstantItem
{
    public Int64Constant(int data) : base(8, [])
    {
        var bytes = ByteConverter.IntToI4(data);
        Data = bytes;
    }
}