using UnityEngine;
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
            objectPropertySetting.clearUiBuffer();

        ObjectPropertySetting lSetting = null;
        if(pObject)
        {
            //获取最父级的ObjectPropertySetting
            lSetting = pObject.GetComponent<ObjectPropertySetting>();
            Transform lTransform = pObject.transform.parent;
            while (lTransform)
            {
                lSetting = lTransform.GetComponent<ObjectPropertySetting>();
                lTransform = lTransform.parent;
            }

        }
        objectPropertySetting = lSetting;
    }

}