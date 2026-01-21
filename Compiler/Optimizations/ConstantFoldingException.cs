using System.Linq.Expressions;

namespace Compiler.Optimizations;

public class ConstantFoldingException (string msg) : Exception(msg)
{
}