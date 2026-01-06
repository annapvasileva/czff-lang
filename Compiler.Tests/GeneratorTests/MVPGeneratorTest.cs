using Compiler.Generation;
using Compiler.Operations;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis.Models;
using Compiler.SourceFiles;
using Compiler.Util;

namespace Compiler.Tests.GeneratorTests;

public class MVPGeneratorTest
{
    [Fact]
    public static void Generate_MVP_Ball_Success()
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
                            new PrintStatementNode(new IdentifierExpressionNode("res"))
                        }) { Scope = mainBodyTable }
                )
            }));
        
        var constants = new List<ConstantItem>
        {
            new ConstantItem(5,"Main"),
            new ConstantItem(5, ""),
            new ConstantItem(5, "void"),
            new ConstantItem(4, [0,0,0,2]),
            new ConstantItem(4, [0,0,0,3]),
        };
        
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
            new Halt()
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

        var constantPool = new ConstantPool(constants);
        var functionPool = new FunctionPool([mainFunc]);
        var classPool = new ClassPool();

        var header = new Header([0, 0, 0], 0);
        
        var expectedBall = new Ball
        {
            ConstantPool = constantPool,
            FunctionPool = functionPool,
            ClassPool = classPool,
            Header = header
        };

        var generator = new Generator(new GeneratorSettings()
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

            for (int j = 0; i < a.Operations.Count; i++)
            {
                var operationA = a.Operations[j];
                var operationB = b.Operations[j];
                
                Assert.Equal(operationA.GetString(), operationB.GetString());
            }
        }
    }
}