using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class GenericResource<T>
{
    public GenericResource()
    {
    }

    public GenericResource(T t)
    {
        resource = t;
        resourceType = ResourceType.realTime;
        resourceID = generateID();
    }

    public GenericResource(T t, string pResourceID)
    {
        resource = t;
        resourceType = ResourceType.realTime;
        resourceID = pResourceID;
    }

    public T resource;
    public ResourceType resourceType;
    public string resourceID;
    public static string generateID()
    {
        return System.Guid.NewGuid().ToString();
    }
}

public class GenericResourceManager<T>
{
    Dictionary<string, GenericResource<T>> IdToResource
        = new Dictionary<string,GenericResource<T>>();

    public GenericResource<T>  getResource(string pResourceId)
    {
        if (IdToResource.ContainsKey(pResourceId))
            return IdToResource[pResourceId];
        else
            return null;
    }

    public void setResource(GenericResource<T> pResource,string pResourceId)
    {
        IdToResource[pResourceId] = pResource;
    }

    public GenericResource<T> addResource(GenericResource<T> pResource,string pResourceId)
    {
        pResource.resourceID = pResourceId;
        IdToResource[pResourceId] = pResource;
        return pResource;
    }

    //public GenericResource<T> addResource(GenericResource<T> pResource)
    //{
    //    return addResource(pResource,System.Guid.NewGuid().ToString());
    //}
}

public enum ResourceType
{
    unknown = 0,

    //内建
    builtin,

    ////工程
    //project,

    ////外部文件
    //external,

    //实时生成
    realTime,
}

//public class ModelResourceManager
//{
//    GenericResourceManager<PaintingModelData> projectResourceManager;
//    GenericResourceManager<PaintingModelData> realTimeResourceManager;

//    public GenericResource<PaintingModelData> addResource(ResourceType pType,
//        PaintingModelData pData)
//    {
//        return addResource(pType, pData, pResource, System.Guid.NewGuid().ToString());
//    }

//    public GenericResourceManager<PaintingModelData> getManager(ResourceType pType)
//    {
//        GenericResourceManager<PaintingModelData> lManager = null;
//        switch (pType)
//        {
//            case ResourceType.project:
//                lManager = projectResourceManager;
//                break;
//            case ResourceType.realTime:
//                lManager = realTimeResourceManager;
//                break;
//            default:
//                Debug.LogError("addResource:" + pType);
//        }
//        return lManager;
//    }

//    public GenericResource<PaintingModelData> addResource(ResourceType pType,
//        PaintingModelData pData, string pResourceId)
//    {
//        var lManager = getManager(pType);
//        var lOut = new GenericResource<PaintingModelData>();
//        lOut.resource = pData;
//        lOut.resourceType = pType;
//        return lManager.addResource(lOut, pResourceId);
//    }

//    public GenericResource<PaintingModelData> getResource(ResourceType pType,
//        string pResourceId)
//    {
//        var lManager = getManager(pType);
//        var lResource = lManager.getResource(pResourceId);
//        if (lResource != null)
//            return lResource;

//        if (pType == ResourceType.project)
//        {
//            var lData = GameResourceManager.Main.getModelFromGuid(pResourceId);
//            return addResource(ResourceType.project, lData, pResourceId);
//        }

//        Debug.LogError("[getResource] Type:" + pType + ", ID: " + pResourceId);
//        return null;
//    }
//}

//public class GameResourceManagerIO
//{
//    Dictionary<string, string> mTexGuidToPath = new Dictionary<string, string>();

//    Dictionary<string, string> mModelGuidToPath = new Dictionary<string, string>();

//    public Texture2D readImage(string pGuid)
//    {
//        lOut = new Texture2D(4, 4, TextureFormat.ARGB32, false);

//        using (var lImageFile = new FileStream(mTexGuidToPath[pGuid], FileMode.Open))
//        {
//            BinaryReader lBinaryReader = new BinaryReader(lImageFile);
//            lOut.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));
//        }
//        return lOut;

//    }

//    public PaintingModelData readModel(string pGuid)
//    {
//        PaintingModelData lOut;
//        using (var lModelFile = new FileStream(mModelGuidToPath[pGuid], FileMode.Open))
//        {
//            StreamReader lStreamReader = new StreamReader(lModelFile);
//            lOut = PaintingModelData.createDataFromString(lStreamReader.ReadToEnd());
//        }
//        return lOut;
//    }

//    public static string getFileGuid(FileInfo pFile)
//    {
//        return Path.GetFileNameWithoutExtension(pFile.FullName);
//    }

//    public void saveModel(PaintingModelData pData,string pName)
//    {
//        using (var lImageFile = new FileStream(rootDirectory + "/" + pName,
//            FileMode.Create))
//        {
//            StreamWriter lWriter = new StreamWriter(lImageFile);
//            lWriter.Write(lModelDataSave.Key.serializeToString());
//        }

