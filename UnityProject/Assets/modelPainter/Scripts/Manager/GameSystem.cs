﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        public string name;
        public string showName;
        public GameObject prefab;
    }

    Dictionary<string, GameObject> nameToPrefab = new Dictionary<string,GameObject>();

    [System.Serializable]
    public class RenderMaterialInfo
    {
        public string name;
        public string showName;
        public Material material;
    }

    public PrefabInfo[] PrefabInfoList;

    public GameObject createObject(string pTypeName,Vector3 position,Quaternion rotation)
    {
        var lOut =(GameObject)Instantiate(nameToPrefab[pTypeName], position, rotation);
        lOut.GetComponent<ObjectPropertySetting>().TypeName = pTypeName;
        return lOut;
    }

    public GameObject createObject(string pTypeName)
    {
        var lOut = (GameObject)Instantiate(nameToPrefab[pTypeName]);
        lOut.GetComponent<ObjectPropertySetting>().TypeName = pTypeName;
        return lOut;
    }

    public RenderMaterialInfo[] renderMaterialInfoList;

    RenderMaterialResource[] renderMaterialResources;

    public RenderMaterialResource getRenderMaterial(int index)
    {
        return renderMaterialResources[index];
    }

    static protected GameSystem singletonInstance;

    public GameObject controlPointLinePrefab;

    public GameObject paintingObjectPrefab;

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

        foreach (var lPrefabInfo in PrefabInfoList)
        {
            nameToPrefab[lPrefabInfo.name] = lPrefabInfo.prefab;
        }

    }

}