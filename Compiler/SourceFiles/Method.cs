using Compiler.Operations;

namespace Compiler.SourceFiles;

public class Method
{
    public int NameIndex { get; set; }
    
    public int ParameterDescriptorIndex { get; set; }
    
    public int ReturnTypeIndex { get; set; }

    public int MaxStackUsed {get; set;}
    
    public int LocalsLength { get; set; }
    
    public int OperationsLength => Operations.Count;
    
    public List<IOperation> Operations { get; set; }
}