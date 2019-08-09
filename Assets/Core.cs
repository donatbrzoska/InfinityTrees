using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class Core : MonoBehaviour, GrowerListener {
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (true) {
            UnityEngine.Debug.Log("DEBUG: TreeCreator: " + callerName + "(): " + message);
        }
    }

    PseudoEllipsoid attractionPoints;

    GrowthProperties growthProperties;
    Grower grower;
    GeometryProperties geometryProperties;
    Tree tree;

    // Start is called before the first frame update
    void Start() {

        //int initialAge = 30;
        //float initialRadius_x = 3;
        //float initialRadius_y = 5;
        //float initialRadius_z = 3.5f;

        //int initialAge = 60;
        //float initialRadius_x = 2;
        //float initialRadius_y = 9;
        //float initialRadius_z = 2;

        //int initialAge = 30;
        //float initialRadius_x = 5;
        //float initialRadius_y = 5;
        //float initialRadius_z = 5;

        //float initialRadius_x = 2.5f;
        //float initialRadius_y = 5;
        //float initialRadius_z = 2.5f;

        //int initialAge = 60;
        //float initialRadius_x = 10;
        //float initialRadius_y = 10;
        //float initialRadius_z = 10;

        //float initialRadius_x = 4;
        //float initialRadius_y = 10;
        //float initialRadius_z = 4;


        //load_normalTree_hangingBranches();
        //load_bugTree();
        //load_normalTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_advancedNormalTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_advancedNormalTree_particleTest(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_testTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_stemTestTree160_15(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_stemTestTree90_30(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_excurrentTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_nearTheEnvelopeTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_advancedNormalTree_attractionPointTest(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_advancedNormalTree_particleTest_lowVertices(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_excurrentTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_testTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);

        LoadDefaultGrowth();
        //LoadNNATestGrowth(); //13.3 / 2.7, 19 / ...
        //LoadTestGrowth();
        //LoadAttractionPointDensityGrowth();
        LoadDefaultGeometry();

        grower.Grow(tree);
    }

    //initialize UI at the beginning
    bool initialized;
    void Update() {
        //debug("sqcd: " + growthProperties.GetSquaredClearDistance(1));
        if (!initialized) {
            InitializeUI();
            initialized = true;
        }
    }

    void LoadDefaultGrowth() {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 5, 3.5f, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.2f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(30);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadNNATestGrowth() {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 6, 6, 6, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.2f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(40);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadTestGrowth() {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 5, 3.5f, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.2f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;

        growthProperties.SetGrowthDistance(0.125f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(30);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadAttractionPointDensityGrowth() {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 5, 3.5f, 15, 0.5f, 0.0f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.3f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(1f);
        growthProperties.SetBranchDensityEnd(1f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.2f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(30);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadDefaultGeometry() {
        geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.8f);
        geometryProperties.nth_root_min = 1.2f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        //geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.ParticleSquare);
        geometryProperties.SetLeavesEnabled(true);

        geometryProperties.StemColorStrings = new List<string> { "dark_brown", "brown", "light_brown", "grey_brown", "greyish" };
        geometryProperties.CurrentStemColorStringsIndex = 0;

        geometryProperties.LeafColorStrings = new List<string> { "yellow", "orange", "red", "lime_green", "light_green", "green", "dark_green", "light_turquoise", "dark_turquoise", "blue" };
        geometryProperties.CurrentLeafColorStringsIndex = 3;

        geometryProperties.LeafTypeStrings = Leaf.LeafTypeStrings;
        geometryProperties.CurrentLeafTypeStringsIndex = 0;


        tree = new Tree(geometryProperties);
    }

    void InitializeUI() {
        GameObject.Find("Stem Length Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.StemLength);
        GameObject.Find("Stem Thickness Slider").GetComponent<Slider>().SetValueWithoutNotify(geometryProperties.StemThickness);


        GameObject.Find("Age Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetIterations());

        GameObject.Find("Crown Width Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetRadius_x());
        GameObject.Find("Crown Height Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetRadius_y());
        GameObject.Find("Crown Depth Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetRadius_z());
        GameObject.Find("Crown Top Cutoff Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetCutoffRatio_top());
        GameObject.Find("Crown Bottom Cutoff Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetCutoffRatio_bottom());

        GameObject.Find("Stem / Crown Ratio Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetClearDistanceBegin_clearDistanceEnd_Ratio());
        GameObject.Find("Branch Density Begin Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetBranchDensityBegin());
        GameObject.Find("Branch Density End Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetBranchDensityEnd());

        //GameObject.Find("Hanging Branches Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetHangingBranchesIntensity());

        //GameObject.Find("Grow Towards Light Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.UpTropismWeightRatio);


        GameObject.Find("Stem Color Dropdown").GetComponent<StemColor>().Initialize(geometryProperties.StemColorStrings);
        GameObject.Find("Stem Color Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(geometryProperties.CurrentStemColorStringsIndex);
        GameObject.Find("Stem Color Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Leaf Color Dropdown").GetComponent<LeafColor>().Initialize(geometryProperties.LeafColorStrings);
        GameObject.Find("Leaf Color Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(geometryProperties.CurrentLeafColorStringsIndex);
        GameObject.Find("Leaf Color Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Leaf Type Dropdown").GetComponent<LeafType>().Initialize(geometryProperties.LeafTypeStrings);
        GameObject.Find("Leaf Type Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(geometryProperties.CurrentLeafTypeStringsIndex);
        GameObject.Find("Leaf Type Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Leaves Enabled Toggle").GetComponent<Toggle>().SetIsOnWithoutNotify(geometryProperties.GetLeavesEnabled());


        // SPACE COLONIZATION PARAMETERS
        GameObject.Find("Density Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetDensity());
        GameObject.Find("Clear Distance Begin Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetClearDistanceBegin());
        GameObject.Find("Clear Distance End Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetClearDistanceEnd());
        GameObject.Find("Influence Distance Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetInfluenceDistance());
        GameObject.Find("Growth Distance Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetGrowthDistance());
        GameObject.Find("Perception Angle Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetPerceptionAngle());
    }

    //#######################################################################################
    //##########                              MESH AGENT                           ##########
    //#######################################################################################

    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;

    bool recalculateMesh;

    //called by TreeRenderer
    public bool MeshReady() {
        return recalculateMesh;
    }

    //called by TreeRenderer
    public void GetMesh(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, ref int[] triangles) {
        tree.GetMesh(ref this.vertices, ref this.normals, ref this.uvs, ref this.triangles);

        vertices = this.vertices;
        normals = this.normals;
        uvs = this.uvs;
        triangles = this.triangles;

        recalculateMesh = false;
    }

    //#######################################################################################
    //##########                           GROWER LISTENER                         ##########
    //#######################################################################################

    public void OnUpdate() {
        recalculateMesh = true;
    }


    //#######################################################################################
    //##########                           POINT CLOUD AGENT                       ##########
    //#######################################################################################

    bool pointCloudReady = true;

    // called by PointCloudRenderer
    public bool PointCloudReady() {
        return pointCloudReady;
    }

    // called by PointCloudRenderer
    public List<Vector3> GetPointCloud() {
        pointCloudReady = false;
        return attractionPoints.Backup;
    }


    //#######################################################################################
    //##########                          CAMERA MOVEMENT                          ##########
    //#######################################################################################

    public enum CameraMode {
        Tree,
        AttractionPoints
    }

    CameraMode cameraMode;

    public Vector3 GetLookAt() {
        //if (cameraMode == CameraMode.Tree) {
        //return new Vector3(0, grower.GetTreeHeight() / 2, 0);
        //} else {
        return attractionPoints.GetCenter();
        //}
    }

    public float GetLookAtTop() {
        return attractionPoints.GetHeight();
    }

    public float GetDistanceToAttractionPoints() {
        if (cameraMode == CameraMode.Tree) { //normal case
            return -1;
        } else { //case when attraction points are beeing modified
            return attractionPoints.GetHeight() / 3 + attractionPoints.GetRadius_x() / 3 + attractionPoints.GetRadius_z() / 3;
        }
    }

    //#######################################################################################
    //##########                             CROWN SHAPE                           ##########
    //#######################################################################################

    public void OnLength(float value) {
        //cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        //tree.UpdateStemLength(value);
        growthProperties.StemLength = value;
        pointCloudReady = true; // this is note precise, but a new PointCloud will follow soon

        grower.RegrowStem(tree);
    }

    public void OnThickness(float value) {
        geometryProperties.StemThickness = value;

        tree.StemRoot.RecalculateRadii();

        recalculateMesh = true;
    }


    private void UpdateTropisms() {
        // the smaller growthProperties.UpTropismRemovalRatio, the later the condition hits
        if (attractionPoints.GetRadius_y() < growthProperties.UpTropismsDampRatio * (attractionPoints.GetRadius_x() + attractionPoints.GetRadius_z())) {
            //growthProperties.SetTropismsWeights(new Vector3(growthProperties.GetTropismsWeights().x, 0, growthProperties.GetTropismsWeights().z));
            growthProperties.SetTropismsWeights(
                new Vector3(1,
                            (float) (growthProperties.UpTropismsWhenDamped
                                * growthProperties.UpTropismsDampRatio * (attractionPoints.GetRadius_x() + attractionPoints.GetRadius_z() / attractionPoints.GetRadius_y())), //this is about 1 when the border is crossed, and decreases as the y radius gets smaller
                            1)
                );
        } else {
            growthProperties.SetTropismsWeights(new Vector3(1, 1, 1));
        }
    }
    
    public void OnCrownWidth(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        attractionPoints.UpdateRadius_x(value);
        UpdateTropisms();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownHeight(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        attractionPoints.UpdateRadius_y(value);
        UpdateTropisms();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownDepth(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        attractionPoints.UpdateRadius_z(value);
        UpdateTropisms();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownTopCutoff(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        attractionPoints.UpdateCutoffRatio_top(value);
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownBottomCutoff(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        attractionPoints.UpdateCutoffRatio_bottom(value);
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownShapeDone() {
        cameraMode = CameraMode.Tree;
    }




    public void OnClearDistanceBegin_clearDistanceEnd_Ratio(float value) {
        grower.Stop();

        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }


    public void OnBranchDensityBegin(float value) {
        grower.Stop();

        growthProperties.SetBranchDensityBegin(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnBranchDensityEnd(float value) {
        grower.Stop();

        growthProperties.SetBranchDensityEnd(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }



    // TODO WHAT IF Y DAMPED
    public void OnHangingBranches(float value) {
        grower.Stop();

        growthProperties.SetHangingBranchesIntensity(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }



    // SPACE COLONIZATION

    public void OnDensity(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        attractionPoints.UpdateDensity(value);
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnClearDistanceBegin(float value) {
        grower.Stop();

        growthProperties.SetClearDistanceBegin(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnClearDistanceEnd(float value) {
        grower.Stop();

        growthProperties.SetClearDistanceEnd(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnInfluenceDistance(float value) {
        grower.Stop();

        growthProperties.SetInfluenceDistance(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnGrowthDistance(float value) {
        grower.Stop();

        growthProperties.SetGrowthDistance(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnPerceptionAngle(float value) {
        grower.Stop();

        growthProperties.SetPerceptionAngle(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    //#######################################################################################
    //##########                           STEM AND LEAVES                         ##########
    //#######################################################################################

    public void OnLeafType(int value) {
        geometryProperties.UpdateLeafType(value);

        recalculateMesh = true;
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

    public void OnLeavesEnabled(bool enabled) {
        geometryProperties.SetLeavesEnabled(enabled);

        recalculateMesh = true;
    }


    //#######################################################################################
    //##########                                  MISC                             ##########
    //#######################################################################################

    public void OnGrowTowardsLight(float value) {
        grower.Stop();

        growthProperties.UpTropismWeightRatio = value;
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnAge(int value) {
        grower.Stop();

        growthProperties.SetIterations(value);
        attractionPoints.Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnResetToDefaults() {
        grower.Stop();

        LoadDefaultGrowth();
        LoadDefaultGeometry();
        InitializeUI();

        grower.Grow(tree);
    }

    public void OnNewSeed() {
        grower.Stop();

        attractionPoints.NewSeed();

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
        string meshFilename = "tree_" + vertices.Length + "_" + triangles.Length + ".obj";
        ObjExporter.MeshToFile(vertices, normals, uvs, triangles, meshFilename);

        //Important: make sure that the textures have "Alpha is Transparency" checked, otherwise there are black borders
        //string textureFilename = geometryProperties.StemColorStrings[geometryProperties.CurrentStemColorStringsIndex] + "_" + geometryProperties.LeafColorStrings[geometryProperties.CurrentLeafColorStringsIndex] + "_" + Leaf.LeafTypeToFilename[geometryProperties.GetLeafType()] + ".png";
        string textureFilename = GetTexture().Replace("/", "_") + ".png";
        byte[] texture_bytes = DuplicateTexture(Resources.Load(GetTexture()) as Texture2D).EncodeToPNG();
        File.WriteAllBytes(textureFilename, texture_bytes);

        UnityEngine.Debug.Log("Saved mesh to " + meshFilename);
        UnityEngine.Debug.Log("Saved texture to " + textureFilename);
    }

    public void OnApplicationQuit() {
        //UnityEngine.Debug.Log("Application ending after " + Time.time + " seconds");
        grower.Stop();
    }



    //void load_bugTree(int age=30, float radius_x=3, float radius_y=5, float radius_z=3.5f) {

    //    //influence distance und cleardistance müssen auf jeden Fall von density / Radien abhängen

    //    attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


    //    growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(1f);
    //    growthProperties.SetPerceptionAngle(160f);
    //    growthProperties.SetClearDistance(0.9f, 0.9f);
    //    growthProperties.SetTropisms(new Vector3(0, 1f, 0));
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);
    //    growthProperties.SetIterations(age);


    //    grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    geometryProperties.SetNthRoot(1.3f);
    //    geometryProperties.SetCircleResolution(3);
    //    //geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);
    //    geometryProperties.SetMinRadiusRatioForNormalConnection(0.75f);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetLeafSize(0.5f);
    //    geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(2);
    //    geometryProperties.SetLeavesEnabled(false);


    //    tree = new Tree(position, grower, geometryProperties, this);
    //    //SET LISTENER
    //    grower.SetGrowerListener(tree);
    //    //geometryProperties.SetGeometryPropertiesListener(tree);
    //}



    //void load_normalTree(int age, float radius_x, float radius_y, float radius_z) {
    //    //Vector3 center = new Vector3(0, 5f, 0);
    //    //float maxDistance = 5f;
    //    //int amount = 8000;
    //    ////int amount = 30000;
    //    //List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);
    //    //AttractionPoints attractionPoints = new AttractionPoints(Vector3.zero, 0, 5, 8000, 0.2f);

    //    //AttractionPoints attractionPoints = new AttractionPoints(new Vector3(0, 0, 0), 5, 8000, 0.15f);
    //    //AttractionPoints attractionPoints = new SphereCut(new Vector3(0, 0, 0), 5, 15, 0.2f);

    //    //also not working yet:
    //    //AttractionPoints attractionPoints = new SpheroidAttractionPoints(new Vector3(0, 0, 0), 5, 2, 20);

    //    //not working yet:
    //    //AttractionPoints attractionPoints = new EllipsoidAttractionPoints(new Vector3(0, 0, 0), 1.5f, 5, 1.5f, 15);

    //    //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 4f, 2f, 15, 0.2f);
    //    //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 4f, 2f, 15, 0.0f);

    //    //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 8f, 4f, 15, 0.3f);

    //    //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 3f, 10f, 3f, 15, 0.15f, 0.1f);


    //    //influence distance und cleardistance müssen auf jeden Fall von density / Radien abhängen

    //    attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


    //    growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(1f);
    //    //growthProperties.SetInfluenceDistance(1.1f);
    //    //growthProperties.SetInfluenceDistance(1.2f);
    //    growthProperties.SetPerceptionAngle(160f);
    //    growthProperties.SetClearDistance(0.9f, 0.9f);
    //    //growthProperties.SetClearDistance(1f);
    //    //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
    //    //growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
    //    growthProperties.SetTropisms(new Vector3(0, 1f, 0));
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);
    //    growthProperties.SetIterations(age);


    //    grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    geometryProperties.SetNthRoot(1.3f);
    //    geometryProperties.SetCircleResolution(3);
    //    geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetLeafSize(0.5f);
    //    geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(2);
    //    geometryProperties.SetLeavesEnabled(true);


    //    tree = new Tree(position, grower, geometryProperties, this);
    //    //SET LISTENER
    //    grower.SetGrowerListener(tree);
    //    //geometryProperties.SetGeometryPropertiesListener(tree);
    //}

    //void load_advancedNormalTree(int age, float radius_x, float radius_y, float radius_z) {
    //    attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


    //    growthProperties = new GrowthProperties();

    //    growthProperties.SetInfluenceDistance(1);
    //    growthProperties.SetPerceptionAngle(160f);
    //    growthProperties.SetClearDistance(0.925f, 0.7f);


    //    //growthProperties.SetInfluenceDistance(1.3f);
    //    //growthProperties.SetPerceptionAngle(160f);
    //    //growthProperties.SetClearDistance(1.25f);
    //    //growthProperties.SetClearDistance_2(0.5f);
    //    //growthProperties.SetClearDistance(1f);
    //    //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
    //    growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
    //    //growthProperties.SetTropisms(new Vector3(0, 1f, 0));
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);
    //    growthProperties.SetIterations(age);


    //    grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    geometryProperties.SetNthRoot(1.7f);
    //    geometryProperties.SetCircleResolution(3);
    //    geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetLeafSize(0.7f);
    //    geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(1);
    //    geometryProperties.SetLeavesEnabled(true);


    //    tree = new Tree(position, grower, geometryProperties, this);
    //    //SET LISTENER
    //    grower.SetGrowerListener(tree);
    //    //geometryProperties.SetGeometryPropertiesListener(tree);
    //}

    void load_advancedNormalTree_particleTest(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(/*new Vector3(0, 0, 0), */radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.925f, 0.7f);
        growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);


        grower = new SpaceColonization(growthProperties, this);
    }

    void load_testTree(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(/*new Vector3(0, 0, 0), */radius_x, radius_y, radius_z, 30, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1);
        growthProperties.SetPerceptionAngle(90);

        //growthProperties.SetClearDistance(0.925f, 0.7f);
        growthProperties.SetMinClearDistanceRatio(0.4f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0.2f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);

        growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.4f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);


        grower = new SpaceColonization(growthProperties, this);
    }

    void load_stemTestTree160_15(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        //growthProperties.SetClearDistance(0.925f, 0.7f);
        growthProperties.SetMinClearDistanceRatio(0.3f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        //growthProperties.SetMaxClearDistanceRatio(0.9f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(1f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.4f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 1;

        growthProperties.StemLength = 2;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);


        grower = new SpaceColonization(growthProperties, this);
    }

    void load_stemTestTree90_30(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 2, 0), radius_x, radius_y, radius_z, 30, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1.2f);
        growthProperties.SetPerceptionAngle(90);
        //growthProperties.SetClearDistance(0.925f, 0.7f);
        growthProperties.SetMinClearDistanceRatio(0.4f);
        growthProperties.SetMaxClearDistanceRatio(0.95f);
        growthProperties.SetBranchDensityBegin(0.08f);
        growthProperties.SetBranchDensityEnd(0.7f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.4f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);


        grower = new SpaceColonization(growthProperties, this);
    }

    void load_excurrentTree(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(/*new Vector3(0, 0, 0), */radius_x, radius_y, radius_z, 30, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(10);
        growthProperties.SetPerceptionAngle(130);
        //growthProperties.SetClearDistance(0.925f, 0.7f);
        growthProperties.SetMinClearDistanceRatio(0.04f);
        growthProperties.SetMaxClearDistanceRatio(0.9f);
        growthProperties.SetBranchDensityBegin(1f);
        growthProperties.SetBranchDensityEnd(1f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0, 1, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 0, 1));
        growthProperties.UpTropismsDampRatio = 0.4f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);


        grower = new SpaceColonization(growthProperties, this);
    }

    void load_nearTheEnvelopeTree(int age, float radius_x, float radius_y, float radius_z) {
        //growthProperties.StemLength = 2;
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 2, 0), radius_x, radius_y, radius_z, 25, 0.35f, 0f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1000);
        growthProperties.SetPerceptionAngle(180);
        growthProperties.SetClearDistance(0.7f);
        growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 0.2f, 1));
        growthProperties.UpTropismsDampRatio = 0.4f;
        growthProperties.UpTropismsWhenDamped = 0.3f;
        growthProperties.SetHangingBranchesIntensity(0);

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);


        grower = new SpaceColonization(growthProperties, this);
    }

    void load_advancedNormalTree_attractionPointTest(int age, float radius_x, float radius_y, float radius_z) {
        //attractionPoints = new PseudoEllipsoid(/*new Vector3(0, 0, 0), */radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);
        attractionPoints = new PseudoEllipsoid(/*new Vector3(0, 0, 0), */radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1);
        growthProperties.SetPerceptionAngle(160f);
        //growthProperties.SetClearDistance(0.925f, 0.7f);
        growthProperties.SetClearDistance(0.5f, 0.5f);
        growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1, 1));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(2);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);


        grower = new SpaceColonization(growthProperties, this);
    }

    //void load_advancedNormalTree_particleTest_lowVertices(int age, float radius_x, float radius_y, float radius_z) {
    //    attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


    //    growthProperties = new GrowthProperties();

    //    growthProperties.SetInfluenceDistance(1);
    //    growthProperties.SetPerceptionAngle(160f);
    //    growthProperties.SetClearDistance(0.925f, 0.7f);
    //    growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
    //    //growthProperties.SetTropisms(new Vector3(0, 1f, 0));
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);
    //    growthProperties.SetIterations(age);


    //    grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    geometryProperties.SetNthRoot(1.7f);
    //    geometryProperties.SetCircleResolution(3);
    //    geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetLeafSize(1.6f);
    //    geometryProperties.SetLeafType(Leaf.LeafType.ParticleSquare);
    //    geometryProperties.SetLeavesPerNode(0.5f);
    //    //geometryProperties.SetLeavesPerNode(1.3f); // tiny
    //    geometryProperties.SetLeavesEnabled(true);


    //    tree = new Tree(position, grower, geometryProperties, this);
    //    //SET LISTENER
    //    grower.SetGrowerListener(tree);
    //    //geometryProperties.SetGeometryPropertiesListener(tree);
    //}

    //void load_excurrentTree(int age, float radius_x, float radius_y, float radius_z) {
    //    attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 2, 0.15f, 0.05f);


    //    growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(5f);
    //    //growthProperties.SetInfluenceDistance(1.1f);
    //    growthProperties.SetPerceptionAngle(160f);
    //    growthProperties.SetClearDistance(0.8f, 0.8f);
    //    //growthProperties.SetClearDistance(1f);
    //    //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
    //    //growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
    //    //growthProperties.SetTropisms(new Vector3(0, 1f, 0));
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);
    //    growthProperties.SetIterations(age);


    //    grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    geometryProperties.SetNthRoot(1.3f);
    //    geometryProperties.SetCircleResolution(3);
    //    geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetLeafSize(0.5f);
    //    geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(2);
    //    geometryProperties.SetLeavesEnabled(true);


    //    tree = new Tree(position, grower, geometryProperties, this);
    //    //SET LISTENER
    //    grower.SetGrowerListener(tree);
    //    //geometryProperties.SetGeometryPropertiesListener(tree);
    //}

    //void load_testTree(int age, float radius_x, float radius_y, float radius_z) {
    //    attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 7.5f, 0.15f, 0.05f);


    //    growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(2f);
    //    //growthProperties.SetInfluenceDistance(1.1f);
    //    growthProperties.SetPerceptionAngle(160f);
    //    growthProperties.SetClearDistance(1.8f, 1.8f);
    //    //growthProperties.SetClearDistance(1f);
    //    //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
    //    growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);
    //    growthProperties.SetIterations(age);


    //    grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    geometryProperties.SetNthRoot(1.3f);
    //    geometryProperties.SetCircleResolution(3);
    //    geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetLeafSize(0.5f);
    //    geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(2);
    //    geometryProperties.SetLeavesEnabled(true);


    //    tree = new Tree(position, grower, geometryProperties, this);
    //    //SET LISTENER
    //    grower.SetGrowerListener(tree);
    //    //geometryProperties.SetGeometryPropertiesListener(tree);
    //}

}






//void load_normalTree_hangingBranches(int age) {
//    Vector3 center = new Vector3(0, 5f, 0);
//    float maxDistance = 5f;
//    int amount = 8000;
//    //int amount = 30000;
//    List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

//    growthProperties = new GrowthProperties();
//    growthProperties.SetInfluenceDistance(0.9f);
//    growthProperties.SetPerceptionAngle(160f);
//    growthProperties.SetClearDistance(0.8f);
//    growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
//    growthProperties.SetHangingBranchesEnabled(true);
//    growthProperties.SetHangingBranchesFromAgeRatio(0.6f);
//    growthProperties.SetGrowthDistance(0.25f);
//    growthProperties.SetAttractionPoints(attractionPoints);
//    growthProperties.SetIterations(age);

//    Grower grower = new SpaceColonization(growthProperties);


//    Vector3 position = Vector3.zero;

//    geometryProperties = new GeometryProperties();
//    geometryProperties.SetTipRadius(0.007f);
//    geometryProperties.SetNthRoot(1.3f);
//    geometryProperties.SetCircleResolution(3);

//    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
//    geometryProperties.SetMinLeafSize(0.1f);
//    geometryProperties.SetMaxLeafSize(0.5f);
//    geometryProperties.SetLeafType(LeafType.Triangle);
//    geometryProperties.SetLeavesPerNode(2);
//    geometryProperties.SetLeavesEnabled(true);


//    tree = new Tree(position, grower, geometryProperties);

//    tree.Grow();
//    //tree.Grow(30);
//}

//void testTree() {
//    Vector3 center = new Vector3(0, 5f, 0);
//    float maxDistance = 5f;
//    int amount = 8000;
//    //int amount = 30000;
//    List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

//    GrowthProperties growthProperties = new GrowthProperties();
//    growthProperties.SetInfluenceDistance(0.9f);
//    //growthProperties.SetInfluenceDistance(1.5f);
//    growthProperties.SetPerceptionAngle(160f);
//    //growthProperties.SetPerceptionAngle(100f);
//    growthProperties.SetClearDistance(0.8f);
//    //growthProperties.SetClearDistance(0.5f);
//    growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
//    growthProperties.SetGrowthDistance(0.25f);
//    growthProperties.SetAttractionPoints(attractionPoints);

//    SpaceColonization grower = new SpaceColonization(growthProperties);


//    Vector3 position = Vector3.zero;

//    GeometryProperties geometryProperties = new GeometryProperties();
//    geometryProperties.SetTipRadius(0.007f);
//    //geometryProperties.SetNthRoot(1.6f);
//    geometryProperties.SetNthRoot(1.3f);
//    geometryProperties.SetCircleResolution(3);

//    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
//    geometryProperties.SetMinLeafSize(0.1f);
//    geometryProperties.SetMaxLeafSize(0.5f);
//    geometryProperties.SetLeafType(LeafType.Triangle);
//    geometryProperties.SetLeavesPerNode(2);
//    geometryProperties.SetLeavesEnabled(true);


//    tree = new Tree(position, grower, geometryProperties);

//    tree.Grow(50);
//    //tree.Grow(30);
//}


//void highRes() {
//    Vector3 center = new Vector3(0, 5f, 0);
//    float maxDistance = 5f;
//    int amount = 8000;
//    //int amount = 30000;
//    List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

//    GrowthProperties growthProperties = new GrowthProperties();
//    growthProperties.SetInfluenceDistance(0.9f);
//    //growthProperties.SetInfluenceDistance(1.5f);
//    growthProperties.SetPerceptionAngle(160f);
//    //growthProperties.SetPerceptionAngle(100f);
//    growthProperties.SetClearDistance(0.8f);
//    //growthProperties.SetClearDistance(0.5f);
//    growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
//    growthProperties.SetGrowthDistance(0.125f);
//    growthProperties.SetAttractionPoints(attractionPoints);

//    SpaceColonization grower = new SpaceColonization(growthProperties);


//    Vector3 position = Vector3.zero;

//    GeometryProperties geometryProperties = new GeometryProperties();
//    geometryProperties.SetTipRadius(0.007f);
//    //geometryProperties.SetNthRoot(1.6f);
//    geometryProperties.SetNthRoot(1.3f);
//    geometryProperties.SetCircleResolution(3);

//    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
//    geometryProperties.SetMinLeafSize(0.1f);
//    geometryProperties.SetMaxLeafSize(0.65f);
//    geometryProperties.SetLeafType(LeafType.Triangle);
//    geometryProperties.SetLeavesPerNode(2);
//    geometryProperties.SetLeavesEnabled(true);


//    tree = new Tree(position, grower, geometryProperties);

//    tree.Grow(100);
//    //tree.Grow(30);
//}


//void bigTree() {
//    Vector3 center = new Vector3(0, 10f, 0);
//    float maxDistance = 10f;
//    int amount = 50000;
//    HashSet<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

//    GrowthProperties growthProperties = new GrowthProperties();
//    growthProperties.SetInfluenceDistance(0.9f);
//    growthProperties.SetGrowthDistance(0.25f);
//    growthProperties.SetClearDistance(0.8f);
//    growthProperties.SetTropisms(new Vector3(0, 1, 0));

//    growthProperties.SetMaxTwigRadiusForLeaves(0.02f);
//    growthProperties.SetMinLeafSize(0.1f);
//    growthProperties.SetMaxLeafSize(0.5f);
//    growthProperties.SetLeavesPerNode(2);

//    growthProperties.SetMaxBranchingAngle(80);
//    SpaceColonization grower = new SpaceColonization(attractionPoints, growthProperties);
//    //SimpleSpaceColonization grower = new SimpleSpaceColonization(attractionPoints, growthProperties);


//    Vector3 position = Vector3.zero;
//    tree = new Tree(position, grower, growthProperties);


//    cylinderResolution = 7;

//    tree.Grow(65, cylinderResolution);
//}
