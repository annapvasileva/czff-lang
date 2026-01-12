using Compiler.Lexer;
using Compiler.Parser;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis.Models;

namespace Compiler.SemanticAnalysis;

public class SemanticAnalyzer(SymbolTable scope) : INodeVisitor
{
    private SymbolTable _scope = scope;
    private Stack<HashSet<VariableSymbol>> _initStack = new();
    private int _loopCount = 0;

    public void Visit(LiteralExpressionNode literalExpressionNode) { }

    public void Visit(IdentifierExpressionNode identifierExpressionNode)
    {
        var symb = _scope.Lookup(identifierExpressionNode.Name);
        if (symb is VariableSymbol variableSymbol && !IsInitialized(variableSymbol))
        {
            throw new SemanticException($"Variable {identifierExpressionNode.Name} is not initialized");
        }
    }

    public void Visit(SimpleTypeNode simpleTypeNode)
    {
        if (IsIntType(simpleTypeNode.GetName) || simpleTypeNode.GetName == "B;" || simpleTypeNode.GetName == "void;")
        {
            return;
        }
        // тут потом проверять существование класса, если тип - это класс
        
        throw new Exception($"Type {simpleTypeNode.Name} does not exist");
    }

    public void Visit(ArrayTypeNode arrayTypeNode) { }

    public void Visit(BinaryExpressionNode binaryExpressionNode)
    {
        binaryExpressionNode.LeftExpression.Accept(this);
        binaryExpressionNode.RightExpression.Accept(this);

        // type checking
        GetBinaryExpressionType(binaryExpressionNode);
    }

    public void Visit(UnaryExpressionNode unaryExpressionNode)
    {
        unaryExpressionNode.Expression.Accept(this);
        // type checking
        GetUnaryExpressionType(unaryExpressionNode);
    }

    public void Visit(FunctionCallExpressionNode functionCallExpressionNode)
    {
        var symb =  _scope.Lookup(functionCallExpressionNode.Name);
        if (!(symb is FunctionSymbol))
            throw new SemanticException($"{functionCallExpressionNode.Name} is not function. You can call only function");

        var funcSymb = (FunctionSymbol)symb;
        if (funcSymb.Parameters.Count != functionCallExpressionNode.Arguments.Count)
        {
            throw new SemanticException($"Function {functionCallExpressionNode.Name} must have the same number of parameters - {funcSymb.Parameters.Count}");
        }
        
        var paramsCount = funcSymb.Parameters.Count;
        for (int i = 0; i < paramsCount; i++)
        {
            var paramType = funcSymb.Parameters[i].Type;
            var argType = GetExpressionType(functionCallExpressionNode.Arguments[i]);
            if (argType != paramType)
            {
                throw new SemanticException($"Invalid argument '{funcSymb.Parameters[i].Name}' type. Expected '{paramType}' but found '{argType}'");
            }
        }
    }

    public void Visit(VariableDeclarationNode variableDeclarationNode)
    {
        // check type
        variableDeclarationNode.Type.Accept(this);
        if (variableDeclarationNode.Expression != null)
        {
            variableDeclarationNode.Expression.Accept(this);
            var symbol = _scope.Lookup(variableDeclarationNode.Name);
            _initStack.Peek().Add((VariableSymbol)symbol);

            var varTypeName = variableDeclarationNode.Type.GetName;
            var initExpressionTypeName = GetExpressionType(variableDeclarationNode.Expression);
            if (varTypeName != initExpressionTypeName)
            {
                throw new SemanticException($"Variable {variableDeclarationNode.Name}: type - {initExpressionTypeName} does not match {varTypeName}");
            }
        }
        if (variableDeclarationNode.Type is ArrayTypeNode && variableDeclarationNode.Expression == null)
        {

            throw new SemanticException("You must provide an array size");
        }
        
        if (CheckVoidType(variableDeclarationNode.Type))
        {
            throw new SemanticException("Variable declaration must not have void type");
        }
    }

    public void Visit(FunctionDeclarationNode functionDeclarationNode)
    {
        _scope = functionDeclarationNode.Body.Scope;
        EnterScope();
        functionDeclarationNode.Parameters.Accept(this);
        functionDeclarationNode.Body.Accept(this);
        var expectedReturnType = functionDeclarationNode.ReturnType.GetName;
        var returnType =
            GetExpressionType(((ReturnStatementNode)functionDeclarationNode.Body.Statements.Last()).Expression);
        if (expectedReturnType != returnType)
        {
            throw new SemanticException($"Function {functionDeclarationNode.Name} expected return type is {expectedReturnType} but got {returnType}");
        }
        ExitScope();
        _scope = _scope.Parent!;
    }