//    }

//    private string _rootDirectory;

//    public string rootDirectory
//    {
//        get { return _rootDirectory; }
//        set
//        { 
//            _rootDirectory = value;
//            scanDirectory(_rootDirectory);
//        }
//    }

//    void scanDirectory(string pDirectory)
//    {
//        DirectoryInfo lDirectory = new DirectoryInfo(pDirectory);
//        foreach (var lTexFile in lDirectory.GetFiles("*.png"))
//        {
//            mTexGuidToPath[getFileGuid(lTexFile)] = lTexFile.FullName;
//        }
//        foreach (var lModelFile in lDirectory.GetFiles("*.pmb"))
//        {
//            mModelGuidToPath[getFileGuid(lModelFile)] = lModelFile.FullName;
//        }
//    }

//}

public class SceneReader
{
    string rootDictionary;

    Dictionary<string, string> nameToPath = new Dictionary<string, string>();
    Dictionary<string, GenericResource<PaintingModelData>> nameToData = new Dictionary<string, GenericResource<PaintingModelData>>();

    void scanDirectory(string pDirectory)
    {
        DirectoryInfo lDirectory = new DirectoryInfo(pDirectory);
        foreach (var lFile in lDirectory.GetFiles("*.pmb"))
        {
            nameToPath[getFileName(lFile)] = lFile.FullName;
        }
    }

    public static string getFileName(FileInfo pFile)
    {
        return Path.GetFileNameWithoutExtension(pFile.FullName);
    }

    public void beginReadScene( string rootDirName)
    {
        rootDictionary = rootDirName;
        scanDirectory(rootDirName);
    }

    public GenericResource<PaintingModelData> getModel(string pID)
    {
        GenericResource<PaintingModelData> lOut = null;
        if(!nameToData.ContainsKey(pID))
        {
            lOut = new GenericResource<PaintingModelData>(_getModel(nameToPath[pID]), pID);
            nameToData[pID] = lOut;
        }
        else
            lOut = nameToData[pID];
        return lOut;
    }

    //GenericResource<Texture2D> getImage(string pID)
    //{

    //}

    static PaintingModelData _getModel(string pFullName)
    {
        PaintingModelData lOut;
        using (var lFile = new FileStream(pFullName, FileMode.Open))
        {
            StreamReader lStreamReader = new StreamReader(lFile);
            lOut = PaintingModelData.createDataFromString(lStreamReader.ReadToEnd());
        }
        return lOut;
    }

    public void endReadScene()
    {
    }

}
public class SceneWriter
{
    string rootDictionary;

    HashSet<string> savedModelID = new HashSet<string>();

    public void beginSaveScene(string rootDirName)
    {
        rootDictionary = rootDirName;
    }

    public void saveModel(GenericResource<PaintingModelData> pResourceData)
    {
        string lDataID = pResourceData.resourceID;
        if (!savedModelID.Contains(lDataID))
        {
            _saveModel(pResourceData.resource, rootDictionary + "/" + lDataID + ".pmb");
            savedModelID.Add(lDataID);
        }
    }

    static void _saveModel(PaintingModelData pData,string pFullName)
    {
        using (var lFile =new FileStream(pFullName, FileMode.Create))
        {
            StreamWriter lWriter = new StreamWriter(lFile); 
            lWriter.AutoFlush = true;
            lWriter.Write(pData.serializeToString());
        }
    }

    //public void saveImage(GenericResource<Texture2D> pResourceData)
    //{

    //}

    public void endSaveScene()
    {
        savedModelID.Clear();
    }

}

public class GameResourceManager:MonoBehaviour
{

    void Awake()
    {
        singletonInstance = this;
        path = path;
    }

    static protected GameResourceManager singletonInstance;

    public static GameResourceManager Main
    {
        get { return singletonInstance; }
    }

    SceneReader sceneReader = new SceneReader();
    SceneWriter sceneWriter = new SceneWriter();

    public SerializeScene serializeScene;

    [SerializeField]
    public string _path;

    public string path
    {
        get { return _path; }
        set 
        {
            _path = value;
            fullPath = Application.dataPath + "/../" + _path;
        }
    }

    public GenericResource<PaintingModelData> getModel(string pID)
    {
        return sceneReader.getModel(pID);
    }

    public string sceneFileName = "main.zzScene";

    string fullPath;

    [ContextMenu("save")]
    public void save()
    {
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        sceneWriter.beginSaveScene(fullPath);
        var lSceneSave = zzSerializeString.Singleton.pack(serializeScene.serializeTo());
        using (var lSceneFile = new FileStream(fullPath + "/" + sceneFileName,
            FileMode.Create))
        {
            print(lSceneSave);
            StreamWriter lStreamWriter = new StreamWriter(lSceneFile);
            lStreamWriter.AutoFlush = true;
            lStreamWriter.Write(lSceneSave);
        }
        foreach (Transform lObject in serializeScene.enumerateObject)
        {
            var lPaintingMesh = lObject.GetComponent<PaintingMesh>();
            if (lPaintingMesh)
                sceneWriter.saveModel(lPaintingMesh.modelResource);
        }
        sceneWriter.endSaveScene();
    }

