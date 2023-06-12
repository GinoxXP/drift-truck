using Zenject;

public class Installer : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Level>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Car>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SaveSystem>().FromComponentInHierarchy().AsSingle();
    }
}
