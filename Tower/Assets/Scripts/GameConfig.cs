using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObject/Game Config")]
public class GameConfig : ScriptableObject
{
    [Serializable]
    public struct ConfigStruct
    {
        public Figure prefab;
        public int count;
    }

    public List<ConfigStruct> figuresForSpawn;
}
