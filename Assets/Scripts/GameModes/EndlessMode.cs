public class EndlessMode : BaseGameMode
{
    public override string ModeName => "Endless";
    public override string Description => "No limits! How long can you last?";

    public override void Update(float deltaTime)
    {
        
    }

    public override void Reset()
    {
        _isCompleted.Value = false;
    }

    public override string GetCompletionMessage()
    {
        return $"Session ended! Final score: {_gameModel.Score.Value}";
    }
}