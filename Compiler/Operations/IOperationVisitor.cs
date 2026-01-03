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
    public void Visit(Halt operation);
}