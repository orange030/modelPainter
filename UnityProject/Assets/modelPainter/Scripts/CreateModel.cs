using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CreateModel : MonoBehaviour
{

    GameObject UIRootObject;
    zzSceneObjectMap UIMap;
    public zzSceneImageGUI sceneImageUI;
    public Transform modelPosition;
    public Vector2 modelMaxSize;
    public Material modelMaterial;

    public zzSceneManager sceneManager;

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

        logFile = new FileStream(Application.dataPath + "/../modelPainter.log",FileMode.Create);
        logWriter = new StreamWriter(logFile);
        logWriter.AutoFlush = true;
        Application.RegisterLogCallback(HandleLog);

        //Time.timeScale = 0.0f;

        //fileBrowserDialogObject = new GameObject("fileBrowserDialog");
        //fileBrowserDialogObject.transform.parent = transform.parent;
    }

    FileStream logFile;
    StreamWriter logWriter;

    void HandleLog(string logString, string stackTrace, LogType pLogType)
    {

        string lLog = pLogType.ToString() + ": " + logString + "\n" + stackTrace + "\n";

        logWriter.Write(lLog);
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
            fileBrowserDialog.addFileFilter("", new string[] { "*.png" });
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
        if (nowPainterOutData!=null && !drawTimer)
        {
            if (nowPainterOutData.haveModelData)
            {
                createObject(nowPainterOutData);
            }
            else
            {
                flatModelPainter = GetComponent<zzFlatModelPainter>();

                flatModelPainter.clear();
                flatModelPainter.picture = nowPainterOutData.modelImage;
                flatModelPainter.thickness = 1.0f;
                drawTimer = gameObject.AddComponent<zzCoroutineTimer>();
                drawTimer.setImpFunction(stepDrawModel);

            }
        }
    }

    zzCoroutineTimer drawTimer;
    //public GameObject[] modelObjectList = new GameObject[0];

    void clearModelObjects()
    {
        foreach (Transform lObject in sceneManager)
        {
            Destroy(lObject.gameObject);
        }

    }

    void addModelObjects(GameObject[] pModelObjects)
    {
        sceneManager.addObjects(pModelObjects);
        updateObjectState(pModelObjects);
    }

    void stepDrawModel()
    {
        if (!flatModelPainter.doStep())
        {
            Destroy(drawTimer);
            var lModelsTransform = flatModelPainter.models.transform;
            lModelsTransform.position = modelPosition.position;
            var lFitScale = getFitScale(flatModelPainter.modelsSize, modelMaxSize);
            var lScale =  new Vector3(lFitScale.x, lFitScale.y,1.0f);
            lModelsTransform.localScale = lScale;
            GameObject[] lModelObjectList = new GameObject[lModelsTransform.childCount];
            nowPainterOutData.paintingModelDatas = new PaintingModelData[lModelsTransform.childCount];
            nowPainterOutData.transforms = new zzTransform[lModelsTransform.childCount];
            int i = 0;
            foreach (Transform lSub in lModelsTransform)
            {
                var lGameObject = lSub.gameObject;
                lModelObjectList[i] = lGameObject;
                lGameObject.AddComponent<Rigidbody>();
                lGameObject.AddComponent<zzObjectElement>();
                lGameObject.AddComponent<zzEditableObject>();
                var lPaintingMesh = lGameObject.AddComponent<PaintingMesh>();
                var lRender = lSub.Find("Render").GetComponent<MeshRenderer>();

                lPaintingMesh.paintRenderer = lRender;
                //lPaintingMesh.pictureSize = flatModelPainter.modelsSize;
                lPaintingMesh.modelData = PaintingModelData
                    .createData(lGameObject, flatModelPainter.modelsSize);

                //useDefaultMaterial(lPaintingMesh);
                useImageMaterial(lPaintingMesh);

                nowPainterOutData.paintingModelDatas[i] = lPaintingMesh.modelData;
                nowPainterOutData.transforms[i] = new zzTransform(lSub);


                //var lWorldScale = lRender.transform.lossyScale;
                //var lExtraTextureScale = flatModelPainter.modelsSize;
                //lExtraTextureScale.Scale(lFitScale);
                //lPaintingMesh.extraTextureScale = lExtraTextureScale;

                //lSub.GetComponentInChildren<zzFlatMeshEdit>().resetUV();
                ++i;
            }
            foreach (var lObject in lModelObjectList)
            {
                lObject.transform.parent = null;
            }
            addModelObjects(lModelObjectList);
            Destroy(flatModelPainter.models);
            Destroy(drawTimer);
        }
    }

    public void useDefaultMaterial(PaintingMesh pPaintingMesh)
    {
        var lMaterial = new Material(modelMaterial);
        pPaintingMesh.sharedMaterial = modelMaterial;
        pPaintingMesh.material = lMaterial;

        var lWorldScale = pPaintingMesh.paintRenderer.transform.lossyScale;
        var lExtraTextureScale = pPaintingMesh.modelData.pictureSize;
        lExtraTextureScale.Scale(lWorldScale);
        pPaintingMesh.extraTextureScale = lExtraTextureScale;
    }

    public void useImageMaterial(PaintingMesh pPaintingMesh)
    {
        pPaintingMesh.useImageMaterial(nowPainterOutData.modelImage);

    }

    public bool inPlaying = false;

    public void    OnPlayButtonClick(object sender)
    {
        //Time.timeScale = 1.0f;
        updateState(true);
    }

    void updateObjectState(GameObject[] pObjectList)
    {
        foreach (var lObject in pObjectList)
        {
            updateObjectState(lObject);
        }
    }

    void updateObjectState(GameObject pObject)
    {
        bool lIsKinematic = !inPlaying;
        var lRigidbody = pObject.GetComponent<Rigidbody>();
        lRigidbody.isKinematic = lIsKinematic;
        if (inPlaying)
            lRigidbody.WakeUp();

    }

    void updateState(bool pIsPlay)
    {
        if (inPlaying == pIsPlay)
            return;
        inPlaying = pIsPlay;
        foreach (Transform lSub in sceneManager)
        {
            updateObjectState(lSub.gameObject);
        }

    }

    public void     OnPauseButtonClick(object sender)
    {
        updateState(false);
    }

    public void    OnClearButtonClick(object sender)
    {
        clearModelObjects();
    }


    class PainterOutData
    {
        public Texture2D modelImage;
        public PaintingModelData[] paintingModelDatas;
        public zzTransform[] transforms;

        public bool haveModelData
        {
            get{ return paintingModelDatas!=null ;}
        }
    }

    Dictionary<string, PainterOutData> imgPathToData = new Dictionary<string,PainterOutData>();
    PainterOutData nowPainterOutData;

    void OnReadImage(zzInterfaceGUI pGUI)
    {
        //Texture2D lImage;
        //if (imgPathToData.ContainsValue)
        //{
        //}
        //lastLocation = fileBrowserDialog.selectedLocation;
        //zzInterfaceGUI lImageUI = UIMap.getObject("imageUI").GetComponent<zzInterfaceGUI>();
        //lImage = new Texture2D(4, 4, TextureFormat.ARGB32,false);
        
        //using( var lImageFile = new FileStream(fileBrowserDialog.selectedLocation, FileMode.Open) )
        //{
        //    BinaryReader lBinaryReader = new BinaryReader(lImageFile);
        //    lImage.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));

        //}
        //sceneImageUI.image = lImage;
        //modelImage = lImage;
        lastLocation = fileBrowserDialog.selectedLocation;
        readImage(lastLocation);

    }

    void readImage(string pPath)
    {
        FileInfo lFileInfo = new FileInfo(pPath);
        if (imgPathToData.ContainsKey(lFileInfo.ToString()))
        {
            nowPainterOutData = imgPathToData[lFileInfo.ToString()];
        }
        else
        {
            Texture2D lImage = new Texture2D(4, 4, TextureFormat.ARGB32, false);

            using (var lImageFile = new FileStream(fileBrowserDialog.selectedLocation, FileMode.Open))
            {
                BinaryReader lBinaryReader = new BinaryReader(lImageFile);
                lImage.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));

            }
            nowPainterOutData = new PainterOutData();
            nowPainterOutData.modelImage = lImage;
            imgPathToData[lFileInfo.ToString()] = nowPainterOutData;

        }
        sceneImageUI.image = nowPainterOutData.modelImage;

    }

    public string saveName = "temp";

    [ContextMenu("SaveModel")]
    void saveModel()
    {
        saveModel(GameConfig.Singleton.ModelDir + "/" + saveName);
    }

    public void OnTest(object sender)
    {
        readModel();
    }

    [ContextMenu("ReadModel")]
    void readModel()
    {
        readGroup(GameConfig.Singleton.ModelDir + "/" + saveName);
    }

    static string groupFileName = "ModelGroup.txt";

    [ContextMenu("Model IO Test")]
    void modelIOTest()
    {
        saveModel();

    }

    void saveModel(string pName)
    {
        if (!Directory.Exists(pName))
            Directory.CreateDirectory(pName);

        //确定资源共用关系,定义唯一值
        var lTextures = new Dictionary<Texture2D, string>();
        var lPaintingModelData = new Dictionary<PaintingModelData, string>();
        foreach (Transform lModelObjectTransform in sceneManager)
        {
            GameObject lModelObject = lModelObjectTransform.gameObject;
            var lPaintingMesh =  lModelObject.GetComponent<PaintingMesh>();
            var lTexture2D = lPaintingMesh.material.mainTexture as Texture2D;
            if (!lTextures.ContainsKey(lTexture2D))
                lTextures[lTexture2D] = System.Guid.NewGuid().ToString();

            if (!lPaintingModelData.ContainsKey(lPaintingMesh.modelData))
                lPaintingModelData[lPaintingMesh.modelData]
                    = System.Guid.NewGuid().ToString();
        }

        //保存图片
        foreach (var lImgSave in lTextures)
        {
            using (var lImageFile = new FileStream(pName + "/" + lImgSave.Value + ".png",
                FileMode.Create))
            {
                BinaryWriter lWriter = new BinaryWriter(lImageFile);
                lWriter.Write(lImgSave.Key.EncodeToPNG());
            }
        }

        //保存模型
        foreach (var lModelDataSave in lPaintingModelData)
        {
            using (var lImageFile = new FileStream(pName + "/" + lModelDataSave.Value + ".pmb",
                FileMode.Create))
            {
                BinaryWriter lWriter = new BinaryWriter(lImageFile);
                lWriter.Write(lModelDataSave.Key.serializeToString());
            }
        }

        //保存场景
        {
            Hashtable lGroupData = new Hashtable();
            ArrayList lObjectList = new ArrayList(sceneManager.objectCount);
            foreach (Transform lTransform in sceneManager)
            {
                GameObject lModelObject = lTransform.gameObject;
                var lPaintingMesh = lModelObject.GetComponent<PaintingMesh>();
                Hashtable lObjectData = new Hashtable();
                lObjectData["modelData"] = lPaintingModelData[lPaintingMesh.modelData];
                lObjectData["customImage"]
                    = lTextures[lPaintingMesh.material.mainTexture as Texture2D];
                lObjectData["position"] = lTransform.position;
                lObjectData["rotation"] = lTransform.rotation;
                lObjectData["scale"] = lTransform.localScale;
                lObjectList.Add(lObjectData);
            }
            lGroupData["objectList"] = lObjectList;
            using (var lGroupFile = new FileStream(pName + "/" + groupFileName,
                FileMode.Create))
            {
                BinaryWriter lWriter = new BinaryWriter(lGroupFile);
                lWriter.Write(zzSerializeString.Singleton.pack(lGroupData));
            }

        }

    }

    GameResourceManager0 resourceManager = new GameResourceManager0();

    void    readGroup(string pPath)
    {
        resourceManager.scanDirectory(pPath);
        string pStringData;
        using (var lGroupFile = new FileStream(pPath + "/" + groupFileName,
            FileMode.Open))
        {
            BinaryReader lReader = new BinaryReader(lGroupFile);
            pStringData = lReader.ReadString();
        }
        readGroupFromTable((Hashtable)zzSerializeString
            .Singleton.unpackToData(pStringData));
    }

    void    readGroupFromTable(Hashtable pData )
    {
        ArrayList lObjectList = (ArrayList)pData["objectList"];
        List<GameObject> lModelList = new List<GameObject>();

        int i = 0;
        foreach (Hashtable lObjectData in lObjectList)
        {
            GameObject lGameObject = new GameObject("readMoedl"+i);
            Transform lTransform = lGameObject.transform;
            string lModelDataName = (string)lObjectData["modelData"];
            string lCustomImageName = (string)lObjectData["customImage"];
            var lPaintingModelData = resourceManager.getModelFromGuid(lModelDataName);
            PaintingMesh lPaintingMesh = PaintingMesh.create(lGameObject, lPaintingModelData);
            lPaintingMesh.useImageMaterial(resourceManager.getImageFromGuid(lCustomImageName));

            //先设置Transform,在增加凸碰撞模型时,有时会长生错误
            lTransform.position = (Vector3)lObjectData["position"];
            lTransform.rotation = (Quaternion)lObjectData["rotation"];
            lTransform.localScale = (Vector3)lObjectData["scale"];

            lModelList.Add(lGameObject);
            ++i;
            //addModelObjects()
            //lGameObject.GetComponent<Rigidbody>().isKinematic = !inPlaying;

        }

        addModelObjects(lModelList.ToArray());
    }

    void createObject(PainterOutData pPainterOutData)
    {
        List<GameObject> lModelList = new List<GameObject>();

        for (int i=0;i<pPainterOutData.paintingModelDatas.Length;++i)
        {
            GameObject lGameObject = new GameObject();
            lModelList.Add(lGameObject);
            var lPaintingMesh
                = PaintingMesh.create(lGameObject,pPainterOutData.paintingModelDatas[i]);
            lPaintingMesh.useImageMaterial(pPainterOutData.modelImage);
            pPainterOutData.transforms[i].setToTransform(lGameObject.transform);
        }
        addModelObjects(lModelList.ToArray());

    }

    // Update is called once per frame
    //void Update () {

    //}
}
