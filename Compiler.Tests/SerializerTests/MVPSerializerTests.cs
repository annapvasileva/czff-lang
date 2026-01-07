using System.Text;
using Compiler.Operations;
using Compiler.Serialization;
using Compiler.SourceFiles;
using Compiler.SourceFiles.Constants;
using Compiler.Util;

namespace Compiler.Tests.SerializerTests;

public class TestContext : IDisposable
{
    public Ball Ball { get; }

    public List<IOperation> Operations; 

    public TestContext()
    {
        Operations =
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
            new Halt()
        ];

        var mainFunc = new Function
        {
            NameIndex = 0,
            LocalsLength = 3,
            MaxStackUsed = 0,
            Operations = Operations,
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
        
        Ball = new Ball
        {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };
    }

    public void Dispose()
    {
    }
}

public class MVPSerializerTests: IClassFixture<TestContext>
{
    private readonly TestContext _ctx;
    
    public MVPSerializerTests(TestContext ctx)
    {
        _ctx = ctx;
    }
    
    [Fact]
    public void Serialize_MVP_Ball_Success()
    {
        // Arrange
        

        var serializer = new Serializer();

        // Act
        byte[] result = serializer.SerializeToArray(_ctx.Ball);

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
        expected.AddRange([0x5, 0x0, 0x4]); expected.AddRange(Encoding.UTF8.GetBytes("Main"));
        expected.AddRange([0x5, 0x0, 0x0]); expected.AddRange(Encoding.UTF8.GetBytes(""));
        expected.AddRange([0x5, 0x0, 0x4]); expected.AddRange(Encoding.UTF8.GetBytes("void"));
        expected.Add(0x4); expected.AddRange([0,0,0,2]);
        expected.Add(0x4); expected.AddRange([0,0,0,3]);

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
        List<IOperation> operations = _ctx.Operations;
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

        Ball ball = _ctx.Ball;
        
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
}