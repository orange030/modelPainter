using UnityEngine;
using System.Collections;

class zzRigidbodyDrag:MonoBehaviour
{
    public LayerMask dragLayerMask;
    public float detectDistance = 1000.0f;
    public Vector3 originalPosition;
    public Vector3 dragPosInBody;
    public Rigidbody dragedRigidbody;
    public Camera dragCamera;
    public float dragForceRangeDistance;
    public float maxDragForce;
    public float maxSpeed;

    public Joint jointDrag;

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

    static Vector3 getZFlatCrossPoint(Vector3 pFlatPos, Ray pRay)
    {
        if (Mathf.Approximately(pRay.origin.z, pFlatPos.z))
            return pFlatPos;
        return pRay.origin + pRay.direction * (pFlatPos.z - pRay.origin.z) / pRay.direction.z;
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
        jointDrag.connectedBody = null;
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
        if (Input.GetKeyDown(KeyCode.Mouse0) && dragCheck(out lRaycastHit))
        {
            dragedRigidbody = lRaycastHit.rigidbody;
            rigidbodyIsKinematic = dragedRigidbody.isKinematic;
            dragedRigidbody.isKinematic = false;
            dragedRigidbody.useGravity = false;
            var lDragedTransform = lRaycastHit.rigidbody.transform;
            var lDragWorldPos =
                getZFlatCrossPoint(lDragedTransform.position,getCameraRay());
            originalPosition = lDragWorldPos;
            jointDrag.transform.position = lDragWorldPos;
            jointDrag.connectedBody = dragedRigidbody;
            //dragPosInBody = lDragedTransform.InverseTransformPoint(lDragWorldPos);
        }
        if (dragedRigidbody && Input.GetKeyUp(KeyCode.Mouse0))
            endDrag();
    }

    void FixedUpdate()
    {
        if (dragedRigidbody)
        {
            wantPos = getXYWantPos();
            //var lVelocity = dragedRigidbody.velocity;
            //if (lVelocity.sqrMagnitude > (maxSpeed * maxSpeed))
            //{
            //    dragedRigidbody.velocity = lVelocity.normalized * maxSpeed;
            //}
            //Vector3 ldragPosInWorld = dragedRigidbody.transform.TransformPoint(dragPosInBody);
            //dragedRigidbody.AddForceAtPosition(getToAimForce(ldragPosInWorld, wantPos),
            //    ldragPosInWorld);
            jointDrag.transform.position = wantPos;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(wantPos,0.3f);
    }
}