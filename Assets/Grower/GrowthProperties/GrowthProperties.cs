using System;
using System.Collections.Generic;
using UnityEngine;

public class GrowthProperties {
    public float StemLength { get; set; }

    private float influenceDistance; //FREE
    private float squaredInfluenceDistance; //FREE
    private float perceptionAngle; //SET

    private float clearDistance;
    private float squaredClearDistance;
    private float clearDistance_begin; //DEPENDS
    private float squaredClearDistance_begin; //DEPENDS
    private float clearDistance_end; //DEPENDS
    private float squaredClearDistance_end; //DEPENDS

    private Vector3 tropismsBackup;
    private Vector3 tropisms;
    private Vector3 tropismsWeights;
    //private bool hangingBranchesEnabled;
    private float hangingBranchesIntensity;

    private float growthDistance; //SET

    private int leavesPerNode;

    //private List<Vector3> attractionPointsBackup;
    private PseudoEllipsoid attractionPoints;

    private int iterations;

    //GrowthPropertiesListener growthPropertiesListener;

    //public void SetGrowthPropertiesListener(GrowthPropertiesListener growthPropertiesListener) {
    //    this.growthPropertiesListener = growthPropertiesListener;
    //}

    //THIS OR clearDistance
    public void SetInfluenceDistance(float influenceDistance) {
        this.influenceDistance = influenceDistance;
        this.squaredInfluenceDistance = influenceDistance * influenceDistance;

        minClearDistance = influenceDistance * minClearDistanceRatio; //refresh
        maxClearDistance = influenceDistance * maxClearDistanceRatio; 

        SetBranchDensityBegin(branchDensityBegin); //refresh
        SetBranchDensityEnd(branchDensityEnd);
    }

    public float GetInfluenceDistance() {
        return influenceDistance;
    }

    public float GetSquaredInfluenceDistance() {
        return squaredInfluenceDistance;
    }







    public void SetPerceptionAngle(float perceptionAngle) {
        this.perceptionAngle = perceptionAngle;
    }

    public float GetPerceptionAngle() {
        return perceptionAngle;
    }



    public void SetClearDistance(float clearDistance) {
        this.clearDistance = clearDistance;
        this.squaredClearDistance = clearDistance* clearDistance;
    }

    public void SetClearDistance(float begin, float end) {
        //if (begin < minClearDistance | end < minClearDistance) {
        //    throw new Exception("Keep the minClearDistance in mind!");
        //}

        this.clearDistance_begin = begin * begin;
        this.clearDistance_end = end * end;
    }

    public void SetClearDistanceBegin(float begin) {
        //Debug.Log("clear distance begin: " + begin);
        //if (begin < minClearDistance) {
        //    throw new Exception("Keep the minClearDistance in mind!");
        //}
        this.clearDistance_begin = begin;
        this.squaredClearDistance_begin = begin * begin;
    }

    public float GetClearDistanceBegin() {
        return clearDistance_begin;
    }

    public void SetClearDistanceEnd(float end) {
        //Debug.Log("clear distance end: " + end);
        //if (end < minClearDistance) {
        //    throw new Exception("Keep the minClearDistance in mind!");
        //}
        this.clearDistance_end = end;
        this.squaredClearDistance_end = end * end;
    }

