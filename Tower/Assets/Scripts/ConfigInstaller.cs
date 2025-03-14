using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ConfigInstaller", menuName = "ScriptableObject/Config Installer")]
public class ConfigInstaller : ScriptableObjectInstaller<ConfigInstaller>
{
    [SerializeField]
    private GameConfig _gameConfig;

    public override void InstallBindings()
    {
        Container.BindInstance(_gameConfig).AsSingle();
    }
}
