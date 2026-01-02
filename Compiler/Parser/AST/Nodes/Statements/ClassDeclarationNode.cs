using Compiler.Parser.AST.Nodes.Core;

namespace Compiler.Parser.AST.Nodes.Statements;

public class ClassDeclarationNode : DeclarationNode
{
    public IdentifierNode Identifier { get; }
    // think about class, methods, fields tables
    public IList<VariableDeclarationNode> Fields { get; }
    public IList<FunctionDeclarationNode> Methods { get; }

    public ClassDeclarationNode(IdentifierNode identifier, IList<VariableDeclarationNode> fields,
        IList<FunctionDeclarationNode> methods)
    {
        Identifier = identifier;
        Fields = fields;
        Methods = methods;
    }
    
    public override void Accept(IVisitor visitor)
    {
        visitor.VisitClassDeclarationNode(this);
    }
}