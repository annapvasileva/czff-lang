using Compiler.Operations;
using Compiler.Parser;
using Compiler.Parser.AST.Nodes;
using Compiler.Parser.AST.Nodes.Core;
using Compiler.Parser.AST.Nodes.Expressions;
using Compiler.Parser.AST.Nodes.Statements;
using Compiler.SemanticAnalysis.Models;
using Compiler.SourceFiles;
using Compiler.SourceFiles.Constants;
using Compiler.Util;

namespace Compiler.Generation;

public class BallGeneratingVisitor(Ball target, SymbolTable scope) : INodeVisitor
{
    private readonly Ball _target = target;
    private Function? _currentFunction;
    private SymbolTable _scope = scope;
    
    public void Visit(LiteralExpressionNode literalExpressionNode)
    {
        ConstantItem constant;
        switch (literalExpressionNode.Type)
        {
            case LiteralType.IntegerLiteral:
                int number = Convert.ToInt32(literalExpressionNode.Value);
                constant = new IntConstant(number);
            
                break;
            case LiteralType.Integer64Literal:
                long number64 = Convert.ToInt64(literalExpressionNode.Value);
                constant = new Int64Constant(number64);
            
                break;
            case LiteralType.BooleanLiteral:
                bool flag;
                switch (literalExpressionNode.Value)
                {
                    case "true":
                        flag = true;
                        break;
                    case "false": 
                        flag = false;
                        break;
                    default:
                        throw new GeneratorException($"Literal {literalExpressionNode.Value} is not a boolean.");
                }
                constant = new BoolConstant(flag);
                break;
            case LiteralType.StringLiteral:
                string line = literalExpressionNode.Value;
                constant = new StringConstant(line);
                break;
            default:
                throw new NotImplementedException();
        }
        
        int idx = _target.ConstantPool.GetIndexOrAddConstant(constant);
        
        _currentFunction!.Operations.Add(new Ldc(idx));
    }

    public void Visit(IdentifierExpressionNode identifierExpressionNode)
    {
        Symbol symbol = _scope.Lookup(identifierExpressionNode.Name);

        if (symbol is VariableSymbol variable)
        {
            _currentFunction!.Operations.Add(new Ldv(variable.Index));
        }
        else
        {
            throw new GeneratorException($"Symbol {identifierExpressionNode.Name} is not a variable.");
        }
    }

    public void Visit(SimpleTypeNode simpleTypeNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ArrayTypeNode arrayTypeNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BinaryExpressionNode binaryExpressionNode)
    {
        binaryExpressionNode.LeftExpression.Accept(this);
        binaryExpressionNode.RightExpression.Accept(this);

        switch (binaryExpressionNode.BinaryOperatorType)
        {
            case BinaryOperatorType.Addition:
                _currentFunction!.Operations.Add(new Add());
                break;
            case BinaryOperatorType.Subtraction:
                _currentFunction!.Operations.Add(new Sub());
                break;
            case BinaryOperatorType.Multiplication:
                _currentFunction!.Operations.Add(new Mul());
                break;
            case BinaryOperatorType.Division:
                _currentFunction!.Operations.Add(new Div());
                break;
            case BinaryOperatorType.Less:
                _currentFunction!.Operations.Add(new Lt());
                break;
            case BinaryOperatorType.LessOrEqual:
                _currentFunction!.Operations.Add(new Leq());
                break;
            case BinaryOperatorType.Equal:
                _currentFunction!.Operations.Add(new Eq());
                break;
            case BinaryOperatorType.Greater:
                _currentFunction!.Operations.Add(new Swap());
                _currentFunction!.Operations.Add(new Lt());
                break;
            case BinaryOperatorType.GreaterOrEqual:
                _currentFunction!.Operations.Add(new Swap());
                _currentFunction!.Operations.Add(new Leq());
                break;
            case BinaryOperatorType.NotEqual:
                _currentFunction!.Operations.Add(new Eq());
                _currentFunction!.Operations.Add(new Neg());
                break;
            case BinaryOperatorType.LogicalOr:
                _currentFunction!.Operations.Add(new Lor());
                break;
            case BinaryOperatorType.LogicalAnd:
                _currentFunction!.Operations.Add(new Land());
                break;
            case BinaryOperatorType.Modulus:
                _currentFunction!.Operations.Add(new Mod());
                break;
            default:
                throw new GeneratorException("Binary operator not supported.");
        }
    }

    public void Visit(UnaryExpressionNode unaryExpressionNode)
    {
        unaryExpressionNode.Expression.Accept(this);

        switch (unaryExpressionNode.UnaryOperatorType)
        {
            case UnaryOperatorType.Minus:
                _currentFunction!.Operations.Add(new Min());
                break;
            case UnaryOperatorType.Negation:
                _currentFunction!.Operations.Add(new Neg());
                break;
            default:
                throw new GeneratorException("Unary operator not supported.");
        }
    }

