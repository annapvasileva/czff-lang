using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis;
using Compiler.SemanticAnalysis.Models;

namespace Compiler.Tests.SemanticAnalysisTests;

public class SemanticAnalyzerTests
{
    [Fact]
    public void OneEmptyFunctionTest()
    {
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode>
                        {
                            new ReturnStatementNode(null)
                        })
                )
            }));
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }
    
    [Fact]
    public void OneFunctionWithVariableDeclarationsTest()
    {
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode>
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
                            new ReturnStatementNode(null)
                        })
                )
            }));
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }

    [Fact]
    public void SeveralFunctionsWithVariableDeclarationsTest()
    {
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode>
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
                            new ReturnStatementNode(null)
                        })
                ),
                new (
                    new SimpleTypeNode("void"),
                    "foo",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode>
                        {
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "a",
                                new LiteralExpressionNode("6", LiteralType.IntegerLiteral)),
                            new VariableDeclarationNode(
                                new SimpleTypeNode("int"),
                                "b",
                                new LiteralExpressionNode("5", LiteralType.IntegerLiteral)),
                            new ReturnStatementNode(null)
                        }
                    )
                )
            }));
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }

    [Fact]
    public void ArrayDeclarationAndIndexingTest()
    {
        var ast = new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new (
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode { },
                    new BlockNode(
                        new List<StatementNode>
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
                                new ArrayTypeNode(
                                    new SimpleTypeNode("int")), 
                                "arr",
                                new ArrayCreationExpressionNode(
                                    new SimpleTypeNode("int"),
                                    new LiteralExpressionNode("4", LiteralType.IntegerLiteral))),
                            new ArrayAssignmentStatementNode(
                                new ArrayIndexExpressionNode(
                                    new IdentifierExpressionNode("arr"),
                                    new LiteralExpressionNode("0", LiteralType.IntegerLiteral)),
                                new IdentifierExpressionNode("a")),
                            new ArrayAssignmentStatementNode(
                                new ArrayIndexExpressionNode(
                                    new IdentifierExpressionNode("arr"),
                                    new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                                new IdentifierExpressionNode("b")),
                            new ReturnStatementNode(null)
                        })
                )
            }));
        var symbolTableBuilder = new SymbolTableBuilder();
        ast.Root.Accept(symbolTableBuilder);
        var semanticAnalyzer = new SemanticAnalyzer(symbolTableBuilder.SymbolTable);

        var exception = Record.Exception(() => ast.Root.Accept(semanticAnalyzer));
        Assert.Null(exception);
    }
}