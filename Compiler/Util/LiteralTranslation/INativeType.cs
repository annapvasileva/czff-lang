using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.SourceFiles;

namespace Compiler.Util.LiteralTranslation;

public interface INativeType
{
    public LiteralExpressionNode GetExpressionView();
    public ConstantItem GetConstantView();
    
    public void Add(INativeType operand);
    public void Subtract(INativeType operand);
    public void Multiply(INativeType operand);
    public void Divide(INativeType operand);
    public bool Less(INativeType operand);
    public bool LessOrEqual(INativeType operand);
    public bool Equal(INativeType operand);
    public bool Greater(INativeType operand);
    public bool GreaterOrEqual(INativeType operand);
    public bool NotEqual(INativeType operand);
    public void LogicalOr(INativeType operand);
    public void LogicalAnd(INativeType operand);
    public void Mod(INativeType operand);
    public void Minus();
    public void Negative();
}