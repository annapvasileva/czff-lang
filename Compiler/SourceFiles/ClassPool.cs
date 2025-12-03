namespace Compiler.SourceFiles;

public class ClassPool
{
    public int Length => Classes.Count;
    
    public List<ClassItem> Classes { get; }

    public ClassPool()
    {
        Classes = new List<ClassItem>();
    }
}