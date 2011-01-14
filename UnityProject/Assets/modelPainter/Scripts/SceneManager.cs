using UnityEngine;
using System.Collections;

public class SceneManager:MonoBehaviour , IEnumerable
{
    public Transform managerRoot;
    public string managerName = "SceneManager";

    void Start()
    {
        if (!managerRoot)
            managerRoot = (new GameObject(managerName)).transform;
    }

    public void addObject(GameObject[] pObjects)
    {
        foreach (var lObject in pObjects)
        {
            lObject.transform.parent = managerRoot;
        }
    }

    public int objectCount
    {
        get { return managerRoot.GetChildCount(); }
    }

    public IEnumerator GetEnumerator()
    {
        return managerRoot.GetEnumerator();
    }
}