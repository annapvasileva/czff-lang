namespace Compiler.Operations;

public abstract class JumpOperation(int idx) : IOperation
{
    public int JumpIndex { get; set; } = idx;

    public abstract void Accept(IOperationVisitor operationVisitor);

    public abstract string GetString();
}


public class Jmp(int idx) : JumpOperation(idx)
{
    public override void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public override string GetString()
    {
        return "jmp" + JumpIndex;
    }
}

public class Jz(int idx) : JumpOperation(idx)
{
    public override void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public override string GetString()
    {
        return "jmp" + JumpIndex;
    }
}

public class Jnz(int idx) : JumpOperation(idx)
{
    public override void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public override string GetString()
    {
        return "jnz" + JumpIndex;
    }
}