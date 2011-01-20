using System;
using System.Reflection;
using UnityEngine;

public abstract class UiAttributeBase:Attribute
{
    public GUISkin skin;

    public string label;

    public abstract void impUI(object pObject, MemberInfo pMemberInfo);

    public virtual void clearBuffer()
    {

    }
}