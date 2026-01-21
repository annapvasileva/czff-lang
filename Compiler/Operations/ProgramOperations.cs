namespace Compiler.Operations;

public class Ret : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "ret";
    }
}

public class Halt : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "halt";
    }
}

public class Print : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "print";
    }
}

public class Call(int functionIndex) : IOperation
{
    public int FunctionIndex { get; set; } = functionIndex;
    
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "call " + FunctionIndex;
    }
}

