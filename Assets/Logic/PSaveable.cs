public class PSaveable<T>
{
    public T _obj;

    public T GetObj() {
        return _obj;
    }

    public void SetObj(T obj)
    {
        _obj = obj;
    }
}
