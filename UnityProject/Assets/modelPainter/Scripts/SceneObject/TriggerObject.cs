using UnityEngine;
using System.Collections;

class TriggerObject : zzEditableObject
{
    public OutPoint triggerPoint;

    public override void transformScale(Vector3 pScale)
    {

    }

    bool isTriggered = false;

    void Update()
    {
        if (isTriggered)
        {
            triggerPoint.sendFull();
            isTriggered = false;
        }
        else
        {
            triggerPoint.sendNull();
        }
    }


    void OnTriggerStay(Collider other)
    {
        isTriggered = true;
    }

    public override OutPoint[] outPoints
    {
        get
        {
            return new OutPoint[] { triggerPoint };
        }
    }


}