namespace Compiler.SourceFiles;

public class ConstantPool
{
    public int Length => _pool.Count;
    
    private readonly List<ConstantItem> _pool;
    
    private readonly Dictionary<ConstantItem, int> _mapping;

    public ConstantPool()
    {
        _pool = new List<ConstantItem>();
        _mapping = new Dictionary<ConstantItem, int>();
    }
    
    public ConstantPool(IList<ConstantItem> pool)
    {
        _pool = pool.ToList();
        _mapping = new Dictionary<ConstantItem, int>();
        for (int i = 0; i < _pool.Count; i++)
        {
            _mapping[_pool[i]] = i;
        }
    }

    public ConstantItem GetConstant(int index)
    {
        return _pool[index];
    } 
    
    public int GetIndex(ConstantItem constantItem)
    {
        return _mapping.GetValueOrDefault(constantItem, -1);
    }

    public void AddConstant(ConstantItem constantItem)
    {
        if (!_mapping.ContainsKey(constantItem))
        {
            _pool.Add(constantItem);
            _mapping[constantItem] = _pool.Count - 1;
        }
    }
    
    public void AddConstant(IEnumerable<ConstantItem> constants)
    {
        foreach (var constant in constants)
        {
            AddConstant(constant);
        }
    }

    public IList<ConstantItem> GetConstants()
    {
        var list = new List<ConstantItem>();
        list.AddRange(_pool);
        
        return list;
    }
}