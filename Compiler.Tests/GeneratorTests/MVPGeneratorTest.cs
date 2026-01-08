using Compiler.Generation;
using Compiler.Operations;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis.Models;
using Compiler.SourceFiles;
using Compiler.SourceFiles.Constants;
using Compiler.Util;

namespace Compiler.Tests.GeneratorTests;

public class MVPGeneratorTest
{
    [Fact]
    public static void Generate_First_Ball_Success()
    {
        // Arrange
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void") { LocalsLength = 3 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I", 0));
        mainBodyTable.Symbols.Add("b", new VariableSymbol("b", "I", 1));
        mainBodyTable.Symbols.Add("res", new VariableSymbol("res", "I", 2));
        
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(){ },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "a",
                                new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new LiteralExpressionNode("3", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "res",
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("a"),
                                    new IdentifierExpressionNode("b"),
                                    BinaryOperatorType.Addition)),
                            new PrintStatementNode(new IdentifierExpressionNode("res")),
                            new ReturnStatementNode(null),
                        }) { Scope = mainBodyTable }
                )
            }));
        
        var constantPool = new ConstantPool();
        constantPool.AddConstant(new StringConstant("Main"));
        constantPool.AddConstant(new StringConstant(""));
        constantPool.AddConstant(new StringConstant("void"));
        constantPool.AddConstant(new IntConstant(2));
        constantPool.AddConstant(new IntConstant(3));
        
        var functionPool = new FunctionPool();
        functionPool.AddFunction(new Function()
        {
            NameIndex = 0,
            LocalsLength = 3,
            MaxStackUsed = 0,
            Operations = [
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
                new Ret()],
            ParameterDescriptorIndex = 1,
            ReturnTypeIndex = 2

        });
        
        var classPool = new ClassPool();

        var header = new Header([0, 0, 0], 0);
        
        var expectedBall = new Ball
        {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };

        var generator = new Generator(new CompilerSettings()
        {
            Version = [0, 0, 0]
        });
        
        // Act

        Ball result = generator.Generate(ast, expectedTable);
        
        // Assert

        IList<ConstantItem> expectedConstantsPool = result.ConstantPool.GetConstants();
        IList<ConstantItem> resultConstantsPool = expectedBall.ConstantPool.GetConstants();
        Assert.Equal(expectedConstantsPool.Count, resultConstantsPool.Count);
        
        for (int i = 0; i < expectedConstantsPool.Count; i++)
        {
            Assert.Equal(expectedConstantsPool[i].Tag, resultConstantsPool[i].Tag);
            Assert.Equal(expectedConstantsPool[i].Data.ToString(), resultConstantsPool[i].Data.ToString());
        }
        
        IList<Function> expectedFunctionPool = result.FunctionPool.GetFunctions();
        IList<Function> resultFunctionPool = expectedBall.FunctionPool.GetFunctions();
        Assert.Equal(expectedFunctionPool.Count, resultFunctionPool.Count);

        for (int i = 0; i < expectedFunctionPool.Count; i++)
        {
            Function a = expectedFunctionPool[i];
            Function b = resultFunctionPool[i];
            
            Assert.Equal(a.NameIndex, b.NameIndex);
            Assert.Equal(a.ParameterDescriptorIndex, b.ParameterDescriptorIndex);
            Assert.Equal(a.ReturnTypeIndex, b.ReturnTypeIndex);
            Assert.Equal(a.MaxStackUsed, b.MaxStackUsed);
            Assert.Equal(a.LocalsLength, b.LocalsLength);
            Assert.Equal(a.OperationsLength, b.OperationsLength);

            for (int j = 0; j < a.Operations.Count; j++)
            {
                var operationA = a.Operations[j];
                var operationB = b.Operations[j];
                
                Assert.Equal(operationA.GetString(), operationB.GetString());
            }
        }
    }
    
    [Fact]
    public static void Generate_Second_Ball_Success()
    {
        // Arrange
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void") { LocalsLength = 2 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("n", new VariableSymbol("n", "I", 0));
        mainBodyTable.Symbols.Add("arr", new VariableSymbol("arr", "[I", 1));
        expectedTable.Children.Add(mainBodyTable);
        
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(){ },
                    new BlockNode(
                        new List<StatementNode>()
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "n",
                                new LiteralExpressionNode("5", LiteralType.IntegerLiteral)),
                            
                            new VariableDeclarationNode(
                                new ArrayTypeNode(new SimpleTypeNode("int")),
                                "arr",
                                new ArrayCreationExpressionNode(
                                        new SimpleTypeNode("int"),
                                        new IdentifierExpressionNode("n"))),
                            
                            new ArrayAssignmentStatementNode(
                                new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new LiteralExpressionNode("0",  LiteralType.IntegerLiteral)
                                ),
                                new LiteralExpressionNode("-1", LiteralType.IntegerLiteral)),
                            
                            new ArrayAssignmentStatementNode(
                                new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new LiteralExpressionNode("1",  LiteralType.IntegerLiteral)
                                ),
                                new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),

                            new ArrayAssignmentStatementNode(
                                new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new LiteralExpressionNode("2",  LiteralType.IntegerLiteral)
                                ),
                                new BinaryExpressionNode(
                                    new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("0",  LiteralType.IntegerLiteral)),
                                    new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("1",  LiteralType.IntegerLiteral)),
                                    BinaryOperatorType.Addition)),
                            
                            new ArrayAssignmentStatementNode(
                                new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new LiteralExpressionNode("3",  LiteralType.IntegerLiteral)),
                                new UnaryExpressionNode(
                                        UnaryOperatorType.Minus
                                        ,new BinaryExpressionNode(
                                        new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("0",  LiteralType.IntegerLiteral)),
                                        new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("1",  LiteralType.IntegerLiteral)),
                                        BinaryOperatorType.Multiplication))),
                                
                            new ArrayAssignmentStatementNode(
                                new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new LiteralExpressionNode("4",  LiteralType.IntegerLiteral)
                                ),
                                new BinaryExpressionNode(
                                    new BinaryExpressionNode(
                                        new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("0",  LiteralType.IntegerLiteral)),
                                        new BinaryExpressionNode(
                                            new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("1",  LiteralType.IntegerLiteral)),
                                            new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("2",  LiteralType.IntegerLiteral)),
                                            BinaryOperatorType.Addition),
                                        BinaryOperatorType.Multiplication),
                                    new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("3",  LiteralType.IntegerLiteral)),
                                    BinaryOperatorType.Addition)),
                            
                            new PrintStatementNode(new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("0",  LiteralType.IntegerLiteral))),
                            new PrintStatementNode(new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("1",  LiteralType.IntegerLiteral))),
                            new PrintStatementNode(new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("2",  LiteralType.IntegerLiteral))),
                            new PrintStatementNode(new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("3",  LiteralType.IntegerLiteral))),
                            new PrintStatementNode(new ArrayIndexExpressionNode(new IdentifierExpressionNode("arr"), new  LiteralExpressionNode("4",  LiteralType.IntegerLiteral))),
                            
                            new ReturnStatementNode(null),
                        }) { Scope = mainBodyTable }
                )
            }));
        
        var constantPool = new ConstantPool();
        constantPool.AddConstant(new StringConstant("Main"));
        constantPool.AddConstant(new StringConstant(""));
        constantPool.AddConstant(new StringConstant("void"));
        constantPool.AddConstant(new IntConstant(5));
        constantPool.AddConstant(new StringConstant("I"));
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
        
        var expectedBall = new Ball
        {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };

        var generator = new Generator(new CompilerSettings()
        {
            Version = [0, 0, 0]
        });
        
        // Act

        Ball result = generator.Generate(ast, expectedTable);
        
        // Assert

        IList<ConstantItem> expectedConstantsPool = result.ConstantPool.GetConstants();
        IList<ConstantItem> resultConstantsPool = expectedBall.ConstantPool.GetConstants();
        Assert.Equal(expectedConstantsPool.Count, resultConstantsPool.Count);
        
        for (int i = 0; i < expectedConstantsPool.Count; i++)
        {
            Assert.Equal(expectedConstantsPool[i].Tag, resultConstantsPool[i].Tag);
            Assert.Equal(expectedConstantsPool[i].Data.ToString(), resultConstantsPool[i].Data.ToString());
        }
        
        IList<Function> expectedFunctionPool = result.FunctionPool.GetFunctions();
        IList<Function> resultFunctionPool = expectedBall.FunctionPool.GetFunctions();
        Assert.Equal(expectedFunctionPool.Count, resultFunctionPool.Count);

        for (int i = 0; i < expectedFunctionPool.Count; i++)
        {
            Function a = expectedFunctionPool[i];
            Function b = resultFunctionPool[i];
            
            Assert.Equal(a.NameIndex, b.NameIndex);
            Assert.Equal(a.ParameterDescriptorIndex, b.ParameterDescriptorIndex);
            Assert.Equal(a.ReturnTypeIndex, b.ReturnTypeIndex);
            Assert.Equal(a.MaxStackUsed, b.MaxStackUsed);
            Assert.Equal(a.LocalsLength, b.LocalsLength);
            Assert.Equal(a.OperationsLength, b.OperationsLength);

            for (int j = 0; j < a.Operations.Count; j++)
            {
                var operationA = a.Operations[j];
                var operationB = b.Operations[j];
                
                Assert.Equal(operationA.GetString(), operationB.GetString());
            }
        }
    }
}