    public void Visit(ClassDeclarationNode classDeclarationNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionParametersNode functionParametersNode)
    {
        foreach (var param in functionParametersNode.Parameters)
        {
            param.Type.Accept(this);
            var symbol = _scope.Lookup(param.Name);
            if (symbol is VariableSymbol variableSymbol)
            {
                _initStack.Peek().Add(variableSymbol);
            }
            else
            {
                throw new SemanticException($"Variable {param.Name} not found");
            }
        }
    }

    public void Visit(ExpressionStatementNode expressionStatementNode)
    {
        if (expressionStatementNode.Expression is FunctionCallExpressionNode functionCallExpressionNode)
        {
            var funcName = functionCallExpressionNode.Name;
            var funcSymb = _scope.Lookup(funcName);
            if (funcSymb is FunctionSymbol functionSymbol)
            {
                if (functionSymbol.ReturnType != "void;")
                {
                    throw new SemanticException("Result of expression must be used");
                }
            }
            else
            {
                throw new SemanticException($"{funcName} is not function. You can call only function");
            }
            expressionStatementNode.Expression.Accept(this);
        }
        else
        {
            throw new SemanticException("Result of expression must be used");
        }
    }

    public void Visit(IdentifierAssignmentStatementNode assigmentStatementNode)
    {
        var symb = _scope.Lookup(assigmentStatementNode.Left.Name);
        if (symb is VariableSymbol variableSymbol)
        {
            _initStack.Peek().Add(variableSymbol);
        }
        else
        {
            throw new SemanticException($"Expected that {assigmentStatementNode.Left.Name} is a variable");
        }
        
        assigmentStatementNode.Right.Accept(this);

        var left = GetExpressionType(assigmentStatementNode.Left);
        var right = GetExpressionType(assigmentStatementNode.Right);
        if (left != right)
        {
            throw new SemanticException($"Assigment: identifier have type: {left} but got type: {right}");
        }
    }

    public void Visit(ArrayAssignmentStatementNode assigmentStatementNode)
    {
        assigmentStatementNode.Left.Accept(this);
        assigmentStatementNode.Right.Accept(this);
        var left = GetExpressionType(assigmentStatementNode.Left);
        var right = GetExpressionType(assigmentStatementNode.Right);
        if (left != right)
        {
            throw new SemanticException($"Assigment: array have type: {left} but got type: {right}");
        }
    }

    public void Visit(BlockNode blockNode)
    {
        foreach (var statement in blockNode.Statements)
        {
            statement.Accept(this);
        }
    }

    public void Visit(ArrayCreationExpressionNode arrayCreationExpressionNode)
    {
        arrayCreationExpressionNode.ElementType.Accept(this);
        arrayCreationExpressionNode.Size.Accept(this);
        
        var sizeType = GetExpressionType(arrayCreationExpressionNode.Size);
        if (!IsIntType(sizeType))
            throw new SemanticException($"Array creation: size type is {sizeType} but must be int");
    }

    public void Visit(ArrayIndexExpressionNode arrayIndexExpressionNode)
    {
        arrayIndexExpressionNode.Array.Accept(this);
        arrayIndexExpressionNode.Index.Accept(this);
        
        var indexType = GetExpressionType(arrayIndexExpressionNode.Index);
        if (!IsIntType(indexType))
            throw new SemanticException($"Array index: index type is {indexType} but must be int");
    }

    public void Visit(MemberAccessNode memberAccessNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BreakStatementNode breakStatementNode)
    {
        if (!IsInLoop())
        {
            throw new SemanticException("Break statement is not in loop");
        }
    }

    public void Visit(ContinueStatementNode continueStatementNode)
    {
        if (!IsInLoop())
        {
            throw new SemanticException("Continue statement is not in loop");
        }
    }

    public void Visit(ReturnStatementNode returnStatementNode)
    {
        if (returnStatementNode.Expression != null)
        {
            returnStatementNode.Expression.Accept(this);
        }
    }

    public void Visit(IfStatementNode ifStatementNode)
    {
        _scope = ifStatementNode.IfBlock.Scope;
        EnterScope();
        var conditionType = GetExpressionType(ifStatementNode.Condition);
        if (conditionType != "B;")
        {
            throw new SemanticException($"If statement: condition type is {conditionType}. Expected: B;");
        }
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
        var conditionType = GetExpressionType(whileStatementNode.Condition);
        if (conditionType != "B;")
        {
            throw new SemanticException($"While statement: condition type is {conditionType}. Expected: B;");
        }
        whileStatementNode.Condition.Accept(this);
        EnterLoop();
        whileStatementNode.Body.Accept(this);
        ExitLoop();
        ExitScope();
        _scope = _scope.Parent!;
    }

