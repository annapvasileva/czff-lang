using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.SemanticAnalysis;
using Compiler.SourceFiles;
using Compiler.SourceFiles.Constants;

namespace Compiler.Util.LiteralTranslation;

public class NativeBool(bool value) : INativeType
{
    private bool _value = value;
    
    public LiteralExpressionNode GetExpressionView()
    {
        string value = _value ? "true" : "false";
        return new LiteralExpressionNode(value, LiteralType.BooleanLiteral);
    }

    public ConstantItem GetConstantView()
    {
        return new BoolConstant(_value);
    }

    public void Add(INativeType operand)
    {
        throw new NativeTypeException("Can't apply arithmetics to bool.");
    }

    public void Subtract(INativeType operand)
    {
        throw new NativeTypeException("Can't apply arithmetics to bool.");
    }

    public void Multiply(INativeType operand)
    {
        throw new NativeTypeException("Can't apply arithmetics to bool.");
    }

    public void Divide(INativeType operand)
    {
        throw new NativeTypeException("Can't apply arithmetics to bool.");
    }

    public bool Less(INativeType operand)
    {  
        throw new NativeTypeException("Can't apply equations to bool.");
    }

    public bool LessOrEqual(INativeType operand)
    {
        throw new NativeTypeException("Can't apply equations to bool.");
    }

    public bool Equal(INativeType operand)
    {
        if (operand is not NativeBool right)
        {
            throw new NativeTypeException("Can't compare bool and not bool.");
        }

        return _value;
    }

    public bool Greater(INativeType operand)
    {
        throw new NativeTypeException("Can't apply equations to bool.");
    }

    public bool GreaterOrEqual(INativeType operand)
    {
        throw new NativeTypeException("Can't apply equations to bool.");
    }

    public bool NotEqual(INativeType operand)
    {
        if (operand is not NativeBool right)
        {
            throw new NativeTypeException("Can't compare bool and not bool.");
        }

        return _value;
    }

    public void LogicalOr(INativeType operand)
    {
        if (operand is not NativeBool right)
        {
            throw new NativeTypeException("Can't lor bool and not bool.");
        }

        _value = _value || right._value;
    }

    public void LogicalAnd(INativeType operand)
    {
        if (operand is not NativeBool right)
        {
            throw new NativeTypeException("Can't land bool and not bool.");
        }

        _value = _value && right._value;
    }

    public void Mod(INativeType operand)
    {
        throw new NativeTypeException("Can't apply arithmetics to bool.");
    }

    public void Minus()
    {
        throw new NativeTypeException("Can't apply arithmetics to bool.");
    }

    public void Negative()
    {
        _value = !_value;
    }
}