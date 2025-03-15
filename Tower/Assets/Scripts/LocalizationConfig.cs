using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationConfig", menuName = "ScriptableObject/Localization Config")]
public class LocalizationConfig : ScriptableObject
{
    public List<LocalizationStruct> config = new();
}