    public void Visit(FunctionCallExpressionNode functionCallExpressionNode)
    {
        foreach (var arg in functionCallExpressionNode.Arguments)
        {
            arg.Accept(this);
        }

        Symbol symbol = _scope.Lookup(functionCallExpressionNode.Name);
        if (symbol is not FunctionSymbol function)
        {
            throw new GeneratorException($"Symbol {functionCallExpressionNode.Name} is not a function.");
        }
        
        _currentFunction!.Operations.Add(new Call(function.Index));
    }

    public void Visit(VariableDeclarationNode variableDeclarationNode)
    {
        if (variableDeclarationNode.Expression == null)
        {
            return;
        } 
        
        variableDeclarationNode.Expression.Accept(this);

        Symbol symbol = _scope.Lookup(variableDeclarationNode.Name);
        
        if (symbol is VariableSymbol variable)
        {
            _currentFunction!.Operations.Add(new Store(variable.Index));
        }
        else
        {
            throw new GeneratorException($"Symbol {variableDeclarationNode.Name} is not a variable.");
        }
    }

    public void Visit(FunctionDeclarationNode functionDeclarationNode)
    {
        _currentFunction = new Function();
        Symbol symbol = _scope.Lookup(functionDeclarationNode.Name);
        if (symbol is not FunctionSymbol functionSymbol)
        {
            throw new GeneratorException($"Symbol {functionDeclarationNode.Name} is not a function.");
        }
        _scope = functionDeclarationNode.Body.Scope;
        
        // Name
        ConstantItem item =  new StringConstant(functionSymbol.Name);
        int idx = _target.ConstantPool.GetIndexOrAddConstant(item);
    
        _currentFunction.NameIndex = idx;
        
        // Parameters
        functionDeclarationNode.Parameters.Accept(this);

        // Return Type
        string returnDescriptor = functionSymbol.ReturnType;
    
        item =  new StringConstant(returnDescriptor);
        idx = _target.ConstantPool.GetIndexOrAddConstant(item);
        _currentFunction.ReturnTypeIndex = idx;
    
        // Body
        functionDeclarationNode.Body.Accept(this);

        _currentFunction.MaxStackUsed = 0;
        _currentFunction.LocalsLength = functionSymbol.LocalsLength;
        _currentFunction.MaxStackUsed = 0;
        
        _target.FunctionPool.AddFunction(_currentFunction);
        
        _scope = functionDeclarationNode.Body.Scope.Parent!;
        _currentFunction = null;
    }

