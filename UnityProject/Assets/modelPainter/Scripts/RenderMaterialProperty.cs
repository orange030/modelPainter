using UnityEngine;
using System.Collections;


public class RenderMaterialResource
{
    public string materialName;
    public Material material;
    public Texture2D image;
    public ResourceType resourceType;
}

public class RenderMaterialProperty:MonoBehaviour
{

    private Material _sharedMaterial;

    public Material sharedMaterial
    {
        get { return _sharedMaterial; }
        set 
        {
            //共享材质 必须先被赋值
            _sharedMaterial = value;
            material = new Material(value);
        }
    }

    [SerializeField]
    private Material _material;

    ResourceType _resourceType;

    void Start()
    {
        if (_resourceType== ResourceType.builtin)
        {
            sharedMaterial = _meshRenderer.material;
        }
    }

    void releaseMaterial()
    {
        if (_material)
        {
            Destroy(_material);
            _material = null;
            _sharedMaterial = null;
        }
    }

    //--------------------------------------------------------------

    [SerializeField]
    Vector2 _extraTextureOffset = Vector2.zero;

    public Vector2 extraTextureOffset
    {
        get { return _extraTextureOffset; }
        set
        {
            _extraTextureOffset = value;
            //material.mainTextureOffset
            //    = material.mainTextureOffset + _extraTextureOffset;
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
            //var lMainTextureScale = material.mainTextureScale;
            //lMainTextureScale.Scale(_extraTextureScale);
            //material.mainTextureScale = lMainTextureScale;
        }
    }

    void updateTextureTransform()
    {
        if (sharedMaterial)
        {
            material.mainTextureOffset
                = sharedMaterial.mainTextureOffset + _extraTextureOffset;
            var lMainTextureScale = sharedMaterial.mainTextureScale;
            lMainTextureScale.Scale(_extraTextureScale);
            material.mainTextureScale = lMainTextureScale;
        }
        else
        {
            material.mainTextureOffset = _extraTextureOffset;
            material.mainTextureScale = _extraTextureScale;
        }
    }


    public string materialName;

    //public string imageName;

    public Material material
    {
        get { return _material; }
        set
        {
            _material = value;
            _meshRenderer.material = value;
            updateTextureTransform();
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
        }
    }

    public void setMaterial(RenderMaterialResource pRenderMaterialInfo)
    {
        releaseMaterial();
        sharedMaterial = pRenderMaterialInfo.material;
        materialName = pRenderMaterialInfo.materialName;

    }

    void OnDisable()
    {
        if (!gameObject.active)
        {
            releaseMaterial();
        }
    }


    static RenderMaterialGUI renderMaterialGUI;

    public static IPropertyGUI PropertyGUI
    {
        get 
        {
            if (renderMaterialGUI==null)
                renderMaterialGUI = new RenderMaterialGUI();
            return renderMaterialGUI;
        }
    }

}

public class RenderMaterialGUI:IPropertyGUI
{
    Vector2 scrollPosition;

    public override void OnPropertyGUI(MonoBehaviour pObject)
    {
        var lRenderMaterialProperty = (RenderMaterialProperty)pObject;
        GUILayout.Label("材质列表");
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(100));
        var lSelected = drawMaterialSelectList();
        GUILayout.EndScrollView();

        if (lSelected!=null
            &&lSelected.material != lRenderMaterialProperty.sharedMaterial)
        {
            lRenderMaterialProperty.setMaterial(lSelected);
        }
    }

    RenderMaterialResource drawMaterialSelectList()
    {
        RenderMaterialResource lOut = null;
        var lRenderMaterialInfoList = GameSystem.Singleton.renderMaterialInfoList;
        for (int i = 0; i < lRenderMaterialInfoList.Length;++i )
        {
            if (GUILayout.Button(lRenderMaterialInfoList[i].showName))
                lOut = GameSystem.Singleton.getRenderMaterial(i);
        }
        return lOut;
    }
}