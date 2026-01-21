using Compiler.Util;

namespace Compiler.Operations;

public class SerializingVisitor(IList<byte> buffer) : IOperationVisitor
{
    public IList<byte> Buffer { get; } = buffer;

    public void Visit(Ldc operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x1);

        byte[] idx = ByteConverter.IntToU2(operation.ConstantIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Dup operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x2);
    }

    public void Visit(Swap operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x3);
    }

    public void Visit(Store operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x4);
        
        byte[] idx = ByteConverter.IntToU2(operation.VariableIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Ldv operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x5);
        
        byte[] idx = ByteConverter.IntToU2(operation.VariableIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Add operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x6);
    }

    public void Visit(Print operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x7);
    }

    public void Visit(Ret operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x8);
    }

    public void Visit(Halt operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x9);
    }

    public void Visit(Newarr operation)
    {
        Buffer.Add(0);
        Buffer.Add(0xA);
        
        byte[] idx = ByteConverter.IntToU2(operation.DescriptorIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Stelem operation)
    {
        Buffer.Add(0);
        Buffer.Add(0xB);
    }

    public void Visit(Ldelem operation)
    {
        Buffer.Add(0);
        Buffer.Add(0xC);
    }

    public void Visit(Mul operation)
    {
        Buffer.Add(0);
        Buffer.Add(0xD);
    }

    public void Visit(Min operation)
    {
        Buffer.Add(0);
        Buffer.Add(0xE);
    }

    public void Visit(Sub operation)
    {
        Buffer.Add(0);
        Buffer.Add(0xF);
    }

    public void Visit(Div operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x10);
    }

    public void Visit(Call operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x11);
        
        byte[] idx = ByteConverter.IntToU2(operation.FunctionIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Eq operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x12);
    }

    public void Visit(Lt operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x13);
    }

    public void Visit(Leq operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x14);
    }

    public void Visit(Jmp operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x15);
        
        byte[] idx = ByteConverter.IntToU2(operation.JumpIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }
    
    public void Visit(Jz operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x16);
        
        byte[] idx = ByteConverter.IntToU2(operation.JumpIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Jnz operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x17);
        
        byte[] idx = ByteConverter.IntToU2(operation.JumpIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }
    
    public void Visit(Neg operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x18);
    }

    public void Visit(Mod operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x19);
    }

    public void Visit(Lor operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x1A);
    }

    public void Visit(Land operation)
    {
        Buffer.Add(0);
        Buffer.Add(0x1B);
    }
}