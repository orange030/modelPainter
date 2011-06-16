using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public partial class ObjectPropertySetting:MonoBehaviour
{
    static Dictionary<System.Type, IPropertyGUI> typeToUiItems
        = new Dictionary<System.Type, IPropertyGUI>();

    public string TypeName;

    public MonoBehaviour[] UiObjects;

    public IPropertyGUI[] PropertyGUIList;

    public Hashtable serializeToTable()
    {
        var lOut = new Hashtable();
        var lPropertyData = new Hashtable();
        foreach (var lObject in UiObjects)
        {
            lPropertyData[lObject.GetType().Name]
                = zzSerializeObject.Singleton.serializeToTable(lObject);
        }
        lOut["#Property"] = lPropertyData;
        return lOut;
    }

    //public MonoBehaviour addObject(System.Type pType)
    //{
    //    var lOut = (MonoBehaviour)gameObject.AddComponent(pType);
    //    var lNewList = new List<MonoBehaviour>(UiObjects);
    //    lNewList.Add(lOut);
    //    UiObjects = lNewList.ToArray();
    //    return lOut;
    //}

    public MonoBehaviour getObject(string pType)
    {
        var lOut = System.Array.Find(UiObjects, (x) => x.GetType().Name == pType);
        //if (!lOut)
        //{
        //    lOut = addObject(pType);
        //}
        return lOut;
    }

    void impFuncByName(object pObject,string pMethodName)
    {
        var lMethod = pObject.GetType().GetMethod(pMethodName);
        if (lMethod != null)
            lMethod.Invoke(pObject, null);
    }

    public void serializeFromTable(Hashtable pTable)
    {
        Hashtable lPropertyData = (Hashtable)pTable["#Property"];
        foreach (DictionaryEntry lDir in lPropertyData)
        {
            var lObject = getObject(lDir.Key as string);
            impFuncByName(lObject, "BeginSerialization");
            zzSerializeObject.Singleton
                .serializeFromTable(lObject, lDir.Value as Hashtable);
        }

        foreach (var lObject in UiObjects)
        {
            impFuncByName(lObject, "EndSerialization");
        }
    }

    void Start()
    {
        PropertyGUIList = new IPropertyGUI[UiObjects.Length];
        for (int i = 0; i < UiObjects.Length;++i )
        {
            PropertyGUIList[i] = getPropertyGUI(UiObjects[i].GetType());
        }
    }

    static IPropertyGUI getPropertyGUI(System.Type lType)
    {
        IPropertyGUI lOut;
        if (!typeToUiItems.TryGetValue(lType, out lOut))
        {
            var lGetPropertyGUI = lType.GetMethod("get_PropertyGUI");
            if (lGetPropertyGUI != null)
            {
                lOut = (IPropertyGUI)lGetPropertyGUI.Invoke(null,null);
            }
            else
            {
                lOut = new CPropertyGUI(getUiItemList(lType));
            }
            typeToUiItems[lType] = lOut;
        }
        return lOut;
    }

    public  void beginImpUI(ObjectPropertyWindow pWindow)
    {
        foreach (var lPropertyGUI in PropertyGUIList)
        {
            lPropertyGUI.beginImpGUI(pWindow);
        }
    }

    public void endImpUI()
    {
        foreach (var lPropertyGUI in PropertyGUIList)
        {
            lPropertyGUI.endImpGUI();
        }
    }

    public void impUI(Rect rect)
    {
        for( int i=0;i< PropertyGUIList.Length;++i)
        {
            var lPropertyGUI = PropertyGUIList[i];
            lPropertyGUI.skin = skin;
            lPropertyGUI.windowRect = rect;
            lPropertyGUI.OnPropertyGUI(UiObjects[i]);
        }
    }

    public GUISkin skin;

}