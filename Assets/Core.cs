using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Core : MonoBehaviour, GrowerListener {
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (true) {
            UnityEngine.Debug.Log("DEBUG: TreeCreator: " + callerName + "(): " + message);
        }
    }

    SpaceColonization grower;
    Tree tree;

    // Start is called before the first frame update
    void Start() {

        //LoadYoungGrowth();
        //LoadBaobabGrowth();
        //LoadCrownStemLengthTestGrowth();
        //LoadNoTropismsDampGrowth();
        //LoadNNATestGrowth(); //13.3 / 2.7, 19 / ...
        //LoadTestGrowth();
        //LoadAttractionPointDensityGrowth();

        //vl, tl || v, t
        //7728, 7210 || 3856, 5274
        LoadDefaultGrowth();
        LoadDefaultGeometry();

        //1764, 1658 || 884, 1218
        //LoadYoungGrowth();
        //LoadYoungGeometry();

        //9152, 8334 || 4352, 5934
        //LoadExcurrentGrowth();
        //LoadExcurrentGeometry();

        //7024, 6514 || 3392, 4698
        //4896, 5450 || 3392, 4698 //0.1 displayed leaves per node
        //LoadBushGrowth();
        //LoadBushGeometry();

        //8516, 7864 || 4404, 5808 |||| 5108, 6260 (low vertices with leaves)
        //LoadPseudoPoplarGrowth();
        //LoadDefaultGeometry();

        //
        //LoadBush2Growth();
        //LoadBushGeometry();

        //LoadMediumBigGrowth();
        //LoadDefaultGeometry();

        //LoadBigGrowth();
        //LoadDefaultGeometry2();

        //LoadExactLimitedGrowth();
        //LoadExactLimitedGeometry();

        //LoadBitLimitedGrowth();
        //LoadBitLimitedGeometry();

        //LoadExtremeLimitedGrowth();
        //LoadExtremeLimitedGeometry();


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
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 5, 3.5f, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(30);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadDefaultGeometry() {
        GeometryProperties geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.8f);
        geometryProperties.nth_root_min = 1f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 1f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.5f);
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

    void LoadYoungGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 5, 3.5f, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.95f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 1.2f;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(20);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadYoungGeometry() {
        GeometryProperties geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.8f);
        geometryProperties.nth_root_min = 1f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 1f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.5f);
        //geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.ParticleCrossFoil);

        geometryProperties.StemColorStrings = new List<string> { "dark_brown", "brown", "light_brown", "grey_brown", "greyish" };
        geometryProperties.CurrentStemColorStringsIndex = 3;

        geometryProperties.LeafColorStrings = new List<string> { "yellow", "orange", "red", "lime_green", "light_green", "green", "dark_green", "light_turquoise", "dark_turquoise", "blue" };
        geometryProperties.CurrentLeafColorStringsIndex = 2;

        geometryProperties.LeafTypeStrings = Leaf.LeafTypeStrings;
        geometryProperties.CurrentLeafTypeStringsIndex = 1;


        tree = new Tree(geometryProperties);
    }

    void LoadExcurrentGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 10, 3.2f, 15, 0.5f, 0);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.95f);
        growthProperties.SetBranchDensityBegin(0.3f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 1f;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 1f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(30);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadExcurrentGeometry() {
        GeometryProperties geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.8f);
        geometryProperties.nth_root_min = 1f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 1f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.5f);
        //geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.ParticleCrossFoil);

        geometryProperties.StemColorStrings = new List<string> { "dark_brown", "brown", "light_brown", "grey_brown", "greyish" };
        geometryProperties.CurrentStemColorStringsIndex = 0;

        geometryProperties.LeafColorStrings = new List<string> { "yellow", "orange", "red", "lime_green", "light_green", "green", "dark_green", "light_turquoise", "dark_turquoise", "blue" };
        geometryProperties.CurrentLeafColorStringsIndex = 6;

        geometryProperties.LeafTypeStrings = Leaf.LeafTypeStrings;
        geometryProperties.CurrentLeafTypeStringsIndex = 1;


        tree = new Tree(geometryProperties);
    }


    void LoadBushGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 7, 4, 7f, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.95f);
        growthProperties.SetBranchDensityBegin(0.2f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 0.3f, 1)); //adjusted by hand
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 0f;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(20);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadBushGeometry() {
        GeometryProperties geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.8f);
        geometryProperties.nth_root_min = 1f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 0.8f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.2f);
        //geometryProperties.SetLeafSize(0.5f);
        geometryProperties.SetLeafType(Leaf.LeafType.ParticleCrossFoil);

        geometryProperties.StemColorStrings = new List<string> { "dark_brown", "brown", "light_brown", "grey_brown", "greyish" };
        geometryProperties.CurrentStemColorStringsIndex = 0;

        geometryProperties.LeafColorStrings = new List<string> { "yellow", "orange", "red", "lime_green", "light_green", "green", "dark_green", "light_turquoise", "dark_turquoise", "blue" };
        geometryProperties.CurrentLeafColorStringsIndex = 6;

        geometryProperties.LeafTypeStrings = Leaf.LeafTypeStrings;
        geometryProperties.CurrentLeafTypeStringsIndex = 1;


        tree = new Tree(geometryProperties);
    }

    void LoadPseudoPoplarGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 2.1f, 7, 1.9f, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.95f);
        growthProperties.SetBranchDensityBegin(0.6f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 1;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(40);


        grower = new SpaceColonization(growthProperties, this);
    }


    void LoadMediumBigGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 10, 10, 10, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.95f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(35);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadBigGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 10, 10, 10, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.95f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(50);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadDefaultGeometry2() {
        GeometryProperties geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.8f);
        geometryProperties.nth_root_min = 1f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 1f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.5f);
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

    void LoadExactLimitedGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 6, 6, 6, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 0.5f;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(45);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadExactLimitedGeometry() {
        GeometryProperties geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(2f);
        geometryProperties.nth_root_min = 1f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 1f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0.3f);
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

    void LoadBitLimitedGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 4, 4, 4, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 0;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(33);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadBitLimitedGeometry() {
        GeometryProperties geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.8f);
        geometryProperties.nth_root_min = 1f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 1f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0f);
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

    void LoadExtremeLimitedGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 4, 3f, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.36f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 0;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(50);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadExtremeLimitedGeometry() {
        GeometryProperties geometryProperties = new GeometryProperties();

        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.8f);
        geometryProperties.nth_root_min = 1f;
        geometryProperties.nth_root_max = 3;

        geometryProperties.SetCircleResolution(3);
        //geometryProperties.SetCircleResolution(6);
        geometryProperties.SetMinRadiusRatioForNormalConnection(0.49f);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.0071f);
        geometryProperties.SetLeafSize(Leaf.LeafType.Triangle, 0.4f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleSquare, 1f);
        geometryProperties.SetLeafSize(Leaf.LeafType.ParticleCrossFoil, 1f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.Triangle, 2f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleSquare, 0.5f);
        geometryProperties.SetDisplayedLeavesPerNode(Leaf.LeafType.ParticleCrossFoil, 0f);
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

    //void LoadBush2Growth() {
    //    PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 7, 4, 7f, 15, 0.15f, 0.05f);


    //    GrowthProperties growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(1f);
    //    growthProperties.SetPerceptionAngle(160);
    //    growthProperties.SetMinClearDistanceRatio(0.1f);
    //    growthProperties.SetMaxClearDistanceRatio(0.95f);
    //    growthProperties.SetBranchDensityBegin(0f);
    //    growthProperties.SetBranchDensityEnd(0.8f);
    //    growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
    //    growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
    //    growthProperties.SetTropismsWeights(new Vector3(1, 0.3f, 1)); //adjusted by hand
    //    growthProperties.UpTropismsDampRatio = 0.36f;
    //    growthProperties.UpTropismsWhenDamped = 0.3f;
    //    growthProperties.SetHangingBranchesIntensity(0);

    //    growthProperties.UpTropismWeight_min = 0;
    //    growthProperties.UpTropismWeight_max = 5;
    //    growthProperties.UpTropismWeightRatio = 0.2f;

    //    growthProperties.StemLength = 0f;
    //    growthProperties.StemAngleRange = 2;
    //    growthProperties.CrownStemLengthRatio = 0f;

    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetLeavesPerNode(10);
    //    growthProperties.SetAttractionPoints(attractionPoints);
    //    growthProperties.SetIterations(30);


    //    grower = new SpaceColonization(growthProperties, this);
    //}

    void InitializeUI() {
        GameObject.Find("Initial Stem Length Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().StemLength);
        GameObject.Find("Stem Thickness Slider").GetComponent<Slider>().SetValueWithoutNotify(tree.GetGeometryProperties().StemThickness);

        GameObject.Find("Crown Stem Length Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().CrownStemLengthRatio);


        GameObject.Find("Age Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetIterations());

        GameObject.Find("Crown Width Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetAttractionPoints().GetRadius_x());
        GameObject.Find("Crown Height Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetAttractionPoints().GetRadius_y());
        GameObject.Find("Crown Depth Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetAttractionPoints().GetRadius_z());
        GameObject.Find("Crown Top Cutoff Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetAttractionPoints().GetCutoffRatio_top());
        GameObject.Find("Crown Bottom Cutoff Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetAttractionPoints().GetCutoffRatio_bottom());

        //GameObject.Find("Stem / Crown Ratio Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetClearDistanceBegin_clearDistanceEnd_Ratio());
        GameObject.Find("Branch Density Begin Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetBranchDensityBegin());
        GameObject.Find("Branch Density End Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetBranchDensityEnd());

        //GameObject.Find("Hanging Branches Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.GetHangingBranchesIntensity());

        //GameObject.Find("Grow Towards Light Slider").GetComponent<Slider>().SetValueWithoutNotify(growthProperties.UpTropismWeightRatio);


        GameObject.Find("Stem Color Dropdown").GetComponent<BasicsController>().Initialize_StemColors(tree.GetGeometryProperties().StemColorStrings);
        GameObject.Find("Stem Color Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(tree.GetGeometryProperties().CurrentStemColorStringsIndex);
        GameObject.Find("Stem Color Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Leaf Color Dropdown").GetComponent<BasicsController>().Initialize_LeafColors(tree.GetGeometryProperties().LeafColorStrings);
        GameObject.Find("Leaf Color Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(tree.GetGeometryProperties().CurrentLeafColorStringsIndex);
        GameObject.Find("Leaf Color Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Leaf Type Dropdown").GetComponent<BasicsController>().Initialize_LeafTypes(tree.GetGeometryProperties().LeafTypeStrings);
        GameObject.Find("Leaf Type Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(tree.GetGeometryProperties().CurrentLeafTypeStringsIndex);
        GameObject.Find("Leaf Type Dropdown").GetComponent<Dropdown>().RefreshShownValue();

        GameObject.Find("Foliage Density Slider").GetComponent<Slider>().SetValueWithoutNotify(tree.GetGeometryProperties().GetDisplayedLeavesPerNode());
        GameObject.Find("Foliage Lobe Size Slider").GetComponent<Slider>().SetValueWithoutNotify(tree.GetGeometryProperties().GetLeafSize());

        GameObject.Find("Circle Resolution Slider").GetComponent<Slider>().SetValueWithoutNotify(tree.GetGeometryProperties().GetCircleResolution());


        // SPACE COLONIZATION PARAMETERS
        //GameObject.Find("Density Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetAttractionPoints().GetDensity());
        //GameObject.Find("Clear Distance Begin Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetClearDistanceBegin());
        //GameObject.Find("Clear Distance End Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetClearDistanceEnd());
        //GameObject.Find("Influence Distance Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetInfluenceDistance());
        //GameObject.Find("Growth Distance Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetGrowthDistance());
        //GameObject.Find("Perception Angle Slider").GetComponent<Slider>().SetValueWithoutNotify(grower.GetGrowthProperties().GetPerceptionAngle());
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

        GameObject.Find("Vertices Text").GetComponent<Text>().text = vertices.Length + " vertices";
        GameObject.Find("Triangles Text").GetComponent<Text>().text = triangles.Length / 3 + " triangles";
    }

    //#######################################################################################
    //##########                           GROWER LISTENER                         ##########
    //#######################################################################################

    public void OnIterationFinished() {
        recalculateMesh = true;
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
    public List<Vector3> GetPointCloud() {
        pointCloudReady = false;
        return grower.GetGrowthProperties().GetAttractionPoints().Backup;
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
        return grower.GetGrowthProperties().GetAttractionPoints().GetCenter() + new Vector3(0, grower.GetGrowthProperties().StemLength / 2, 0);
        //}
    }

    public float GetLookAtTop() {
        return grower.GetGrowthProperties().GetAttractionPoints().GetHighestPoint();
    }

    public float GetDistanceToAttractionPoints() {
        if (cameraMode == CameraMode.Tree) { //normal case
            return -1;
        } else { //case when attraction points are beeing modified
            return grower.GetGrowthProperties().GetAttractionPoints().GetHeight() / 4
                + grower.GetGrowthProperties().GetAttractionPoints().GetDepth() / 4
                + grower.GetGrowthProperties().GetAttractionPoints().GetWidth() / 4
                + grower.GetGrowthProperties().StemLength / 4;
        }
    }

    //#######################################################################################
    //##########                             CROWN SHAPE                           ##########
    //#######################################################################################

    public void OnLength(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        grower.GetGrowthProperties().StemLength = value;
        pointCloudReady = true; // ---// this is note precise, but a new PointCloud will follow soon---

        grower.RegrowStem(tree);
    }

    public void OnThickness(float value) {
        tree.GetGeometryProperties().StemThickness = value;

        tree.StemRoot.RecalculateRadii();

        recalculateMesh = true;
    }

    public void OnCrownStemLength(float value) {
        grower.Stop();

        grower.GetGrowthProperties().CrownStemLengthRatio = value;
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }


    private void UpdateTropisms() {
        // the smaller growthProperties.UpTropismRemovalRatio, the later the condition hits
        if (grower.GetGrowthProperties().GetAttractionPoints().GetHeight() < grower.GetGrowthProperties().UpTropismsDampRatio * (grower.GetGrowthProperties().GetAttractionPoints().GetWidth() + grower.GetGrowthProperties().GetAttractionPoints().GetDepth())) {
            //growthProperties.SetTropismsWeights(new Vector3(growthProperties.GetTropismsWeights().x, 0, growthProperties.GetTropismsWeights().z));
            grower.GetGrowthProperties().SetTropismsWeights(
                new Vector3(1,
                            //(float) (grower.GetGrowthProperties().UpTropismsWhenDamped
                            //    * grower.GetGrowthProperties().UpTropismsDampRatio * (grower.GetGrowthProperties().GetAttractionPoints().GetRadius_x() + grower.GetGrowthProperties().GetAttractionPoints().GetRadius_z() / grower.GetGrowthProperties().GetAttractionPoints().GetRadius_y())), //this is about 1 when the border is crossed, and decreases as the y radius gets smaller
                            0,
                            1)
                );
            debug("damped tropisms");
        } else {
            grower.GetGrowthProperties().SetTropismsWeights(new Vector3(1, 1, 1));
            debug("undamped tropisms");
        }
    }

    public void OnCrownWidth(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        grower.GetGrowthProperties().GetAttractionPoints().UpdateRadius_x(value);
        UpdateTropisms();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownHeight(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        grower.GetGrowthProperties().GetAttractionPoints().UpdateRadius_y(value);
        UpdateTropisms();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownDepth(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        grower.GetGrowthProperties().GetAttractionPoints().UpdateRadius_z(value);
        UpdateTropisms();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownTopCutoff(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        grower.GetGrowthProperties().GetAttractionPoints().UpdateCutoffRatio_top(value);
        UpdateTropisms();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownBottomCutoff(float value) {
        cameraMode = CameraMode.AttractionPoints;

        grower.Stop();

        grower.GetGrowthProperties().GetAttractionPoints().UpdateCutoffRatio_bottom(value);
        UpdateTropisms();
        pointCloudReady = true;

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnCrownShapeDone() {
        cameraMode = CameraMode.Tree;
    }




    public void OnClearDistanceBegin_clearDistanceEnd_Ratio(float value) {
        grower.Stop();

        grower.GetGrowthProperties().SetClearDistanceBegin_clearDistanceEnd_Ratio(value);
        grower.GetGrowthProperties().GetAttractionPoints().Reset();

        tree.Reset();

        grower.Grow(tree);
    }


    public void OnBranchDensityBegin(float value) {
        grower.Stop();

        grower.GetGrowthProperties().SetBranchDensityBegin(value);
        grower.GetGrowthProperties().GetAttractionPoints().Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnBranchDensityEnd(float value) {
        grower.Stop();

        grower.GetGrowthProperties().SetBranchDensityEnd(value);
        grower.GetGrowthProperties().GetAttractionPoints().Reset();

        tree.Reset();

        grower.Grow(tree);
    }



    // SPACE COLONIZATION

    //public void OnDensity(float value) {
    //    cameraMode = CameraMode.AttractionPoints;

    //    grower.Stop();

    //    grower.GetGrowthProperties().GetAttractionPoints().UpdateDensity(value);
    //    pointCloudReady = true;

    //    tree.Reset();

    //    grower.Grow(tree);
    //}

    //public void OnClearDistanceBegin(float value) {
    //    grower.Stop();

    //    grower.GetGrowthProperties().SetClearDistanceBegin(value);
    //    grower.GetGrowthProperties().GetAttractionPoints().Reset();

    //    tree.Reset();

    //    grower.Grow(tree);
    //}

    //public void OnClearDistanceEnd(float value) {
    //    grower.Stop();

    //    grower.GetGrowthProperties().SetClearDistanceEnd(value);
    //    grower.GetGrowthProperties().GetAttractionPoints().Reset();

    //    tree.Reset();

    //    grower.Grow(tree);
    //}

    //public void OnInfluenceDistance(float value) {
    //    grower.Stop();

    //    grower.GetGrowthProperties().SetInfluenceDistance(value);
    //    grower.GetGrowthProperties().GetAttractionPoints().Reset();

    //    tree.Reset();

    //    grower.Grow(tree);
    //}

    //public void OnGrowthDistance(float value) {
    //    grower.Stop();

    //    grower.GetGrowthProperties().SetGrowthDistance(value);
    //    grower.GetGrowthProperties().GetAttractionPoints().Reset();

    //    tree.Reset();

    //    grower.Grow(tree);
    //}

    //public void OnPerceptionAngle(float value) {
    //    grower.Stop();

    //    grower.GetGrowthProperties().SetPerceptionAngle(value);
    //    grower.GetGrowthProperties().GetAttractionPoints().Reset();

    //    tree.Reset();

    //    grower.Grow(tree);
    //}

    //#######################################################################################
    //##########                           STEM AND LEAVES                         ##########
    //#######################################################################################

    public void OnLeafType(int value) {
        tree.GetGeometryProperties().UpdateLeafType(value);

        recalculateMesh = true;

        InitializeUI();
    }

    public void OnStemColor(int value) {
        tree.GetGeometryProperties().CurrentStemColorStringsIndex = value;
    }

    public void OnLeafColor(int value) {
        tree.GetGeometryProperties().CurrentLeafColorStringsIndex = value;
    }

    public string GetTexture() {
        return Leaf.LeafTypeToFilename[tree.GetGeometryProperties().GetLeafType()] + "/" + tree.GetGeometryProperties().StemColorStrings[tree.GetGeometryProperties().CurrentStemColorStringsIndex] + "_" + tree.GetGeometryProperties().LeafColorStrings[tree.GetGeometryProperties().CurrentLeafColorStringsIndex];
    }

    public void OnFoliageDensity(float value) {
        tree.GetGeometryProperties().SetDisplayedLeavesPerNode(tree.GetGeometryProperties().GetLeafType(), value);

        recalculateMesh = true;
    }

    public void OnFoliageLobeSize(float value) {
        tree.GetGeometryProperties().SetLeafSize(tree.GetGeometryProperties().GetLeafType(), value);

        recalculateMesh = true;
    }

    public void OnCircleResolution(int value) {
        tree.GetGeometryProperties().SetCircleResolution(value);

        recalculateMesh = true;
    }

    //#######################################################################################
    //##########                                  MISC                             ##########
    //#######################################################################################

    //public void OnGrowTowardsLight(float value) {
    //    grower.Stop();

    //    grower.GetGrowthProperties().UpTropismWeightRatio = value;
    //    grower.GetGrowthProperties().GetAttractionPoints().Reset();

    //    tree.Reset();

    //    grower.Grow(tree);
    //}

    public void OnAge(int value) {
        grower.Stop();

        grower.GetGrowthProperties().SetIterations(value);
        grower.GetGrowthProperties().GetAttractionPoints().Reset();

        tree.Reset();

        grower.Grow(tree);
    }

    public void OnResetToDefaults() {

        grower.Stop();

        LoadDefaultGrowth();
        LoadDefaultGeometry();
        InitializeUI();

        pointCloudReady = true;

        grower.Grow(tree);
    }

    public void OnNewSeed() {
        grower.Stop();

        grower.GetGrowthProperties().GetAttractionPoints().NewSeed();

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





    void LoadBaobabGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 5, 3, 5, 15, 0.5f, 0.0f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.5f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 6;
        growthProperties.StemAngleRange = 2;
        growthProperties.CrownStemLengthRatio = 0.2f;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(30);


        grower = new SpaceColonization(growthProperties, this);
    }


    void LoadCrownStemLengthTestGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 5, 3, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0.5f;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;
        growthProperties.CrownStemLengthRatio = 1;
        growthProperties.StemAngleRange = 5;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(30);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadNoTropismsDampGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 6, 3, 6, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        growthProperties.SetPerceptionAngle(160);
        growthProperties.SetMinClearDistanceRatio(0.1f);
        growthProperties.SetMaxClearDistanceRatio(0.925f);
        growthProperties.SetBranchDensityBegin(0f);
        growthProperties.SetBranchDensityEnd(0.8f);
        growthProperties.SetClearDistanceBegin_clearDistanceEnd_Ratio(0.5f);
        growthProperties.SetTropisms(new Vector3(0f, 1f, 0));
        growthProperties.SetTropismsWeights(new Vector3(1, 1f, 1));
        growthProperties.UpTropismsDampRatio = 0;
        growthProperties.UpTropismsWhenDamped = 0.3f;

        growthProperties.UpTropismWeight_min = 0;
        growthProperties.UpTropismWeight_max = 5;
        growthProperties.UpTropismWeightRatio = 0.2f;

        growthProperties.StemLength = 2;
        growthProperties.StemAngleRange = 5;

        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetLeavesPerNode(10);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(30);


        grower = new SpaceColonization(growthProperties, this);
    }

    void LoadNNATestGrowth() {
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 6, 6, 6, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
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
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 5, 3.5f, 15, 0.15f, 0.05f);


        GrowthProperties growthProperties = new GrowthProperties();
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
        PseudoEllipsoid attractionPoints = new PseudoEllipsoid(new Vector3(0, 0f, 0), 3, 5, 3.5f, 15, 0.5f, 0.0f);


        GrowthProperties growthProperties = new GrowthProperties();
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
}