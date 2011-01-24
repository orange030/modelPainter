using UnityEngine;
using System.Collections;

public class GameSystem:MonoBehaviour
{
    [System.Serializable]
    public class PrefabInfo
    {
        public string showName;
        public GameObject prefab;
    }

    public PrefabInfo[] PrefabInfoList;
    static protected GameSystem singletonInstance;

    public GameObject controlPointLinePrefab;

    public static GameSystem Singleton
    {
        get { return singletonInstance; }
    }

    void Awake()
    {
        if (singletonInstance)
        {
            Debug.LogError("GameSystem.singletonInstance");
        }
        singletonInstance = this;

    }

}