using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField]
    private GameController _uiController;
    [SerializeField]
    private MessageController _messageController;

    public override void InstallBindings()
    {
        Container.BindInstance(_uiController).AsSingle();
        Container.Bind<IAnimController>().To<AnimController>().AsSingle();
        Container.Bind<IMessageController>().FromInstance(_messageController).AsSingle();
    }
}
