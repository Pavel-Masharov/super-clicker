using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

public class GameModeManager : IInitializable
{
    private readonly GameModel _gameModel;
    private readonly Dictionary<GameModeType, IGameMode> _modes = new Dictionary<GameModeType, IGameMode>();
    private IGameMode _currentMode;

    private ReactiveProperty<IGameMode> _currentModeReactive = new ReactiveProperty<IGameMode>();

    public IReadOnlyReactiveProperty<IGameMode> CurrentMode => _currentModeReactive;

    public GameModeManager(GameModel gameModel)
    {
        _gameModel = gameModel;
        InitializeModes();
    }

    public void Initialize()
    {
        SetGameMode(GameModeType.TimeAttack);
    }

    private void InitializeModes()
    {
        _modes[GameModeType.TimeAttack] = new TimeAttackMode(60f);
        _modes[GameModeType.TargetHunter] = new TargetHunterMode(30);
        _modes[GameModeType.ScoreAttack] = new ScoreAttackMode(500);
        _modes[GameModeType.Endless] = new EndlessMode();

        foreach (var mode in _modes.Values)
        {
            mode.Initialize(_gameModel);
        }
    }

    public void SetGameMode(GameModeType modeType)
    {
        if (_modes.TryGetValue(modeType, out var newMode))
        {
            _currentMode = newMode;
            _currentModeReactive.Value = newMode;
            UnityEngine.Debug.Log($"Game mode set to: {newMode.ModeName} - {newMode.Description}");
        }
    }

    public void CompleteCurrentMode()
    {
        if (_currentMode != null && _gameModel.IsGameRunning.Value)
        {
            _gameModel.IsGameRunning.Value = false;
            UnityEngine.Debug.Log(_currentMode.GetCompletionMessage());
        }
    }

    public IEnumerable<KeyValuePair<GameModeType, IGameMode>> GetAllModes()
    {
        return _modes;
    }
}