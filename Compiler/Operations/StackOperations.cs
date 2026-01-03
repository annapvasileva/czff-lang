namespace Compiler.Operations;

public class Ldc(int constantIndex) : IOperation
{
    public int ConstantIndex { get; } = constantIndex;

    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }
}

public class Store(int variableIndex) : IOperation
{
    public int VariableIndex { get; } = variableIndex;

    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }
}

public class Ldv(int variableIndex) : IOperation
{
    public int VariableIndex { get; } = variableIndex;

    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }
}

public class Swap : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }
}

public class Dup : IOperation
{
    public void Accept(IOperationVisitor operationVisitor)
    {
        operationVisitor.Visit(this);
    }
}
