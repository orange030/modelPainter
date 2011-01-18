using UnityEngine;
using System.Collections;

class ForceObject : zzEditableObject
{
    public InPoint controlPoint;

    [SerializeField]
    float _maxForce = 10f;

    [SerializeField]
    float _minForce = 0f;

    public ConstantForce myConstantForce;

    public override void scale(Vector3 pScale)
    {

    }

    void Start()
    {
        controlPoint.addProcessFuncFloatArg(setForceRate);
        myConstantForce = constantForce;
        setForceRate(0f);
    }

    //0-1
    public void setForceRate(float pRate)
    {
        myConstantForce.relativeForce
            = new Vector3(Mathf.Lerp(_minForce,_maxForce,pRate), 0f, 0f);
    }

    public override InPoint[] inPoints
    {
        get
        {
            return new InPoint[] { controlPoint };
        }
    }

}