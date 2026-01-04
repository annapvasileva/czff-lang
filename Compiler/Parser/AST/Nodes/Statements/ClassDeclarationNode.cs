namespace Compiler.Parser.AST.Nodes.Statements;

public class ClassDeclarationNode : DeclarationNode
{
    public string Name { get; }
    // think about class, methods, fields tables
    public IList<VariableDeclarationNode> Fields { get; }
    public IList<FunctionDeclarationNode> Methods { get; }

    public ClassDeclarationNode(string name, IList<VariableDeclarationNode> fields,
        IList<FunctionDeclarationNode> methods)
    {
        Name = name;
        Fields = fields;
        Methods = methods;
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }
}