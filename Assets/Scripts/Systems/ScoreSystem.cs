using UniRx;
using Zenject;

public class ScoreSystem : IInitializable
{
    private readonly GameModel _gameModel;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public ScoreSystem(GameModel gameModel)
    {
        _gameModel = gameModel;
    }

    public void Initialize()
    {
        _gameModel.TargetsDestroyed
            .Pairwise()
            .Where(pair => pair.Current > pair.Previous && pair.Current % 10 == 0)
            .Subscribe(_ => AddBonusScore())
            .AddTo(_disposables);
    }

    private void AddBonusScore()
    {
        _gameModel.AddScore(50);
    }

    public void AddPointsForTarget(int basePoints)
    {
        int points = basePoints;
        _gameModel.AddScore(points);
        _gameModel.TargetDestroyed();
    }
}