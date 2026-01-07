using Compiler.Util;

namespace Compiler.Operations;

public class SerializingVisitor(IList<byte> buffer) : IOperationVisitor
{
    public IList<byte> Buffer { get; } = buffer;

    public void Visit(Ldc operation)
    {
        Buffer.Add(0);
        Buffer.Add(1);

        byte[] idx = ByteConverter.IntToU2(operation.ConstantIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Dup operation)
    {
        Buffer.Add(0);
        Buffer.Add(2);
    }

    public void Visit(Swap operation)
    {
        Buffer.Add(0);
        Buffer.Add(3);
    }

    public void Visit(Store operation)
    {
        Buffer.Add(0);
        Buffer.Add(4);
        
        byte[] idx = ByteConverter.IntToU2(operation.VariableIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Ldv operation)
    {
        Buffer.Add(0);
        Buffer.Add(5);
        
        byte[] idx = ByteConverter.IntToU2(operation.VariableIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Add operation)
    {
        Buffer.Add(0);
        Buffer.Add(6);
    }

    public void Visit(Print operation)
    {
        Buffer.Add(0);
        Buffer.Add(7);
    }

    public void Visit(Ret operation)
    {
        Buffer.Add(0);
        Buffer.Add(8);
    }

    public void Visit(Halt operation)
    {
        Buffer.Add(0);
        Buffer.Add(9);
        
        byte[] code = ByteConverter.IntToU2(operation.ExitValue);
        Buffer.Add(code[0]);
        Buffer.Add(code[1]);
    }

    public void Visit(Newarr operation)
    {
        Buffer.Add(0);
        Buffer.Add(10);
        
        byte[] idx = ByteConverter.IntToU2(operation.DescriptorIndex);
        Buffer.Add(idx[0]);
        Buffer.Add(idx[1]);
    }

    public void Visit(Stelem operation)
    {
        Buffer.Add(0);
        Buffer.Add(11);
    }

    public void Visit(Ldelem operation)
    {
        Buffer.Add(0);
        Buffer.Add(12);
    }

    public void Visit(Mul operation)
    {
        Buffer.Add(0);
        Buffer.Add(13);
    }

    public void Visit(Min operation)
    {
        Buffer.Add(0);
        Buffer.Add(14);
    }

    public void Visit(Sub operation)
    {
        Buffer.Add(0);
        Buffer.Add(15);
    }

    public void Visit(Div operation)
    {
        Buffer.Add(0);
        Buffer.Add(16);
    }
}