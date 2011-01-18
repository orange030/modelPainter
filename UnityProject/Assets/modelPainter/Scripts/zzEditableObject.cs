using UnityEngine;
using System.Collections;

public abstract class zzEditableObject : MonoBehaviour
{
    [SerializeField]
    GameObjectType _gameObjectType;

    public GameObject[] objectList;

    public virtual void scale(Vector3 pScale)
    {

    }

    protected virtual void Awake()
    {
        setObjectType(_gameObjectType);
    }

    protected void setObjectType(GameObjectType pType)
    {
        int lLayer = GameConfig.Singleton.getLayer(pType);
        foreach (var lObject in objectList)
        {
            lObject.layer = lLayer;
        }
    }

    public virtual InPoint[] inPoints
    {
        get { return new InPoint[0]; }
    }

    public virtual OutPoint[] outPoints
    {
        get { return new OutPoint[0]; }
    }


    protected void    setInPointsId()
    {
        int lID = -1;
        foreach (var lPoint in inPoints)
        {
            lPoint.pointId = ++lID;
        }
    }
}