using System;
using System.Collections.Generic;

public class ObjectPool<T> : IObjectPool<T> where T : class
{
    private readonly Stack<T> _pool = new Stack<T>();
    private readonly Func<T> _createFunc;
    private readonly Action<T> _onGet;
    private readonly Action<T> _onReturn;
    private readonly Action<T> _onDestroy;

    public int CountAll { get; private set; }
    public int CountActive => CountAll - CountInactive;
    public int CountInactive => _pool.Count;

    public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onReturn = null, Action<T> onDestroy = null)
    {
        _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        _onGet = onGet;
        _onReturn = onReturn;
        _onDestroy = onDestroy;
    }

    public T Get()
    {
        T obj;
        if (_pool.Count == 0)
        {
            obj = _createFunc();
            CountAll++;
        }
        else
        {
            obj = _pool.Pop();
        }

        _onGet?.Invoke(obj);
        return obj;
    }

    public void Return(T obj)
    {
        if (obj == null) return;

        _onReturn?.Invoke(obj);
        _pool.Push(obj);
    }

    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var obj = _createFunc();
            CountAll++;
            Return(obj);
        }
    }

    public void Clear()
    {
        while (_pool.Count > 0)
        {
            var obj = _pool.Pop();
            _onDestroy?.Invoke(obj);
        }
        CountAll = 0;
    }
}