    public void Visit(ForStatementNode forStatementNode)
    {
        _scope = forStatementNode.Body.Scope;
        EnterScope();
        if (forStatementNode.Init.Expression == null)
        {
            throw new SemanticException("For statement: you must init variable");
        }

        var conditionType = GetExpressionType(forStatementNode.Condition);
        if (conditionType != "B;")
        {
            throw new SemanticException($"For statement: condition type is {conditionType}. Expected: B;");
        }
        forStatementNode.Init.Accept(this);
        forStatementNode.Condition.Accept(this);
        forStatementNode.Post.Accept(this);
        EnterLoop();
        forStatementNode.Body.Accept(this);
        ExitLoop();
        ExitScope();
        _scope = _scope.Parent!;
    }

    public void Visit(PrintStatementNode printStatementNode)
    {
        // подумать о том, что можно вывести
        // по необходимости добавить проверку
        printStatementNode.Expression.Accept(this);
    }

    public void Visit(ProgramNode programNode)
    {
        foreach (var func in programNode.Functions)
        {
            func.Accept(this);
        }
    }
    
    public void EnterScope()
    {
        _initStack.Push(new HashSet<VariableSymbol>());
    }

    public void ExitScope()
    {
        foreach (var symbol in _scope.Symbols)
        {
            if (symbol.Value is VariableSymbol variableSymbol && !_initStack.First().Contains(variableSymbol))
            {
                throw new SemanticException($"Variable {variableSymbol.Name} is not initialized");
            }
        }
        _initStack.Pop();
    }
    
    private bool IsInitialized(VariableSymbol symbol)
    {
        foreach (var scope in _initStack)
        {
            if (scope.Contains(symbol))
                return true;
        }

        return false;
    }

    private string GetExpressionType(ExpressionNode? expressionNode)
    {
        if (expressionNode is null)
        {
            return "void;";
        }
        if (expressionNode is ArrayCreationExpressionNode arrayCreationExpression)
        {
            return GetArrayCreationType(arrayCreationExpression);
        }

        if (expressionNode is ArrayIndexExpressionNode arrayIndexExpression)
        {
            return GetExpressionType(arrayIndexExpression.Array).Remove(0, 1); // to get a type
            // return GetArrayIndexType(arrayIndexExpression);
        }

        if (expressionNode is BinaryExpressionNode binaryExpression)
        {
            return GetBinaryExpressionType(binaryExpression);
        }

        if (expressionNode is IdentifierExpressionNode identifierExpressionNode)
        {
            return GetIdentifierType(identifierExpressionNode);
        }
        
        if (expressionNode is LiteralExpressionNode literalExpressionNode)
        {
            return GetLiteralType(literalExpressionNode);
        }

        if (expressionNode is UnaryExpressionNode unaryExpression)
        {
            return GetUnaryExpressionType(unaryExpression);
        }

        if (expressionNode is FunctionCallExpressionNode functionCallExpression)
        {
            return GetFunctionCallExpressionType(functionCallExpression);
        }

        throw new SemanticException("Unknown expression type");
    }

    private string GetArrayCreationType(ArrayCreationExpressionNode arrayCreationExpression)
    {
        return "[" + arrayCreationExpression.ElementType.GetName;
    }

    private string GetArrayIndexType(ArrayIndexExpressionNode arrayIndexExpression)
    {
        return GetExpressionType(arrayIndexExpression.Index);
    }

    private string GetBinaryExpressionType(BinaryExpressionNode binaryExpression)
    {
        var leftType = GetExpressionType(binaryExpression.LeftExpression);
        var rightType = GetExpressionType(binaryExpression.RightExpression);
        
        return binaryExpression.BinaryOperatorType switch
        {
           BinaryOperatorType.Addition or 
               BinaryOperatorType.Subtraction or 
               BinaryOperatorType.Multiplication or 
               BinaryOperatorType.Division or 
               BinaryOperatorType.Modulus =>
                GetArithmeticOperationType(leftType, rightType),
           BinaryOperatorType.Less or 
               BinaryOperatorType.Greater or 
               BinaryOperatorType.LessOrEqual or
               BinaryOperatorType.GreaterOrEqual or
               BinaryOperatorType.Equal or
               BinaryOperatorType.NotEqual =>
                GetComparisonOperationType(leftType, rightType, binaryExpression.BinaryOperatorType),
            BinaryOperatorType.LogicalOr or 
                BinaryOperatorType.LogicalAnd =>
                 GetLogicalOperationType(leftType, rightType),
            _ => throw new SemanticException($"Unsupported binary operator {binaryExpression.BinaryOperatorType}")
        };
    }

