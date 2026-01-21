using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;

namespace Compiler.Tests.Storage;

public static class AstStore
{
    public static AstTree GetAst(string name)
    {
        switch (name)
        {
            case "OneEmptyFunction":
                return GetOneEmptyFunctionAst();
            case "OneFunctionWithVariableDeclarations":
                return GetOneFunctionWithVariableDeclarationsAst();
            case "SeveralFunctionsWithVariableDeclarations":
                return GetSeveralFunctionsWithVariableDeclarationsAst();
            case "ArrayDeclarationAndIndexing":
                return GetArrayDeclarationAndIndexingAst();
            case "SecondExample":
                return GetSecondExampleAst();
            case "ThirdExample":
                return GetThirdExampleAst();
            case "FourthExample":
                return GetFourthExampleAst();
            case "Int64":
                return GetInt64Int128Ast();
            default:
                throw new Exception($"Unknown ast name '{name}'");
        }
    }

    private static AstTree GetOneEmptyFunctionAst()
    {
        return new AstTree(new ProgramNode(
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
    }

    private static AstTree GetOneFunctionWithVariableDeclarationsAst()
    {
        return new AstTree(new ProgramNode(
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
    }

    private static AstTree GetSeveralFunctionsWithVariableDeclarationsAst()
    {
        return new AstTree(new ProgramNode(
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
    }

    private static AstTree GetArrayDeclarationAndIndexingAst()
    {
        return new AstTree(new ProgramNode(
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
    }

    private static AstTree GetSecondExampleAst()
    {
        return new AstTree(new ProgramNode(
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
                                        UnaryOperatorType.Minus,
                                        new BinaryExpressionNode(
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
                        })
                )
            }));
    }

    private static AstTree GetThirdExampleAst()
    {
        return new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>
            {
                new FunctionDeclarationNode(
                    new SimpleTypeNode("int"), 
                    "Sum",
                    new FunctionParametersNode(new List<FunctionParametersNode.Variable>()
                    {
                        new FunctionParametersNode.Variable("a", new SimpleTypeNode("int")),
                        new FunctionParametersNode.Variable("b", new SimpleTypeNode("int"))
                    }),
                    new BlockNode(new List<StatementNode>()
                    {
                        new ReturnStatementNode(new BinaryExpressionNode(
                            new IdentifierExpressionNode("a"),
                            new IdentifierExpressionNode("b"),
                            BinaryOperatorType.Addition)),
                    })
                ),
                new FunctionDeclarationNode(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode(),
                    new BlockNode(new List<StatementNode>()
                    {
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int"),
                            "x",
                            new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int"),
                            "y",
                            new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int"),
                            "z",
                            new FunctionCallExpressionNode("Sum", new List<ExpressionNode>(){
                                new IdentifierExpressionNode("x"),
                                new IdentifierExpressionNode("y")
                            })),
                        new PrintStatementNode(new IdentifierExpressionNode("z")),
                        new ReturnStatementNode(null),
                    })
                )
            }));
    }

    private static AstTree GetFourthExampleAst()
    {
        return new AstTree(new ProgramNode(
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
                            })),
                        new IfStatementNode(
                            new BinaryExpressionNode(
                                new IdentifierExpressionNode("x"),
                                new LiteralExpressionNode("10", LiteralType.IntegerLiteral),
                                BinaryOperatorType.Less),
                            new BlockNode(new List<StatementNode>()
                            {
                                new PrintStatementNode(
                                    new LiteralExpressionNode("1", LiteralType.IntegerLiteral)),
                            }),
                            new List<ElifStatementNode>(),
                            new BlockNode(new List<StatementNode>()
                            {
                                new PrintStatementNode(
                                    new LiteralExpressionNode("2", LiteralType.IntegerLiteral)),
                            })),
                        new ReturnStatementNode(null),
                    })
                )
            }));
    }

    private static AstTree GetInt64Int128Ast()
    {
        return new AstTree(new ProgramNode(
            new List<FunctionDeclarationNode>()
            {
                new(
                    new SimpleTypeNode("void"),
                    "Main",
                    new FunctionParametersNode() { },
                    new BlockNode(new List<StatementNode>()
                    {
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int64"),
                            "a",
                            new LiteralExpressionNode("10", LiteralType.Integer64Literal)),
                        new VariableDeclarationNode(
                            new SimpleTypeNode("int64"),
                            "c"),
                        new IdentifierAssignmentStatementNode(
                            new IdentifierExpressionNode("c"),
                            new BinaryExpressionNode(
                                new LiteralExpressionNode("2", LiteralType.Integer64Literal),
                                new IdentifierExpressionNode("a"),
                                BinaryOperatorType.Multiplication)),
                        new ReturnStatementNode(null)
                    })
                )
            }));
    }
}