    public void Visit(ClassDeclarationNode classDeclarationNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(FunctionParametersNode functionParametersNode)
    {
        string descriptor = "";
        
        foreach (var parameter in functionParametersNode.Parameters)
        {
            descriptor += parameter.Type.GetName;
            Symbol symbol = _scope.Lookup(parameter.Name);
            if (symbol is not VariableSymbol variableSymbol)
            {
                throw new GeneratorException($"Symbol {parameter.Name} is not a variable.");
            }
            
            _currentFunction!.Operations.Add(new Store(variableSymbol.Index));
        }
        
        int idx = _target.ConstantPool.GetIndexOrAddConstant(new StringConstant(descriptor));
        
        _currentFunction!.ParameterDescriptorIndex = idx;
    }

    public void Visit(ExpressionStatementNode expressionStatementNode)
    {
        expressionStatementNode.Expression.Accept(this);
    }

    public void Visit(ArrayAssignmentStatementNode arrayAssigmentStatementNode)
    {
        var array = arrayAssigmentStatementNode.Left;
        
        array.Array.Accept(this);
        array.Index.Accept(this);
            
        arrayAssigmentStatementNode.Right.Accept(this);
            
        _currentFunction!.Operations.Add(new Stelem());
    }

    public void Visit(IdentifierAssignmentStatementNode identifierAssignmentStatementNode)
    {
        string name = identifierAssignmentStatementNode.Left.Name;
        
        Symbol symbol = _scope.Lookup(name);
        if (symbol is VariableSymbol variable)
        {
            identifierAssignmentStatementNode.Right.Accept(this);
            int idx = variable.Index;
            _currentFunction!.Operations.Add(new Store(idx));
        }
        else
        {
            throw new GeneratorException($"Symbol {name} is not a variable.");
        }
    }

    public void Visit(BlockNode blockNode)
    {
        IList<StatementNode> statements = blockNode.Statements;
        
        foreach (var statement in statements)
        {
            statement.Accept(this);
        }
    }

    public void Visit(ArrayCreationExpressionNode arrayCreationExpressionNode)
    {
        arrayCreationExpressionNode.Size.Accept(this);

        var item = new StringConstant(arrayCreationExpressionNode.ElementType.GetName);
        int idx = _target.ConstantPool.GetIndexOrAddConstant(item);
        
        _currentFunction!.Operations.Add(new Newarr(idx));
    }

    public void Visit(ArrayIndexExpressionNode arrayIndexExpressionNode)
    {
        arrayIndexExpressionNode.Array.Accept(this);
        arrayIndexExpressionNode.Index.Accept(this);
        
        _currentFunction!.Operations.Add(new Ldelem());
    }

    public void Visit(MemberAccessNode memberAccessNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(BreakStatementNode breakStatementNode)
    {
        _currentFunction!.Operations.Add(new Jmp(-1));
    }

    public void Visit(ContinueStatementNode continueStatementNode)
    {
        _currentFunction!.Operations.Add(new Jmp(-2));
    }

    public void Visit(ReturnStatementNode returnStatementNode)
    {
        if (returnStatementNode.Expression != null)
        {
            returnStatementNode.Expression.Accept(this);
        }

        _currentFunction!.Operations.Add(new Ret());
    }

    public void Visit(IfStatementNode ifStatementNode)
    {
        _scope = ifStatementNode.IfBlock.Scope;
        
        ifStatementNode.Condition.Accept(this);
        
        Jz jumpToElse = new Jz(-1);
        _currentFunction!.Operations.Add(jumpToElse);

        ifStatementNode.IfBlock.Accept(this);

        int endIndex;
        if (ifStatementNode.ElseBlock != null)
        {
            Jmp jumpOverElse = new Jmp(-1);
            _currentFunction.Operations.Add(jumpOverElse);
            
            int elseIndex = _currentFunction.Operations.Count;
            jumpToElse.JumpIndex = elseIndex;
            
            ifStatementNode.ElseBlock.Accept(this);
            
            endIndex = _currentFunction.Operations.Count;
            jumpOverElse.JumpIndex = endIndex;
        }
        else
        {
            endIndex = _currentFunction.Operations.Count;
            jumpToElse.JumpIndex = endIndex;
        }
        
        _scope = _scope.Parent!;
    }

    public void Visit(ElifStatementNode elifStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileStatementNode whileStatementNode)
    {
        _scope = whileStatementNode.Body.Scope;
        
        int startIndex = _currentFunction!.Operations.Count;
        whileStatementNode.Condition.Accept(this);
        
        Jz jumpToEnd = new Jz(-1);
        _currentFunction.Operations.Add(jumpToEnd);

        whileStatementNode.Body.Accept(this);
        
        _currentFunction.Operations.Add(new Jmp(startIndex));
        
        int endIndex = _currentFunction.Operations.Count;
        jumpToEnd.JumpIndex = endIndex;
        
        foreach (var op in _currentFunction.Operations.Slice(startIndex,
                     _currentFunction.Operations.Count - startIndex)) 
        {
            if (op is JumpOperation jmp)
            {
                if (jmp.JumpIndex == -1)
                {
                    jmp.JumpIndex = endIndex;
                }

                if (jmp.JumpIndex == -2)
                {
                    jmp.JumpIndex = startIndex;
                }
            }
        }
        _scope = _scope.Parent!;
    }

    public void Visit(ForStatementNode forStatementNode)
    {
        _scope = forStatementNode.Body.Scope;
        forStatementNode.Init.Accept(this);
        
        int startIndex = _currentFunction!.Operations.Count;
        forStatementNode.Condition.Accept(this);
        
        Jz jumpToEnd = new Jz(-1);
        _currentFunction.Operations.Add(jumpToEnd);

        forStatementNode.Body.Accept(this);
        
        forStatementNode.Post.Accept(this);
        
        _currentFunction.Operations.Add(new Jmp(startIndex));
        
        int endIndex = _currentFunction.Operations.Count;
        jumpToEnd.JumpIndex = endIndex;
        
        foreach (var op in _currentFunction.Operations.Slice(startIndex,
                     _currentFunction.Operations.Count - startIndex)) 
        {
            if (op is JumpOperation jmp)
            {
                if (jmp.JumpIndex == -1)
                {
                    jmp.JumpIndex = endIndex;
                }

                if (jmp.JumpIndex == -2)
                {
                    jmp.JumpIndex = startIndex;
                }
            }
        }
        _scope = _scope.Parent!;
    }

    public void Visit(PrintStatementNode printStatementNode)
    {
        printStatementNode.Expression.Accept(this);
        
        _currentFunction!.Operations.Add(new Print());
    }

    public void Visit(ProgramNode programNode)
    {
        foreach (var func in programNode.Functions)
        {
            func.Accept(this);
        }
        
        // There will be classes
    }
}