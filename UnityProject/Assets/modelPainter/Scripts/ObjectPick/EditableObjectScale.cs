﻿using UnityEngine;
using System.Collections;

public class EditableObjectScale : ObjectPickBase
{
    public DragMode dragMode = DragMode.none;

    public float scaleValue = 0.01f;

    zzEditableObject editableObject;

    public Vector2 lastMousePosition;

    public void endDrag()
    {
        if (dragMode == DragMode.none)
            return;
        dragMode = DragMode.none;
        editableObject.draged = false;
    }

    void OnDragObject(GameObject pObject, DragMode pMode)
    {
        if (dragMode != DragMode.none)
            return;
        var lEditableObject = zzEditableObject.findRoot(pObject);
        if (!lEditableObject)
            return;
        editableObject = lEditableObject;
        lEditableObject.draged = true;
        dragMode = pMode;

        lastMousePosition = getMousePosition();
    }

    Vector2 getMousePosition()
    {
        var lMousePosition = Input.mousePosition;
        return new Vector2(lMousePosition.x, lMousePosition.y);
    }

    float getScale(float pPosChange)
    {
        if (pPosChange > 0)
            return 1f + pPosChange;
        else if(pPosChange<0)
            return 1f/(1-pPosChange);
        return 1f;
    }

    Vector3 getScale(Vector2 pPosChange,DragMode pDragMode)
    {
        pPosChange *= scaleValue;
        //switch (dragMode)
        //{
        //    case DragMode.XY:
        //        return new Vector3(getScale(pPosChange.x), getScale(pPosChange.y), 1f);
        //    case DragMode.XZ:
        //        return new Vector3(getScale(pPosChange.x), 1f, getScale(pPosChange.y));
        //}
        switch (dragMode)
        {
            case DragMode.XY:
                return new Vector3((pPosChange.x), (pPosChange.y), 0f);
            case DragMode.XZ:
                return new Vector3((pPosChange.x), 0f, (pPosChange.y));
        }
        Debug.LogError("dragMode error");
        return Vector3.one;

    }

    void FixedUpdate()
    {
        if (dragMode != DragMode.none)
        {
            var lNewPos = getMousePosition();
            //print(lNewPos);
            //print(lastMousePosition);
            //print(lNewPos - lastMousePosition);
            //print(getScale(lNewPos - lastMousePosition, dragMode));
            editableObject.transformScale(getScale(lNewPos - lastMousePosition, dragMode) );
            lastMousePosition = lNewPos;
        }
    }

    public override void OnLeftOn(GameObject pObject)
    {
        OnDragObject(pObject, DragMode.XY);
    }

    public override void OnLeftOff(GameObject pObject)
    {
        endDrag();
    }

    public override void OnRightOn(GameObject pObject)
    {
        OnDragObject(pObject, DragMode.XZ);
    }

    public override void OnRightOff(GameObject pObject)
    {
        endDrag();
    }

}