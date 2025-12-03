namespace Compiler.SourceFiles;

public class ClassItem
{
    public int NameIndex { get; set; }
    
    public int FiledsLength => Fields.Count;
    
    public List<Field> Fields { get; set; }
    
    public int MethodsLength => Methods.Count;
    
    public List<Method> Methods { get; set; }
}