    private string GetIdentifierType(IdentifierExpressionNode identifierNode)
    {
        var symbol = _scope.Lookup(identifierNode.Name);
        if (symbol is VariableSymbol variableSymbol)
            return variableSymbol.Type;

        if (symbol is FunctionSymbol)
            throw new SemanticException("Functions as types are not supported now");
        
        throw new SemanticException($"Unknown identifier {identifierNode.Name}");
    }

    private string GetLiteralType(LiteralExpressionNode expressionNode)
    {
        switch (expressionNode.Type)
        {
            case LiteralType.IntegerLiteral:
                return "I;";
            case LiteralType.Integer64Literal:
                return "I8;";
            case LiteralType.BooleanLiteral:
                return "B;";
            case LiteralType.StringLiteral:
                return "string;";
            default:
                throw new SemanticException($"Unknown literal type {expressionNode.Type}");
        }
    }

    private string GetUnaryExpressionType(UnaryExpressionNode unaryExpression)
    {
        var expressionType = GetExpressionType(unaryExpression.Expression);
        if (unaryExpression.UnaryOperatorType == UnaryOperatorType.Minus && IsIntType(expressionType))
            return expressionType;
        if (unaryExpression.UnaryOperatorType == UnaryOperatorType.Negation && expressionType == "B;")
            return expressionType;
        throw new SemanticException($"Unsupported unary operation for operator {unaryExpression.UnaryOperatorType} and type {expressionType}");
    }

    private string GetFunctionCallExpressionType(FunctionCallExpressionNode functionCallExpression)
    {
        var symbol = _scope.Lookup(functionCallExpression.Name);
        if (symbol is FunctionSymbol functionSymbol)
        {
            return functionSymbol.ReturnType;
        }
        
        throw new SemanticException($"Unknown function {functionCallExpression.Name}");
    }

    private string GetArithmeticOperationType(string left, string right)
    {
        if (!IsIntType(left) || !IsIntType(right))
        {
            throw new SemanticException($"Now arithmetic operations only for int. Got: {left} and {right}");
        }

        if (left == right)
            return left;
        throw new SemanticException($"Different types in arithmetic exception: {left} and {right}");
    }

    private string GetComparisonOperationType(string left, string right, BinaryOperatorType operatorType)
    {
        if (operatorType != BinaryOperatorType.Equal && operatorType != BinaryOperatorType.NotEqual)
        {
            if (!IsIntType(left) || !IsIntType(right))
            {
                throw new SemanticException($"Now comparison operations only for int. Got: {left} and {right}");
            }

            return "B;";
        }

        if (left == right)
            return "B;";
        throw new SemanticException($"Different types in comparison: {left} and {right}");
    }
    
    private string GetLogicalOperationType(string left, string right)
    {
        if (left != "B;" || right != "B;")
        {
            throw new SemanticException($"Logical operations only for bool. Got: {left} and {right}");
        }

        return "B;";
    }
    
    private bool CheckVoidType(TypeAnnotationNode node)
    {
        if (node is SimpleTypeNode simpleTypeNode)
            return simpleTypeNode.Name == "void";
        if (node is ArrayTypeNode arrayTypeNode)
            return CheckVoidType(arrayTypeNode.ElementType);
        return false;
    }

    private bool IsIntType(string strType)
    {
        return strType == "I;" || strType == "I8;" || strType == "I16;";
    }

    // private bool CanAssign(string left, string right)
    // {
    //     if (IsIntType(left) && IsIntType(right))
    //     {
    //         return CompareInt(left, right);
    //     }
    //     return left == right;
    // }

    // private bool CompareInt(string left, string right)
    // {
    //     int leftInt = GetIntNumber(left);
    //     int rightInt = GetIntNumber(right);
    //     
    //     return leftInt >= rightInt;
    // }

    // private int GetIntNumber(string strInt)
    // {
    //     int result = 1;
    //     switch (strInt)
    //     {
    //         case "I16;":
    //             result = 3;
    //             break;
    //         case "I8;":
    //             result = 2;
    //             break;
    //         default:
    //             result = 1;
    //             break;
    //     }
    //     
    //     return result;
    // }
    //
    // private string GetBinaryIntOperationResult(string left, string right)
    // {
    //     int leftInt = GetIntNumber(left);
    //     int rightInt = GetIntNumber(right);
    //     int mx = leftInt;
    //     if (rightInt > leftInt)
    //         mx = rightInt;
    //
    //     if (mx == 3)
    //     {
    //         return "I16;";
    //     }
    //
    //     if (mx == 2)
    //     {
    //         return "I8;";
    //     }
    //     
    //     return "I;";
    // }
    
    private void EnterLoop() => _loopCount++;
    private void ExitLoop() => _loopCount--;
    private bool IsInLoop() => _loopCount > 0;
}