using Compiler.Generation;
using Compiler.Lexer;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;
using Compiler.Serialization;
using Compiler.SourceFiles;
using Compiler.Tests.Storage;
using Compiler.Util;

namespace Compiler.Tests.Snapshots;

public class SnapshotTests
{
    private Ball Compile(string sourceText)
    {
        var lexer = new CompilerLexer(sourceText);

        IEnumerable<Token> tokens = lexer.GetTokens();
        
        var parser = new CompilerParser(tokens.ToList());
        AstTree ast = parser.Parse();

        var analyzer = new SymbolTableBuilder();
        ast.Root.Accept(analyzer);

        var analyzerSecondStage = new SemanticAnalyzer(analyzer.SymbolTable);
        ast.Root.Accept(analyzerSecondStage);

        SymbolTable scope = analyzer.SymbolTable;
        
        var generator = new Generator(new CompilerSettings()
        {
            Version = [0,0,0]
        });
        
        Ball ball = generator.Generate(ast, scope);

        return ball;
    }

    [Fact]
    public void FirstExampleTest()
    {
        // Arrange
        // Act
        var result = Compile("""
                             =/
                             Our first simple program on CZFF 
                             /=
                             func void Main() {
                                 var int a = 2;
                                 var int b = 3;
                                 var int res = a + b;
                                 print res;
                                 return;
                             }
                             """);

        var expectedBall = BallStore.ReturnBall("SimpleBall");
        
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