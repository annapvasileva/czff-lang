namespace Compiler.Operations;

public interface IOperationVisitor
{
    public void Visit(Ldc operation);
    public void Visit(Dup operation);
    public void Visit(Swap operation);
    public void Visit(Store operation);
    public void Visit(Ldv operation);
    public void Visit(Add operation);
    public void Visit(Print operation);
    public void Visit(Ret operation);
    public void Visit(Halt operation);
    public void Visit(Newarr operation);
    public void Visit(Stelem operation);
    public void Visit(Ldelem operation);
    public void Visit(Mul operation);
    public void Visit(Min operation);
    public void Visit(Sub operation);
    public void Visit(Div operation);
    public void Visit(Call operation);
    public void Visit(Eq operation);
    public void Visit(Lt operation);
    public void Visit(Leq operation);
    public void Visit(Jmp operation);
    public void Visit(Jz operation);
    public void Visit(Jnz operation);
    public void Visit(Neg operation);
}