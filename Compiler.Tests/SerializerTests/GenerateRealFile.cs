using Compiler.Operations;
using Compiler.Serialization;
using Compiler.SourceFiles;
using Compiler.SourceFiles.Constants;

namespace Compiler.Tests.SerializerTests;

public class GenerateRealFile
{
    public static void CreateFile()
    {
        // arrange
        
        
        List<IOperation> operations =
        [
            new Ldc(3), // ldc const[0]
            new Store(0), // store var a
            new Ldc(4), // ldc const[1]
            new Store(1), // store var b
            new Ldv(0), // load a
            new Ldv(1), // load b
            new Add(),
            new Store(2), // store res
            new Ldv(2), // load res
            new Print(),
            new Ret()
        ];

        var mainFunc = new Function
        {
            NameIndex = 0,
            LocalsLength = 3,
            MaxStackUsed = 0,
            Operations = operations,
            ParameterDescriptorIndex = 1,
            ReturnTypeIndex = 2
        };

        var constantPool = new ConstantPool();
        constantPool.AddConstant(new StringConstant("Main"));
        constantPool.AddConstant(new StringConstant(""));
        constantPool.AddConstant(new StringConstant("void"));
        constantPool.AddConstant(new IntConstant(2));
        constantPool.AddConstant(new IntConstant(3));
        
        var functionPool = new FunctionPool([mainFunc]);
        var classPool = new ClassPool();

        var header = new Header([0, 0, 0], 0);
        
        var ball = new Ball
        {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };
        
        string tempFile = "C:\\Users\\jpegMushrum\\Desktop\\FirstProgram.ball";
        if (File.Exists(tempFile))
        {
            File.Delete(tempFile);
        }
        
        var serializer = new Serializer();

        try
        {
            serializer.Serialize(ball, tempFile);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}