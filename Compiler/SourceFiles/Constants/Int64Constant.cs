
using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class Int64Constant : ConstantItem
{
    public Int64Constant(long data) : base(8, [])
    {
        var bytes = ByteConverter.IntToI8(data);
        Data = bytes;
    }
}