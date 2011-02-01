using UnityEngine;
using System.Collections;

public class AddObjectToManager:MonoBehaviour
{
    public SceneManager sceneManager;
    public PlayStateManager playStateManager;

    public void addObject(GameObject pObject)
    {
        sceneManager.addObject(pObject);
        playStateManager.updateObject(pObject);
    }
}