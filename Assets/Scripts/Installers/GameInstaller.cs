using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameModel>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<ScoreSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<TargetSpawnSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameController>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameModeManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<TargetPool>().AsSingle();

        Container.BindFactory<TargetView, TargetView.Factory>()
            .FromComponentInNewPrefabResource("Target");

    }
}

