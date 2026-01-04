namespace Compiler.Operations;

public interface IOperation
{
    public void Accept(IOperationVisitor operationVisitor);
}