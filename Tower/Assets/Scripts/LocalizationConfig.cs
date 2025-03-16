using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationConfig", menuName = "ScriptableObject/Localization Config")]
public class LocalizationConfig : ScriptableObject, ILocalizationConfig
{
    [SerializeField]
    private List<LocalizationStruct> config = new();

    public List<LocalizationStruct> GetConfig()
    {
        return config;
    }
}
