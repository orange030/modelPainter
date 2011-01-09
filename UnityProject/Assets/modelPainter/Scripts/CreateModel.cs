using UnityEngine;
using System.IO;
using System.Collections;

public class CreateModel : MonoBehaviour
{

    GameObject UIRootObject;
    zzSceneObjectMap UIMap;
    public zzSceneImageGUI sceneImageUI;
    public Transform modelPosition;
    public Vector2 modelMaxSize;
    public Material modelMaterial;

    static Vector2 getFitSize(Vector2 pRealSize, Vector2 pMaxSize)
    {
        float lWidth = pRealSize.x;
        float lHeigth = pRealSize.y;

        float lWidthHeigthRate = lWidth / lHeigth;

        if ((pMaxSize.x / pMaxSize.y) > lWidthHeigthRate)
        {
            pMaxSize.x = lWidthHeigthRate * pMaxSize.y;
        }
        else
        {
            pMaxSize.y = pMaxSize.x / lWidthHeigthRate;
        }
        return pMaxSize;
    }

    static Vector2 getFitScale(Vector2 pRealSize, Vector2 pMaxSize)
    {
        var lFitSize = getFitSize(pRealSize, pMaxSize);
        Vector2 lOut = new Vector2(lFitSize.x / pRealSize.x, lFitSize.y / pRealSize.y);
        return lOut;
    }

    void OnDrawGizmos()
    {
        if(modelPosition)
        {
            Gizmos.color = Color.white;
            Vector3 lModelCenter = modelPosition.position;
            Vector3 lMaxExtent = modelMaxSize / 2.0f;
            Vector3 lPoint1 = new Vector3(lModelCenter.x - lMaxExtent.x,
                lModelCenter.y + lMaxExtent.y,
                lModelCenter.z);

            Vector3 lPoint2 = new Vector3(lModelCenter.x + lMaxExtent.x,
                lModelCenter.y + lMaxExtent.y,
                lModelCenter.z);

            Vector3 lPoint3 = new Vector3(lModelCenter.x + lMaxExtent.x,
                lModelCenter.y - lMaxExtent.y,
                lModelCenter.z);

            Vector3 lPoint4 = new Vector3(lModelCenter.x - lMaxExtent.x,
                lModelCenter.y - lMaxExtent.y,
                lModelCenter.z);

            Gizmos.DrawLine(lPoint1, lPoint2);
            Gizmos.DrawLine(lPoint2, lPoint3);
            Gizmos.DrawLine(lPoint3, lPoint4);
            Gizmos.DrawLine(lPoint4, lPoint1);
        }
    }

    void Start()
    {
        UIRootObject = zzObjectMap.getObject("UI");
        UIMap = UIRootObject.GetComponent<zzSceneObjectMap>();
        zzButton lReadImageButton = UIMap.getObject("readImageButton")
            .GetComponent<zzButton>();
        lReadImageButton.clickCall = OnOpenImageClick;

        Time.timeScale = 0.0f;

        //fileBrowserDialogObject = new GameObject("fileBrowserDialog");
        //fileBrowserDialogObject.transform.parent = transform.parent;
    }

    GameObject fileBrowserDialogObject;
    zzFileBrowserDialog fileBrowserDialog;
    public string lastLocation = "";

    public void OnOpenImageClick(zzInterfaceGUI pGUI)
    {
        fileOpenDialog();
    }

    public void OnOpenImage(object sender)
    {
        fileOpenDialog();
    }

    void fileOpenDialog()
    {
        if (!fileBrowserDialog)
        {
            fileBrowserDialog = zzFileBrowserDialog.createDialog(UIRootObject.transform);
            fileBrowserDialog.addExtensionFilter("", new string[] { "png" });
            fileBrowserDialog.relativePosition = new Rect(0.0f, 0.0f, 6.0f / 7.0f, 4.0f / 5.0f);
            fileBrowserDialog.useRelativePosition = new zzGUIRelativeUsedInfo(false, false, true, true);
            fileBrowserDialog.horizontalDockPosition = zzGUIDockPos.center;
            fileBrowserDialog.verticalDockPosition = zzGUIDockPos.center;
            fileBrowserDialog.selectedLocation = lastLocation;
            fileBrowserDialog.fileSelectedCallFunc = OnReadImage;

        }

    }

    zzFlatModelPainter flatModelPainter;

    public void drawModel(object sender)
    {
        if (modelImage)
        {
            flatModelPainter = GetComponent<zzFlatModelPainter>();
            foreach (var lObject in modelObjectList)
            {
                Destroy(lObject);
            }
            flatModelPainter.clear();
            flatModelPainter.picture = modelImage;
            flatModelPainter.thickness = 1.0f;
            drawTimer = gameObject.AddComponent<zzCoroutineTimer>();
            drawTimer.setImpFunction(stepDrawModel);
        }
    }

    zzCoroutineTimer drawTimer;
    public GameObject[] modelObjectList = new GameObject[0];

    void stepDrawModel()
    {
        if (!flatModelPainter.doStep())
        {
            Destroy(drawTimer);
            var lModelsTransform = flatModelPainter.models.transform;
            lModelsTransform.position = modelPosition.position;
            var lScale = getFitScale(flatModelPainter.modelsSize, modelMaxSize);
            lModelsTransform.localScale = new Vector3(lScale.x, lScale.y,1.0f);
            GameObject[] lModelObjectList = new GameObject[lModelsTransform.childCount];
            int i = 0;
            foreach (Transform lSub in lModelsTransform)
            {
                var lGameObject = lSub.gameObject;
                lModelObjectList[i] = lGameObject;
                lGameObject.AddComponent<Rigidbody>();
                lGameObject.AddComponent<zzObjectElement>();
                lGameObject.AddComponent<zzEditableObject>();
                lSub.Find("Render").GetComponent<MeshRenderer>().material = modelMaterial;
                lSub.GetComponentInChildren<zzFlatMeshEdit>().resetUV();
                ++i;
            }
            foreach (var lObject in lModelObjectList)
            {
                lObject.transform.parent = null;
            }
            modelObjectList = lModelObjectList;
        }
    }

    public void    OnPlayButtonClick(object sender)
    {
        Time.timeScale = 1.0f;

    }

    Texture2D modelImage;

    void OnReadImage(zzInterfaceGUI pGUI)
    {
        lastLocation = fileBrowserDialog.selectedLocation;
        zzInterfaceGUI lImageUI = UIMap.getObject("imageUI").GetComponent<zzInterfaceGUI>();
        Texture2D lImage = new Texture2D(4, 4, TextureFormat.ARGB32,false);
        FileStream lImageFile = new FileStream(fileBrowserDialog.selectedLocation, FileMode.Open);
        using( BinaryReader lBinaryReader = new BinaryReader(lImageFile) )
        {
            lImage.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));

        }
        sceneImageUI.image = lImage;
        modelImage = lImage;
        //lImageUI.setImage(lImage);
    }

    // Update is called once per frame
    //void Update () {

    //}
}
