using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

class GameResourceManager
{

    static protected GameResourceManager singletonInstance = new GameResourceManager();

    Dictionary<string, Texture2D> mGuidToTex = new Dictionary<string, Texture2D>();
    Dictionary<string, string> mTexGuidToPath = new Dictionary<string, string>();
    //Dictionary<string, Texture2D> mPathToTex = new Dictionary<string, Texture2D>();

    Dictionary<string, PaintingModelData> mGuidToModel = new Dictionary<string, PaintingModelData>();
    Dictionary<string, string> mModelGuidToPath = new Dictionary<string, string>();
    //Dictionary<string, PaintingModelData> mPathToModel = new Dictionary<string, PaintingModelData>();
    
    public Texture2D    getImageFromGuid(string pGuid)
    {
        Texture2D lOut;
        if (!mGuidToTex.ContainsKey(pGuid))
        {
            lOut = new Texture2D(4, 4, TextureFormat.ARGB32, false);

            using (var lImageFile = new FileStream(mTexGuidToPath[pGuid], FileMode.Open))
            {
                BinaryReader lBinaryReader = new BinaryReader(lImageFile);
                lOut.LoadImage(lBinaryReader.ReadBytes((int)lImageFile.Length));
            }
            mGuidToTex[pGuid] = lOut;
        }
        else
        {
            lOut = mGuidToTex[pGuid];
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
        if (!mGuidToModel.ContainsKey(pGuid))
        {

            using (var lModelFile = new FileStream(mModelGuidToPath[pGuid], FileMode.Open))
            {
                BinaryReader lBinaryReader = new BinaryReader(lModelFile);
                lOut = PaintingModelData.createDataFromString(lBinaryReader.ReadString());
            }
            mGuidToModel[pGuid] = lOut;
        }
        else
        {
            lOut = mGuidToModel[pGuid];
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

    public static GameResourceManager Main
    {
        get { return singletonInstance; }
    }

    //public static GameResourceManager getSingleton()
    //{
    //    return singletonInstance;
    //}

}