using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class zzSerializeObject
{
    static protected zzSerializeObject singletonInstance = new zzSerializeObject();

    public static zzSerializeObject Singleton
    {
        get { return singletonInstance; }
    }

    public Dictionary<System.Type, PropertyInfo[]> typeToSerializeMethod
        = new Dictionary<System.Type,PropertyInfo[]>();

    public Hashtable serializeToTable(object pObject)
    {
        var lList = getSerializeMethod(pObject.GetType());
        var lOut = new Hashtable();
        lOut["#ClassType"] = pObject.GetType().Name;
        foreach (var lPropertyInfo in lList)
        {
            var lValue = lPropertyInfo.GetValue(pObject, null);

            ////非支持类型,则转为table
            //if(!zzSerializeString.Singleton.isSupportedType(lValue.GetType()))
            //{
            //    if (lValue is System.Array)
            //    {
            //        var lArrayList = new ArrayList();
            //        foreach (var lElement in lValue as System.Array)
            //        {
            //            lArrayList.Add(serializeToTable(lElement));
            //        }
            //        lValue = lArrayList;
            //    }
            //    else
            //        lValue = serializeToTable(lValue);

            //}

            //Debug.Log("Name:" + lPropertyInfo.Name + " " + lValue);
            lOut[lPropertyInfo.Name] = lValue;
        }
        return lOut;
    }

    public void serializeFromTable(object pObject, Hashtable pTable)
    {
        var lList = getSerializeMethod(pObject.GetType());
        foreach (var lPropertyInfo in lList)
        {
            var lPropertyType = lPropertyInfo.GetType();

            if (pTable.Contains(lPropertyInfo.Name))
            {
                var lValue = pTable[lPropertyInfo.Name];
            //    var lTable = lValue as Hashtable;
            //    var lArray = lValue as System.Array;
            //    if (lTable!=null
            //        &&lTable.Contains("#ClassName"))
            //    {

            //    }
                //    else

                //Debug.Log("Name:" + lPropertyInfo.Name + " " + lValue);
                lPropertyInfo.SetValue(pObject, lValue, null);
            }
        }
        
    }

    PropertyInfo[] getSerializeMethod(System.Type lType)
    {
        if(typeToSerializeMethod.ContainsKey(lType))
            return typeToSerializeMethod[lType];
        var lOut = createSerializeMethod(lType);
        typeToSerializeMethod[lType] = lOut;
        return lOut;
    }

    PropertyInfo[] createSerializeMethod(System.Type lType)
    {
        List<PropertyInfo> lOut = new List<PropertyInfo>();
        var lMembers = lType.GetMembers();
        foreach (var lMember in lMembers)
        {
            zzSerializeAttribute[] lAttributes =
                (zzSerializeAttribute[])lMember.GetCustomAttributes(typeof(zzSerializeAttribute), false);
            if (lAttributes.Length > 0)
            {
                lOut.Add((PropertyInfo)lMember);
            }
        }

        return lOut.ToArray();
    }

}