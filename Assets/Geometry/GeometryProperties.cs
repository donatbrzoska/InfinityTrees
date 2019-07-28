using System;
using UnityEngine;

public class GeometryProperties {

    System.Random random = new System.Random();

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
    private float minLeafSize; //FREE
    private float maxLeafSize; //FREE
    private LeafType leafType;
    private int leavesPerNode; //TODO: move to growthProperties
    private bool leavesEnabled;



    //FIXED?
    public void SetMaxTwigRadiusForLeaves(float maxTwigRadiusForLeaves) {
        this.maxTwigRadiusForLeaves = maxTwigRadiusForLeaves;
    }

    public float GetMaxTwigRadiusForLeaves() {
        return maxTwigRadiusForLeaves;
    }



    public void SetMinLeafSize(float minLeafSize) {
        this.minLeafSize = minLeafSize;
    }

    public void SetMaxLeafSize(float maxLeafSize) {
        this.maxLeafSize = maxLeafSize;
    }

    public float GetLeafSize() {
        return Util.RandomInRange(minLeafSize, maxLeafSize);
    }



    public void SetLeavesPerNode(int leavesPerNode) {
        this.leavesPerNode = leavesPerNode;
    }

    public int GetLeavesPerNode() {
        return leavesPerNode;
    }



    public void SetLeafType(LeafType leafType) {
        this.leafType = leafType;
    }

    public LeafType GetLeafType() {
        return leafType;
    }




    public Vector3 GetRandomLeafRotationAxis() {
		//float x = UnityEngine.Random.Range(-1, 1);
		//float y = UnityEngine.Random.Range(-1, 1);
		//float z = UnityEngine.Random.Range(-1, 1);
		float x = (float)(random.NextDouble() * 2) - 1;
		float y = (float)(random.NextDouble() * 2) - 1;
		float z = (float)(random.NextDouble() * 2) - 1;
		return new Vector3(x, y, z);
    }

    public float GetRandomLeafRotationAngle() {
		//return UnityEngine.Random.Range(0, 360);
		return (float) random.NextDouble() * 360;
	}



    public void SetLeavesEnabled(bool leavesEnabled) {
        this.leavesEnabled = leavesEnabled;
    }

    public bool GetLeavesEnabled() {
        return leavesEnabled;
    }

}
