using UniRx;

public abstract class BaseGameMode : IGameMode
{
    public abstract string ModeName { get; }
    public abstract string Description { get; }
    public IReadOnlyReactiveProperty<bool> IsCompleted => _isCompleted;

    protected ReactiveProperty<bool> _isCompleted = new ReactiveProperty<bool>();
    protected GameModel _gameModel;

    public virtual void Initialize(GameModel model)
    {
        _gameModel = model;
        _isCompleted.Value = false;
    }

    public abstract void Update(float deltaTime);
    public abstract void Reset();
    public abstract string GetCompletionMessage();
}