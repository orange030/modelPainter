using UnityEngine;
using System.Collections;
using System.Reflection;


[System.AttributeUsage(System.AttributeTargets.Property)]
public class FieldUIAttribute:UiAttributeBase
{

    public string stringBuffer = null;

    public override void clearBuffer()
    {
        stringBuffer = null;
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

    public bool isFloat(string pText)
    {
        bool lOut = false;
        try
        {
            float.Parse(pText);
            lOut = true;
        }
        catch{}
        return lOut;
    }

    public void floatField(object pObject, PropertyInfo pPropertyInfo)
    {
        float lPreValue = (float)pPropertyInfo.GetValue(pObject, null);
        ////无内容时 获取初始值
        //if (stringBuffer.Length == 0)
        //{
        //    stringBuffer = lPreValue.ToString();
        //}

        string lPreText;
        if(stringBuffer!=null)
        {
            lPreText = stringBuffer;
        }
        else
        {
            lPreText = lPreValue.ToString();
        }

        string lNewText = TextField(lPreText, 8);
        if(lPreText!=lNewText)
        {
            try
            {
                pPropertyInfo.SetValue(pObject, float.Parse(lNewText), null);
                stringBuffer = null;
            }
            catch (System.Exception e)
            {
                stringBuffer = lNewText;
            }
        }

        //string lNewText;
        //// 为可用的数字时,更新
        //if (isFloat(stringBuffer))
        //{
        //    string lPreText = lPreValue.ToString();
        //    lNewText
        //        = TextField(lPreValue.ToString(), 8);
        //}
        ////////无缓存内容时,显示属性内的值
        //////if (stringBuffer.Length == 0)
        //////    stringBuffer
        //////        = TextField(pPropertyInfo.GetValue(pObject, null).ToString(), 8);
        ////else
        //{
        //    lNewText = TextField(stringBuffer, 8);
        //}

        //if (lNewText != lPreValue.ToString())
        //{
        //    stringBuffer = lNewText;
        //    try
        //    {
        //        pPropertyInfo.SetValue(pObject, float.Parse(stringBuffer), null);
        //    }
        //    catch (System.Exception e)
        //    {
        //    }
        //}
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