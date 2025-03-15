using UniRx;
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
        Container.Bind<AnimController>().AsSingle().NonLazy();
        Container.BindInstance(_messageController).AsSingle();
    }
}
