﻿using UnityEngine;
using System.Collections;

public class ObjectPropertyWindow : zzWindow
{
    public ObjectPropertySetting objectPropertySetting;
    public GUISkin objectPropertyUiSkin;
    public Vector2 scrollPosition;

    public override void impWindow(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginVertical();

        if (objectPropertySetting)
        {
            objectPropertySetting.skin = objectPropertyUiSkin;
            objectPropertySetting.impUI();
        }

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    public void setObjectToShow(GameObject pObject)
    {
        if (objectPropertySetting)
        {
            objectPropertySetting.endImpUI();
        }

        ObjectPropertySetting lSetting = null;
        if(pObject)
        {
            //获取最父级的ObjectPropertySetting
            lSetting = pObject.GetComponent<ObjectPropertySetting>();
            Transform lTransform = pObject.transform.parent;
            while (lTransform)
            {
                var lParentSetting = lTransform.GetComponent<ObjectPropertySetting>();
                if (lParentSetting)
                    lSetting = lParentSetting;
                lTransform = lTransform.parent;
            }
        }
        objectPropertySetting = lSetting;
        if (objectPropertySetting)
            objectPropertySetting.beginImpUI(this);
    }

}