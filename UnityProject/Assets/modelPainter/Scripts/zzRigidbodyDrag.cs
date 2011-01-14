using UnityEngine;
using System.Collections;

class zzRigidbodyDrag:MonoBehaviour
{
    public enum DragMode
    {
        none,
        XY,
        XZ,
    }

    public DragMode dragMode = DragMode.none;

    public LayerMask dragLayerMask;
    public float detectDistance = 1000.0f;
    public Vector3 originalPosition;
    public Vector3 dragPosInBody;
    public Rigidbody dragedRigidbody;
    public Camera dragCamera;
    public float dragForceRangeDistance;
    public float maxDragForce;
    public float maxSpeed;

    public Joint jointXyDrag;
    public Joint jointXzDrag;

    public Joint nowDrag;

    public bool rigidbodyIsKinematic;

    void Start()
    {
        dragedRigidbody = rigidbody;
        dragCamera = Camera.main;
    }

    Ray getCameraRay()
    {
        var lMousePos = Input.mousePosition;
        var lRay = dragCamera.ScreenPointToRay(
            new Vector3(lMousePos.x, lMousePos.y, dragCamera.nearClipPlane));
        return lRay;

    }

    Vector3 getXYWantPos()
    {
        return getZFlatCrossPoint(originalPosition, getCameraRay());
    }

    Vector3 getXZWantPos()
    {
        return getYFlatCrossPoint(originalPosition, getCameraRay());
    }

    static Vector3 getFlatCrossPoint(Vector3 pFlatPos, Ray pRay,DragMode pMode)
    {
        switch (pMode)
        {
            case DragMode.XY:
                return getZFlatCrossPoint(pFlatPos, pRay);
                break;
            case DragMode.XZ:
                return getYFlatCrossPoint( pFlatPos, pRay);
                break;
        }
        Debug.LogError("getFlatCrossPoint");
        return pFlatPos;
    }


    static Vector3 getZFlatCrossPoint(Vector3 pFlatPos, Ray pRay)
    {
        if (Mathf.Approximately(pRay.origin.z, pFlatPos.z))
            return pFlatPos;
        return pRay.origin + pRay.direction * (pFlatPos.z - pRay.origin.z) / pRay.direction.z;
    }

    static Vector3 getYFlatCrossPoint(Vector3 pFlatPos, Ray pRay)
    {
        if (Mathf.Approximately(pRay.origin.y, pFlatPos.y))
            return pFlatPos;
        return pRay.origin + pRay.direction * (pFlatPos.y - pRay.origin.y) / pRay.direction.y;
    }

    Vector3 getToAimForce(Vector3 pOriginalPos,Vector3 pAimPos)
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

    //void beginDrag(Rigidbody pDragedRigidbody,Vector3 pDragBodyPos)
    //{
    //    dragedRigidbody = pDragedRigidbody;
    //    dragBodyPos = pDragBodyPos;
    //}

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
        if (Physics.Raycast(lCameraRay, out pRaycastHit, detectDistance,dragLayerMask)
            &&pRaycastHit.rigidbody)
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
        if (dragMode!= DragMode.none)
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
            //var lVelocity = dragedRigidbody.velocity;
            //if (lVelocity.sqrMagnitude > (maxSpeed * maxSpeed))
            //{
            //    dragedRigidbody.velocity = lVelocity.normalized * maxSpeed;
            //}
            //Vector3 ldragPosInWorld = dragedRigidbody.transform.TransformPoint(dragPosInBody);
            //dragedRigidbody.AddForceAtPosition(getToAimForce(ldragPosInWorld, wantPos),
            //    ldragPosInWorld);
            nowDrag.transform.position = wantPos;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(wantPos,0.3f);
    }
}