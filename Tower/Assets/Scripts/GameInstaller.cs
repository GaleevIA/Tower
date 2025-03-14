using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField]
    private UIController _uiController;

    public override void InstallBindings()
    {
        Container.Bind<GameController>().AsSingle().NonLazy();
        Container.BindInstance(_uiController).AsSingle().NonLazy();
    }
}
