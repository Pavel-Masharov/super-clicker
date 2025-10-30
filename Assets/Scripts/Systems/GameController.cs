using UniRx;
using UnityEngine;
using Zenject;

public class GameController : IInitializable
{
    private readonly GameModel _gameModel;
    private readonly GameModeManager _gameModeManager;
    private readonly TargetPool _targetPool;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public GameController(GameModel gameModel, GameModeManager gameModeManager, TargetPool targetPool)
    {
        _gameModel = gameModel;
        _gameModeManager = gameModeManager;
        _targetPool = targetPool;
    }

    public void Initialize()
    {
        StartGame();


        // Обновление времени и режима
        Observable.EveryUpdate()
            .Where(_ => _gameModel.IsGameRunning.Value)
            .Subscribe(_ =>
            {
                UpdateGameTime();
                UpdateGameModes();
            })
            .AddTo(_disposables);

        // Проверка завершения режима
        _gameModeManager.CurrentMode
            .SelectMany(mode => mode?.IsCompleted ?? Observable.Empty<bool>())
            .Where(completed => completed)
            .Subscribe(_ => _gameModeManager.CompleteCurrentMode())
            .AddTo(_disposables);

        // Обработка рестарта
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => RestartGame())
            .AddTo(_disposables);
    }

    private void StartGame()
    {
        _gameModel.Reset();
        _gameModel.IsGameRunning.Value = true;

        var currentMode = _gameModeManager.CurrentMode.Value;
        if (currentMode != null)
        {
            Debug.Log($"Game Started! Mode: {currentMode.ModeName} - {currentMode.Description}");
        }
    }

    private void UpdateGameTime()
    {
        _gameModel.GameTime.Value += Time.deltaTime;
    }

    private void UpdateGameModes()
    {
        var currentMode = _gameModeManager.CurrentMode.Value;
        if (currentMode != null)
        {
            currentMode.Update(Time.deltaTime);
        }
    }


    public void RestartGame()
    {
        _gameModel.IsGameRunning.Value = false;
        _targetPool.ReturnAllTargets();
        StartGame();
    }

    public void SetGameMode(GameModeType modeType)
    {
        _gameModeManager.SetGameMode(modeType);
        RestartGame();
    }
}