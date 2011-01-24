using UnityEngine;
using System.Collections;
using System.Reflection;


[System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
public class FieldUIAttribute:UiAttributeBase
{

    public string stringBuffer = "";

    public override void clearBuffer()
    {
        stringBuffer = "";
    }

    public FieldUIAttribute(string labelName)
    {
        label = labelName;
    }

    string TextField(string text,int maxLength)
    {
        var lStyle = skin.FindStyle("FieldUI");
        if (lStyle!=null)
            return GUILayout.TextField(text, maxLength, lStyle);
        return GUILayout.TextField(text, maxLength);
    }

    public void floatField(object pObject, PropertyInfo pPropertyInfo)
    {
        //无缓存内容时,显示属性内的值
        if (stringBuffer.Length == 0)
            stringBuffer
                = TextField(pPropertyInfo.GetValue(pObject, null).ToString(), 8);
        else
        {
            string lNewText = TextField(stringBuffer, 8);
            if (lNewText != stringBuffer)
            {
                stringBuffer = lNewText;
                try
                {
                    pPropertyInfo.SetValue(pObject, float.Parse(stringBuffer), null);
                }
                catch (System.Exception e)
                {
                }
            }
        }
    }

    public void boolField(object pObject, PropertyInfo pPropertyInfo)
    {
        bool lValue = (bool)pPropertyInfo.GetValue(pObject, null);
        bool lNewValue = GUILayout.Toggle(lValue,"");
        if(lValue!=lNewValue)
            pPropertyInfo.SetValue(pObject, lNewValue, null);
    }

    public override void impUI(object pObject, MemberInfo pMemberInfo)
    {
        PropertyInfo pPropertyInfo = (PropertyInfo)pMemberInfo;
        //float lValue = (float)pPropertyInfo.GetValue(pObject, null);
        GUILayout.Label(label);
        var lPropertyType = pPropertyInfo.PropertyType;
        if (lPropertyType == typeof(float))
            floatField(pObject, pPropertyInfo);
        else if (lPropertyType == typeof(bool))
            boolField(pObject, pPropertyInfo);
        else
            Debug.LogError("no ui in the type ");
    }
}