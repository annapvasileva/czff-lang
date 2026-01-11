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
            case "FuncBall":
                return GetFunctionBall();
            case "CyclesBall":
                return GetCyclesBall(); 
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

    private static Ball GetFunctionBall()
    {
        var constantPool = new ConstantPool();
        constantPool.AddConstant(new StringConstant("Sum"));     // 0
        constantPool.AddConstant(new StringConstant("I;I;"));     // params Sum(int,int)
        constantPool.AddConstant(new StringConstant("I;"));      // return int
        constantPool.AddConstant(new StringConstant("Main"));    // 1
        constantPool.AddConstant(new StringConstant(""));        // params Main
        constantPool.AddConstant(new StringConstant("void;"));   // return void
        constantPool.AddConstant(new IntConstant(1));            // 6
        constantPool.AddConstant(new IntConstant(2));            // 7
        
        var sumFunc = new Function
        {
            NameIndex = 0,                  // "Sum"
            ParameterDescriptorIndex = 1,   // "I;I;"
            ReturnTypeIndex = 2,            // "I;"
            LocalsLength = 2,               // a, b
            MaxStackUsed = 0,
            Operations =
            [
                new Store(0),   // a
                new Store(1),   // b
                
                new Ldv(0), // a
                new Ldv(1), // b
                new Add(),
                new Ret()
            ]
        };

        var mainFunc = new Function
        {
            NameIndex = 3, // "Main"
            ParameterDescriptorIndex = 4, // ""
            ReturnTypeIndex = 5, // "void;"
            LocalsLength = 3, // x, y, z
            MaxStackUsed = 0,
            Operations =
            [
                // var int x = 1;
                new Ldc(6),
                new Store(0),

                // var int y = 2;
                new Ldc(7),
                new Store(1),

                // var int z = Sum(x, y);
                new Ldv(0),
                new Ldv(1),
                new Call(0), // Sum
                new Store(2),

                // print z;
                new Ldv(2),
                new Print(),

                new Ret()
            ]
        };
        var functionPool = new FunctionPool([ sumFunc, mainFunc ]);
        var classPool = new ClassPool();
        var header = new Header([0,0,0], 0);

        return new Ball {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };
    }

    private static Ball GetCyclesBall()
    {
        var constantPool = new ConstantPool();
        constantPool.AddConstant(new StringConstant("Main"));   // 0
        constantPool.AddConstant(new StringConstant(""));       // Main()
        constantPool.AddConstant(new StringConstant("void;"));  // void Main
        constantPool.AddConstant(new IntConstant(0));           // i = 0
        constantPool.AddConstant(new IntConstant(5));           // i < 5
        constantPool.AddConstant(new IntConstant(1));           // x++
        constantPool.AddConstant(new IntConstant(10));           // x < 10
        constantPool.AddConstant(new IntConstant(2));          // return 2

        var mainFunc = new Function
        {
            NameIndex = 0,
            ParameterDescriptorIndex = 1,
            ReturnTypeIndex = 2,
            LocalsLength = 2, // x, i
            MaxStackUsed = 0,
            Operations =
            [
                // x = 0
                new Ldc(3),
                new Store(0),

                // i = 0
                new Ldc(3),
                new Store(1),

                // ---- loop_start (pc = 4) ----
                // i < 5
                new Ldv(1),
                new Ldc(4),
                new Lt(),
                new Jz(18),   // goto after loop

                // x = x + i
                new Ldv(0),
                new Ldv(1),
                new Add(),
                new Store(0),

                // i++
                new Ldv(1),
                new Ldc(5),
                new Add(),
                new Store(1),

                new Jmp(4),           // goto loop_start

                // ---- after loop (pc = 18) ----
                // if (i < 10)
                new Ldv(1),
                new Ldc(6),
                new Lt(),
                new Jz(25),

                // then: print 1
                new Ldc(4),
                new Print(),
                new Jmp(27),

                // else:
                new Ldc(7),
                new Print(),

                new Ret()
            ]
        };
        
        var functionPool = new FunctionPool([ mainFunc ]);
        var classPool = new ClassPool();
        var header = new Header([0,0,0], 0);

        return new Ball {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };
    }
}