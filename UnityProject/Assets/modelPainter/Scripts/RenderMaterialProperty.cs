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
    GenericResource<Texture2D> _imageResource;

    public void useImageMaterial(Texture2D pTexture)
    {
        var lMaterial = new Material(Shader.Find("Diffuse"));
        lMaterial.mainTexture = pTexture;
        _sharedMaterial = null;
        material = lMaterial;
    }

    public GenericResource<Texture2D> imageResource
    {
        get { return _imageResource; }
        set 
        { 
            _imageResource = value;
            _resourceType = value.resourceType;
            materialName = value.resourceID;
            useImageMaterial(value.resource);
        }
    }


    [SerializeField]
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

    ResourceType _resourceType = ResourceType.unknown;

    public ResourceType resourceType
    {
        get { return _resourceType; }
    }

    void Awake()
    {
        _resourceType = ResourceType.unknown;
    }

    void Start()
    {
        //if (_resourceType== ResourceType.builtin)
        //{
        //    sharedMaterial = _meshRenderer.material;
        //}
        if (_resourceType == ResourceType.unknown)
        {
            _resourceType = ResourceType.builtin;
            updateMaterial();
        }
    }

    void releaseMaterial()
    {
        if (_material)
        {
            Destroy(_material);
            _material = null;
            //_sharedMaterial = null;
        }
    }

    //--------------------------------------------------------------

    [SerializeField]
    Vector2 _extraTextureOffset = Vector2.zero;

    [zzSerialize]
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

    [zzSerialize]
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

    [zzSerialize]
    public string resourceID
    {
        get { return materialName; }
        set
        {
            materialName = value;
            if (_resourceType != ResourceType.unknown)
                updateMaterial();
        }
    }

    [zzSerialize]
    public string resourceTypeID
    {
        get { return _resourceType.ToString(); }
        set
        {
            _resourceType = (ResourceType)System.Enum.Parse(typeof(ResourceType), value);
            if (materialName.Length > 0)
                updateMaterial();
        }
    }

    public void updateMaterial()
    {
        switch(_resourceType)
        {
            case ResourceType.builtin:
                setMaterial(GameSystem.Singleton.getRenderMaterial(materialName));
                break;
            case ResourceType.realTime:
                imageResource = GameResourceManager.Main.getImage(resourceID);
                break;
        }
    }

    //public string imageName;

    public Material material
    {
        get { return _material; }
        set
        {
            releaseMaterial();
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
        sharedMaterial = pRenderMaterialInfo.material;
        materialName = pRenderMaterialInfo.materialName;
        _resourceType = pRenderMaterialInfo.resourceType;

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