using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.SemanticAnalysis;
using Compiler.SourceFiles;
using Compiler.SourceFiles.Constants;

namespace Compiler.Util.LiteralTranslation;

public class NativeInteger(int value) : INativeType
{
    private int _value = value;

    public LiteralExpressionNode GetExpressionView()
    {
        return new LiteralExpressionNode(_value.ToString(), LiteralType.IntegerLiteral);
    }

    public ConstantItem GetConstantView()
    {
        return new IntConstant(_value);
    }

    public void Add(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't add int and not int.");

        _value += right._value;
    }

    public void Subtract(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't subtract int and not int.");

        _value -= right._value;
    }

    public void Multiply(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't multiply int and not int.");

        _value *= right._value;
    }

    public void Divide(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't divide int and not int.");

        if (right._value == 0)
            throw new NativeTypeException("Division by zero.");

        _value /= right._value;
    }

    public void Mod(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't mod int and not int.");

        if (right._value == 0)
            throw new NativeTypeException("Division by zero.");

        _value %= right._value;
    }

    public void Less(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't compare int and not int.");

        _value = _value < right._value ? 1 : 0;
    }

    public void LessOrEqual(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't compare int and not int.");

        _value = _value <= right._value ? 1 : 0;
    }

    public void Equal(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't compare int and not int.");

        _value = _value == right._value ? 1 : 0;
    }

    public void Greater(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't compare int and not int.");

        _value = _value > right._value ? 1 : 0;
    }

    public void GreaterOrEqual(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't compare int and not int.");

        _value = _value >= right._value ? 1 : 0;
    }

    public void NotEqual(INativeType operand)
    {
        if (operand is not NativeInteger right)
            throw new NativeTypeException("Can't compare int and not int.");

        _value = _value != right._value ? 1 : 0;
    }

    public void LogicalOr(INativeType operand)
    {
        throw new NativeTypeException("Can't apply logical OR to int.");
    }

    public void LogicalAnd(INativeType operand)
    {
        throw new NativeTypeException("Can't apply logical AND to int.");
    }
}
