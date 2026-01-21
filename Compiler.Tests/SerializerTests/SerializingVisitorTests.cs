using Compiler.Operations;
using Compiler.Util;

namespace Compiler.Tests.SerializerTests;

public class SerializingVisitorTests
{
    public static IEnumerable<object[]> NoArgOpcodes()
    {
        yield return new object[] { new Dup(),  0x02 };
        yield return new object[] { new Swap(), 0x03 };
        yield return new object[] { new Add(),  0x06 };
        yield return new object[] { new Print(),0x07 };
        yield return new object[] { new Ret(),  0x08 };
        yield return new object[] { new Halt(), 0x09 };
        yield return new object[] { new Stelem(),0x0B };
        yield return new object[] { new Ldelem(),0x0C };
        yield return new object[] { new Mul(),  0x0D };
        yield return new object[] { new Min(),  0x0E };
        yield return new object[] { new Sub(),  0x0F };
        yield return new object[] { new Div(),  0x10 };
        yield return new object[] { new Eq(),   0x12 };
        yield return new object[] { new Lt(),   0x13 };
        yield return new object[] { new Leq(),  0x14 };
        yield return new object[] { new Neg(),  0x18 };
        yield return new object[] { new Mod(),  0x19 };
        yield return new object[] { new Lor(),  0x1A };
        yield return new object[] { new Land(), 0x1B };
    }

    public static IEnumerable<object[]> U2ArgOpcodes()
    {
        yield return new object[] { new Ldc(42),    0x01, 42 };
        yield return new object[] { new Store(7),   0x04, 7 };
        yield return new object[] { new Ldv(9),     0x05, 9 };
        yield return new object[] { new Newarr(3), 0x0A, 3 };
        yield return new object[] { new Call(5),    0x11, 5 };
        yield return new object[] { new Jmp(100),          0x15, 100 };
        yield return new object[] { new Jz(200),           0x16, 200 };
        yield return new object[] { new Jnz(255),          0x17, 255 };
    }

    
    private static byte[] U2(int value) => ByteConverter.IntToU2(value);

    [Theory]
    [MemberData(nameof(NoArgOpcodes))]
    public void Visit_NoArgOpcode_WritesCorrectBytes(IOperation operation, byte opcode)
    {
        var buffer = new List<byte>();
        var visitor = new SerializingVisitor(buffer);

        operation.Accept(visitor);

        Assert.Equal(new byte[]
        {
            0x00, opcode
        }, buffer);
    }

    [Theory]
    [MemberData(nameof(U2ArgOpcodes))]
    public void Visit_U2Opcode_WritesCorrectBytes(IOperation operation, byte opcode, int arg)
    {
        var buffer = new List<byte>();
        var visitor = new SerializingVisitor(buffer);

        operation.Accept(visitor);

        var idx = U2(arg);

        Assert.Equal(new byte[]
        {
            0x00, opcode,
            idx[0], idx[1]
        }, buffer);
    }
}

