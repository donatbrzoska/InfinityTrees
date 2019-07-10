using System;
using System.Collections.Generic;
using UnityEngine;

public class GrowthProperties {
    private float influenceDistance; //FREE
    private float perceptionAngle; //SET

    private float clearDistance; //DEPENDS

    private Vector3 tropismsBackup;
    private Vector3 tropisms;
    private bool hangingBranchesEnabled;
    private float hangingBranchesFromAgeRatio;

    private float growthDistance; //SET

    private List<Vector3> attractionPointsBackup;
    private List<Vector3> attractionPoints;


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



    public void SetPerceptionAngle(float perceptionAngle) {
        this.perceptionAngle = perceptionAngle;
    }

    public float GetPerceptionAngle() {
        return perceptionAngle;
    }



    //THIS OR influenceDistance
    public void SetClearDistance(float clearDistance) {
        this.clearDistance = clearDistance * clearDistance;
    }

    //public float GetClearDistance() {
    //    return clearDistance;
    //}

    public float GetSquaredClearDistance() {
        return clearDistance;
    }



    public void SetTropisms(Vector3 tropisms) {
        this.tropisms = tropisms.normalized;
        this.tropismsBackup = new Vector3(tropisms.x, tropisms.y, tropisms.z);
    }

    public Vector3 GetTropisms() {
        return tropisms;
    }



    public void SetHangingBranchesEnabled(bool hangingBranchesEnabled) {
        this.hangingBranchesEnabled = hangingBranchesEnabled;
    }

    public bool GetHangingBranchesEnabled() {
        return hangingBranchesEnabled;
    }



    public void SetHangingBranchesFromAgeRatio(float hangingBranchesFromAgeRatio) {
        this.hangingBranchesFromAgeRatio = hangingBranchesFromAgeRatio;
    }

    public float GetHangingBranchesFromAgeRatio() {
        return hangingBranchesFromAgeRatio;
    }



    //FIXED?
    public void SetGrowthDistance(float growthDistance) {
        this.growthDistance = growthDistance;
    }

    public float GetGrowthDistance() {
        return growthDistance;
    }



    public void SetAttractionPoints(List<Vector3> attractionPoints) {
        this.attractionPoints = attractionPoints;

        this.attractionPointsBackup = new List<Vector3>();
        foreach (Vector3 p in attractionPoints) {
            this.attractionPointsBackup.Add(p);
        }
    }

    public List<Vector3> GetAttractionPoints() {
        return attractionPoints;
    }



    public void Reset() {
        this.tropisms.x = tropismsBackup.x;
        this.tropisms.y = tropismsBackup.y;
        this.tropisms.z = tropismsBackup.z;

        foreach (Vector3 p in attractionPointsBackup) {
            this.attractionPoints.Add(p);
        }
    }
}