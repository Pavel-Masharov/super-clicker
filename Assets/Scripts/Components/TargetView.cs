using UnityEngine;
using Zenject;

public class TargetView : MonoBehaviour
{
    public class Factory : PlaceholderFactory<TargetView> { }

    private const float LIFETIME = 3f;
    private const int BASE_POINTS = 10;

    private ScoreSystem _scoreSystem;
    private TargetPool _targetPool;

    [Inject]
    private void Construct(ScoreSystem scoreSystem, TargetPool targetPool)
    {
        _scoreSystem = scoreSystem;
        _targetPool = targetPool;
    }

    private void Start()
    {
        StartCoroutine(AutoReturnCoroutine());
    }

    private System.Collections.IEnumerator AutoReturnCoroutine()
    {
        yield return new WaitForSeconds(LIFETIME);
        ReturnToPool();
    }

    private void OnMouseDown()
    {
        OnTargetClicked();
    }

    private void OnTargetClicked()
    {
        if (_scoreSystem != null)
        {
            _scoreSystem.AddPointsForTarget(BASE_POINTS);
        }
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        StopAllCoroutines();
        _targetPool?.ReturnTarget(this);
    }

    public void Reactivate(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
        StartCoroutine(AutoReturnCoroutine());
    }
}