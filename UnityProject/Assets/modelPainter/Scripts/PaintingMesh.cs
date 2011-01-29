﻿using UnityEngine;
using System.Collections;


public class PaintingModelData
{
    //public PaintingModelData()
    //{
    //    Debug.Log("new PaintingModelData");
    //}

    public struct ModelData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;

        public void setTransform(Transform pTransform)
        {
            localPosition = pTransform.localPosition;
            localRotation = pTransform.localRotation;
            localScale = pTransform.localScale;
        }

        public void setToTransform(Transform pTransform)
        {
            pTransform.localPosition = localPosition;
            pTransform.localRotation = localRotation;
            pTransform.localScale = localScale;
        }

        public Mesh mesh;
    }

    public ModelData renderMesh;
    public ModelData[] colliderMeshes;
    public Vector2 pictureSize;

    static public Hashtable serializeToTable(ModelData pModelData)
    {
        var lData = new Hashtable();
        lData["localPosition"] = pModelData.localPosition;
        lData["localRotation"] = pModelData.localRotation;
        lData["localScale"] = pModelData.localScale;
        lData["mesh"] = serializeToTable(pModelData.mesh);
        return lData;
    }

    static public Hashtable serializeToTable(Mesh pMesh)
    {
        var lData = new Hashtable();
        lData["vertices"] = pMesh.vertices;
        //lData["uv"] = pMesh.uv;
        lData["triangles"] = pMesh.triangles;

        return lData;
    }

    static public ModelData createModelDataFromTable(Hashtable pData, Vector2 pPictureSize)
    {
        ModelData lOut;
        lOut.localPosition = (Vector3)pData["localPosition"];
        lOut.localRotation = (Quaternion)pData["localRotation"];
        lOut.localScale = (Vector3)pData["localScale"];
        lOut.mesh = createMeshFromTable((Hashtable)pData["mesh"], pPictureSize);
        return lOut;
    }

    static public Mesh  createMeshFromTable(Hashtable pData,Vector2 pPictureSize)
    {
        Mesh lOut = new Mesh();
        lOut.vertices = (Vector3[])pData["vertices"];
        lOut.triangles = (int[])pData["triangles"];
        lOut.uv = zzFlatMeshUtility.verticesCoordToUV(lOut.vertices,
            zzFlatMeshUtility.getUvScaleFromImgSize(pPictureSize));
        lOut.normals = zzFlatMeshUtility.getNormals(lOut.vertices.Length);
        return lOut;
    }

    public string serializeToString()
    {
        var lData = new Hashtable();
        lData["pictureSize"] = pictureSize;
        lData["renderMesh"] = serializeToTable(renderMesh);
        var lColliderMeshesData = new ArrayList(colliderMeshes.Length);
        foreach (var lColliderMeshe in colliderMeshes)
        {
            lColliderMeshesData.Add(serializeToTable(lColliderMeshe));
        }
        lData["colliderMeshes"] = lColliderMeshesData;
        return zzSerializeString.Singleton.pack(lData);
    }

    static public PaintingModelData createDataFromString(string pStringData)
    {
        PaintingModelData lOut = new PaintingModelData();
        Hashtable lData = (Hashtable)zzSerializeString.Singleton.unpackToData(pStringData);
        lOut.pictureSize = (Vector2)lData["pictureSize"];
        lOut.renderMesh =
            createModelDataFromTable((Hashtable)lData["renderMesh"], lOut.pictureSize);

        ArrayList lColliderMeshesData = (ArrayList)lData["colliderMeshes"];
        lOut.colliderMeshes = new ModelData[lColliderMeshesData.Count];
        for (int i = 0; i < lColliderMeshesData.Count;++i )
        {
            lOut.colliderMeshes[i] =
            createModelDataFromTable((Hashtable)lColliderMeshesData[i], lOut.pictureSize);
        }
        return lOut;
    }

    static public PaintingModelData createData(GameObject pModelObject, Vector2 pPictureSize)
    {
        var lPaintingMesh = new PaintingModelData();
        var lMeshFilter = pModelObject.transform.Find("Render").GetComponent<MeshFilter>();
        lPaintingMesh.renderMesh.mesh = lMeshFilter.mesh;
        lPaintingMesh.renderMesh.setTransform(lMeshFilter.transform);
        var lMeshCollider = pModelObject.GetComponentsInChildren<MeshCollider>();
        //var lColliderMeshes = new Mesh[lMeshCollider.Length]; 
        var lColliderMeshes = new ModelData[lMeshCollider.Length];
        for (int i = 0; i < lMeshCollider.Length; ++i)
        {
            lColliderMeshes[i].mesh = lMeshCollider[i].mesh;
            lColliderMeshes[i].setTransform(lMeshCollider[i].transform);
        }
        lPaintingMesh.colliderMeshes = lColliderMeshes;

        lPaintingMesh.pictureSize = pPictureSize;

        return lPaintingMesh;

    }

}

