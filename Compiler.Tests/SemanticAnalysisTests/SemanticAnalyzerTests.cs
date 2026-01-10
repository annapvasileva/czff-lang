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
}