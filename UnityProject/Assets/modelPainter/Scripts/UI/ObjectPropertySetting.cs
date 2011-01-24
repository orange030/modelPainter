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

    static Dictionary<System.Type, UiItem[]> typeToUiItems
        = new Dictionary<System.Type,UiItem[]>();


    public MonoBehaviour UiObject;
    UiItem[] uiItemList;

    void Start()
    {
        uiItemList = getUiItemList(UiObject.GetType());
    }

    private static UiItem[] getUiItemList(System.Type lType)
    {
        UiItem[] lOut;
        if (typeToUiItems.ContainsKey(lType))
            lOut = typeToUiItems[lType];
        else
        {
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
            typeToUiItems[lType] = lOut;

        }
        return lOut;

    }

    public  void beginImpUI(ObjectPropertyWindow pWindow)
    {

    }

    public void endImpUI()
    {
        clearUiBuffer();
    }

    public  void impUI()
    {
        foreach (var lUiItem in uiItemList)
        {
            GUILayout.BeginHorizontal();
            lUiItem.uiType.skin = skin;
            lUiItem.uiType.impUI(UiObject, lUiItem.memberInfo);
            GUILayout.EndHorizontal();
        }
    }

    public GUISkin skin;

    public void clearUiBuffer()
    {
        foreach (var lUiItem in uiItemList)
        {
            lUiItem.uiType.clearBuffer();
        }
    }
}