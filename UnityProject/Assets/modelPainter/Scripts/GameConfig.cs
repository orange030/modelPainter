using UnityEngine;
using System.Collections;

public class GameConfig
{
    public string ModelDir = Application.dataPath + "/../Models";

    static protected GameConfig singletonInstance = new GameConfig();

    public static GameConfig Singleton
    {
        get { return singletonInstance; }
    }

    public static GameConfig getSingleton()
    {
        return singletonInstance;
    }

    //void Awake()
    //{
    //    if (singletonInstance!=null)
    //        Debug.LogError("have singletonInstance");
    //    singletonInstance = this;
    //}

}