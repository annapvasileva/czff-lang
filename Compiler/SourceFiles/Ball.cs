namespace Compiler.SourceFiles;

public class Ball
{
    public Header Header;

    public FunctionPool FunctionPool;
 
    public ConstantPool ConstantPool;
    
    public ClassPool ClassPool;

    public Ball()
    {
        Header = new Header();
        FunctionPool = new FunctionPool();
        ConstantPool = new ConstantPool();
        ClassPool = new ClassPool();
    }
    
    public Ball(Header header)
    {
        Header = header;
        
        FunctionPool = new FunctionPool();
        
        ConstantPool =  new ConstantPool();

        ClassPool = new ClassPool();
    }
    
}