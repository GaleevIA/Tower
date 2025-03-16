using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObject/Game Config")]
[Serializable]
public class GameConfig : ScriptableObject, IGameConfig
{
    [SerializeField]
    private List<ConfigStruct> figuresForSpawn;

    public List<ConfigStruct> GetConfig()
    {
        return figuresForSpawn;
    }
}
