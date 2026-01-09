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
            default:
                throw new NotImplementedException();
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
            default:
                throw new NotImplementedException();
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
            
            _currentFunction!.Operations.Add(new Ldv(variableSymbol.Index));
        }
        
        int idx = _target.ConstantPool.GetIndexOrAddConstant(new StringConstant(descriptor));
        
        _currentFunction!.ParameterDescriptorIndex = idx;
    }

    public void Visit(ExpressionStatementNode expressionStatementNode)
    {
        throw new NotImplementedException();
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
        _scope = blockNode.Scope;
        
        foreach (var statement in statements)
        {
            statement.Accept(this);
        }

        _scope = _scope.Parent!;
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
        throw new NotImplementedException();
    }

    public void Visit(ContinueStatementNode continueStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ReturnStatementNode returnStatementNode)
    {
        _currentFunction!.Operations.Add(new Ret());
    }

    public void Visit(IfStatementNode ifStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ElifStatementNode elifStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(WhileStatementNode whileStatementNode)
    {
        throw new NotImplementedException();
    }

    public void Visit(ForStatementNode forStatementNode)
    {
        throw new NotImplementedException();
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