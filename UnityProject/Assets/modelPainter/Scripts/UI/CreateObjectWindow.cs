using UnityEngine;
using System.Collections;

public class CreateObjectWindow : zzWindow
{
    public delegate void AddObjectEvent(GameObject pObject);

    static void nullAddObjectEvent(GameObject pObject)
    {

    }

    AddObjectEvent addObjectEvent;

    public void addAddObjectEventReceiver(AddObjectEvent pFunc)
    {
        addObjectEvent += pFunc;
    }

    void Start()
    {
        if (addObjectEvent == null)
            addObjectEvent = nullAddObjectEvent;
        if(getCreateInfoFromSystem)
        {
            var lPrefabInfos = GameSystem.Singleton.PrefabInfoList;
            objectCreateInfos = new ObjectCreateInfo[lPrefabInfos.Length];
            for (int i = 0; i < lPrefabInfos.Length; ++i)
            {
                var lObjectCreateInfo = new ObjectCreateInfo();
                var lPrefabInfo = lPrefabInfos[i];
                lObjectCreateInfo.name = lPrefabInfo.showName;
                lObjectCreateInfo.prefab = lPrefabInfo.prefab;
                objectCreateInfos[i] = lObjectCreateInfo;
            }

        }

    }

    [System.Serializable]
    public class ObjectCreateInfo
    {
        public string name;
        public GameObject prefab;
    }

    public float createObjectDistance = 5f;

    public bool getCreateInfoFromSystem = false;

    public ObjectCreateInfo[] objectCreateInfos = new ObjectCreateInfo[0];

    Vector3 getCreatePos()
    {
        Transform lTransform = Camera.main.transform;
        return lTransform.position + lTransform.forward * createObjectDistance;
    }

    public bool showCreatePosition = false;

    void OnDrawGizmosSelected()
    {
        if (showCreatePosition)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(getCreatePos(), 0.2f);
        }
    }

    public override void impWindow(int windowID)
    {
        GUILayout.BeginVertical();
        foreach (var lInfo in objectCreateInfos)
        {
            if (GUILayout.Button(lInfo.name, GUILayout.ExpandWidth(false)))
            {
                var lObject = (GameObject)Instantiate(lInfo.prefab,
                    getCreatePos(), Quaternion.identity);
                addObjectEvent(lObject);
            }
        }
        GUILayout.EndVertical();
    }
}