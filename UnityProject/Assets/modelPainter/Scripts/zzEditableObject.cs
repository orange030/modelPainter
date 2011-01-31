using UnityEngine;
using System.Collections;

public class zzEditableObject : MonoBehaviour
{
    [SerializeField]
    bool _inPlaying;

    public string objectTypeName;

    protected virtual void applyState()
    {
        if(_inPlaying)
        {
            rigidbody.isKinematic = _fixed;
            rigidbody.useGravity = _useGravity;
            rigidbody.freezeRotation = _freezeRotation;
        }
        else
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            rigidbody.freezeRotation = false;
        }
    }

    public bool play
    {
        get { return _inPlaying; }
        set
        {
            if (_inPlaying == value)
                return;
            _inPlaying = value;
            applyState();
        }
    }

    public bool draged
    {
        set
        {
            if (value)
            {
                rigidbody.isKinematic = false;
            }
            else
                applyState();
        }
    }

    [SerializeField]
    bool _fixed = false;

    [FieldUI("固定物体")]
    public bool fixedObject
    {
        get { return _fixed; }
        set
        { 
            _fixed = value;
            if (_inPlaying)
            {
                rigidbody.isKinematic = _fixed;
                rigidbody.WakeUp();
            }
        }
    }

    [SerializeField]
    bool _useGravity;

    [FieldUI("使用重力")]
    public bool useGravity
    {
        get { return _useGravity; }
        set 
        {
            _useGravity = value;
            if (_inPlaying)
                rigidbody.useGravity = _useGravity;
        }
    }


    [FieldUI("质量(kg)")]
    public float mass
    {
        get { return rigidbody.mass; }
        set 
        { 
            rigidbody.mass = value;
        }
    }

    bool _freezeRotation;

    [FieldUI("冻结旋转")]
    public bool freezeRotation
    {
        get { return _freezeRotation; }
        set 
        {
            _freezeRotation = value;
            if (_inPlaying)
            {
                rigidbody.freezeRotation = _freezeRotation;
                rigidbody.WakeUp();
            }
        }
    }

    [SerializeField]
    GameObjectType _gameObjectType;

    public GameObject[] objectList = new GameObject[0];

    [SerializeField]
    bool _uniformScale;

    public bool uniformScale
    {
        get { return _uniformScale; }
    }

    public virtual void transformScale(Vector3 pScaleChange)
    {
        var lLocalScale = transform.localScale;
        Vector3 lScale;
        if (_uniformScale)
        {
            float lLength = (pScaleChange.x>0?1f:-1f)*pScaleChange.magnitude;
            float lUniformValue = lLocalScale.x + lLength;
            lScale = new Vector3(lUniformValue, lUniformValue, lUniformValue);
        }
        else
            lScale= lLocalScale + pScaleChange;
        transform.localScale = lScale;
    }

    protected virtual void Awake()
    {
        _inPlaying = false;
        applyState();
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

    public static zzEditableObject findRoot(GameObject pObject)
    {
        if (!pObject)
            return null;
        var lEditable = pObject.GetComponent<zzEditableObject>();
        Transform lTransform = pObject.transform.parent;
        while (lTransform)
        {
            var lParentEditable = lTransform.GetComponent<zzEditableObject>();
            if (lParentEditable)
                lEditable = lParentEditable;
            lTransform = lTransform.parent;
        }
        if (lEditable)
            return lEditable;
        return null;
    }
}