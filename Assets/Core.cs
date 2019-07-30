﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.Threading;

//https://gamedev.stackexchange.com/questions/133372/system-drawing-dll-not-found
//System.Drawing.dll FROM Unity.app -> Contents -> Mono -> lib -> mono -> 2.0 -> System.Drawing
//https://stackoverflow.com/a/17061246
//Configure: File -> Build Settings -> Player Settings -> Other Settings -> API Compatibility := .NET 4.x

public class Core : MonoBehaviour {
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (true) {
            UnityEngine.Debug.Log("DEBUG: TreeCreator: " + callerName + "(): " + message);
        }
    }


    PseudoEllipsoid attractionPoints;

    public AttractionPoints GetAttractionPoints() {
        return attractionPoints;
    }

    GrowthProperties growthProperties;
    Grower grower;
    GeometryProperties geometryProperties;
    Tree tree;

    // Start is called before the first frame update
    void Start() {

        //Bitmap bmp = new Bitmap(600, 600);

        //smallTree_hangingBranches(); //TODO
        //normalTree();
        //testTree();
        //bigTree();


        int initialAge = 30;
        float initialRadius_x = 3;
        float initialRadius_y = 5;
        float initialRadius_z = 3.5f;

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
        load_advancedNormalTree_particleTest_lowVertices(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);

        geometryProperties.StemColorStrings = new List<string> { "dark_brown", "brown", "greyish"};
        geometryProperties.CurrentStemColorStringsIndex = 1;
        geometryProperties.CurrentStemColorStringsIndex = 0;

        geometryProperties.LeafColorStrings = new List<string> { "yellow", "orange", "red", "green", "blue" };
        geometryProperties.CurrentLeafColorStringsIndex = 3;

        geometryProperties.LeafTypeStrings = Leaf.LeafTypeStrings;
        geometryProperties.CurrentLeafTypeStringsIndex = 1;
        geometryProperties.CurrentLeafTypeStringsIndex = 0;
        //load_excurrentTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);
        //load_testTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);


        // UI HAS TO BE SET TO THE SAME VALUES AS SPECIFIED IN DATASTRUCTURES

        tree.Grow();
    }

    bool initialized;
    void Update() {
        if (!initialized) {
            GameObject.Find("Age Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetIterations());
            GameObject.Find("Crown Width Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetRadius_x());
            GameObject.Find("Crown Height Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetRadius_y());
            GameObject.Find("Crown Depth Slider").GetComponent<Slider>().SetValueWithoutNotify(attractionPoints.GetRadius_z());


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
            //GameObject.Find("Leaf Color Dropdown").GetComponent<Dropdown>().value = 0;
            //GameObject.Find("Leaf Type Dropdown").GetComponent<Dropdown>().value = 0;
            //GameObject.Find("Leaves Enabled Toggle").GetComponent<Toggle>().enabled = true;

            initialized = true;
        }
    }

    //#######################################################################################
    //##########                              MESH AGENT                           ##########
    //#######################################################################################

    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;

    bool meshReady;

    object getMeshLock = new object();

    public void OnMeshReady(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles) {
        lock (getMeshLock) {
            this.vertices = vertices;
            this.normals = normals;
            this.uvs = uvs;
            this.triangles = triangles;

            meshReady = true;
        }
    }

    public bool MeshReady() {
        return meshReady;
    }

    public void GetMesh(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, ref int[] triangles) {
        lock (getMeshLock) {
            vertices = this.vertices;
            normals = this.normals;
            uvs = this.uvs;
            triangles = this.triangles;
        }
        meshReady = false;
    }


    //#######################################################################################
    //##########                           POINT CLOUD AGENT                       ##########
    //#######################################################################################

    bool pointCloudReady;

    //object getPointCloudLock = new object();

    public void OnAttractionPointsChanged() {
        pointCloudReady = true;
    }

    public bool PointCloudReady() {
        return pointCloudReady;
    }

    public List<Vector3> GetPointCloud() {
        pointCloudReady = false;
        return attractionPoints.GetBackup();
    }


    //#######################################################################################
    //##########                         UI ELEMENT LISTENER                       ##########
    //#######################################################################################

    public void OnAge(int value) {
        //debug("received");
        grower.Stop();

        growthProperties.UpdateIterations(value);
    }

    public void OnCrownRadius_x(float value) {
        //debug("received");
        grower.Stop();

        ((PseudoEllipsoid)attractionPoints).UpdateRadius_x(value);
    }

    public void OnCrownRadius_y(float value) {
        //debug("received");
        grower.Stop();

        ((PseudoEllipsoid)attractionPoints).UpdateRadius_y(value);
    }

    public void OnCrownRadius_z(float value) {
        //debug("received");
        grower.Stop();

        ((PseudoEllipsoid)attractionPoints).UpdateRadius_z(value);
    }


    //#######################################################################################
    //##########                           STEM AND LEAVES                         ##########
    //#######################################################################################

    public void OnLeafType(int value) {
        if (geometryProperties.LeafTypeStrings[value] .Equals (Leaf.LeafTypeToString[Leaf.LeafType.Triangle])) {
            geometryProperties.UpdateLeafSize(0.5f);
            geometryProperties.UpdateLeavesPerNode(2f);
        } else if (geometryProperties.LeafTypeStrings[value] .Equals (Leaf.LeafTypeToString[Leaf.LeafType.ParticleSquare])) {
            geometryProperties.UpdateLeafSize(1.6f);
            geometryProperties.UpdateLeavesPerNode(0.5f);
        }
        geometryProperties.UpdateLeafType(value);
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
        geometryProperties.UpdateLeavesEnabled(enabled);
    }

    //#######################################################################################
    //##########                                  MISC                             ##########
    //#######################################################################################

    public void OnNewSeed() {
        //debug("received");
        grower.Stop();

        attractionPoints.NewSeed();
    }

    //TODO: specify location &| unique naming
    public void OnSave() {
        ObjExporter.MeshToFile(vertices, normals, uvs, triangles, "tree_" + vertices.Length + "_" + triangles.Length + ".obj");
    }

    public void OnApplicationQuit() {
        //UnityEngine.Debug.Log("Application ending after " + Time.time + " seconds");
        grower.Stop();
        ThreadManager.Join();
    }

    void load_bugTree(int age=30, float radius_x=3, float radius_y=5, float radius_z=3.5f) {

        //influence distance und cleardistance müssen auf jeden Fall von density / Radien abhängen

        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.9f, 0.9f);
        growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);
        //SET LISTENER
        attractionPoints.SetAttractionPointsListener(growthProperties);


        grower = new SpaceColonization(growthProperties);
        //SET LISTENER
        growthProperties.SetGrowthPropertiesListener(grower);


        Vector3 position = Vector3.zero;

        geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.3f);
        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.75f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(false);


        tree = new Tree(position, grower, geometryProperties, this);
        //SET LISTENER
        grower.SetGrowerListener(tree);
        //geometryProperties.SetGeometryPropertiesListener(tree);
    }



    void load_normalTree(int age, float radius_x, float radius_y, float radius_z) {
        //Vector3 center = new Vector3(0, 5f, 0);
        //float maxDistance = 5f;
        //int amount = 8000;
        ////int amount = 30000;
        //List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);
        //AttractionPoints attractionPoints = new AttractionPoints(Vector3.zero, 0, 5, 8000, 0.2f);

        //AttractionPoints attractionPoints = new AttractionPoints(new Vector3(0, 0, 0), 5, 8000, 0.15f);
        //AttractionPoints attractionPoints = new SphereCut(new Vector3(0, 0, 0), 5, 15, 0.2f);

        //also not working yet:
        //AttractionPoints attractionPoints = new SpheroidAttractionPoints(new Vector3(0, 0, 0), 5, 2, 20);

        //not working yet:
        //AttractionPoints attractionPoints = new EllipsoidAttractionPoints(new Vector3(0, 0, 0), 1.5f, 5, 1.5f, 15);

        //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 4f, 2f, 15, 0.2f);
        //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 4f, 2f, 15, 0.0f);

        //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 8f, 4f, 15, 0.3f);

        //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 3f, 10f, 3f, 15, 0.15f, 0.1f);


        //influence distance und cleardistance müssen auf jeden Fall von density / Radien abhängen

        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        //growthProperties.SetInfluenceDistance(1.1f);
        //growthProperties.SetInfluenceDistance(1.2f);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.9f, 0.9f);
        //growthProperties.SetClearDistance(1f);
        //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        //growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
        growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);
        //SET LISTENER
        attractionPoints.SetAttractionPointsListener(growthProperties);


        grower = new SpaceColonization(growthProperties);
        //SET LISTENER
        growthProperties.SetGrowthPropertiesListener(grower);


        Vector3 position = Vector3.zero;

        geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.3f);
        geometryProperties.SetCircleResolution(3);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties, this);
        //SET LISTENER
        grower.SetGrowerListener(tree);
        //geometryProperties.SetGeometryPropertiesListener(tree);
    }

    void load_advancedNormalTree(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();

        growthProperties.SetInfluenceDistance(1);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.925f, 0.7f);


        //growthProperties.SetInfluenceDistance(1.3f);
        //growthProperties.SetPerceptionAngle(160f);
        //growthProperties.SetClearDistance(1.25f);
        //growthProperties.SetClearDistance_2(0.5f);
        //growthProperties.SetClearDistance(1f);
        //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
        //growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);
        //SET LISTENER
        attractionPoints.SetAttractionPointsListener(growthProperties);


        grower = new SpaceColonization(growthProperties);
        //SET LISTENER
        growthProperties.SetGrowthPropertiesListener(grower);


        Vector3 position = Vector3.zero;

        geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.7f);
        geometryProperties.SetCircleResolution(3);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetLeafSize(0.7f);
        geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(1);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties, this);
        //SET LISTENER
        grower.SetGrowerListener(tree);
        //geometryProperties.SetGeometryPropertiesListener(tree);
    }

    void load_advancedNormalTree_particleTest(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();

        growthProperties.SetInfluenceDistance(1);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.925f, 0.7f);
        growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
        //growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);
        //SET LISTENER
        attractionPoints.SetAttractionPointsListener(growthProperties);


        grower = new SpaceColonization(growthProperties);
        //SET LISTENER
        growthProperties.SetGrowthPropertiesListener(grower);


        Vector3 position = Vector3.zero;

        geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.7f);
        geometryProperties.SetCircleResolution(3);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetLeafSize(1.6f);
        geometryProperties.SetLeafType(Leaf.LeafType.ParticleSquare);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties, this);
        //SET LISTENER
        grower.SetGrowerListener(tree);
        //geometryProperties.SetGeometryPropertiesListener(tree);
    }

    void load_advancedNormalTree_particleTest_lowVertices(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();

        growthProperties.SetInfluenceDistance(1);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.925f, 0.7f);
        growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
        //growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);
        //SET LISTENER
        attractionPoints.SetAttractionPointsListener(growthProperties);


        grower = new SpaceColonization(growthProperties);
        //SET LISTENER
        growthProperties.SetGrowthPropertiesListener(grower);


        Vector3 position = Vector3.zero;

        geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.7f);
        geometryProperties.SetCircleResolution(3);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetLeafSize(1.6f);
        geometryProperties.SetLeafType(Leaf.LeafType.ParticleSquare);
        geometryProperties.SetLeavesPerNode(0.5f);
        //geometryProperties.SetLeavesPerNode(1.3f); // tiny
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties, this);
        //SET LISTENER
        grower.SetGrowerListener(tree);
        //geometryProperties.SetGeometryPropertiesListener(tree);
    }

    void load_excurrentTree(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 2, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(5f);
        //growthProperties.SetInfluenceDistance(1.1f);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.8f, 0.8f);
        //growthProperties.SetClearDistance(1f);
        //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        //growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
        //growthProperties.SetTropisms(new Vector3(0, 1f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);
        //SET LISTENER
        attractionPoints.SetAttractionPointsListener(growthProperties);


        grower = new SpaceColonization(growthProperties);
        //SET LISTENER
        growthProperties.SetGrowthPropertiesListener(grower);


        Vector3 position = Vector3.zero;

        geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.3f);
        geometryProperties.SetCircleResolution(3);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties, this);
        //SET LISTENER
        grower.SetGrowerListener(tree);
        //geometryProperties.SetGeometryPropertiesListener(tree);
    }

    void load_testTree(int age, float radius_x, float radius_y, float radius_z) {
        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 7.5f, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(2f);
        //growthProperties.SetInfluenceDistance(1.1f);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(1.8f, 1.8f);
        //growthProperties.SetClearDistance(1f);
        //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);
        //SET LISTENER
        attractionPoints.SetAttractionPointsListener(growthProperties);


        grower = new SpaceColonization(growthProperties);
        //SET LISTENER
        growthProperties.SetGrowthPropertiesListener(grower);


        Vector3 position = Vector3.zero;

        geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.3f);
        geometryProperties.SetCircleResolution(3);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties, this);
        //SET LISTENER
        grower.SetGrowerListener(tree);
        //geometryProperties.SetGeometryPropertiesListener(tree);
    }

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