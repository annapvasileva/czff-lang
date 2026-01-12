namespace Compiler.Parser.AST.Nodes.Core;

public class SimpleTypeNode : TypeAnnotationNode
{
    public string Name { get; }
    // ссылка на class symbol?
    public SimpleTypeNode(string name)
    {
        switch (name)
        {
            case "int":
                Name = "I";
                break;
            case "int64":
                Name = "I8";
                break;
            case "int128":
                Name = "I16";
                break;
            case "bool":
                Name = "B";
                break;
            default:
                Name = name;
                break;
        }
    }
    
    public override void Accept(INodeVisitor visitor)
    {
        visitor.Visit(this);
    }

    public override string GetName => Name + ";";
}