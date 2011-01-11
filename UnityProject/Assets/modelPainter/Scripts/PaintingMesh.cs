using UnityEngine;
using System.Collections;

public class PaintingMesh:MonoBehaviour
{
    public Vector2 pictureSize;

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

    public Mesh mesh;
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
}