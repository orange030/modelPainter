using UnityEngine;
using System.Collections;

class PointConnecter : zzRigidbodyDrag
{
    public LayerMask pointLayerMask;
    public KeyCode connectKeyCode;

    public InPoint choosedInPoint;
    public OutPoint choosedOutPoint;

    void Update()
    {
        if(Input.GetKeyDown(connectKeyCode))
        {
            check();
        }
        else if(Input.GetKeyUp(connectKeyCode))
        {
            check();
            if (choosedInPoint && choosedOutPoint)
                choosedOutPoint.connect(choosedInPoint);
            choosedInPoint = null;
            choosedOutPoint = null;
        }
    }

    void check()
    {
        RaycastHit lRaycastHit;
        if (Physics.Raycast(getCameraRay(), out lRaycastHit, detectDistance, pointLayerMask))
            setPoint(lRaycastHit.transform);

    }

    void setPoint(Transform pObject)
    {
        if (pObject.GetComponent<InPoint>())
            choosedInPoint = pObject.GetComponent<InPoint>();

        else if (pObject.GetComponent<OutPoint>())
            choosedOutPoint = pObject.GetComponent<OutPoint>();
    }

}