    public float GetClearDistanceEnd() {
        return clearDistance_end;
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

    // represents how much of the tree is begin
    // 1-stemCrownRatio is the end part
    private float clearDistanceBegin_clearDistanceEnd_Ratio;

    public void SetClearDistanceBegin_clearDistanceEnd_Ratio(float value) {
        this.clearDistanceBegin_clearDistanceEnd_Ratio = value;
    }

    public float GetClearDistanceBegin_clearDistanceEnd_Ratio() {
        return clearDistanceBegin_clearDistanceEnd_Ratio;
    }

    //this is based on a sigmoid funcion and basically returns the lower bound of the looked at part of the sigmoid function within a given range
    private float GetSigmoidStartValue(float range) {
        //return -4 - clearDistanceBegin_clearDistanceEnd_Ratio * range; 
        return - 8 - clearDistanceBegin_clearDistanceEnd_Ratio * range; // earlier version
    }

    //this is based on a sigmoid funcion and basically returns the lower bound of the looked at part of the sigmoid function within a given range
    private float GetSigmoidEndValue(float range) {
        //return 4 + (1 - clearDistanceBegin_clearDistanceEnd_Ratio) * range;
        return 4 + (1 - clearDistanceBegin_clearDistanceEnd_Ratio) * range; // earlier version
    }


    //InfluenceDistance -> max/minClearDistance -> branchDensityBegin/End


    private float minClearDistanceRatio; //0..1
    private float minClearDistance;
    public void SetMinClearDistanceRatio(float value) {
        minClearDistanceRatio = value;
        minClearDistance = influenceDistance * minClearDistanceRatio;

        SetBranchDensityBegin(branchDensityBegin); //refresh
        SetBranchDensityBegin(branchDensityEnd);
    }

    //private float minDelta_ClearDistance_to_InfluenceDistance = 0.1f;
    private float maxClearDistanceRatio; //0..1
    private float maxClearDistance;// = 0.075f;
    public void SetMaxClearDistanceRatio(float value) {
        maxClearDistanceRatio = value;
        maxClearDistance = influenceDistance * maxClearDistanceRatio;

        SetBranchDensityBegin(branchDensityBegin); //refresh
        SetBranchDensityBegin(branchDensityEnd);
    }

    private float branchDensityBegin;
    public void SetBranchDensityBegin(float value) {
        //Debug.Log("branch density begin: " + value);
        this.branchDensityBegin = value;

        float range = maxClearDistance - minClearDistance;
        SetClearDistanceBegin(minClearDistance + (1-value) * range);
    }

    public float GetBranchDensityBegin() {
        return branchDensityBegin;
    }

    private float branchDensityEnd;
    public void SetBranchDensityEnd(float value) {
        //Debug.Log("branch density end: " + value);
        this.branchDensityEnd = value;

        float range = maxClearDistance - minClearDistance;
        SetClearDistanceEnd(minClearDistance + (1-value) * range);
    }

    public float GetBranchDensityEnd() {
        return branchDensityEnd;
    }


    private float MapIteration(int iteration, float begin, float end) {
        float d = end - begin;
        float step = d / iterations;

        return begin + step * iteration;
    }

    private float ExponentialInterpolation(int iteration) {
        float d = squaredClearDistance_begin - squaredClearDistance_end;

        return (float)(squaredClearDistance_begin - Math.Exp(MapIteration(iteration, -4, 0)) * d);
    }

    private float Sigmoid(float x) {
        return (float) (Math.Exp(x) / (1 + Math.Exp(x)));
    }

    // TODO: make independant from class attributes
    private float SigmoidInterpolation(int iteration) {
        float d = squaredClearDistance_begin - squaredClearDistance_end;
        //float d = squaredClearDistance_end - squaredClearDistance_begin;

        float range = 0;
        return (float)(squaredClearDistance_begin - Sigmoid(MapIteration(iteration, GetSigmoidStartValue(range), GetSigmoidEndValue(range))) * d);
        //return (float)(squaredClearDistance_begin - Sigmoid(MapIteration(iteration, -4, 4)) * d);
    }

    private float LinearInterpolation(int iteration) {
        float d = squaredClearDistance_end - squaredClearDistance_begin;
        float step = d / iterations;
        return squaredClearDistance_begin + step * (iteration+1);
    }

    public float GetSquaredClearDistance(int iteration) {

        return SigmoidInterpolation(iteration);

        //return LinearInterpolation(iteration);
    }

    public float GetSquaredClearDistance() {
        return squaredClearDistance;
    }

    //0..1
    //the biger, the earlier up tropisms get damped
    //when 0.5, tropisms get damped when the height is equal to (width+depth)/2
    public float UpTropismsDampRatio { get; set; }

    public float UpTropismsWhenDamped { get; set; }

    public void SetTropisms(Vector3 tropisms, bool temporary=false) {
        this.tropisms = tropisms.normalized;
        if (!temporary) {
            this.tropismsBackup = new Vector3(tropisms.x, tropisms.y, tropisms.z);
        }
    }

    public Vector3 GetTropisms(int iteration) {
        bool hanging = iteration > (1-hangingBranchesIntensity) * iterations;

        if (hanging) {
            return Util.Hadamard(tropisms, new Vector3(1, -1, 1));
        } else {
            return tropisms;
        }
    }



    public float UpTropismWeight_min { private get; set; }
    public float UpTropismWeight_max { private get; set; }
    public float UpTropismWeightRatio { //0..1
        set {
            this.tropismsWeights.y = value * (UpTropismWeight_max - UpTropismWeight_min);
        }
        get {
            //this.tropismsWeights.y = value * (UpTropismWeight_max - UpTropismWeight_min);
            //this.tropismsWeights.y / (UpTropismWeight_max - UpTropismWeight_min) = value;
            return this.tropismsWeights.y / (UpTropismWeight_max - UpTropismWeight_min);
        }
    }

    public void SetTropismsWeights(Vector3 value) {
        this.tropismsWeights = value;
    }

    public Vector3 GetTropismsWeights() {
        return tropismsWeights;
    }




    //public void SetHangingBranchesEnabled(bool hangingBranchesEnabled) {
    //    this.hangingBranchesEnabled = hangingBranchesEnabled;
    //}

    //public bool GetHangingBranchesEnabled() {
    //    return hangingBranchesEnabled;
    //}



    public void SetHangingBranchesIntensity(float value) {
        this.hangingBranchesIntensity = value;
    }

    public float GetHangingBranchesIntensity() {
        return hangingBranchesIntensity;
    }






    //FIXED?
    public void SetGrowthDistance(float growthDistance) {
        this.growthDistance = growthDistance;
    }

    public float GetGrowthDistance() {
        return growthDistance;
    }



    public void SetLeavesPerNode(int leavesPerNode) {
        this.leavesPerNode = leavesPerNode;
    }

    public int GetLeavesPerNode() {
        return leavesPerNode;
    }



    public void SetAttractionPoints(PseudoEllipsoid attractionPoints) {
        this.attractionPoints = attractionPoints;
    }

    public PseudoEllipsoid GetAttractionPoints() {
        return attractionPoints;
    }



    public void SetIterations(int iterations) {
        this.iterations = iterations;
    }

    public int GetIterations() {
        return iterations;
    }




    public void ResetTropisms() {
        this.tropisms.x = tropismsBackup.x;
        this.tropisms.y = tropismsBackup.y;
        this.tropisms.z = tropismsBackup.z;
    }
}