using UniRx;

public class GameModel
{
    public ReactiveProperty<int> Score { get; private set; }
    public ReactiveProperty<int> TargetsDestroyed { get; private set; }
    public ReactiveProperty<float> GameTime { get; private set; }
    public ReactiveProperty<bool> IsGameRunning { get; private set; }

    public GameModel()
    {
        Score = new ReactiveProperty<int>(0);
        TargetsDestroyed = new ReactiveProperty<int>(0);
        GameTime = new ReactiveProperty<float>(0f);
        IsGameRunning = new ReactiveProperty<bool>(false);
    }

    public void AddScore(int points)
    {
        Score.Value += points;
    }

    public void TargetDestroyed()
    {
        TargetsDestroyed.Value++;
    }

    public void Reset()
    {
        Score.Value = 0;
        TargetsDestroyed.Value = 0;
        GameTime.Value = 0f;
    }
}