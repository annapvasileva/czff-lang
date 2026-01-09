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

public class Halt(int exitValue) : IOperation
{
    public int ExitValue { get; set; } = exitValue;

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


public class Mul : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "mul";
    }
}

public class Min : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "min";
    }
}

public class Sub : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "sub";
    }
}

public class Div : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "div";
    }
}