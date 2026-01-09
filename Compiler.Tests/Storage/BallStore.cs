using Compiler.Operations;
using Compiler.SourceFiles;
using Compiler.SourceFiles.Constants;

namespace Compiler.Tests.Storage;

public static class BallStore
{
    public static Ball ReturnBall(string name) 
    {
        switch (name)
        {
            case "SimpleBall":
                return GetSimpleBall();
            case "ArrayBall":
                return GetArrayBall();
            default:
                throw new ArgumentOutOfRangeException(nameof(name), name, null);
        }
    }
    
    private static Ball GetSimpleBall()
    {
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
        constantPool.AddConstant(new StringConstant ("Main"));
        constantPool.AddConstant(new StringConstant(""));
        constantPool.AddConstant(new StringConstant("void;"));
        constantPool.AddConstant(new IntConstant(2));
        constantPool.AddConstant(new IntConstant(3));
        var functionPool = new FunctionPool([mainFunc]);
        var classPool = new ClassPool();

        var header = new Header([0, 0, 0], 0);
        
        return new Ball
        {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };
    }

    private static Ball GetArrayBall()
    {
        var constantPool = new ConstantPool();
        constantPool.AddConstant(new StringConstant("Main"));
        constantPool.AddConstant(new StringConstant(""));
        constantPool.AddConstant(new StringConstant("void;"));
        constantPool.AddConstant(new IntConstant(5));
        constantPool.AddConstant(new StringConstant("I;"));
        constantPool.AddConstant(new IntConstant(0));
        constantPool.AddConstant(new IntConstant(-1));
        constantPool.AddConstant(new IntConstant(1));
        constantPool.AddConstant(new IntConstant(2));
        constantPool.AddConstant(new IntConstant(3));
        constantPool.AddConstant(new IntConstant(4));
        
        var functionPool = new FunctionPool();
        functionPool.AddFunction(new Function
        {
            NameIndex = 0,
            LocalsLength = 2,
            MaxStackUsed = 0,
            Operations =
            [
                // var int n = 5;
                new Ldc(3),          // 5
                new Store(0),        // n
            
                // var array<int> arr = new int(n)[];
                new Ldv(0),          // n
                new Newarr(4),       // I
                new Store(1),        // arr
            
                // arr[0] = -1;
                new Ldv(1),          // arr
                new Ldc(5),          // 0
                new Ldc(6),          // -1
                new Stelem(),
            
                // arr[1] = 2;
                new Ldv(1),          // arr
                new Ldc(7),          // 1
                new Ldc(8),          // 2
                new Stelem(),
            
                // arr[2] = arr[0] + arr[1];
                new Ldv(1),          // arr
                new Ldc(8),          // 2 (index)
            
                new Ldv(1),          // arr
                new Ldc(5),          // 0
                new Ldelem(),
            
                new Ldv(1),          // arr
                new Ldc(7),          // 1
                new Ldelem(),
            
                new Add(),
                new Stelem(),
            
                // arr[3] = -(arr[0] * arr[1]);
                new Ldv(1),          // arr
                new Ldc(9),          // 3 (index)
            
                new Ldv(1),          // arr
                new Ldc(5),          // 0
                new Ldelem(),
            
                new Ldv(1),          // arr
                new Ldc(7),          // 1
                new Ldelem(),
            
                new Mul(),
                new Min(),
                new Stelem(),
            
                // arr[4] = arr[0] * (arr[1] + arr[2]) + arr[3];
                new Ldv(1),          // arr
                new Ldc(10),         // 4 (index)
            
                // arr[0]
                new Ldv(1),
                new Ldc(5),
                new Ldelem(),
            
                // (arr[1] + arr[2])
                new Ldv(1),
                new Ldc(7),
                new Ldelem(),
            
                new Ldv(1),
                new Ldc(8),
                new Ldelem(),
            
                new Add(),
                new Mul(),
            
                // + arr[3]
                new Ldv(1),
                new Ldc(9),
                new Ldelem(),
            
                new Add(),
                new Stelem(),
            
                // print arr[0]
                new Ldv(1),
                new Ldc(5),
                new Ldelem(),
                new Print(),
            
                // print arr[1]
                new Ldv(1),
                new Ldc(7),
                new Ldelem(),
                new Print(),
            
                // print arr[2]
                new Ldv(1),
                new Ldc(8),
                new Ldelem(),
                new Print(),
            
                // print arr[3]
                new Ldv(1),
                new Ldc(9),
                new Ldelem(),
                new Print(),
            
                // print arr[4]
                new Ldv(1),
                new Ldc(10),
                new Ldelem(),
                new Print(),
            
                new Ret()
            ],
            ParameterDescriptorIndex = 1,
            ReturnTypeIndex = 2

        });
        
        var classPool = new ClassPool();

        var header = new Header([0, 0, 0], 0);
        
        return new Ball
        {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };
    }
}