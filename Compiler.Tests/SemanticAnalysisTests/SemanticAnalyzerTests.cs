using Compiler.Lexer;
using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;
using Compiler.Tests.Storage;

namespace Compiler.Tests.SemanticAnalysisTests;

public class SemanticAnalyzerTests
{
    [Fact]
    public void OneEmptyFunctionTest()
    {
        var ast = AstStore.GetAst("OneEmptyFunction");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }
    
    [Fact]
    public void OneFunctionWithVariableDeclarationsTest()
    {
        var ast = AstStore.GetAst("OneFunctionWithVariableDeclarations");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }

    [Fact]
    public void SeveralFunctionsWithVariableDeclarationsTest()
    {
        var ast = AstStore.GetAst("SeveralFunctionsWithVariableDeclarations");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }

    [Fact]
    public void ArrayDeclarationAndIndexingTest()
    {
        var ast = AstStore.GetAst("ArrayDeclarationAndIndexing");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }

    [Fact]
    public void SecondExampleTest()
    {
        var ast = AstStore.GetAst("SecondExample");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }
    
    [Fact]
    public void ThirdExampleTest()
    {
        var ast = AstStore.GetAst("ThirdExample");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }
    
    [Fact]
    public void OperationsTest()
    {
        string source = """
                        func void Main() {
                            var bool flag = 1 == 3;
                            var bool flag2 = 1 < 6;
                            var bool flag3 = !flag;
                            var bool flag4 = !flag;
                            var bool flag5 = !flag && (2 > 5) || (1 < 7);
                            return;
                        }
                        """;
        var lexer = new CompilerLexer(source);
        var tokens = lexer.GetTokens().ToList();
        var parser = new CompilerParser(tokens);
        var ast = parser.Parse();
        var builder = new SymbolTableBuilder();
        ast.Root.Accept(builder);
        var semanticAnalyzer = new SemanticAnalyzer(builder.SymbolTable);
        
        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }
    
    [Fact]
    public void FourthExampleTest()
    {
        var ast = AstStore.GetAst("FourthExample");
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }
}