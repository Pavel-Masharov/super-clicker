public class TargetHunterMode : BaseGameMode
{
    private int _targetLimit;

    public TargetHunterMode(int targetLimit = 30)
    {
        _targetLimit = targetLimit;
    }

    public override string ModeName => "Target Hunter";
    public override string Description => $"Destroy {_targetLimit} targets!";

    public override void Update(float deltaTime)
    {
        if (_gameModel.TargetsDestroyed.Value >= _targetLimit)
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
        return $"Amazing! You destroyed {_targetLimit} targets! Final score: {_gameModel.Score.Value}";
    }
}