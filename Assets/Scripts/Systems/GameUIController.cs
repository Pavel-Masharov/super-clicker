using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameUIController : MonoBehaviour
{
    [Header("Game Info Panel")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text targetsText;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Mode Selection Panel")]
    [SerializeField] private GameObject modeSelectionPanel;
    [SerializeField] private Button timeAttackButton;
    [SerializeField] private Button targetHunterButton;
    [SerializeField] private Button scoreAttackButton;
    [SerializeField] private Button endlessButton;

    [Header("Current Mode Display")]
    [SerializeField] private TMP_Text currentModeText;
    [SerializeField] private TMP_Text modeDescriptionText;
    [SerializeField] private TMP_Text gameStatsText;

    [Header("Control Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button changeModeButton;

    private GameController _gameController;
    private GameModeManager _gameModeManager;
    private GameModel _gameModel;
    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(GameController gameController, GameModeManager gameModeManager, GameModel gameModel)
    {
        _gameController = gameController;
        _gameModeManager = gameModeManager;
        _gameModel = gameModel;
    }

    private void Start()
    {
        SetupButtons();
        SetupSubscriptions();
        UpdateModeDisplay();

        modeSelectionPanel.SetActive(false);
    }

    private void SetupButtons()
    {
        timeAttackButton.onClick.AddListener(() => SelectMode(GameModeType.TimeAttack));
        targetHunterButton.onClick.AddListener(() => SelectMode(GameModeType.TargetHunter));
        scoreAttackButton.onClick.AddListener(() => SelectMode(GameModeType.ScoreAttack));
        endlessButton.onClick.AddListener(() => SelectMode(GameModeType.Endless));

        restartButton.onClick.AddListener(() => _gameController.RestartGame());
        changeModeButton.onClick.AddListener(ShowModeSelection);
    }

    private void SetupSubscriptions()
    {
        _gameModel.Score
            .Subscribe(score =>
            {
                if (scoreText != null)
                    scoreText.text = $"Score: {score}";
            })
            .AddTo(_disposables);

        _gameModel.GameTime
            .Subscribe(time =>
            {
                if (timeText != null)
                    timeText.text = $"Time: {time:F1}s";
            })
            .AddTo(_disposables);

        _gameModel.TargetsDestroyed
            .Subscribe(count =>
            {
                if (targetsText != null)
                    targetsText.text = $"Targets: {count}";
            })
            .AddTo(_disposables);

        _gameModel.IsGameRunning
            .Subscribe(isRunning =>
            {
                if (gameOverPanel != null)
                    gameOverPanel.SetActive(!isRunning);
            })
            .AddTo(_disposables);

        _gameModeManager.CurrentMode
            .Subscribe(_ => UpdateModeDisplay())
            .AddTo(_disposables);

        _gameModel.Score
            .CombineLatest(_gameModel.TargetsDestroyed, _gameModel.GameTime,
                (score, targets, time) => (score, targets, time))
            .Subscribe(_ => UpdateGameStats())
            .AddTo(_disposables);
    }

    private void SelectMode(GameModeType modeType)
    {
        _gameController.SetGameMode(modeType);
        modeSelectionPanel.SetActive(false);
        Debug.Log($"Selected mode: {modeType}");
    }

    private void UpdateModeDisplay()
    {
        var currentMode = _gameModeManager.CurrentMode.Value;
        if (currentMode != null)
        {
            currentModeText.text = currentMode.ModeName;
            modeDescriptionText.text = currentMode.Description;
        }
    }

    private void UpdateGameStats()
    {
        var currentMode = _gameModeManager.CurrentMode.Value;
        if (currentMode == null) return;

        string stats = currentMode switch
        {
            TimeAttackMode => $"Time: {_gameModel.GameTime.Value:F1}s / 60s",
            TargetHunterMode => $"Targets: {_gameModel.TargetsDestroyed.Value} / 30",
            ScoreAttackMode => $"Score: {_gameModel.Score.Value} / 500",
            EndlessMode => $"Score: {_gameModel.Score.Value} | Targets: {_gameModel.TargetsDestroyed.Value}",
            _ => ""
        };

        if (gameStatsText != null)
            gameStatsText.text = stats;
    }

    private void ShowModeSelection()
    {
        modeSelectionPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        _disposables?.Dispose();
    }
}