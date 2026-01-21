using System.Text;
using Compiler.Operations;
using Compiler.Serialization;
using Compiler.SourceFiles;
using Compiler.Tests.Storage;
using Compiler.Util;

namespace Compiler.Tests.SerializerTests;

public class MVPSerializerTests
{
    [Fact]
    public void Serialize_Simple_Ball_Success()
    {
        // Arrange
        var ball = BallStore.ReturnBall("SimpleBall");        

        var serializer = new Serializer();

        // Act
        byte[] result = serializer.SerializeToArray(ball);

        // Expected header
        var expected = new List<byte>();

        // Magical "ball"
        expected.AddRange(new byte[] { 0x62, 0x61, 0x6c, 0x6c });

        // Version 3 bytes
        expected.AddRange(new byte[] { 0, 0, 0 });

        // Flags (debug = false, has_checksum = false)
        expected.Add(0);

        // Constant pool length
        expected.AddRange(ByteConverter.IntToU2(5));

        // Constant entries
        // Tag u1 + value
        
        // new ConstantItem(5,"Main"),
        // new ConstantItem(5, ""),
        // new ConstantItem(5, "void"),
        // new ConstantItem(4, [2]),
        // new ConstantItem(4, [3]),
        expected.AddRange([0xB, 0x0, 0x4]); expected.AddRange(Encoding.UTF8.GetBytes("Main"));
        expected.AddRange([0xB, 0x0, 0x0]); expected.AddRange(Encoding.UTF8.GetBytes(""));
        expected.AddRange([0xB, 0x0, 0x5]); expected.AddRange(Encoding.UTF8.GetBytes("void;"));
        expected.Add(0x6); expected.AddRange([0,0,0,2]);
        expected.Add(0x6); expected.AddRange([0,0,0,3]);

        // Functions pool length (1)
        expected.AddRange(ByteConverter.IntToU2(1));

        // Function Main name (string descriptor)
        expected.AddRange(ByteConverter.IntToU2(0));

        // Parameters (empty)
        expected.AddRange(ByteConverter.IntToU2(1));

        // Return type "void"
        expected.AddRange(ByteConverter.IntToU2(2));

        // maxStack
        expected.AddRange(ByteConverter.IntToU2(0));

        // localsCount = 3
        expected.AddRange(ByteConverter.IntToU2(3));
        
        // code length = number of instructions
        List<IOperation> operations = [
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
        expected.AddRange(ByteConverter.IntToU2(operations.Count));

        List<byte> buff = new List<byte>();
        var visitor = new SerializingVisitor(buff);
        
        // instruction bytes (opcodes + args)
        foreach (var oper in operations)
        {
            oper.Accept(visitor);
        }

        expected.AddRange(buff.ToArray());
        
        // Classes pool length = 0
        expected.AddRange(ByteConverter.IntToU2(0));

        // Assert
        Assert.Equal(expected.ToArray(), result);
    }
    
    [Fact]
    public void Serializer_Writes_Ball_Success()
    {
        // arrange
        string tempFile = Path.GetTempFileName();
        var serializer = new Serializer();

        Ball ball = BallStore.ReturnBall("SimpleBall");
        
        try
        {
            // act
            serializer.Serialize(ball, tempFile);

            // assert
            Assert.True(File.Exists(tempFile));

            byte[] bytes = File.ReadAllBytes(tempFile);
            Assert.NotEmpty(bytes);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
    
    [Fact]
    public void Serialize_Array_Ball_Success()
    {   
        // Arrange
        var ball = BallStore.ReturnBall("ArrayBall");        

        var serializer = new Serializer();

        // Act
        byte[] result = serializer.SerializeToArray(ball);

        // Expected header
        var expected = new List<byte>();

        // Magical "ball"
        expected.AddRange(new byte[] { 0x62, 0x61, 0x6c, 0x6c });

        // Version 3 bytes
        expected.AddRange(new byte[] { 0, 0, 0 });

        // Flags (debug = false, has_checksum = false)
        expected.Add(0);

        // Constant pool length
        expected.AddRange(ByteConverter.IntToU2(11));

        // Constant entries
        // Tag u1 + value
        
        
        // new StringConstant("Main"))
        // new StringConstant(""))
        // new StringConstant("void;"))
        // new IntConstant(5))
        // new StringConstant("I"))
        // new IntConstant(0))
        // new IntConstant(-1))
        // new IntConstant(1))
        // new IntConstant(2))
        // new IntConstant(3))
        // new IntConstant(4))
        expected.AddRange([0xB, 0x0, 0x4]); expected.AddRange(Encoding.UTF8.GetBytes("Main"));
        expected.AddRange([0xB, 0x0, 0x0]); expected.AddRange(Encoding.UTF8.GetBytes(""));
        expected.AddRange([0xB, 0x0, 0x5]); expected.AddRange(Encoding.UTF8.GetBytes("void;"));
        expected.Add(0x6); expected.AddRange([0,0,0,5]);
        expected.AddRange([0xB, 0x0, 0x2]); expected.AddRange(Encoding.UTF8.GetBytes("I;"));
        expected.Add(0x6); expected.AddRange([0,0,0,0]);
        expected.Add(0x6); expected.AddRange(ByteConverter.IntToI4(-1));
        expected.Add(0x6); expected.AddRange([0,0,0,1]);
        expected.Add(0x6); expected.AddRange([0,0,0,2]);
        expected.Add(0x6); expected.AddRange([0,0,0,3]);
        expected.Add(0x6); expected.AddRange([0,0,0,4]);

        // Functions pool length (1)
        expected.AddRange(ByteConverter.IntToU2(1));

        // Function Main name (string descriptor)
        expected.AddRange(ByteConverter.IntToU2(0));

        // Parameters (empty)
        expected.AddRange(ByteConverter.IntToU2(1));

        // Return type "void;"
        expected.AddRange(ByteConverter.IntToU2(2));

        // maxStack
        expected.AddRange(ByteConverter.IntToU2(0));

        // localsCount = 2
        expected.AddRange(ByteConverter.IntToU2(2));
        
        // code length = number of instructions
        List<IOperation> operations = [
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
        ];
        expected.AddRange(ByteConverter.IntToU2(operations.Count));

        List<byte> buff = new List<byte>();
        var visitor = new SerializingVisitor(buff);
        
        // instruction bytes (opcodes + args)
        foreach (var oper in operations)
        {
            oper.Accept(visitor);
        }

        expected.AddRange(buff.ToArray());
        
        // Classes pool length = 0
        expected.AddRange(ByteConverter.IntToU2(0));

        // Assert
        Assert.Equal(expected.ToArray(), result);
    }
    
    [Fact]
    public void Serialize_Sum_And_Main_Ball_Success()
    {
        // Arrange
        var ball = BallStore.ReturnBall("FuncBall");        
    
        var serializer = new Serializer();
    
        // Act
        byte[] result = serializer.SerializeToArray(ball);
    
        var expected = new List<byte>();
    
        // "ball"
        expected.AddRange(new byte[] { 0x62, 0x61, 0x6c, 0x6c });
    
        // Version
        expected.AddRange(new byte[] { 0, 0, 0 });
    
        // Flags
        expected.Add(0);
    
        // Constant pool length = 8
        expected.AddRange(ByteConverter.IntToU2(8));
    
        // Constants:
        // "Sum", "Main", "II;", "I;", "", "void;", 1, 2
        expected.AddRange([0xB, 0x0, 0x3]); expected.AddRange(Encoding.UTF8.GetBytes("Sum"));
        expected.AddRange([0xB, 0x0, 0x4]); expected.AddRange(Encoding.UTF8.GetBytes("I;I;"));
        expected.AddRange([0xB, 0x0, 0x2]); expected.AddRange(Encoding.UTF8.GetBytes("I;"));
        expected.AddRange([0xB, 0x0, 0x4]); expected.AddRange(Encoding.UTF8.GetBytes("Main"));
        expected.AddRange([0xB, 0x0, 0x0]); expected.AddRange(Encoding.UTF8.GetBytes(""));
        expected.AddRange([0xB, 0x0, 0x5]); expected.AddRange(Encoding.UTF8.GetBytes("void;"));
        expected.Add(0x6); expected.AddRange([0,0,0,1]);
        expected.Add(0x6); expected.AddRange([0,0,0,2]);
    
        // Functions count = 2
        expected.AddRange(ByteConverter.IntToU2(2));
    
        // ---------- Function Sum ----------
        expected.AddRange(ByteConverter.IntToU2(0)); // "Sum"
        expected.AddRange(ByteConverter.IntToU2(1)); // "I;I;"
        expected.AddRange(ByteConverter.IntToU2(2)); // "I;"
        expected.AddRange(ByteConverter.IntToU2(0)); // maxStack
        expected.AddRange(ByteConverter.IntToU2(2)); // locals
    
        List<IOperation> sumOps =
        [
            new Store(0),   // a
            new Store(1),   // b
                
            new Ldv(0), // a
            new Ldv(1), // b
            new Add(),
            new Ret()
        ];
    
        expected.AddRange(ByteConverter.IntToU2(sumOps.Count));
    
        var buff = new List<byte>();
        var visitor = new SerializingVisitor(buff);
    
        foreach (var op in sumOps)
            op.Accept(visitor);
    
        expected.AddRange(buff);
    
        // ---------- Function Main ----------
        expected.AddRange(ByteConverter.IntToU2(3)); // "Main"
        expected.AddRange(ByteConverter.IntToU2(4)); // ""
        expected.AddRange(ByteConverter.IntToU2(5)); // "void;"
        expected.AddRange(ByteConverter.IntToU2(0)); // maxStack
        expected.AddRange(ByteConverter.IntToU2(3)); // locals
    
        List<IOperation> mainOps =
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
        ];
    
        expected.AddRange(ByteConverter.IntToU2(mainOps.Count));
    
        buff.Clear();
    
        foreach (var op in mainOps)
            op.Accept(visitor);
    
        expected.AddRange(buff);
    
        // Classes = 0
        expected.AddRange(ByteConverter.IntToU2(0));
    
        // Assert
        Assert.Equal(expected.ToArray(), result);
    }

}