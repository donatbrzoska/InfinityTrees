using System;
using UnityEngine;

public class GrowthProperties {
    private float influenceDistance; //FREE
    private float growthDistance; //SET
    private float clearDistance; //DEPENDS
    private Vector3 tropisms;


    private float maxTwigRadiusForLeaves; //DEPENDS on tipRadius?
    private float minLeafSize; //FREE
    private float maxLeafSize; //FREE
    private int leavesPerNode;


    private float maxBranchingAngle; //SET
    //public float minDistanceToExistingNodes; //FREE with min value that DEPENDS on influenceDistance
    private float tipRadius = 0.007f; //SET
    private float nth_root = 1.6f; //smaller values make bigger radii



    //THIS OR clearDistance
    public void SetInfluenceDistance(float influenceDistance) {
        this.influenceDistance = influenceDistance*influenceDistance;
    }

    //public float GetInfluenceDistance() {
    //    return influenceDistance;
    //}

    public float GetSquaredInfluenceDistance() {
        return influenceDistance;
    }

    //FIXED?
    public void SetGrowthDistance(float growthDistance) {
        this.growthDistance = growthDistance;
    }

    public float GetGrowthDistance() {
        return growthDistance;
    }

    //THIS OR influenceDistance
    public void SetClearDistance(float clearDistance) {
        this.clearDistance = clearDistance*clearDistance;
    }

    //public float GetClearDistance() {
    //    return clearDistance;
    //}

    public float GetSquaredClearDistance() {
        return clearDistance;
    }

    public void SetTropisms(Vector3 tropisms) {
        this.tropisms = tropisms.normalized;
    }

    public Vector3 GetTropisms() {
        return tropisms;
    }

    //##############################################################################

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

    public void SetLeavesPerNode(int leavesPerNode) {
        this.leavesPerNode = leavesPerNode;
    }

    public int GetLeavesPerNode() {
        return leavesPerNode;
    }

    public float GetLeafSize() {
        return UnityEngine.Random.Range(minLeafSize, maxLeafSize);
    }

    public Vector3 GetLeafRotationAxis() {
        float x = UnityEngine.Random.Range(-1, 1);
        float y = UnityEngine.Random.Range(-1, 1);
        float z = UnityEngine.Random.Range(-1, 1);
        return new Vector3(x, y, z);
    }

    public float GetLeafRotationAngle() {
        return UnityEngine.Random.Range(0, 360);
    }

    //##############################################################################

    public void SetMaxBranchingAngle(float maxBranchingAngle) {
        this.maxBranchingAngle = maxBranchingAngle;
    }

    public float GetMaxBranchingAngle() {
        return maxBranchingAngle;
    }

    //TODO find good value, make adjustable with min value?
    public float GetMinDistanceToExistingNodes() {
        return 0.5f * growthDistance;
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
}