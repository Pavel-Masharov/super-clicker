using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TargetPool : IInitializable, IDisposable
{
    private readonly DiContainer _container;
    private readonly TargetView.Factory _targetFactory;
    private readonly ObjectPool<TargetView> _pool;

    private List<TargetView> _activeTargets = new List<TargetView>();
    private Transform _poolParent;
    private int _initialCountTargets = 10;

    public IReadOnlyList<TargetView> ActiveTargets => _activeTargets;

    public TargetPool(DiContainer container, TargetView.Factory targetFactory)
    {
        _container = container;
        _targetFactory = targetFactory;

        var poolObject = new GameObject("TargetPool");
        _poolParent = poolObject.transform;

        _pool = new ObjectPool<TargetView>(
            createFunc: CreateTarget,
            onGet: OnTargetGet,
            onReturn: OnTargetReturn
        );
    }

    public void Initialize()
    {
        _pool.Prewarm(_initialCountTargets);
    }

    public TargetView GetTarget()
    {
        var target = _pool.Get();
        _activeTargets.Add(target);
        return target;
    }

    public void ReturnTarget(TargetView target)
    {
        if (target == null) return;

        _activeTargets.Remove(target);
        _pool.Return(target);
    }

    public void ReturnAllTargets()
    {
        for (int i = _activeTargets.Count - 1; i >= 0; i--)
        {
            ReturnTarget(_activeTargets[i]);
        }
    }

    private TargetView CreateTarget()
    {
        var target = _targetFactory.Create();
        target.gameObject.SetActive(false);
        target.transform.SetParent(_poolParent);
        return target;
    }

    private void OnTargetGet(TargetView target)
    {
        target.gameObject.SetActive(true);
        target.transform.SetParent(null);
    }

    private void OnTargetReturn(TargetView target)
    {
        if (target != null)
        {
            target.gameObject.SetActive(false);
            target.transform.SetParent(_poolParent);
            target.transform.position = Vector3.zero;
        }
    }

    public void Dispose()
    {
        _pool.Clear();
        if (_poolParent != null)
        {
            UnityEngine.Object.Destroy(_poolParent.gameObject);
        }
    }
}