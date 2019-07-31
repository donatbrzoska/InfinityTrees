using System.Collections.Generic;
using UnityEngine;

public class GeometryProperties {

    //############################
    //########## TWIGS  ##########
    //############################

    private float tipRadius; //SET //0.007f
    private float nth_root; //smaller values make bigger radii   //1.6f

    private int circleResolution;
    //private int curveResolution = -1;

    private float minRadiusRatioForNormalConnection;


    public void SetTipRadius(float tipRadius) {
        this.tipRadius = tipRadius;
    }

    public float GetTipRadius() {
        return tipRadius;
    }



    public void SetNthRoot(float nth_root) {
        this.nth_root = nth_root;
    }

    public float GetNthRoot() {
        return nth_root;
    }



    public void SetCircleResolution(int circleResolution) {
        this.circleResolution = circleResolution;
    }

    public int GetCircleResolution() {
        return circleResolution;
    }



    public void SetMinRadiusRatioForNormalConnection(float minRadiusRatioForNormalConnection) {
        this.minRadiusRatioForNormalConnection = minRadiusRatioForNormalConnection;
    }

    public float GetMinRadiusRatioForNormalConnection() {
        return minRadiusRatioForNormalConnection;
    }

    //############################
    //########## LEAVES ##########
    //############################

    private float maxTwigRadiusForLeaves; //DEPENDS on tipRadius?
    private float leafSize; //FREE
    private float leafSizeStdDev;
    private Leaf.LeafType leafType;
    private float displayedLeavesPerNode; //TODO: move to growthProperties
    private bool leavesEnabled;



    //FIXED?
    public void SetMaxTwigRadiusForLeaves(float maxTwigRadiusForLeaves) {
        this.maxTwigRadiusForLeaves = maxTwigRadiusForLeaves;
    }

    public float GetMaxTwigRadiusForLeaves() {
        return maxTwigRadiusForLeaves;
    }



    public void SetLeafSize(float leafSize) {
        this.leafSize = leafSize;
        this.leafSizeStdDev = 0.2f * leafSize;
    }

    public float GetLeafSize() {
        return Util.RandomWithStdDev(leafSize, leafSizeStdDev);
    }



    public void SetLeavesEnabled(bool leavesEnabled) {
        this.leavesEnabled = leavesEnabled;
    }

    public bool GetLeavesEnabled() {
        return leavesEnabled;
    }



    public void SetDisplayedLeavesPerNode(float displayedLeavesPerNode) {
        this.displayedLeavesPerNode = displayedLeavesPerNode;
    }

    public float GetDisplayedLeavesPerNode() {
        return displayedLeavesPerNode;
    }



    public List<string> LeafTypeStrings { get; set; }
    public int CurrentLeafTypeStringsIndex { get; set; }

    public void SetLeafType(Leaf.LeafType leafType) {
        this.leafType = leafType;
    }

    public void UpdateLeafType(int leafTypeStringsIndex) {
        this.CurrentLeafTypeStringsIndex = leafTypeStringsIndex;
        this.leafType = Leaf.LeafTypeStringToLeafType[LeafTypeStrings[leafTypeStringsIndex]];
    }

    public Leaf.LeafType GetLeafType() {
        return leafType;
    }


    //##############################
    //########## RENDERER ##########
    //##############################

    public List<string> StemColorStrings { get; set; }
    public int CurrentStemColorStringsIndex { get; set; }

    public List<string> LeafColorStrings { get; set; }
    public int CurrentLeafColorStringsIndex { get; set; }

}
