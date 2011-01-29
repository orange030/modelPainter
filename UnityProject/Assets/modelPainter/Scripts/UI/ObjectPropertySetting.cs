using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class ObjectPropertySetting:MonoBehaviour
{
    //public string[] UiItemInfos = new string[0];

    //enum UiType
    //{
    //    property,
    //}

    struct UiItem
    {
        public UiItem(UiAttributeBase pUiType,MemberInfo pMemberInfo)
        {
            uiType = pUiType;
            memberInfo = pMemberInfo;
        }

        public UiAttributeBase uiType;
        public MemberInfo memberInfo;
    }

    class CPropertyGUI:IPropertyGUI
    {
        UiItem[] uiItemList;

        public CPropertyGUI(UiItem[] pList)
        {
            uiItemList = pList;
        }

        public override void endImpGUI()
        {
            clearUiBuffer();
        }

        public override void OnPropertyGUI(MonoBehaviour pObject)
        {
            foreach (var lUiItem in uiItemList)
            {
                GUILayout.BeginHorizontal();
                lUiItem.uiType.skin = skin;
                lUiItem.uiType.impUI(pObject, lUiItem.memberInfo);
                GUILayout.EndHorizontal();
            }
        }

        public void clearUiBuffer()
        {
            foreach (var lUiItem in uiItemList)
            {
                lUiItem.uiType.clearBuffer();
            }
        }

    }

    static Dictionary<System.Type, IPropertyGUI> typeToUiItems
        = new Dictionary<System.Type, IPropertyGUI>();


    public MonoBehaviour[] UiObjects;

    public IPropertyGUI[] PropertyGUIList;

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
        if (typeToUiItems.ContainsKey(lType))
            lOut = typeToUiItems[lType];
        else
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

    private static UiItem[] getUiItemList(System.Type lType)
    {
        UiItem[] lOut;
        var lUiItems = new List<UiItem>();
        var lMembers = lType.GetMembers();
        foreach (var lMember in lMembers)
        {
            UiAttributeBase[] lUIAttributes =
                (UiAttributeBase[])lMember.GetCustomAttributes(typeof(UiAttributeBase), false);
            if (lUIAttributes.Length > 0)
                lUiItems.Add(new UiItem(lUIAttributes[0], lMember));
        }
        lOut = lUiItems.ToArray();

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

    public  void impUI()
    {
        for( int i=0;i< PropertyGUIList.Length;++i)
        {
            var lPropertyGUI = PropertyGUIList[i];
            lPropertyGUI.skin = skin;
            lPropertyGUI.OnPropertyGUI(UiObjects[i]);
        }
    }

    public GUISkin skin;

}