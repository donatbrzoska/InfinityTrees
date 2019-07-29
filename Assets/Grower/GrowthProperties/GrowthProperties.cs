﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class GrowthProperties : AttractionPointsListener {
    private float influenceDistance; //FREE
    private float perceptionAngle; //SET

    private float clearDistance_begin; //DEPENDS
    private float clearDistance_end; //DEPENDS

    private Vector3 tropismsBackup;
    private Vector3 tropisms;
    private bool hangingBranchesEnabled;
    private float hangingBranchesFromAgeRatio;

    private float growthDistance; //SET

    //private List<Vector3> attractionPointsBackup;
    private AttractionPoints attractionPoints;

    private int iterations;

    GrowthPropertiesListener growthPropertiesListener;

    public void SetGrowthPropertiesListener(GrowthPropertiesListener growthPropertiesListener) {
        this.growthPropertiesListener = growthPropertiesListener;
    }

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
    //public void SetClearDistance(float clearDistance) {
    //    this.clearDistance_begin = clearDistance * clearDistance;
    //    this.clearDistance_end = clearDistance_begin;
    //}

    public void SetClearDistance(float begin, float end) {
        this.clearDistance_begin = begin * begin;
        this.clearDistance_end = end * end;
    }
    //enum ClearDistanceFunction {
    //    linear,
    //    exponential
    //}


    //THIS OR influenceDistance
    //public void SetClearDistance_2(float clearDistance_2) {
    //    this.clearDistance_2 = clearDistance_2 * clearDistance_2;
    //}

    //public float GetClearDistance() {
    //    return clearDistance;
    //}


    private float MapIteration(int iteration, float begin, float end) {
        float d = end - begin;
        float step = d / iterations;

        return begin + step * iteration;
    }

    private float ExponentialInterpolation(int iteration) {
        float d = clearDistance_begin - clearDistance_end;

        return (float)(clearDistance_begin - Math.Exp(MapIteration(iteration, -4, 0)) * d);
    }

    private float Sigmoid(float x) {
        return (float) (Math.Exp(x) / (1 + Math.Exp(x)));
    }

    private float SigmoidInterpolation(int iteration) {
        float d = clearDistance_begin - clearDistance_end;

        return (float)(clearDistance_begin - Sigmoid(MapIteration(iteration, -15, 8)) * d);
    }

    public float GetSquaredClearDistance(int iteration) {

        return SigmoidInterpolation(iteration);

        //float step = d / iterations;
        //return clearDistance_begin + step * iteration;
    }



    public void SetTropisms(Vector3 tropisms, bool temporary=false) {
        this.tropisms = tropisms.normalized;
        if (!temporary) {
            this.tropismsBackup = new Vector3(tropisms.x, tropisms.y, tropisms.z);
        }
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



    public void SetAttractionPoints(AttractionPoints attractionPoints) {
        this.attractionPoints = attractionPoints;

        ////store the attraction points, so you can also decrement the amount of iterations afterwards
        //this.attractionPointsBackup = new List<Vector3>();
        //foreach (Vector3 p in attractionPoints) {
        //    this.attractionPointsBackup.Add(p);
        //}
    }

    public AttractionPoints GetAttractionPoints() {
        return attractionPoints;
    }



    public void SetIterations(int iterations) {
        this.iterations = iterations;
    }

    public void UpdateIterations(int iterations) {
        this.iterations = iterations;

        //TODO: only reset when iterations is less than current iterations
        // -> implement regrow and grow on Tree
        attractionPoints.Reset();
        growthPropertiesListener.OnAgeChanged();
    }

    public int GetIterations() {
        return iterations;
    }




    public void ResetTropisms() {
        this.tropisms.x = tropismsBackup.x;
        this.tropisms.y = tropismsBackup.y;
        this.tropisms.z = tropismsBackup.z;
    }


    //#######################################################################################
    //##########                     INTERFACE IMPLEMENTIATION                     ##########
    //#######################################################################################

    public void OnAttractionPointsChanged() {
        growthPropertiesListener.OnAttractionPointsChanged();
    }
}