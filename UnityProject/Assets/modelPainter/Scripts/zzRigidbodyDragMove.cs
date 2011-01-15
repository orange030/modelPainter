using UnityEngine;
using System.Collections;

class zzRigidbodyDragMove : zzRigidbodyDrag
{
    public Vector3 originalPosition;
    public Vector3 dragPosInBody;
    public Rigidbody dragedRigidbody;
    public float dragForceRangeDistance;
    public float maxDragForce;
    public float maxSpeed;

    public Joint jointXyDrag;
    public Joint jointXzDrag;

    public Joint nowDrag;

    public bool rigidbodyIsKinematic;


    Vector3 getXYWantPos()
    {
        return getZFlatCrossPoint(originalPosition, getCameraRay());
    }

    Vector3 getXZWantPos()
    {
        return getYFlatCrossPoint(originalPosition, getCameraRay());
    }


    Vector3 getToAimForce(Vector3 pOriginalPos, Vector3 pAimPos)
    {
        Vector3 lToAim = pAimPos - pOriginalPos;
        float lForce;
        float lDistance = lToAim.magnitude;
        if (lDistance > dragForceRangeDistance)
            lForce = maxDragForce;
        else
            lForce = maxDragForce * (dragForceRangeDistance - lDistance)
                / dragForceRangeDistance;
        return lToAim.normalized * lForce;
    }

    public Vector3 wantPos;

    void endDrag()
    {
        dragedRigidbody.useGravity = true;
        dragedRigidbody.isKinematic = rigidbodyIsKinematic;
        dragedRigidbody = null;
        nowDrag.connectedBody = null;
        dragMode = DragMode.none;
    }

    bool dragCheck(out RaycastHit pRaycastHit)
    {
        var lCameraRay = getCameraRay();
        if (Physics.Raycast(lCameraRay, out pRaycastHit, detectDistance, dragLayerMask)
            && pRaycastHit.rigidbody)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        RaycastHit lRaycastHit;
        bool lXYButton = Input.GetKey(KeyCode.Mouse0);
        bool lXZButton = Input.GetKey(KeyCode.Mouse1);
        if (dragMode == DragMode.none
            && (lXYButton | lXZButton)
            && dragCheck(out lRaycastHit))
        {
            dragMode = lXYButton ? DragMode.XY : DragMode.XZ;
            nowDrag = lXYButton ? jointXyDrag : jointXzDrag;
            dragedRigidbody = lRaycastHit.rigidbody;
            rigidbodyIsKinematic = dragedRigidbody.isKinematic;
            dragedRigidbody.isKinematic = false;
            dragedRigidbody.useGravity = false;
            var lDragedTransform = lRaycastHit.rigidbody.transform;
            var lDragWorldPos =
                getZFlatCrossPoint(lDragedTransform.position, getCameraRay());
            //getFlatCrossPoint(lDragedTransform.position, getCameraRay(), dragMode);
            originalPosition = lDragWorldPos;
            nowDrag.transform.position = lDragWorldPos;
            nowDrag.connectedBody = dragedRigidbody;

        }

        if (
            (dragMode == DragMode.XY && (!lXYButton))
            || (dragMode == DragMode.XZ && (!lXZButton))
            )
            endDrag();
    }

    void FixedUpdate()
    {
        if (dragMode != DragMode.none)
        {
            switch (dragMode)
            {
                case DragMode.XY:
                    wantPos = getXYWantPos();
                    break;
                case DragMode.XZ:
                    wantPos = getXZWantPos();
                    break;
            }
            nowDrag.transform.position = wantPos;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(wantPos, 0.3f);
    }

}