namespace Compiler.SourceFiles;

public class FunctionPool
{
    public int Length => _functions.Count;

    private List<Function> _functions;

    public FunctionPool()
    {
        _functions = new List<Function>();
    }
    
    public FunctionPool(IList<Function> functions)
    {
        _functions = new List<Function>(functions);
    }

    public IEnumerable<Function> GetClasses()
    {
        return _functions;
    }

    public void AddFunction(Function function)
    {
        _functions.Add(function);
    }

    public IList<Function> GetFunctions()
    {
        return _functions;
    }
}