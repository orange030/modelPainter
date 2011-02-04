using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlatModelCreator : MonoBehaviour
{
    public Texture2D picture;
    //Texture2D prePicture;
    public float ignoreDistanceInSweeping = 1.7f;
    public float thickness = 10.0f;

    List<zz2DConcave> concaves;
    List<Dictionary<Vector2, int>> pointToIndexList;
    List<zzSimplyPolygon[]> convexesList;

    enum Step
    {
        nothing = 1,
        //showPocture,
        pickPicture,
        sweepPicture,
        convexDecompose,
        draw,
        //clear,
    }

    public enum SweepMode
    {
        ignoreColor,
        designatedColor,
        ignoreAlphaZero,
    }

    [SerializeField]
    Step step = Step.nothing;

    public int pointNumber = 0;

    public SweepMode sweepMode;

    public Color colorInSweepSetting;

    Renderer getRenderer(Transform pTransform)
    {
        Renderer lOut = null;
        //pTransform = pTransform.parent;
        while (!lOut && pTransform)
        {
            lOut = pTransform.GetComponent<Renderer>();
            pTransform = pTransform.parent;
        }
        return lOut;
    }

    void pickPicture()
    {
        activeChart = new zzOutlineSweeper.ActiveChart(picture.width, picture.height);
        if (sweepMode == SweepMode.ignoreColor)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y) != colorInSweepSetting);
                }
            }

        }
        else if (sweepMode == SweepMode.designatedColor)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y) == colorInSweepSetting);
                }
            }

        }
        else //if (sweepMode == SweepMode.ignoreAlphaZero)
        {
            for (int x = 0; x < picture.width; ++x)
            {
                for (int y = 0; y < picture.height; ++y)
                {
                    activeChart.setActive(x, y, picture.GetPixel(x, y).a != 0);
                }
            }

        }
        zzOutlineSweeper.removeIsolatedPoint(activeChart);

    }

    zzOutlineSweeper.ActiveChart activeChart;

    public int polygonNumber = 0;
    public int holeNumber = 0;

    void sweepPicture()
    {
        pointNumber = 0;
        polygonNumber = 0;
        holeNumber = 0;
        var lSweeperResults = zzOutlineSweeper.sweeper(activeChart, ignoreDistanceInSweeping);
        modelsSize = new Vector2((float)activeChart.width, (float)activeChart.height);

        concaves = new List<zz2DConcave>();
        foreach (var lSweeperResult in lSweeperResults)
        {

            if (lSweeperResult.edge.Length < 2)
                continue;

            zzSimplyPolygon lPolygon = new zzSimplyPolygon();
            lPolygon.setShape(lSweeperResult.edge);

            zz2DConcave lConcave = new zz2DConcave();
            lConcave.setShape(lPolygon);
            ++polygonNumber;

            foreach (var lHole in lSweeperResult.holes)
            {
                if (lHole.Length < 2)
                    continue;
                zzSimplyPolygon lHolePolygon = new zzSimplyPolygon();
                lHolePolygon.setShape(lHole);
                lConcave.addHole(lHolePolygon);
                ++holeNumber;
            }

            concaves.Add(lConcave);
        }
    }

    zzSimplyPolygon addSimplyPolygon(Vector2[] pPoints, string pName, Transform pDebugerObject)
    {
        return addSimplyPolygon(pPoints, pName, pDebugerObject, Color.red);
    }

    zzSimplyPolygon addSimplyPolygon(Vector2[] pPoints, string pName, Transform pDebugerObject, Color lDebugLineColor)
    {
        if (pPoints.Length < 3)
            return null;

        zzSimplyPolygon lPolygon = new zzSimplyPolygon();
        lPolygon.setShape(pPoints);

        zzSimplyPolygonDebuger
            .createDebuger(lPolygon, pName, pDebugerObject, lDebugLineColor);

        pointNumber += lPolygon.pointNum;
        return lPolygon;
    }

    void convexDecompose()
    {
        convexesList = new List<zzSimplyPolygon[]>();
        int index = 0;
        foreach (var lConcave in concaves)
        {
            zzSimplyPolygon[] ldecomposed = lConcave.decompose();
            convexesList.Add(ldecomposed);
        }
    }

    void draw()
    {
        models = new GameObject("PaintModel");
        models.transform.position = new Vector3(modelsSize.x / 2.0f, modelsSize.y / 2.0f, 0.0f);
        int i = 0;
        foreach (var lConvexs in convexesList)
        {
            var lSurfaceList = new List<Vector2[]>(lConvexs.Length);
            foreach (var lConvex in lConvexs)
            {
                lSurfaceList.Add(lConvex.getShape());
            }

            string lPolygonName = "polygon" + i;
            GameObject lConvexsObject = new GameObject(lPolygonName);
            lConvexsObject.transform.parent = models.transform;

            var lRenderObject = createFlatMesh(concaves[i], lSurfaceList, "Render",
                    lConvexsObject.transform, thickness,
                    new Vector2(1.0f / modelsSize.x, 1.0f / modelsSize.y));

            lRenderObject.AddComponent<zzFlatMeshEdit>();
            Vector3 lCenter = lRenderObject.GetComponent<MeshFilter>().mesh.bounds.center;
            lCenter.z = 0;
            //lConvexsObject是lRenderObject的父物体
            lConvexsObject.transform.position += lCenter;
            lRenderObject.transform.position -= lCenter;

            ++i;

            int lSubIndex = 0;
            string lSubName = "Collider";
            foreach (var lConvex in lConvexs)
            {
                var lColliderObject = createFlatCollider(lConvex.getShape(),
                    lSubName + lSubIndex,
                    lConvexsObject.transform, thickness);
                //因为是先创建,后关联父级的,所以不用移动
                //lColliderObject.transform.position -= lCenter;
                ++lSubIndex;
            }
        }

    }

    public GameObject models;
    public Vector2 modelsSize;


    [ContextMenu("Step")]
    public bool doStep()
    {
        int lStepValue = (int)step;
        if (lStepValue < (int)Step.draw)
            step = (Step)(lStepValue + 1);
        switch (step)
        {
            case Step.pickPicture:
                pickPicture();
                break;
            case Step.sweepPicture:
                sweepPicture();
                break;
            case Step.convexDecompose:
                convexDecompose();
                break;
            case Step.draw:
                draw();
                return false;
        }
        return true;
    }

    [ContextMenu("clear")]
    public void clear()
    {
        step = Step.nothing;
        Destroy(models);
    }

    static GameObject createFlatCollider(Vector2[] points, string pName, Transform parent, float zThickness)
    {
        GameObject lOut = new GameObject(pName);
        //lOut.active = false;
        lOut.transform.parent = parent;
        MeshCollider lMeshCollider = lOut.AddComponent<MeshCollider>();
        Mesh lMesh = new Mesh();
        zzFlatMeshUtility.draw(lMesh, points, zThickness);
        //lMeshCollider.convex = true;

        lMeshCollider.sharedMesh = lMesh;

        return lOut;
    }

    static GameObject createFlatMesh(zz2DConcave pConcave, List<Vector2[]> pSurfaceList,
        string pName, Transform parent, float zThickness, Vector2 pUvScale)
    {
        GameObject lOut = new GameObject(pName);
        //lOut.active = false;
        lOut.transform.parent = parent;
        MeshFilter lMeshFilter = lOut.AddComponent<MeshFilter>();
        Mesh lMesh = new Mesh();

        lMeshFilter.mesh = lMesh;

        var lEdgeList = new List<Vector2[]>(pConcave.getHoleNum() + 1);
        lEdgeList.Add(pConcave.getOutSidePolygon().getShape());
        foreach (var lHole in pConcave.getHole())
        {
            lEdgeList.Add(lHole.getShape());
        }
        zzFlatMeshUtility.draw(lMesh, pSurfaceList, lEdgeList, zThickness, pUvScale);
        return lOut;
    }
}