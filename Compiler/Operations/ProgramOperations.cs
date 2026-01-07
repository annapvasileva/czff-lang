namespace Compiler.Operations;

public class Add : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "add";
    }
}

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
