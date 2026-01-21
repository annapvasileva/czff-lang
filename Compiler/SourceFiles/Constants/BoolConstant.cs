using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class BoolConstant : ConstantItem
{
    public BoolConstant(bool data) : base(12, [])
    {
        byte[] bytes;
        if (data)
        {
            bytes = [1];
        }
        else
        {
            bytes = [0];
        }
        
        Data = bytes;
    }
}