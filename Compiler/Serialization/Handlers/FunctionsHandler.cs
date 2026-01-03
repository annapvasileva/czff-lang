using Compiler.Operations;
using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Serialization.Handlers;

public class FunctionsHandler(IOperationVisitor visitor) : Handler
{
    private IOperationVisitor _visitor = visitor;
    
    public override void Handle(Ball source, IList<byte> target)
    {
        var pool = source.FunctionPool;
        
        var functionsLength = ByteConverter.IntToU2(pool.Length);
        target.Add(functionsLength[0]);
        target.Add(functionsLength[1]);

        foreach (var function in pool.GetFunctions())
        {
            var functionNameIndex = ByteConverter.IntToU2(function.NameIndex);
            target.Add(functionNameIndex[0]);
            target.Add(functionNameIndex[1]);

            var parametersDescriptor = ByteConverter.IntToU2(function.ParameterDescriptorIndex);
            target.Add(parametersDescriptor[0]);
            target.Add(parametersDescriptor[1]);
                
            var returnDescriptor = ByteConverter.IntToU2(function.ReturnTypeIndex);
            target.Add(returnDescriptor[0]);
            target.Add(returnDescriptor[1]);

            var maxStackUsed = ByteConverter.IntToU2(function.MaxStackUsed);
            target.Add(maxStackUsed[0]);
            target.Add(maxStackUsed[1]);
                
            var localsLength = ByteConverter.IntToU2(function.LocalsLength);
            target.Add(localsLength[0]);
            target.Add(localsLength[1]);
                
            var operationsLength = ByteConverter.IntToU2(function.OperationsLength);
            target.Add(operationsLength[0]);
            target.Add(operationsLength[1]);

            foreach (var operation in function.Operations)
            {
                operation.Accept(_visitor);
            }
        }
            
        Next?.Handle(source, target);
    }
}