    [ContextMenu("load")]
    public void load()
    {
        sceneReader.beginReadScene(fullPath);
        string lSceneSave;
        using (var lSceneFile = new FileStream(fullPath + "/" + sceneFileName,
            FileMode.Open))
        {
            StreamReader lStreamReader = new StreamReader(lSceneFile);
            lSceneSave = lStreamReader.ReadToEnd();
        }
        serializeScene.serializeFrom(zzSerializeString.Singleton.unpackToData(lSceneSave));
        sceneWriter.endSaveScene();
    }
}

public class GameResourceManager0
{

    static protected GameResourceManager0 singletonInstance = new GameResourceManager0();

    Dictionary<string, Texture2D> mGuidToTex = new Dictionary<string, Texture2D>();
    Dictionary<string, string> mTexGuidToPath = new Dictionary<string, string>();
    //Dictionary<string, Texture2D> mPathToTex = new Dictionary<string, Texture2D>();

    Dictionary<string, PaintingModelData> mGuidToModel = new Dictionary<string, PaintingModelData>();
    Dictionary<string, string> mModelGuidToPath = new Dictionary<string, string>();
    //Dictionary<string, PaintingModelData> mPathToModel = new Dictionary<string, PaintingModelData>();

    public Texture2D getImageFromGuid(string pGuid)
    {
        var lOut = new Texture2D(4, 4, TextureFormat.ARGB32, false);

        using (var lImageFile = new FileStream(mTexGuidToPath[pGuid], FileMode.Open))
        {
            BinaryReader lBinaryReader = new BinaryReader(lImageFile);
            lOut.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));
        }
        return lOut;

    }

    //public Texture2D    getImageFromPath(string pPath)
    //{
    //    Texture2D lOut;
    //    FileInfo lFileInfo = new FileInfo(pPath);
    //    string lGUID = Path.GetFileNameWithoutExtension(lFileInfo.FullName);
    //    if(!mGuidToTex.ContainsKey(lGUID))
    //    {
    //        lOut = new Texture2D(4, 4, TextureFormat.ARGB32, false);

    //        using (var lImageFile = new FileStream(lFileInfo.ToString(), FileMode.Open))
    //        {
    //            BinaryReader lBinaryReader = new BinaryReader(lImageFile);
    //            lOut.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));
    //        }
    //        mGuidToTex[lGUID] = lOut;
    //    }
    //    else
    //    {
    //        lOut = mGuidToTex[lGUID];
    //    }
    //    return lOut;
    //}
    public PaintingModelData getModelFromGuid(string pGuid)
    {
        PaintingModelData lOut;
        using (var lModelFile = new FileStream(mModelGuidToPath[pGuid], FileMode.Open))
        {
            StreamReader lStreamReader = new StreamReader(lModelFile);
            lOut = PaintingModelData.createDataFromString(lStreamReader.ReadToEnd());
        }
        return lOut;
    }

    public static string getFileGuid(FileInfo pFile)
    {
        return Path.GetFileNameWithoutExtension(pFile.FullName);
    }

    public void scanDirectory(string pDirectory)
    {
        DirectoryInfo lDirectory = new DirectoryInfo(pDirectory);
        foreach (var lTexFile in lDirectory.GetFiles("*.png"))
        {
            mTexGuidToPath[getFileGuid(lTexFile)] = lTexFile.FullName;
        }
        foreach (var lModelFile in lDirectory.GetFiles("*.pmb"))
        {
            mModelGuidToPath[getFileGuid(lModelFile)] = lModelFile.FullName;
        }
    }

    //public PaintingModelData getModelFromPath(string pPath)
    //{
    //    PaintingModelData lOut;
    //    FileInfo lFileInfo = new FileInfo(pPath);
    //    string lGUID = Path.GetFileNameWithoutExtension(lFileInfo.FullName);
    //    if (!mGuidToModel.ContainsKey(lGUID))
    //    {

    //        using (var lModelFile = new FileStream(lFileInfo.ToString(), FileMode.Open))
    //        {
    //            BinaryReader lBinaryReader = new BinaryReader(lModelFile);
    //            lOut = PaintingModelData.createDataFromString(lBinaryReader.ReadString());
    //        }
    //        mGuidToModel[lGUID] = lOut;
    //    }
    //    else
    //    {
    //        lOut = mGuidToModel[lGUID];
    //    }
    //    return lOut;
    //}

    public static GameResourceManager0 Main
    {
        get { return singletonInstance; }
    }

    //public static GameResourceManager getSingleton()
    //{
    //    return singletonInstance;
    //}

}