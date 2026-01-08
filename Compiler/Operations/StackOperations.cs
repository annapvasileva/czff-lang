namespace Compiler.Operations;

public class Ldc(int constantIndex) : IOperation
{
    public int ConstantIndex { get; } = constantIndex;

    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "ldc" + ConstantIndex;
    }
}

public class Store(int variableIndex) : IOperation
{
    public int VariableIndex { get; } = variableIndex;

    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "store" + VariableIndex;
    }
}

public class Ldv(int variableIndex) : IOperation
{
    public int VariableIndex { get; } = variableIndex;

    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "ldv" + VariableIndex;
    }
}

public class Swap : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "swap";
    }
}

public class Dup : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "dup";
    }
}

public class Stelem : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "stelem";
    }
}

public class Ldelem : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "ldelem";
    }
}

public class Newarr(int descriptorIndex) : IOperation
{
    public int DescriptorIndex { get; } = descriptorIndex;
    
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }

    public string GetString()
    {
        return "newarr";
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