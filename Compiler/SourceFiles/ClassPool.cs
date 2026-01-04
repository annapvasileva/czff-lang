namespace Compiler.SourceFiles;

public class ClassPool
{
    public int Length => _classes.Count;

    private List<ClassItem> _classes;

    public ClassPool()
    {
        _classes = new List<ClassItem>();
    }

    public IEnumerable<ClassItem> GetClasses()
    {
        return _classes;
    }

    public void AddClass(ClassItem classItem)
    {
        _classes.Add(classItem);
    }
    
    public void AddClass(IEnumerable<ClassItem> classes)
    {
        foreach (var classItem in classes)
        {
            _classes.Add(classItem);
        }
    }
    
}