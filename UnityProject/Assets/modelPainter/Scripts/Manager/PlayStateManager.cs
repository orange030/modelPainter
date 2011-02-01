using UnityEngine;
using System.Collections;

public class PlayStateManager:MonoBehaviour
{
    [SerializeField]
    bool _inPlaying = false;

    public IEnumerable enumerateObject;

    public Transform ToInitState;

    public void Start()
    {
        if(ToInitState)
        {
            var lInitList = ToInitState.GetComponentsInChildren<zzEditableObject>();
            foreach (var lEditableObject in lInitList)
            {
                lEditableObject.play = _inPlaying;
            }
        }
    }

    public bool play
    {
        get { return _inPlaying; }
        set { setPlay(value); }
    }

    public void setPlay(bool pIsPlay)
    {
        if (_inPlaying == pIsPlay)
            return;
        _inPlaying = pIsPlay;
        updateObjects();
    }

    public void updateObject(GameObject pOjbect)
    {
        pOjbect.GetComponent<zzEditableObject>().play = _inPlaying;
    }

    public void updateObjects()
    {
        foreach (Transform lTransform in enumerateObject)
        {
            lTransform.GetComponent<zzEditableObject>().play = _inPlaying;
        }
    }
}