namespace Compiler.Util;

public static class ByteConverter
{
    public static byte[] IntToU2(int value)
    {
        UInt16 value16 = (UInt16)value;
        byte[] result = [(byte)(value16 / 8 % 8), (byte)(value16 % 8)];

        return result;
    }
}