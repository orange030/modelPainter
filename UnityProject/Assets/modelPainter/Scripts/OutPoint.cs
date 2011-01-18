using UnityEngine;
using System.Collections;

public class OutPoint : MonoBehaviour
{

    public int pointId;

    public InPoint[] connectPoints = new InPoint[0];

    [SerializeField]
    float _powerValue;

    void Awake()
    {
        _powerValue = 0f;
    }

    public float powerValue
    {
        get { return _powerValue; }
    }

    public void sendFull()
    {
        send(1.0f);
    }

    public void sendNull()
    {
        send(0.0f);
    }

    public void send(float pValue)
    {
        if (pValue == _powerValue)
            return;
        //print(name + ":" + pValue);
        _powerValue = pValue;
        foreach (var lInPoint in connectPoints)
        {
            lInPoint.send(pValue);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 lSelfPos = transform.position;
        Gizmos.color = Color.blue;
        foreach (var lInPoint in connectPoints)
        {
            if (lInPoint)
                Gizmos.DrawLine(lInPoint.transform.position, lSelfPos);
        }
    }
}