public class PaintingMesh : zzEditableObject
{
    public static PaintingMesh  create(GameObject lObject,PaintingModelData pData)
    {
        //print("GameObject" + lObject.name);
        Transform lTransform = lObject.transform;
        var lOut = lObject.AddComponent<PaintingMesh>();
        lOut.modelData = pData;
        lObject.AddComponent<Rigidbody>();
        lObject.AddComponent<zzObjectElement>();
        lObject.AddComponent<zzEditableObject>();
        var lRenderObject = new GameObject("Render");
        lOut.paintRenderer = lRenderObject.AddComponent<MeshRenderer>();
        lRenderObject.AddComponent<MeshFilter>().mesh = pData.renderMesh.mesh;
        lRenderObject.transform.parent = lTransform;
        pData.renderMesh.setToTransform(lRenderObject.transform);
        int i = 0;
        foreach (var lColliderMeshes in pData.colliderMeshes)
        {
            var lColliderObject = new GameObject("Collider" + i);
            lColliderObject.transform.parent = lTransform;
            var lMeshCollider = lColliderObject.AddComponent<MeshCollider>();
            lMeshCollider.convex = true;
            lMeshCollider.sharedMesh = lColliderMeshes.mesh;
            lColliderMeshes.setToTransform(lColliderObject.transform);
            ++i;
        }
        return lOut;
    }

    public override void transformScale(Vector3 pScale)
    {
        var lLocalScale = transform.localScale;
        lLocalScale.Scale(pScale);
        transform.localScale = lLocalScale;
    }

    [SerializeField]
    Vector2 _extraTextureOffset = Vector2.zero;

    public Vector2 extraTextureOffset
    {
        get { return _extraTextureOffset; }
        set
        {
            _extraTextureOffset = value;
            material.mainTextureOffset
                = material.mainTextureOffset + _extraTextureOffset;
        }
    }

    [SerializeField]
    Vector2 _extraTextureScale = new Vector2(1.0f, 1.0f);

    public Vector2 extraTextureScale
    {
        get { return _extraTextureScale; }
        set 
        {
            _extraTextureScale = value;
            var lMainTextureScale = material.mainTextureScale;
            lMainTextureScale.Scale(_extraTextureScale) ;
            material.mainTextureScale = lMainTextureScale;
        }
    }


    public string materialName;
    public bool useCustomImage = false;
    public string imageName;

    public PaintingModelData modelData;

    public Material sharedMaterial;

    [SerializeField]
    private Material _material;

    public Material material
    {
        get { return _material; }
        set 
        {
            _material = value;
            _meshRenderer.material = value;
        }
    }

    [SerializeField]
    private MeshRenderer _meshRenderer;

    public MeshRenderer paintRenderer
    {
        get { return _meshRenderer; }
        set 
        {
            _meshRenderer = value;
            //_material = value.material;
        }
    }

    public void useImageMaterial(Texture2D pTexture)
    {
        var lMaterial = new Material(Shader.Find("Diffuse"));
        lMaterial.mainTexture = pTexture;
        sharedMaterial = null;
        material = lMaterial;
        useCustomImage = true;
    }
}