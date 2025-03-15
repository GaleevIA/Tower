using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConfigInstaller", menuName = "ScriptableObject/Config Installer")]
public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
{
    [SerializeReference]
    private GameConfig _gameConfig;
    [SerializeField]
    private LocalizationConfig _localizationConfig;

    public override void InstallBindings()
    {
        Container.Bind<IGameConfig>().FromInstance(_gameConfig).AsSingle();
        Container.BindInstance(_localizationConfig).AsSingle();
    }
}
