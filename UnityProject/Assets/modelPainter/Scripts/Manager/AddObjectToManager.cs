using UnityEngine;
using System.Collections;

public class AddObjectToManager : MonoBehaviour, IEnumerable
{
    public SceneManager sceneManager;
    public PlayStateManager playStateManager;

    public void addObject(GameObject pObject)
    {
        sceneManager.addObject(pObject);
        playStateManager.updateObject(pObject);
    }

    public IEnumerator GetEnumerator()
    {
        return sceneManager.GetEnumerator();
    }
}