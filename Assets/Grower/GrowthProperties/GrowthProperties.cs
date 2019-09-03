using System;
using System.Collections.Generic;
using UnityEngine;

public class GrowthProperties {
    public float StemLength { get; set; }
    public float CrownStemLengthRatio { get; set; } //how much of the crown is filled with an initial stem?
    public float StemAngleRange { get; set; }

    private float influenceDistance;
    private float perceptionAngle;


    private Vector3 tropisms;
    public Vector3 TropismsWeights { get; private set; }

    private float growthDistance;

    private PseudoEllipsoid attractionPoints;

    private int iterations;

    public float GetInfluenceDistance() {
        // more gnarlyness means less influence distance
        return GnarlyBranches_min_di + (1-GnarlyBranchesRatio) * (GnarlyBranches_max_di - GnarlyBranches_min_di);
    }

    public float GetSquaredInfluenceDistance() {
        return GetInfluenceDistance() * GetInfluenceDistance();
    }



    public float GnarlyBranches_min_di { private get; set; } //what is the smallest value for the influence distance
    public float GnarlyBranches_max_di { private get; set; } //what is the biggest value for the influence distance
    public float GnarlyBranches_min_dc_min { private get; set; } //what is the smallest value for the clear distance when the gnarlyness is at its minimum
    public float GnarlyBranches_min_dc_max { private get; set; } //what is the biggest value for the clear distance when the gnarlyness is at its minimum
    public float GnarlyBranches_max_dc_min { private get; set; } //what is the smallest value for the clear distance when the gnarlyness is at its minimum
    public float GnarlyBranches_max_dc_max { private get; set; } //what is the biggest value for the clear distance when the gnarlyness is at its minimum
    public float GnarlyBranches_min_pointCloudDensity { private get; set; }
    public float GnarlyBranches_max_pointCloudDensity { private get; set; }

    private float gnarlyBranchesRatio;
    public float GnarlyBranchesRatio {
        get {
            return gnarlyBranchesRatio;
        }
        set {
            gnarlyBranchesRatio = value;

            //more gnarlyness needs more attraction points
            attractionPoints.UpdateDensity(GnarlyBranches_min_pointCloudDensity + value * (GnarlyBranches_max_pointCloudDensity - GnarlyBranches_min_pointCloudDensity));
        }
    } //0..1




    public void SetPerceptionAngle(float perceptionAngle) {
        this.perceptionAngle = perceptionAngle;
    }

    public float GetPerceptionAngle() {
        return perceptionAngle;
    }



    private float branchDensityBegin;
    public void SetBranchDensityBegin(float value) {
        this.branchDensityBegin = value;
    }

    public float GetBranchDensityBegin() {
        return branchDensityBegin;
    }

    private float branchDensityEnd;
    public void SetBranchDensityEnd(float value) {
        this.branchDensityEnd = value;
    }

    public float GetBranchDensityEnd() {
        return branchDensityEnd;
    }


    private float MapIteration(int iteration, float begin, float end) {
        float d = end - begin;
        float step = d / (iterations - 1);

        return begin + step * iteration;
    }

    //private float ExponentialInterpolation(int iteration) {
    //    float d = squaredClearDistance_begin - squaredClearDistance_end;

    //    return (float)(squaredClearDistance_begin - Math.Exp(MapIteration(iteration, -4, 0)) * d);
    //}

    private float Sigmoid(float x) {
        return (float)(Math.Exp(x) / (1 + Math.Exp(x)));
    }

    // TODO: make independant from class attributes
    private float SigmoidInterpolation(float from, float to, int iteration) {
        float d = from - to;

        return from - Sigmoid(MapIteration(iteration, -12, 6)) * d;
    }

    //private float LinearInterpolation(int iteration) {
    //    float d = squaredClearDistance_end - squaredClearDistance_begin;
    //    float step = d / iterations;
    //    return squaredClearDistance_begin + step * (iteration+1);
    //}


    public float GetSquaredClearDistance(int iteration) {
        // bigger values for the Gnarlyness make a smaller clear distance
        // .. this is needed because the influence distance also gets smaller
        float clearDistance_max = GnarlyBranches_min_dc_max + (1-GnarlyBranchesRatio) * (GnarlyBranches_max_dc_max - GnarlyBranches_min_dc_max);
        float clearDistance_min = GnarlyBranches_min_dc_min + (1-GnarlyBranchesRatio) * (GnarlyBranches_max_dc_min - GnarlyBranches_min_dc_min);


        float squaredClearDistance_max = clearDistance_max * clearDistance_max;
        float squaredClearDistance_min = clearDistance_min * clearDistance_min;
        float squaredClearDistance_range = squaredClearDistance_max - squaredClearDistance_min;

        //the density says how much of the squaredClearDistanceRange should be subtracted from the squaredClearDistance_max
        //-> typically the density is low in the beginning
        //-> this should result in a high squaredClearDistance_begin
        //-> therefore density * squaredClearDistanceRange has to be subtracted from the squaredClearDistance_max
        float squaredClearDistance_begin = squaredClearDistance_max - branchDensityBegin * squaredClearDistance_range;

        //the density says how much of the squaredClearDistanceRange should be subtracted from the squaredClearDistance_max
        //-> typically the density is high in the end
        //-> this should result in a low squaredClearDistance_end
        //-> therefore density * squaredClearDistanceRange has to be subtracted from the squaredClearDistance_max
        float squaredClearDistance_end = squaredClearDistance_max - branchDensityEnd * squaredClearDistance_range;

        float result = SigmoidInterpolation(squaredClearDistance_begin, squaredClearDistance_end, iteration);
        return result;
    }



    public void SetTropisms(Vector3 tropisms) {
        this.tropisms = tropisms.normalized;
    }

    public Vector3 GetTropisms() {
        return tropisms;
    }

    public void UpdateTropismsWeights() {
        //this.TropismsWeights = new Vector3(1, 0.1f, 1);

        float w = attractionPoints.GetWidth();
        float h = attractionPoints.GetHeight();
        float d = attractionPoints.GetDepth();

        float verticallyRevelant = Math.Max(w, d);

        if (verticallyRevelant > h) {
            //up tropisms should be smaller, when h is smaller than Max(w, d)
            float upTropismsWeights = h / verticallyRevelant;

            this.TropismsWeights = new Vector3(1, upTropismsWeights * upTropismsWeights, 1);
        } else {
            this.TropismsWeights = new Vector3(1, 1, 1);
        }
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
        UpdateTropismsWeights();
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
