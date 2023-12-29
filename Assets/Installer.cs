using Zenject;

public class Installer : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Level>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Car>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SaveSystem>().FromComponentInHierarchy().AsSingle();

#if CRAZY_GAMES
        Container.Bind<IAds>().To<AdsCrazyGames>().AsSingle();
#else
        Container.Bind<IAds>().To<AdsDummy>().AsSingle();
#endif
    }
}
