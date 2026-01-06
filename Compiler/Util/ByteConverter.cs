namespace Compiler.Util;

public static class ByteConverter
{
    public static byte[] IntToU2(int value)
    {
        UInt16 value16 = (UInt16)value;
        byte[] result = [(byte)(value16 >> 8 % 256), (byte)(value16 % 256)];

        return result;
    }

    public static byte[] IntToI4(int value)
    {
        byte[] result = [(byte)(value >> 24 % 256),(byte)(value >> 16 % 256),(byte)(value >> 8 % 256), (byte)(value % 256)];
        return result;
    }
}