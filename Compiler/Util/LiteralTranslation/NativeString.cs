using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.SemanticAnalysis;
using Compiler.SourceFiles;
using Compiler.SourceFiles.Constants;

namespace Compiler.Util.LiteralTranslation;

public class NativeString(string value) : INativeType
{
    private string _value = value;

    public LiteralExpressionNode GetExpressionView()
    {
        return new LiteralExpressionNode(_value, LiteralType.StringLiteral);
    }

    public ConstantItem GetConstantView()
    {
        return new StringConstant(_value);
    }

    public void Add(INativeType operand)
    {
        if (operand is not NativeString right)
            throw new NativeTypeException("Can't concat string and not string.");

        _value += right._value;
    }

    public void Subtract(INativeType operand)
    {
        throw new NativeTypeException("Can't subtract strings.");
    }

    public void Multiply(INativeType operand)
    {
        throw new NativeTypeException("Can't multiply strings.");
    }

    public void Divide(INativeType operand)
    {
        throw new NativeTypeException("Can't divide strings.");
    }

    public void Mod(INativeType operand)
    {
        throw new NativeTypeException("Can't mod strings.");
    }

    public void Minus()
    {
        throw new NativeTypeException("Can't apply arithmetics to strings");
    }

    public void Negative()
    {
        throw new NativeTypeException("Can't apply logics to strings.");
    }

    public void Less(INativeType operand)
    {
        if (operand is not NativeString right)
            throw new NativeTypeException("Can't compare string and not string.");

        _value = string.Compare(_value, right._value) < 0 ? "true" : "false";
    }

    public void LessOrEqual(INativeType operand)
    {
        if (operand is not NativeString right)
            throw new NativeTypeException("Can't compare string and not string.");

        _value = string.Compare(_value, right._value) <= 0 ? "true" : "false";
    }

    public void Equal(INativeType operand)
    {
        if (operand is not NativeString right)
            throw new NativeTypeException("Can't compare string and not string.");

        _value = _value == right._value ? "true" : "false";
    }

    public void Greater(INativeType operand)
    {
        if (operand is not NativeString right)
            throw new NativeTypeException("Can't compare string and not string.");

        _value = string.Compare(_value, right._value) > 0 ? "true" : "false";
    }

    public void GreaterOrEqual(INativeType operand)
    {
        if (operand is not NativeString right)
            throw new NativeTypeException("Can't compare string and not string.");

        _value = string.Compare(_value, right._value) >= 0 ? "true" : "false";
    }

    public void NotEqual(INativeType operand)
    {
        if (operand is not NativeString right)
            throw new NativeTypeException("Can't compare string and not string.");

        _value = _value != right._value ? "true" : "false";
    }

    public void LogicalOr(INativeType operand)
    {
        throw new NativeTypeException("Can't apply logical OR to string.");
    }

    public void LogicalAnd(INativeType operand)
    {
        throw new NativeTypeException("Can't apply logical AND to string.");
    }
}
