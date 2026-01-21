using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Expressions;

namespace Compiler.Util.LiteralTranslation;

public abstract class NativeTypeBuilder
{
    public static INativeType Build(LiteralExpressionNode node)
    {
        switch (node.Type)
        { 
            case LiteralType.BooleanLiteral:
                bool flag;
                switch (node.Value)
                {
                    case "true":
                        flag = true;
                        break;
                    case "false": 
                        flag = false;
                        break;
                    default:
                        throw new NativeTypeException($"Literal {node.Value} is not a boolean.");
                }
                return new NativeBool(flag);
            case LiteralType.IntegerLiteral:
                int number = Convert.ToInt32(node.Value);
                return new NativeInteger(number);
            case LiteralType.Integer64Literal:
                long number64 = Convert.ToInt64(node.Value);
                return new NativeInteger64(number64);
            case LiteralType.StringLiteral:
                string line = node.Value;
                return new NativeString(line);
            default:
                throw new NotImplementedException();
        }
    }
}