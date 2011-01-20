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
        return GUILayout.TextField(text);
    }

    public override void impUI(object pObject, MemberInfo pMemberInfo)
    {
        PropertyInfo pPropertyInfo = (PropertyInfo)pMemberInfo;
        //float lValue = (float)pPropertyInfo.GetValue(pObject, null);
        GUILayout.Label(label);
        if (stringBuffer.Length == 0)
            stringBuffer
                = TextField(pPropertyInfo.GetValue(pObject, null).ToString(),8);
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
}