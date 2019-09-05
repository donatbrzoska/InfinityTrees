using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System;

// watch out for NOT PRETTY 

public class Core : MonoBehaviour, GrowerListener {
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (true) {
            UnityEngine.Debug.Log("DEBUG: TreeCreator: " + callerName + "(): " + message);
        }
    }

    GrowthProperties growthProperties;
    SpaceColonization grower;

    GeometryProperties geometryProperties;
    Tree tree;

    // Start is called before the first frame update
    void Start() {
        GameObject treeRenderer = new GameObject();
        treeRenderer.AddComponent<MeshRenderer>();
        treeRenderer.AddComponent<MeshFilter>();
        treeRenderer.AddComponent<TreeRenderer>();
        renderers.Add(treeRenderer);

        //vl, tl || v, t
        //7728, 7210 || 3856, 5274
        LoadDefaultGrowth();
        LoadDefaultGeometry();

        //3844, 5112 || 14772, 10576 || 5988, 6184 (low vertices with leaves)
        //LoadPseudoPoplarGrowth();
        //LoadPseudoPoplarGeometry();

        //9152, 8334 || 4352, 5934
        //LoadExcurrentGrowth();
        //LoadExcurrentGeometry();

        //LoadPendulousBranchesGrowth_();
        //LoadPendulousBranchesGeometry_();

        //LoadPendulousBranchesGrowth();
        //LoadPendulousBranchesGeometry();

        //7024, 6514 || 3392, 4698
        //4896, 5450 || 3392, 4698 //0.1 displayed leaves per node
        //LoadBushGrowth();
        //LoadBushGeometry();

        //1764, 1658 || 884, 1218
        //LoadYoungGrowth();
        //LoadYoungGeometry();

        //LoadDecurrentNewSeedGrowth();
        //LoadDecurrentNewSeedGeometry();

        //LoadMinGnarlyBranchesGrowth();
        //LoadMinGnarlyBranchesGeometry();

        //LoadMaxGnarlyBranchesGrowth();
        //LoadMaxGnarlyBranchesGeometry();




        //LoadTestGrowth();
        //LoadTestGrowthGeometry();

        //LoadBigGrowth();
        //LoadBigGrowthGeometry();

        LoadBigGrowth2();
        LoadBigGrowthGeometry();





        //LoadExactLimitedGrowth();
        //LoadExactLimitedGeometry();

        //LoadBitLimitedGrowth();
        //LoadBitLimitedGeometry();

        //LoadExtremeLimitedGrowth();
        //LoadExtremeLimitedGeometry();


        //LoadEarlyBorderGrowth(); //not working as expected
        //LoadNoLeavesGeometry();




        //LoadNotWorkingPendulousGrowth();

        //LoadExtremelyDetailedGeometry();

        //LoadGnarlyGrowth();

        //LoadLowGnarlyGrowth();

        //LoadPendulousGeometry();


        //LoadBaobabGrowth();







        //
        //LoadBush2Growth();
        //LoadBushGeometry();

        //LoadMediumBigGrowth();
        //LoadDefaultGeometry();



        grower.Grow(tree);
    }

    //initialize UI at the beginning
    bool initialized;
    void Update() {
        if (!initialized) {
            InitializeUI();
            initialized = true;
        }
    }

    void LoadDefaultGrowth() {
        growthProperties = new GrowthProperties();
        growthProperties.PerceptionAngle = 160;

        growthProperties.SigmoidMin = -12;
        growthProperties.SigmoidMax = 6;
        growthProperties.BranchDensityBegin = 0f;
        growthProperties.BranchDensityEnd = 0.8f;

        growthProperties.Tropisms = new Vector3(0f, 1f, 0);

        growthProperties.StemLength = 2f;
        growthProperties.StemAngleRange = 5;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.GrowthDistance = 0.25f;
        growthProperties.AttractionPoints = new PseudoEllipsoid(5, 7, 4.5f, 30, 0.15f, 0.05f);
        growthProperties.Iterations = 30;


        //all images where created with this, two values for the point cloud density are not reasoned yet though
        growthProperties.GnarlyBranches_min_dc_min = 0.05f;
        growthProperties.GnarlyBranches_min_dc_max = 0.4125f;
        growthProperties.GnarlyBranches_max_dc_min = 0.1f;
        growthProperties.GnarlyBranches_max_dc_max = 1.25f;
        growthProperties.GnarlyBranches_min_di = 0.5f;
        growthProperties.GnarlyBranches_max_di = 1.3f;
        growthProperties.GnarlyBranches_min_pointCloudDensity = 12;
        growthProperties.GnarlyBranches_max_pointCloudDensity = 30;
        growthProperties.GnarlyBranchesRatio = 0.8f;



        ////working parameters, min and max values are fine but in between the branch density in the beginning gets pretty big
        ////.. also doesn't solve the "problem", that the density doesnt stay the same when changin the GnarlyBranchesRatio
        //growthProperties.GnarlyBranches_min_dc_min = 0.05f;
        //growthProperties.GnarlyBranches_min_dc_max = 0.42f;
        //growthProperties.GnarlyBranches_max_dc_min = 0.1f;
        //growthProperties.GnarlyBranches_max_dc_max = 1.285f;
        //growthProperties.GnarlyBranches_min_di = 0.5f;
        //growthProperties.GnarlyBranches_max_di = 1.3f;
        //growthProperties.GnarlyBranches_min_pointCloudDensity = 30;
        //growthProperties.GnarlyBranches_max_pointCloudDensity = 30;
        //growthProperties.GnarlyBranchesRatio = 0.8f;


        //growthProperties.GnarlyBranchesRatio = 0.1666f;

        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadDefaultGeometry() {
        geometryProperties = new GeometryProperties();

        geometryProperties.TipRadius=0.007f;
        geometryProperties.NthRoot_min = 1f;
        geometryProperties.NthRoot_max = 3f;
        geometryProperties.StemThickness = 0.55f;

        geometryProperties.CircleResolution=3;
        geometryProperties.MinRadiusRatioForNormalConnection = 0.49f;
        //geometryProperties.SetMinRadiusRatioForNormalConnection(0.1f);

        geometryProperties.PendulousBranchesIntensity = 0;

        geometryProperties.PendulousBranchesBeginDepthMin = 0;
        geometryProperties.PendulousBranchesBeginDepthMax = CalculateBranchOrientationBeginDepthMax(growthProperties.Iterations);
        geometryProperties.PendulousBranchesBeginDepthRatio = 0.75f;

        geometryProperties.MaxTwigRadiusForLeaves = 0.009f;
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 0.7f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.7f);
        geometryProperties.DisplayedLeafesPerNodeMaximum = 3;
        //geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.ParticleCrossFoil);

        geometryProperties.StemColorStrings = new List<string> { "dark_brown", "brown", "light_brown", "grey_brown", "greyish" };
        geometryProperties.CurrentStemColorStringsIndex = 0;

        geometryProperties.LeafColorStrings = new List<string> { "yellow", "orange", "red", "lime_green", "light_green", "green", "dark_green", "light_turquoise", "dark_turquoise", "blue" };
        geometryProperties.CurrentLeafColorStringsIndex = 3;

        geometryProperties.LeafTypeStrings = Leaf.LeafTypeStrings;
        geometryProperties.CurrentLeafTypeStringsIndex = 1;


        tree = new Tree(geometryProperties);
    }

    void LoadNoLeavesGeometry() {
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0);
    }



    void LoadPseudoPoplarGrowth() {
        growthProperties.AttractionPoints = new PseudoEllipsoid(2.1f, 7, 1.9f, 30, 0.15f, 0.05f);
        growthProperties.GnarlyBranchesRatio = 0.5f;
        growthProperties.BranchDensityBegin = 0.5f;
        growthProperties.StemLength = 1;
        growthProperties.Iterations = 40;
    }

    void LoadPseudoPoplarGeometry() {
        geometryProperties.StemThickness = 0.6f;

        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 0.7f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 1.5f);
    }


    void LoadExcurrentGrowth() {
        growthProperties.AttractionPoints = new PseudoEllipsoid(3, 15, 3.2f, 30, 0.6f, 0);
        growthProperties.GnarlyBranchesRatio = 0.5f;

        growthProperties.BranchDensityBegin = 0.4f;
        growthProperties.StemLength = 1f;
        growthProperties.CrownStemLengthRatio = 1f;
        growthProperties.Iterations = 14;

        // for limit demo
        geometryProperties.PendulousBranchesBeginDepthMax = CalculateBranchOrientationBeginDepthMax(growthProperties.Iterations);
    }

    void LoadExcurrentGeometry() {
        geometryProperties.CurrentStemColorStringsIndex = 0;
        geometryProperties.CurrentLeafColorStringsIndex = 6;
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.7f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 0.85f);
    }


    void LoadPendulousBranchesGrowth_() {
        growthProperties.StemLength = 1.8f;
        growthProperties.BranchDensityBegin = 0.2f;
    }

    void LoadPendulousBranchesGeometry_() {
        geometryProperties.StemThickness = 0.45f;
        LoadNoLeavesGeometry();
        geometryProperties.PendulousBranchesIntensity = 0.4f;
        geometryProperties.PendulousBranchesBeginDepthRatio = 0.7f;
        debug("depth: " + geometryProperties.PendulousBranchesBeginDepth);
    }


    void LoadPendulousBranchesGrowth() {
        growthProperties.StemLength = 1.8f;
        growthProperties.BranchDensityBegin=0.2f;
    }

    void LoadPendulousBranchesGeometry() {
        geometryProperties.StemThickness = 0.45f;
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 0.3f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 1.5f);
        geometryProperties.PendulousBranchesIntensity = 0.8f;
        geometryProperties.PendulousBranchesBeginDepthRatio = 0.7f;
    }


    void LoadBushGeometry() {
        //geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 1f);
        geometryProperties.CurrentStemColorStringsIndex = 4;
        geometryProperties.CurrentLeafColorStringsIndex = 8;
    }

    void LoadBushGrowth() {
        growthProperties.AttractionPoints = new PseudoEllipsoid(7, 4, 7f, 30, 0.15f, 0.05f);

        growthProperties.BranchDensityBegin=0.2f;
        growthProperties.StemLength = 0f;
        growthProperties.Iterations = 20;
    }


    void LoadYoungGrowth() {
        growthProperties.StemLength = 1.2f;
        growthProperties.Iterations = 20;
    }

    void LoadYoungGeometry() {
        geometryProperties.CurrentStemColorStringsIndex = 3;
        geometryProperties.CurrentLeafColorStringsIndex = 2;
        geometryProperties.StemThickness = 0.65f;
        //geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 0.5f);
        //geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 1f);
    }


    void LoadDecurrentNewSeedGrowth() {
        growthProperties.BranchDensityBegin = 0.1f;
        growthProperties.GnarlyBranchesRatio = 0.8f;
    }

    void LoadDecurrentNewSeedGeometry() {
        LoadNoLeavesGeometry();
    }


    void LoadMinGnarlyBranchesGrowth() {
        growthProperties.StemLength = 1.2f;
        growthProperties.BranchDensityBegin=0.6f;
        growthProperties.BranchDensityEnd=1f;
        growthProperties.GnarlyBranchesRatio = 0;
    }

    void LoadMinGnarlyBranchesGeometry() {
        LoadNoLeavesGeometry();
    }


    void LoadMaxGnarlyBranchesGrowth() {
        growthProperties.StemLength = 1.2f;
        growthProperties.BranchDensityBegin=0.2f;
        growthProperties.GnarlyBranchesRatio = 1;
    }

    void LoadMaxGnarlyBranchesGeometry() {
        geometryProperties.StemThickness = 0.4f;
        LoadNoLeavesGeometry();
    }




    void LoadExactLimitedGrowth() {
        growthProperties.AttractionPoints = new PseudoEllipsoid(6, 6, 6, 30, 0.15f, 0.05f);
        growthProperties.GnarlyBranchesRatio = 0.625f;

        growthProperties.StemLength = 0;
        growthProperties.Iterations = 45;
    }

    void LoadExactLimitedGeometry() {
        geometryProperties.StemThickness = 0.5f;
        //geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.3f);
    }



    void LoadBitLimitedGrowth() {
        growthProperties.AttractionPoints = new PseudoEllipsoid(5, 5, 5, 30, 0.15f, 0.05f);
        growthProperties.GnarlyBranchesRatio = 0.2f;

        growthProperties.StemLength = 0.4f;
        growthProperties.Iterations = 40;
    }

    void LoadBitLimitedGeometry() {
        LoadNoLeavesGeometry();
    }


    void LoadExtremeLimitedGrowth() {
        LoadBitLimitedGrowth();
        growthProperties.Iterations = 80;
    }

    void LoadExtremeLimitedGeometry() {
        LoadNoLeavesGeometry();
    }



    void LoadTestGrowth() {
        growthProperties.AttractionPoints = new PseudoEllipsoid(6, 6, 6, 15, 0.15f, 0.05f);
        growthProperties.GnarlyBranchesRatio = 1;
        debug(growthProperties.AttractionPoints.Points.Length + " attraction points");
        debug(growthProperties.AttractionPoints.Density + " density");

        growthProperties.Iterations = 40;
    }

    void LoadTestGrowthGeometry() {
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0);
    }


    void LoadBigGrowth() {
        growthProperties.AttractionPoints = new PseudoEllipsoid(10, 10, 10, 15, 0.15f, 0.05f);
        growthProperties.GnarlyBranchesRatio = 1;
        debug(growthProperties.AttractionPoints.Points.Length + " attraction points");
        debug(growthProperties.AttractionPoints.Density + " density");

        growthProperties.Iterations = 50;
    }

    void LoadBigGrowthGeometry() {
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0);
    }


    void LoadBigGrowth2() {
        growthProperties.AttractionPoints = new PseudoEllipsoid(10, 10, 10, 15, 0.15f, 0.05f);
        growthProperties.CrownStemLengthRatio = 0.15f;
        growthProperties.BranchDensityBegin = 0.1f;
        growthProperties.GnarlyBranchesRatio = 1;
        debug(growthProperties.AttractionPoints.Points.Length + " attraction points");
        debug(growthProperties.AttractionPoints.Density + " density");

        growthProperties.Iterations = 50;
    }

    void LoadBigGrowthGeometry2() {
        geometryProperties.CircleResolution = 7;
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0);
    }


    private void InitializeUI() {
        GameObject.Find("Initial Stem Length Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.StemLength);
        GameObject.Find("Stem Thickness Slider").GetComponent<Slider>().SetValueWithoutNotify(geometryProperties.StemThickness);

        GameObject.Find("Crown Stem Length Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.CrownStemLengthRatio);


        GameObject.Find("Iterations Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.Iterations);

        GameObject.Find("Crown Width Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.AttractionPoints.Radius_x);
        GameObject.Find("Crown Height Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.AttractionPoints.Radius_y);
        GameObject.Find("Crown Depth Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.AttractionPoints.Radius_z);
        GameObject.Find("Crown Top Cutoff Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.AttractionPoints.CutoffRatio_top);
        GameObject.Find("Crown Bottom Cutoff Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.AttractionPoints.CutoffRatio_bottom);

        //GameObject.Find("Stem / Crown Ratio Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetClearDistanceBegin_clearDistanceEnd_Ratio());
        GameObject.Find("Branch Density Begin Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.BranchDensityBegin);
        GameObject.Find("Branch Density End Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.BranchDensityEnd);

        GameObject.Find("Gnarly Branches Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GnarlyBranchesRatio);

        //GameObject.Find("Grow Towards Light Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.UpTropismWeightRatio);


        GameObject.Find("Stem Color Dropdown").GetComponent<BasicsController>().Initialize_StemColors(geometryProperties.StemColorStrings);
        GameObject.Find("Stem Color Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(geometryProperties.CurrentStemColorStringsIndex);
        GameObject.Find("Stem Color Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Leaf Color Dropdown").GetComponent<BasicsController>().Initialize_LeafColors(geometryProperties.LeafColorStrings);
        GameObject.Find("Leaf Color Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(geometryProperties.CurrentLeafColorStringsIndex);
        GameObject.Find("Leaf Color Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Leaf Type Dropdown").GetComponent<BasicsController>().Initialize_LeafTypes(geometryProperties.LeafTypeStrings);
        GameObject.Find("Leaf Type Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(geometryProperties.CurrentLeafTypeStringsIndex);
        GameObject.Find("Leaf Type Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Pendulous Branches Intensity Slider").GetComponent<Slider>().SetValueWithoutNotify(geometryProperties.PendulousBranchesIntensity);
        GameObject.Find("Pendulous Branches Begin Depth Slider").GetComponent<Slider>().SetValueWithoutNotify(geometryProperties.PendulousBranchesBeginDepthRatio);

        GameObject.Find("Foliage Density Slider").GetComponent<Slider>().minValue = 0;
        GameObject.Find("Foliage Density Slider").GetComponent<Slider>().maxValue = geometryProperties.DisplayedLeafesPerNodeMaximum;
        GameObject.Find("Foliage Density Slider").GetComponent<Slider>().SetValueWithoutNotify(geometryProperties.GetDisplayedLeavesPerNode());
        GameObject.Find("Foliage Lobe Size Slider").GetComponent<Slider>().SetValueWithoutNotify(geometryProperties.GetLeafSize());

        GameObject.Find("Circle Resolution Slider").GetComponent<Slider>().SetValueWithoutNotify(geometryProperties.CircleResolution);


        // SPACE COLONIZATION PARAMETERS
        //GameObject.Find("Density Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.AttractionPoints.GetDensity());
        //GameObject.Find("Clear Distance Begin Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetClearDistanceBegin());
        //GameObject.Find("Clear Distance End Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetClearDistanceEnd());
        //GameObject.Find("Influence Distance Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetInfluenceDistance());
        //GameObject.Find("Growth Distance Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetGrowthDistance());
        //GameObject.Find("Perception Angle Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetPerceptionAngle());
    }

    //#######################################################################################
    //##########                              MESH AGENT                           ##########
    //#######################################################################################

    //Vector3[] vertices;
    //Vector3[] normals;
    //Vector2[] uvs;
    //int[] triangles;

    List<Vector3> vertices;
    List<Vector3> normals;
    List<Vector2> uvs;
    List<int> triangles;

    List<GameObject> renderers = new List<GameObject>();
    List<Vector3[]> vertices_;
    List<Vector3[]> normals_;
    List<Vector2[]> uvs_;
    List<int[]> triangles_;

    bool recalculateMesh;

    
    //private object recalculationLock = new object(); //this is not needed, as the recalculation is called from an Update() method - like the GetMesh() method
    private void RecalculateMesh() {
        if (recalculateMesh) {
            recalculateMesh = false;

            tree.GetMesh(ref this.vertices, ref this.normals, ref this.uvs, ref this.triangles);


            GameObject.Find("Vertices Text").GetComponent<Text>().text = vertices.Count + " vertices";
            GameObject.Find("Triangles Text").GetComponent<Text>().text = triangles.Count / 3 + " triangles";

            //lock (recalculationLock) {
                Util.SplitMesh(vertices, normals, uvs, triangles, ref vertices_, ref normals_, ref uvs_, ref triangles_/*, recalculationLock*/);
            //}

            //add renderers as needed
            while (vertices_.Count > renderers.Count) {
                GameObject tr = new GameObject();
                tr.AddComponent<MeshRenderer>();
                tr.AddComponent<MeshFilter>();
                tr.AddComponent<TreeRenderer>();

                renderers.Add(tr);
            }

            ////remove renderers as they are not needed anymore
            //while (renderers.Count > vertices_.Count) {
            //    Destroy(renderers[renderers.Count - 1]);
            //    renderers.RemoveAt(renderers.Count - 1);
            //    nextRendererId--;
            //}
        }
    }

    //called by TreeRenderer
    int nextRendererId;
    public int GetRendererId() {
        return nextRendererId++;
    }

    //called by TreeRenderer
    public void GetMesh(int rendererId, ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, ref int[] triangles) {
        if (rendererId == 0) {
            RecalculateMesh();
        }

        //lock (recalculationLock) {
            if (vertices_ != null && rendererId < vertices_.Count) {
                vertices = this.vertices_[rendererId];
                normals = this.normals_[rendererId];
                uvs = this.uvs_[rendererId];
                triangles = this.triangles_[rendererId];
            } else {
                vertices = null;
                normals = null;
                uvs = null;
                triangles = null;
            }
        //}
    }



    //#######################################################################################
    //##########                           GROWER LISTENER                         ##########
    //#######################################################################################

    public void OnIterationFinished() {
        recalculateMesh = true;
    }

    public void OnGrowthStopped() {
        if (growthProperties.AttractionPoints.ActiveCount > 0.75*growthProperties.AttractionPoints.Points.Length) {
            SetMessage("You discovered some unfortunate randomness, try a different seed");
        } else {
            SetMessage("Growth stopped unexpectedly, try defining wider crown shape bounds or use less iterations (Age Slider)");
        }
    }


    //#######################################################################################
    //##########                           POINT CLOUD AGENT                       ##########
    //#######################################################################################

    public bool PointCloudRenderingEnabled { get; private set; }

    public void EnablePointCloudRenderer() {
        PointCloudRenderingEnabled = true;
    }

    public void DisablePointCloudRenderer() {
        PointCloudRenderingEnabled = false;
    }

    bool pointCloudReady = true;

    // called by PointCloudRenderer
    public bool PointCloudReady() {
        return pointCloudReady;
    }

    // called by PointCloudRenderer
    public Vector3[] GetPointCloud() {
        pointCloudReady = false;
        return growthProperties.AttractionPoints.Points;
    }


    //#######################################################################################
    //##########                          CAMERA MOVEMENT                          ##########
    //#######################################################################################

    public bool CameraMovementEnabled { get; private set; }

    public void DisableCameraMovement() {
        CameraMovementEnabled = false;
    }

    public void EnableCameraMovement() {
        CameraMovementEnabled = true;
    }

    public enum CameraMode {
        Tree,
        AttractionPoints
    }

    CameraMode cameraMode;

    public Vector3 GetLookAt() {
        //if (cameraMode == CameraMode.Tree) {
        //return new Vector3(0, grower.GetTreeHeight() / 2, 0);
        //} else {
        return growthProperties.AttractionPoints.Center + new Vector3(0, growthProperties.StemLength / 2, 0);
        //}
    }

    public float GetLookAtTop() {
        return growthProperties.AttractionPoints.GetHighestPoint() + growthProperties.StemLength;
    }

    public float GetDistanceToAttractionPoints() {
        if (cameraMode == CameraMode.Tree) { //normal case
            return -1;
        } else { //case when attraction points are beeing modified
                 //float distance = growthProperties.AttractionPoints.GetHeight() / 4
                 //    + growthProperties.AttractionPoints.GetDepth() / 4
                 //    + growthProperties.AttractionPoints.GetWidth() / 4
                 //    + growthProperties.StemLength / 4;

            //float distance = Math.Max(growthProperties.AttractionPoints.GetHeight(), Math.Max(growthProperties.AttractionPoints.GetDepth(), growthProperties.AttractionPoints.GetWidth())) / 2
            //    + growthProperties.StemLength / 2;






            //fraction these value by some bigger factor when big and less when small?
            float h = growthProperties.AttractionPoints.GetHeight() + growthProperties.StemLength;
            float w = Math.Max(growthProperties.AttractionPoints.GetDepth(), growthProperties.AttractionPoints.GetWidth());

            float distance = 4 + Math.Max(h / 5.5f, w / 5.5f);

            //float minDistance = 5;
            //float distance = Math.Max(minDistance, +Math.Max(h / 3, w / 3));


            return distance;
        }
    }

    //#######################################################################################
    //##########                             CROWN SHAPE                           ##########
    //#######################################################################################

    public void OnStemLength(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        growthProperties.StemLength = value;

        // NOT PRETTY
        geometryProperties.PendulousBranchesBeginDepthMax = CalculateBranchOrientationBeginDepthMax(growthProperties.Iterations);
        //geometryProperties.PendulousBranchesBeginDepthRatio = geometryProperties.PrecisePendulousBranchesBeginDepth / (geometryProperties.PendulousBranchesBeginDepthMax - geometryProperties.PendulousBranchesBeginDepthMin);
        //InitializeUI();


        grower.RegrowStem(tree);
        pointCloudReady = true;
    }

    public void OnStemThickness(float value) {
        geometryProperties.StemThickness = value;

        tree.StemRoot.RecalculateRadii();

        recalculateMesh = true;
    }

    public void OnCrownStemLength(float value) {
        SetMessage("");
        grower.Stop();

        growthProperties.CrownStemLengthRatio = value;
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownWidth(float value) {
        SetMessage("");
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        growthProperties.AttractionPoints.UpdateRadius_x(value);
        growthProperties.UpdateTropismsWeights();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownHeight(float value) {
        SetMessage("");
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        growthProperties.AttractionPoints.UpdateRadius_y(value);
        growthProperties.UpdateTropismsWeights();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownDepth(float value) {
        SetMessage("");
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        growthProperties.AttractionPoints.UpdateRadius_z(value);
        growthProperties.UpdateTropismsWeights();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownTopCutoff(float value) {
        SetMessage("");
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        growthProperties.AttractionPoints.UpdateCutoffRatio_top(value);
        growthProperties.UpdateTropismsWeights();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownBottomCutoff(float value) {
        SetMessage("");
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        growthProperties.AttractionPoints.UpdateCutoffRatio_bottom(value);
        growthProperties.UpdateTropismsWeights();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownShapeDone() {
        cameraMode = CameraMode.Tree;
    }




    public void OnPendulousBranchesIntensity(float value) {
        geometryProperties.PendulousBranchesIntensity = value;
        recalculateMesh = true;
    }

    public void OnPendulousBranchesBeginDepth(float value) {
        geometryProperties.PendulousBranchesBeginDepthRatio = value;
        recalculateMesh = true;
    }

    //public void OnClearDistanceBegin_clearDistanceEnd_Ratio(float value) {
    //    grower.Stop();

    //    growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(value);
    //    growthProperties.AttractionPoints.Reset();

    //    tree.Reset();

    //    grower.Grow(tree);
    //}


    public void OnBranchDensityBegin(float value) {
        grower.Stop();

        growthProperties.BranchDensityBegin = value;
        growthProperties.AttractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnBranchDensityEnd(float value) {
        grower.Stop();

        growthProperties.BranchDensityEnd = value;
        growthProperties.AttractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }



    public void OnGnarlyBranches(float value) {
        grower.Stop();

        growthProperties.GnarlyBranchesRatio = value;

        tree.Reset();

        grower.Grow(tree);
    }


    //#######################################################################################
    //##########                           STEM AND LEAVES                         ##########
    //#######################################################################################

    public void OnLeafType(int value) {
        geometryProperties.UpdateLeafType(value);

        recalculateMesh = true;

        InitializeUI();
    }

    public void OnStemColor(int value) {
        geometryProperties.CurrentStemColorStringsIndex = value;
    }

    public void OnLeafColor(int value) {
        geometryProperties.CurrentLeafColorStringsIndex = value;
    }

    public string GetTexture() {
        return Leaf.LeafTypeToFilename[geometryProperties.GetLeafType()] + "/" + geometryProperties.StemColorStrings[geometryProperties.CurrentStemColorStringsIndex] + "_" + geometryProperties.LeafColorStrings[geometryProperties.CurrentLeafColorStringsIndex];
    }

    public void OnFoliageDensity(float value) {
        geometryProperties.SetDisplayedLeavesPerNode(geometryProperties.GetLeafType(), value);

        recalculateMesh = true;
    }

    public void OnFoliageLobeSize(float value) {
        geometryProperties.SetLeafSize(geometryProperties.GetLeafType(), value);

        recalculateMesh = true;
    }

    public void OnCircleResolution(int value) {
        geometryProperties.CircleResolution = value;

        recalculateMesh = true;
    }

    //#######################################################################################
    //##########                                  MISC                             ##########
    //#######################################################################################

    //public void OnGrowTowardsLight(float value) {
    //    grower.Stop();

    //    growthProperties.UpTropismWeightRatio = value;
    //    growthProperties.AttractionPoints.Reset();

    //    tree.Reset();

    //    grower.Grow(tree);
    //}
    string message = "";
    int displayingThreads; //this is needed, otherwise newly arrived messaged would be deleted too early

    //should always get set to "" when changing a parameter that caused the message before
    private void SetMessage(string msg, int displayTime = 6000) {
        message = msg;
        //messageLeft = false;
        Thread resetThread = new Thread(() => {
            displayingThreads++;
            Thread.Sleep(displayTime);
            displayingThreads--;
            if (displayingThreads == 0) {
                message = "";
            }
        });
        resetThread.Start();
    }

    public string GetMessage() {
        return message;
    }

    private int CalculateBranchOrientationBeginDepthMax(int iterations) {
        return (int)Math.Ceiling(growthProperties.StemLength / growthProperties.GrowthDistance)
                                                                + iterations-1;
    }

    public void OnIterations(int value) {
        SetMessage("");

        grower.Stop();

        growthProperties.Iterations = value;
        growthProperties.AttractionPoints.Reset();

        // NOT PRETTY
        geometryProperties.PendulousBranchesBeginDepthMax = CalculateBranchOrientationBeginDepthMax(growthProperties.Iterations);
        InitializeUI();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnResetToDefaults() {
        SetMessage("");
        grower.Stop();

        LoadDefaultGrowth();
        LoadDefaultGeometry();
        InitializeUI();

        pointCloudReady = true;

        grower.Grow(tree);
    }

    public void OnNewSeed() {
        SetMessage("");
        grower.Stop();

        growthProperties.AttractionPoints.NewSeed();

        tree.Reset();

        grower.Grow(tree);
    }

    //https://stackoverflow.com/questions/44733841/how-to-make-texture2d-readable-via-script?noredirect=1&lq=1
    //https://stackoverflow.com/a/44734346
    Texture2D DuplicateTexture(Texture2D source) {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    //TODO: specify location &| unique naming
    public void OnSave() {
        string meshFilename = "tree_" + vertices.Count + "_" + triangles.Count + ".obj";
        ObjExporter.MeshToFile(vertices, normals, uvs, triangles, meshFilename);

        //Important: make sure that the textures have "Alpha is Transparency" checked, otherwise there are black borders
        //string textureFilename = geometryProperties.StemColorStrings[geometryProperties.CurrentStemColorStringsIndex] + "_" + geometryProperties.LeafColorStrings[geometryProperties.CurrentLeafColorStringsIndex] + "_" + Leaf.LeafTypeToFilename[geometryProperties.GetLeafType()] + ".png";
        string textureFilename = GetTexture().Replace("/", "_") + ".png";
        byte[] texture_bytes = DuplicateTexture(Resources.Load(GetTexture()) as Texture2D).EncodeToPNG();
        File.WriteAllBytes(textureFilename, texture_bytes);

        UnityEngine.Debug.Log("Saved mesh to " + meshFilename);
        UnityEngine.Debug.Log("Saved texture to " + textureFilename);
        SetMessage("Done :)", 2000);
    }

    public void OnApplicationQuit() {
        //UnityEngine.Debug.Log("Application ending after " + Time.time + " seconds");
        grower.Stop();
    }
}
