using Compiler.Util;

namespace Compiler.SourceFiles.Constants;

public class StringConstant : ConstantItem
{
    public StringConstant(string line) : base(11, [])
    {
        int size = line.Length;
        Data = new byte[2 + size];

        byte[] bytes = ByteConverter.IntToU2(size);
        Data[0] =  bytes[0];
        Data[1] =  bytes[1];
        for (int i = 0; i < size; i++)
        {
            Data[i + 2] = (byte)line[i];
        }
    }
}