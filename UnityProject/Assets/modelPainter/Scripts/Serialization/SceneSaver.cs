using UnityEngine;
using System.Collections;

public class SceneSaver:MonoBehaviour
{
    [SerializeField]
    string savrFolder = "Scene";

    /// <summary>
    /// 相对路径
    /// </summary>
    [SerializeField]
    string _savePath;

    public string savePath
    {
        get { return _savePath; }
        set { _savePath = value; }
    }

    public void save()
    {
        GameResourceManager.Main.path = savrFolder+"/"+_savePath;
        GameResourceManager.Main.save();
    }
}