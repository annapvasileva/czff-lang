namespace Compiler.SourceFiles;

public class FunctionPool
{
    public int Length => _functions.Count;

    private List<Function> _functions;

    public FunctionPool()
    {
        _functions = new List<Function>();
    }

    public IEnumerable<Function> GetClasses()
    {
        return _functions;
    }

    public IList<Function> GetFunctions()
    {
        return _functions;
    }
}