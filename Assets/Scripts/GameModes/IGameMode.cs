using UniRx;

public interface IGameMode
{
    string ModeName { get; }
    string Description { get; }
    IReadOnlyReactiveProperty<bool> IsCompleted { get; }
    void Initialize(GameModel model);
    void Update(float deltaTime);
    void Reset();
    string GetCompletionMessage();
}