using UnityEngine;
using System.Collections;

public class SerializeScene:MonoBehaviour
{
    public IEnumerable enumerateObject;

    public delegate void AddObjectEvent(GameObject pObject);

    AddObjectEvent addObjectEvent;

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    public object serializeTo()
    {
        return serializeToArray();
    }

    public ArrayList serializeToArray()
    {
        ArrayList lOut = new ArrayList();
        foreach (Transform lObject in enumerateObject)
        {
            lOut.Add(serializeObjectToTable(lObject.gameObject));
        }
        return lOut;
    }

    public Hashtable serializeObjectToTable(GameObject pObject)
    {
        var lPropertySetting = pObject.GetComponent<ObjectPropertySetting>();
        var lOut = lPropertySetting.serializeToTable();
        lOut["#Type"] = lPropertySetting.TypeName;
        var lTransform = pObject.transform;
        lOut["#position"] = lTransform.localPosition;
        lOut["#rotation"] = lTransform.localRotation;
        lOut["#scale"] = lTransform.localScale;
        return lOut;
    }

    public void serializeFrom(object pObject)
    {
        serializeFromArray(pObject as ArrayList);
    }

    public void serializeFromArray(ArrayList pArray)
    {
        foreach (Hashtable lTable in pArray)
        {
            serializeObjectFromTable(lTable);
        }
    }

    public void serializeObjectFromTable(Hashtable pTable)
    {
        var lObject = GameSystem.Singleton
            .createObject((string)pTable["#Type"]);
        addObjectEvent(lObject);

        var lPosition = (Vector3)pTable["#position"];
        var lRotation = (Quaternion)pTable["#rotation"];
        var lScale = (Vector3)pTable["#scale"];
        lObject.GetComponent<ObjectPropertySetting>().serializeFromTable(pTable);
        var lTransform = lObject.transform;
        lTransform.localPosition = lPosition;
        lTransform.localRotation = lRotation;
        lTransform.localScale = lScale;

    }

}