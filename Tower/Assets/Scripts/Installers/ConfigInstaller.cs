using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConfigInstaller", menuName = "ScriptableObject/Config Installer")]
public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
{
    [SerializeField]
    private GameConfig _gameConfig;
    [SerializeField]
    private LocalizationConfig _localizationConfig;

    public override void InstallBindings()
    {
        Container.Bind<IGameConfig>().FromInstance(_gameConfig).AsSingle();
        Container.Bind<ILocalizationConfig>().FromInstance(_localizationConfig).AsSingle();
    }
}
