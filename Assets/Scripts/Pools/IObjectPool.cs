public interface IObjectPool<T> where T : class
{
    T Get();
    void Return(T obj);
    void Prewarm(int count);
    void Clear();
}