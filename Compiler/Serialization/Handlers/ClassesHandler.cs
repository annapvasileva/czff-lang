using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Serialization.Handlers;

public class ClassesHandler: Handler
{
    public override void Handle(Ball source, IList<byte> target)
    {
        ClassPool pool = source.ClassPool;
        
        byte[] length = ByteConverter.IntToU2(pool.Length);
        
        target.Add(length[1]);
        target.Add(length[0]);
        
        foreach (var c in pool.GetClasses())
        {
            var nameIndex = ByteConverter.IntToU2(c.NameIndex);
            target.Add(nameIndex[1]);
            target.Add(nameIndex[0]);
            
            var fieldsLength = ByteConverter.IntToU2(c.FieldsLength);
            target.Add(fieldsLength[1]);
            target.Add(fieldsLength[0]);

            foreach (var field in c.Fields)
            {
                var fieldNameIndex = ByteConverter.IntToU2(field.NameIndex);
                target.Add(fieldNameIndex[1]);
                target.Add(fieldNameIndex[0]);
                
                var fieldDescriptorIndex = ByteConverter.IntToU2(field.DescriptorIndex);
                target.Add(fieldDescriptorIndex[1]);
                target.Add(fieldDescriptorIndex[0]);
            }

            var methodsLength = ByteConverter.IntToU2(c.MethodsLength);
            target.Add(methodsLength[1]);
            target.Add(methodsLength[0]);

            foreach (var method in c.Methods)
            {
                var methodNameIndex = ByteConverter.IntToU2(method.NameIndex);
                target.Add(methodNameIndex[1]);
                target.Add(methodNameIndex[0]);

                var parametersDescriptor = ByteConverter.IntToU2(method.ParameterDescriptorIndex);
                target.Add(parametersDescriptor[1]);
                target.Add(parametersDescriptor[0]);
                
                var returnDescriptor = ByteConverter.IntToU2(method.ReturnTypeIndex);
                target.Add(returnDescriptor[1]);
                target.Add(returnDescriptor[0]);

                var maxStackUsed = ByteConverter.IntToU2(method.MaxStackUsed);
                target.Add(maxStackUsed[1]);
                target.Add(maxStackUsed[0]);
                
                var localsLength = ByteConverter.IntToU2(method.LocalsLength);
                target.Add(localsLength[1]);
                target.Add(localsLength[0]);
                
                var operationsLength = ByteConverter.IntToU2(method.OperationsLength);
                target.Add(operationsLength[1]);
                target.Add(operationsLength[0]);

                foreach (var operation in method.Operations)
                {
                    
                }
            }

        }
    }
}