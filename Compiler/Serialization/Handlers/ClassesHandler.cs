using Compiler.Operations;
using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Serialization.Handlers;

public class ClassesHandler(IOperationVisitor visitor) : Handler
{
    private IOperationVisitor _visitor = visitor;

    public override void Handle(Ball source, IList<byte> target)
    {
        ClassPool pool = source.ClassPool;
        
        byte[] length = ByteConverter.IntToU2(pool.Length);
        
        target.Add(length[0]);
        target.Add(length[1]);
        
        foreach (var c in pool.GetClasses())
        {
            var nameIndex = ByteConverter.IntToU2(c.NameIndex);
            target.Add(nameIndex[0]);
            target.Add(nameIndex[1]);
            
            var fieldsLength = ByteConverter.IntToU2(c.FieldsLength);
            target.Add(fieldsLength[0]);
            target.Add(fieldsLength[1]);

            foreach (var field in c.Fields)
            {
                var fieldNameIndex = ByteConverter.IntToU2(field.NameIndex);
                target.Add(fieldNameIndex[0]);
                target.Add(fieldNameIndex[1]);
                
                var fieldDescriptorIndex = ByteConverter.IntToU2(field.DescriptorIndex);
                target.Add(fieldDescriptorIndex[0]);
                target.Add(fieldDescriptorIndex[1]);
            }

            var methodsLength = ByteConverter.IntToU2(c.MethodsLength);
            target.Add(methodsLength[0]);
            target.Add(methodsLength[1]);

            foreach (var method in c.Methods)
            {
                var methodNameIndex = ByteConverter.IntToU2(method.NameIndex);
                target.Add(methodNameIndex[0]);
                target.Add(methodNameIndex[1]);

                var parametersDescriptor = ByteConverter.IntToU2(method.ParameterDescriptorIndex);
                target.Add(parametersDescriptor[0]);
                target.Add(parametersDescriptor[1]);
                
                var returnDescriptor = ByteConverter.IntToU2(method.ReturnTypeIndex);
                target.Add(returnDescriptor[0]);
                target.Add(returnDescriptor[1]);

                var maxStackUsed = ByteConverter.IntToU2(method.MaxStackUsed);
                target.Add(maxStackUsed[0]);
                target.Add(maxStackUsed[1]);
                
                var localsLength = ByteConverter.IntToU2(method.LocalsLength);
                target.Add(localsLength[0]);
                target.Add(localsLength[1]);
                
                var operationsLength = ByteConverter.IntToU2(method.OperationsLength);
                target.Add(operationsLength[0]);
                target.Add(operationsLength[1]);

                foreach (var operation in method.Operations)
                {
                    operation.Accept(_visitor);
                }
            }
            
            Next?.Handle(source, target);
        }
    }
}