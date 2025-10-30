public class TimeAttackMode : BaseGameMode
{
    private float _timeLimit;

    public TimeAttackMode(float timeLimit = 60f)
    {
        _timeLimit = timeLimit;
    }

    public override string ModeName => "Time Attack";
    public override string Description => $"Survive for {_timeLimit} seconds!";

    public override void Update(float deltaTime)
    {
        if (_gameModel.GameTime.Value >= _timeLimit)
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
        return $"Time's up! You survived {_timeLimit} seconds! Final score: {_gameModel.Score.Value}";
    }
}