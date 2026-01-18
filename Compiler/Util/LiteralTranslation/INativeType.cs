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
    public void Less(INativeType operand);
    public void LessOrEqual(INativeType operand);
    public void Equal(INativeType operand);
    public void Greater(INativeType operand);
    public void GreaterOrEqual(INativeType operand);
    public void NotEqual(INativeType operand);
    public void LogicalOr(INativeType operand);
    public void LogicalAnd(INativeType operand);
    public void Mod(INativeType operand);
    public void Minus();
    public void Negative();
}