using System;
using System.Collections.Generic;
using UnityEngine;

public class GrowthProperties {
    public float StemLength { get; set; }
    public float CrownStemLengthRatio { get; set; } //how much of the crown is filled with an initial stem?
    public float StemAngleRange { get; set; }

    private float influenceDistance;
    private float squaredInfluenceDistance;
    private float perceptionAngle;


    //private float clearDistance;
    private float squaredClearDistance;
    private float clearDistance_min;
    private float squaredClearDistance_min;
    private float clearDistance_max;
    private float squaredClearDistance_max;

    private float squaredClearDistance_range;

    private float squaredClearDistance_begin;
    private float squaredClearDistance_end;



    private Vector3 tropisms;
    private Vector3 tropismsWeights;
    private float hangingBranchesIntensity;

    private float growthDistance;

    private PseudoEllipsoid attractionPoints;

    private int iterations;

    public void SetInfluenceDistance(float influenceDistance) {
        this.influenceDistance = influenceDistance;
        this.squaredInfluenceDistance = influenceDistance * influenceDistance;
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



    //only set once
    public void SetClearDistance(float min, float max) {
        this.clearDistance_min = min;
        this.clearDistance_max = max;

        this.squaredClearDistance_min = min * min;
        this.squaredClearDistance_max = max * max;

        this.squaredClearDistance_range = squaredClearDistance_max - squaredClearDistance_min;
    }


    private float branchDensityBegin;
    public void SetBranchDensityBegin(float value) {
        this.branchDensityBegin = value;

        //the density says how much of the squaredClearDistanceRange should be subtracted from the squaredClearDistance_max
        //-> typically the density is low in the beginning
        //-> this should result in a high squaredClearDistance_begin
        //-> therefore density * squaredClearDistanceRange has to be subtracted from the squaredClearDistance_max
        this.squaredClearDistance_begin = squaredClearDistance_max - value * squaredClearDistance_range;
        //this.squaredClearDistance_begin = squaredClearDistance_min + value * squaredClearDistanceRange; //also possible
    }

    public float GetBranchDensityBegin() {
        return branchDensityBegin;
    }

    private float branchDensityEnd;
    public void SetBranchDensityEnd(float value) {
        this.branchDensityEnd = value;

        //the density says how much of the squaredClearDistanceRange should be subtracted from the squaredClearDistance_max
        //-> typically the density is high in the end
        //-> this should result in a low squaredClearDistance_end
        //-> therefore density * squaredClearDistanceRange has to be subtracted from the squaredClearDistance_max
        this.squaredClearDistance_end = squaredClearDistance_max - value * squaredClearDistance_range;
        //this.squaredClearDistance_end = squaredClearDistance_min + value * squaredClearDistanceRange; //also possible
    }

    public float GetBranchDensityEnd() {
        return branchDensityEnd;
    }


    private float MapIteration(int iteration, float begin, float end) {
        float d = end - begin;
        float step = d / (iterations-1);

        return begin + step * iteration;
    }

    //private float ExponentialInterpolation(int iteration) {
    //    float d = squaredClearDistance_begin - squaredClearDistance_end;

    //    return (float)(squaredClearDistance_begin - Math.Exp(MapIteration(iteration, -4, 0)) * d);
    //}

    private float Sigmoid(float x) {
        return (float) (Math.Exp(x) / (1 + Math.Exp(x)));
    }

    // TODO: make independant from class attributes
    private float SigmoidInterpolation(int iteration) {
        float d = squaredClearDistance_begin - squaredClearDistance_end;

        return squaredClearDistance_begin - Sigmoid(MapIteration(iteration, -8, 4)) * d;
    }

    //private float LinearInterpolation(int iteration) {
    //    float d = squaredClearDistance_end - squaredClearDistance_begin;
    //    float step = d / iterations;
    //    return squaredClearDistance_begin + step * (iteration+1);
    //}

    public float GetSquaredClearDistance(int iteration) {
        return SigmoidInterpolation(iteration);
    }

    public float GetSquaredClearDistance() {
        return squaredClearDistance;
    }

    //0..1
    //the biger, the earlier up tropisms get damped
    //when 0.5, tropisms get damped when the height is equal to (width+depth)/2
    public float UpTropismsDampRatio { get; set; }

    public float UpTropismsWhenDamped { get; set; }

    public void SetTropisms(Vector3 tropisms) {
        this.tropisms = tropisms.normalized;
    }

    public Vector3 GetTropisms(int iteration) {
        bool hanging = iteration > (1-hangingBranchesIntensity) * iterations;

        if (hanging) {
            return Util.Hadamard(tropisms, new Vector3(1, -1, 1));
        } else {
            return tropisms;
        }
    }



    // DELETE THIS OR INTEGRATE?
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





    //FIXED?
    public void SetGrowthDistance(float growthDistance) {
        this.growthDistance = growthDistance;
    }

    public float GetGrowthDistance() {
        return growthDistance;
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
}