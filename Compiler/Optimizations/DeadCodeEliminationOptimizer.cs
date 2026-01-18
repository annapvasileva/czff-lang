using Compiler.Parser;
using Compiler.Parser.AST;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis.Models;

namespace Compiler.Optimizations;

public class DeadCodeEliminationOptimizer(SymbolTable scope) : INodeVisitor
{
    private SymbolTable _scope = scope;
    private bool _terminated = false;
    private Stack<HashSet<string>> _usedStack = new();
    private Dictionary<string, HashSet<string>> _callGraph = new();
    private HashSet<string> _calledFunctions = new();
    private HashSet<string> _reachableFunctions = new();
    private string _currentFunction;

    public void Visit(LiteralExpressionNode literalExpressionNode)
    { }

    public void Visit(IdentifierExpressionNode identifierExpressionNode)
    {
        var current = _scope;
        foreach (var used in _usedStack)
        {
            if (current.Symbols.ContainsKey(identifierExpressionNode.Name))
            {
                used.Add(identifierExpressionNode.Name);
                break;
            }
            
            current = current.Parent;
        }
    }

    public void Visit(SimpleTypeNode simpleTypeNode)
    { }

    public void Visit(ArrayTypeNode arrayTypeNode)
    {
        arrayTypeNode.ElementType.Accept(this);
    }

    public void Visit(BinaryExpressionNode binaryExpressionNode)
    {
        binaryExpressionNode.LeftExpression.Accept(this);
        binaryExpressionNode.RightExpression.Accept(this);
    }

    public void Visit(UnaryExpressionNode unaryExpressionNode)
    {
        unaryExpressionNode.Expression.Accept(this);
    }

    public void Visit(FunctionCallExpressionNode functionCallExpressionNode)
    {
        _calledFunctions.Add(functionCallExpressionNode.Name);
        _callGraph[_currentFunction].Add(functionCallExpressionNode.Name);
        foreach (ExpressionNode argument in functionCallExpressionNode.Arguments)
        {
            argument.Accept(this);
        }
    }

    public void Visit(VariableDeclarationNode variableDeclarationNode)
    {
        variableDeclarationNode.Type.Accept(this);
        if (variableDeclarationNode.Expression != null)
        {
            variableDeclarationNode.Expression.Accept(this);
        }
    }

    public void Visit(FunctionDeclarationNode functionDeclarationNode)
    {
        _scope = functionDeclarationNode.Body.Scope;
        EnterScope();
        _currentFunction = functionDeclarationNode.Name;
        _callGraph[_currentFunction] = new HashSet<string>();
        functionDeclarationNode.ReturnType.Accept(this);
        functionDeclarationNode.Parameters.Accept(this);
        functionDeclarationNode.Body.Accept(this);
        ExitScope();
        _scope = _scope.Parent!;
    }

    public void Visit(ClassDeclarationNode classDeclarationNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionParametersNode functionParametersNode)
    {
        foreach (var parameter in functionParametersNode.Parameters)
        {
            parameter.Type.Accept(this);
        }
    }

    public void Visit(ExpressionStatementNode expressionStatementNode)
    {
        expressionStatementNode.Expression.Accept(this);
    }

    public void Visit(IdentifierAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Left.Accept(this);
        assigmentStatementNode.Right.Accept(this);
    }

    public void Visit(ArrayAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Left.Accept(this);
        assigmentStatementNode.Right.Accept(this);
    }

    public void Visit(BlockNode blockNode)
    {
        _terminated = false;
        var reachableStatements = new List<StatementNode>();
        foreach (var statement in blockNode.Statements)
        {
            if (_terminated)
            {
                break;
            }
            statement.Accept(this);
            reachableStatements.Add(statement);
        }

        var newStatements = new List<StatementNode>();
        foreach (var statement in reachableStatements)
        {
            if (statement is VariableDeclarationNode variableDeclarationNode &&
                !_usedStack.Peek().Contains(variableDeclarationNode.Name))
            {
                continue;
            }
            newStatements.Add(statement);
        }
        blockNode.Statements = newStatements;
        _terminated = false;
    }

    public void Visit(ArrayCreationExpressionNode arrayCreationExpressionNode)
    {
        arrayCreationExpressionNode.ElementType.Accept(this);
        arrayCreationExpressionNode.Size.Accept(this);
    }

    public void Visit(ArrayIndexExpressionNode arrayIndexExpressionNode)
    {
        arrayIndexExpressionNode.Array.Accept(this);
        arrayIndexExpressionNode.Index.Accept(this);
    }

    public void Visit(MemberAccessNode memberAccessNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BreakStatementNode breakStatementNode)
    {
        _terminated = true;
    }

    public void Visit(ContinueStatementNode continueStatementNode)
    {
        _terminated = true;
    }

    public void Visit(ReturnStatementNode returnStatementNode)
    {
        _terminated = true;
    }

    public void Visit(IfStatementNode ifStatementNode)
    {
        _scope = ifStatementNode.IfBlock.Scope;
        EnterScope();
        ifStatementNode.Condition.Accept(this);
        ifStatementNode.IfBlock.Accept(this);
        ExitScope();
        _scope = _scope.Parent!;
        if (ifStatementNode.ElseBlock != null)
        {
            _scope = ifStatementNode.ElseBlock.Scope;
            EnterScope();
            ifStatementNode.ElseBlock.Accept(this);
            ExitScope();
            _scope = _scope.Parent!;
        }
    }

    public void Visit(ElifStatementNode elifStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileStatementNode whileStatementNode)
    {
        _scope = whileStatementNode.Body.Scope;
        EnterScope();
        whileStatementNode.Condition.Accept(this);
        whileStatementNode.Body.Accept(this);
        ExitScope();
        _scope = _scope.Parent!;
    }

    public void Visit(ForStatementNode forStatementNode)
    {
        _scope = forStatementNode.Body.Scope;
        EnterScope();
        forStatementNode.Init.Accept(this);
        forStatementNode.Condition.Accept(this);
        forStatementNode.Post.Accept(this);
        forStatementNode.Body.Accept(this);
        ExitScope();
        _scope = _scope.Parent!;
    }

    public void Visit(PrintStatementNode printStatementNode)
    {
        printStatementNode.Expression.Accept(this);
    }

    public void Visit(ProgramNode programNode)
    {
        foreach (var func in programNode.Functions)
        {
            func.Accept(this);
        }

        MarkReachable("Main");
        
        programNode.Functions = programNode.Functions
            .Where(f => _reachableFunctions.Contains(f.Name))
            .ToList();
    }
    
    public void EnterScope()
    {
        _usedStack.Push(new HashSet<string>());
    }

    public void ExitScope()
    {
        _usedStack.Pop();
    }
    
    private void MarkReachable(string functionName)
    {
        if (_reachableFunctions.Contains(functionName))
            return;

        _reachableFunctions.Add(functionName);

        if (!_callGraph.ContainsKey(functionName))
            return;

        foreach (var callee in _callGraph[functionName])
            MarkReachable(callee);
    }
}