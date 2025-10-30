using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class TargetSpawnSystem : IInitializable, System.IDisposable
{
    private readonly GameModel _gameModel;
    private readonly TargetPool _targetPool;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private CancellationTokenSource _spawnCts;

    private const float SPAWN_INTERVAL = 1.5f;
    private const int MAX_TARGETS = 5;

    public TargetSpawnSystem(GameModel gameModel, TargetPool targetPool)
    {
        _gameModel = gameModel;
        _targetPool = targetPool;
    }

    public void Initialize()
    {
        _gameModel.IsGameRunning
            .Subscribe(isRunning =>
            {
                if (isRunning)
                    StartSpawning();
                else
                    StopSpawning();
            })
            .AddTo(_disposables);
    }

    private void StartSpawning()
    {
        _spawnCts = new CancellationTokenSource();
        SpawnLoop(_spawnCts.Token).Forget();
    }

    private void StopSpawning()
    {
        _spawnCts?.Cancel();
        _spawnCts?.Dispose();
        _spawnCts = null;

        _targetPool.ReturnAllTargets();
    }

    private async UniTaskVoid SpawnLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            if (ShouldSpawnTarget())
            {
                SpawnTarget();
            }

            await UniTask.Delay((int)(SPAWN_INTERVAL * 1000), cancellationToken: token);
        }
    }

    private bool ShouldSpawnTarget()
    {
        return _gameModel.IsGameRunning.Value &&
               _targetPool.ActiveTargets.Count < MAX_TARGETS;
    }

    private void SpawnTarget()
    {
        var target = _targetPool.GetTarget();

        if (target != null)
        {
            Vector2 randomViewport = new Vector2(
                Random.Range(0.1f, 0.9f),
                Random.Range(0.1f, 0.9f)
            );

            Vector3 worldPosition = Camera.main.ViewportToWorldPoint(
                new Vector3(randomViewport.x, randomViewport.y, 10f)
            );

            target.Reactivate(worldPosition);
        }
    }

    public void Dispose()
    {
        _disposables?.Dispose();
        _spawnCts?.Cancel();
        _spawnCts?.Dispose();
        _targetPool?.Dispose();
    }
}