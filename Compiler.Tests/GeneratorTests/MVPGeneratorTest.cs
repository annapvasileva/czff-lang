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
using Compiler.Tests.SerializerTests;
using Compiler.Tests.Storage;
using Compiler.Util;

namespace Compiler.Tests.GeneratorTests;

public class MVPGeneratorTest
{
    [Fact]
    public static void Generate_Simple_Ball_Success()
    {
        // Arrange
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 3 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("a", new VariableSymbol("a", "I;", 0));
        mainBodyTable.Symbols.Add("b", new VariableSymbol("b", "I;", 1));
        mainBodyTable.Symbols.Add("res", new VariableSymbol("res", "I;", 2));
        
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
        
        var expectedBall = BallStore.ReturnBall("SimpleBall");

        var generator = new Generator([0, 0, 0]);
        
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
    public static void Generate_Array_Ball_Success()
    {
        // Arrange
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 2 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("n", new VariableSymbol("n", "I;", 0));
        mainBodyTable.Symbols.Add("arr", new VariableSymbol("arr", "[I;", 1));
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

        var expectedBall = BallStore.ReturnBall("ArrayBall");

        var generator = new Generator([0, 0, 0]);
        
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
    public static void Generate_Cycle_Ball_Success()
    {
        // Arrange
        var expectedTable = new SymbolTable(null);
        expectedTable.Symbols.Add("Main", new FunctionSymbol("Main", "void;") { LocalsLength = 2 });
        var mainBodyTable = new SymbolTable(expectedTable);
        mainBodyTable.Symbols.Add("x", new VariableSymbol("x", "I;", 0));
        
        var forBodyTable = new SymbolTable(mainBodyTable);
        forBodyTable.Symbols.Add("i", new VariableSymbol("i", "I;", 1));
        
        var ifBodyTable = new SymbolTable(mainBodyTable);
        var elseBodyTable = new SymbolTable(mainBodyTable);
        
        
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>
            {
                new FunctionDeclarationNode(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(),
                    new BlockNode(new List<StatementNode>()
                    {
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int"),
                            "x",
                            new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                        new ForStatementNode(
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"), 
                                "i",
                                new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                            new BinaryExpressionNode(
                                new IdentifierExpressionNode("i"),
                                new LiteralExpressionNode("5", LiteralType.IntegerLiteral),
                                BinaryOperatorType.Less),
                            new IdentifierAssignmentStatementNode(
                                new IdentifierExpressionNode("i"),
                                new BinaryExpressionNode(
                                    new IdentifierExpressionNode("i"),
                                    new LiteralExpressionNode("1", LiteralType.IntegerLiteral),
                                    BinaryOperatorType.Addition)),
                            new BlockNode(new List<StatementNode>()
                            {
                                new IdentifierAssignmentStatementNode(
                                    new IdentifierExpressionNode("x"),
                                    new BinaryExpressionNode(
                                        new IdentifierExpressionNode("x"),
                                        new IdentifierExpressionNode("i"),
                                        BinaryOperatorType.Addition)),
                            }) {Scope = forBodyTable }
                            ),
                        new PrintStatementNode(
                            new IdentifierExpressionNode("x")),
                        new IfStatementNode(
                            new BinaryExpressionNode(
                                new IdentifierExpressionNode("x"),
                                new LiteralExpressionNode("10", LiteralType.IntegerLiteral),
                                BinaryOperatorType.Less),
                            new BlockNode(new List<StatementNode>()
                            {
                                new PrintStatementNode(
                                    new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                            }){ Scope = ifBodyTable },
                            new List<ElifStatementNode>(),
                            new BlockNode(new List<StatementNode>()
                            {
                                new PrintStatementNode(
                                    new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            }){ Scope = elseBodyTable }
                            ),
                        new ReturnStatementNode(null),
                    }) { Scope = mainBodyTable }
                )
            }));

        var expectedBall = BallStore.ReturnBall("CyclesBall");

        var generator = new Generator([0, 0, 0]);
        
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
        
        IList<Function> expectedFunctionPool = expectedBall.FunctionPool.GetFunctions();
        IList<Function> resultFunctionPool = result.FunctionPool.GetFunctions();
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