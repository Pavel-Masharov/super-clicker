public class ScoreAttackMode : BaseGameMode
{
    private int _scoreLimit;

    public ScoreAttackMode(int scoreLimit = 500)
    {
        _scoreLimit = scoreLimit;
    }

    public override string ModeName => "Score Attack";
    public override string Description => $"Reach {_scoreLimit} points!";

    public override void Update(float deltaTime)
    {
        if (_gameModel.Score.Value >= _scoreLimit)
        {
            _isCompleted.Value = true;
        }
    }

    public override void Reset()
    {
        _isCompleted.Value = false;
    }

    public override string GetCompletionMessage()
    {
        return $"Incredible! You reached {_scoreLimit} points!";
    }
}