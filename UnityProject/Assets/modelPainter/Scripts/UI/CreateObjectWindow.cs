using UnityEngine;
using System.Collections;

public class CreateObjectWindow : zzWindow
{

    [System.Serializable]
    public class ObjectCreateInfo
    {
        public string name;
        public GameObject prefab;
    }

    public float createObjectDistance = 5f;

    public ObjectCreateInfo[] objectCreateInfos = new ObjectCreateInfo[0];

    Vector3 getCreatePos()
    {
        Transform lTransform = Camera.main.transform;
        return lTransform.position + lTransform.forward * createObjectDistance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(getCreatePos(), 0.2f);
    }

    public override void impWindow(int windowID)
    {
        GUILayout.BeginVertical();
        foreach (var lInfo in objectCreateInfos)
        {
            if (GUILayout.Button(lInfo.name, GUILayout.ExpandWidth(false)))
            {
                Instantiate(lInfo.prefab, getCreatePos(), Quaternion.identity);
            }
        }
        GUILayout.EndVertical();
    }
}