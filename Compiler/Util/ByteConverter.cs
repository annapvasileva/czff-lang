namespace Compiler.Util;

public static class ByteConverter
{
    // ---------- Unsigned ----------

    public static byte[] IntToU1(int value)
    {
        return new[]
        {
            (byte)(value & 0xFF)
        };
    }

    public static byte[] IntToU2(int value)
    {
        ushort v = (ushort)value;
        return new[]
        {
            (byte)((v >> 8) & 0xFF),
            (byte)(v & 0xFF)
        };
    }

    public static byte[] IntToU4(int value)
    {
        uint v = (uint)value;
        return new[]
        {
            (byte)((v >> 24) & 0xFF),
            (byte)((v >> 16) & 0xFF),
            (byte)((v >> 8) & 0xFF),
            (byte)(v & 0xFF)
        };
    }

    public static byte[] IntToU8(long value)
    {
        ulong v = (ulong)value;
        return new[]
        {
            (byte)((v >> 56) & 0xFF),
            (byte)((v >> 48) & 0xFF),
            (byte)((v >> 40) & 0xFF),
            (byte)((v >> 32) & 0xFF),
            (byte)((v >> 24) & 0xFF),
            (byte)((v >> 16) & 0xFF),
            (byte)((v >> 8) & 0xFF),
            (byte)(v & 0xFF)
        };
    }

    public static byte[] IntToI1(int value)
    {
        return new[]
        {
            unchecked((byte)value)
        };
    }

    public static byte[] IntToI2(int value)
    {
        short v = (short)value;
        return new[]
        {
            (byte)((v >> 8) & 0xFF),
            (byte)(v & 0xFF)
        };
    }

    public static byte[] IntToI4(int value)
    {
        return new[]
        {
            (byte)((value >> 24) & 0xFF),
            (byte)((value >> 16) & 0xFF),
            (byte)((value >> 8) & 0xFF),
            (byte)(value & 0xFF)
        };
    }

    public static byte[] IntToI8(long value)
    {
        return new[]
        {
            (byte)((value >> 56) & 0xFF),
            (byte)((value >> 48) & 0xFF),
            (byte)((value >> 40) & 0xFF),
            (byte)((value >> 32) & 0xFF),
            (byte)((value >> 24) & 0xFF),
            (byte)((value >> 16) & 0xFF),
            (byte)((value >> 8) & 0xFF),
            (byte)(value & 0xFF)
        };
    }
}
