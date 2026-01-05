namespace Compiler.SemanticAnalysis.Models;

public class SymbolTableManager
{
    private SymbolTable _currentScope;
    private int _variableCounter;
    public int MaxCounter;
    
    public SymbolTableManager()
    {
        _currentScope = new SymbolTable(null); // global scope
        _variableCounter = 0;
        MaxCounter = 0;
    }

    public void EnterScope(bool resetCounter)
    {
        if (resetCounter)
        {
            _variableCounter = 0;
        }

        _currentScope = new SymbolTable(_currentScope);
    }
    
    public void ExitScope()
    {
        _variableCounter -= _currentScope.LocalCount;
        _currentScope = _currentScope.Parent;

        if (_currentScope.Parent == null && _variableCounter > 0)
        {
            throw new Exception("Variable counter has made a mistake");
        }
    }
    
    public void DeclareVariable(string name, string type)
    {
        var symbol = new VariableSymbol(name, type, _variableCounter);
        
        if (!_currentScope.Declare(symbol))
        {
            throw new SemanticException($"Variable {name} has been already declared");
        }
        
        _variableCounter++;
        if (MaxCounter < _variableCounter)
        {
            MaxCounter = _variableCounter;
        }
    }
    
    public void DeclareFunction(string name, string returnType)
    {
        var symbol = new FunctionSymbol(name, returnType);
        if (!_currentScope.Declare(symbol))
        {
            throw new SemanticException($"Function {name} has been already declared");
        }
    }
    
    public Symbol? Lookup(string name) => _currentScope.Lookup(name);
    
    public SymbolTable CurrentScope => _currentScope;
}