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

//----------------------------------------------------------

public class SceneModelReader:SceneReader<PaintingModelData>
{
    public SceneModelReader()
    {
        fileExtensionFilter = "*.pmb";
    }

    public override PaintingModelData readData(string pFullName)
    {
        PaintingModelData lOut;
        using (var lFile = new FileStream(pFullName, FileMode.Open))
        {
            StreamReader lStreamReader = new StreamReader(lFile);
            lOut = PaintingModelData.createDataFromString(lStreamReader.ReadToEnd());
        }
        return lOut;
    }
}

public class SceneModelWriter:SceneWriter<PaintingModelData>
{
    public SceneModelWriter()
    {
        fileExtension = "pmb";
    }

    public override void writeData(PaintingModelData pData, string pFullName)
    {
        using (var lFile = new FileStream(pFullName, FileMode.Create))
        {
            StreamWriter lWriter = new StreamWriter(lFile);
            lWriter.AutoFlush = true;
            lWriter.Write(pData.serializeToString());
        }
    }
}
//---------------------------------------------------------------



public class SceneImageReader : SceneReader<Texture2D>
{
    public SceneImageReader()
    {
        fileExtensionFilter = "*.png";
    }

    public override Texture2D readData(string pFullName)
    {
        Texture2D lOut = new Texture2D(4, 4, TextureFormat.ARGB32, false); ;
        using (var lFile = new FileStream(pFullName, FileMode.Open))
        {
            BinaryReader lBinaryReader = new BinaryReader(lFile);
            lOut.LoadImage(lBinaryReader.ReadBytes((int)lFile.Length));
        }
        return lOut;
    }
}

public class SceneImageWriter : SceneWriter<Texture2D>
{
    public SceneImageWriter()
    {
        fileExtension = "png";
    }

    public override void writeData(Texture2D pData, string pFullName)
    {
        using (var lFile = new FileStream(pFullName, FileMode.Create))
        {
            BinaryWriter lWriter = new BinaryWriter(lFile);
            lWriter.Write(pData.EncodeToPNG());
        }
    }
}


public abstract class SceneReader<T>
{
    string rootDictionary;

    Dictionary<string, string> nameToPath = new Dictionary<string, string>();
    Dictionary<string, GenericResource<T>> nameToData = new Dictionary<string, GenericResource<T>>();

    protected string fileExtensionFilter = "*.pmb";

    void scanDirectory(string pDirectory)
    {
        DirectoryInfo lDirectory = new DirectoryInfo(pDirectory);
        foreach (var lFile in lDirectory.GetFiles(fileExtensionFilter))
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

    public GenericResource<T> getData(string pID)
    {
        GenericResource<T> lOut = null;
        if(!nameToData.ContainsKey(pID))
        {
            lOut = new GenericResource<T>(readData(nameToPath[pID]), pID);
            nameToData[pID] = lOut;
        }
        else
            lOut = nameToData[pID];
        return lOut;
    }

    //GenericResource<Texture2D> getImage(string pID)
    //{

    //}
    public abstract T readData(string pFullName);

    public void endReadScene()
    {
    }

}
public abstract class SceneWriter<T>
{
    string rootDictionary;
    protected string fileExtension = "pmb";

    HashSet<string> savedModelID = new HashSet<string>();

    public void beginSaveScene(string rootDirName)
    {
        rootDictionary = rootDirName;
    }

    public void saveData(GenericResource<T> pResourceData)
    {
        string lDataID = pResourceData.resourceID;
        if (!savedModelID.Contains(lDataID))
        {
            writeData(pResourceData.resource, rootDictionary + "/" + lDataID + "." + fileExtension);
            savedModelID.Add(lDataID);
        }
    }
    public abstract void writeData(T pData, string pFullName);

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

    SceneModelReader sceneModelReader = new SceneModelReader();
    SceneModelWriter sceneModelWriter = new SceneModelWriter();

    SceneImageReader sceneImageReader = new SceneImageReader();
    SceneImageWriter sceneImageWriter = new SceneImageWriter();

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
        return sceneModelReader.getData(pID);
    }

    public GenericResource<Texture2D> getImage(string pID)
    {
        return sceneImageReader.getData(pID);
    }

    public string sceneFileName = "main.zzScene";

    public string fullPath;

    [ContextMenu("save")]
    public void save()
    {
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        sceneModelWriter.beginSaveScene(fullPath);
        sceneImageWriter.beginSaveScene(fullPath);
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
                sceneModelWriter.saveData(lPaintingMesh.modelResource);

            var lRenderMaterial = lObject.GetComponent<RenderMaterialProperty>();
            if (lRenderMaterial && lRenderMaterial.resourceType == ResourceType.realTime)
                sceneImageWriter.saveData(lRenderMaterial.imageResource);
        }
        sceneModelWriter.endSaveScene();
        sceneImageWriter.endSaveScene();
    }

    [ContextMenu("load")]
    public void load()
    {
        if (File.Exists(fullPath))
            fullPath = (new FileInfo(fullPath)).DirectoryName;

        sceneModelReader.beginReadScene(fullPath);
        sceneImageReader.beginReadScene(fullPath);
        string lSceneSave;
        using (var lSceneFile = new FileStream(fullPath + "/" + sceneFileName,
            FileMode.Open))
        {
            StreamReader lStreamReader = new StreamReader(lSceneFile);
            lSceneSave = lStreamReader.ReadToEnd();
        }
        serializeScene.serializeFrom(zzSerializeString.Singleton.unpackToData(lSceneSave));
        sceneModelReader.endReadScene();
        sceneImageReader.endReadScene();
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