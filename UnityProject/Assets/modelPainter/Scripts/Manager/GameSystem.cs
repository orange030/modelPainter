using UnityEngine;
using System.Collections;

public enum ResourceType
{
    //内建
    builtin,

    //工程
    project,

    //外部文件
    external,

    //实时生成
    realTime,
}

public class GameSystem:MonoBehaviour
{
    [System.Serializable]
    public class PrefabInfo
    {
        public string showName;
        public GameObject prefab;
    }

    [System.Serializable]
    public class RenderMaterialInfo
    {
        public string name;
        public string showName;
        public Material material;
    }

    public PrefabInfo[] PrefabInfoList;

    public RenderMaterialInfo[] renderMaterialInfoList;

    RenderMaterialResource[] renderMaterialResources;

    public RenderMaterialResource getRenderMaterial(int index)
    {
        return renderMaterialResources[index];
    }

    static protected GameSystem singletonInstance;

    public GameObject controlPointLinePrefab;

    public static GameSystem Singleton
    {
        get { return singletonInstance; }
    }

    void Awake()
    {
        if (singletonInstance)
        {
            Debug.LogError("GameSystem.singletonInstance");
        }
        singletonInstance = this;

        renderMaterialResources = new RenderMaterialResource[renderMaterialInfoList.Length];

        for(int i=0;i<renderMaterialInfoList.Length;++i)
        {
            var lResource = new RenderMaterialResource();
            lResource.resourceType = ResourceType.builtin;
            lResource.materialName = renderMaterialInfoList[i].name;
            lResource.material = renderMaterialInfoList[i].material;
            renderMaterialResources[i] = lResource;
        }

    }

}