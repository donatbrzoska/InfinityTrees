using System;
using System.Collections.Generic;
using UnityEngine;

public class GeometryProperties {

    //############################
    //########## TWIGS  ##########
    //############################

    public float TipRadius { get; set; } //SET //0.007f

    public float NthRoot_min { private get; set; }
    public float NthRoot_max { private get; set; }
    public float NthRoot { //smaller values make bigger radii
        get {
            float range = NthRoot_max - NthRoot_min;
            return NthRoot_max - StemThickness * range;
        }
    }

    // NthRoot = NthRoot_max - StemThickness * (NthRoot_max - NthRoot_min)
    // NthRoot + StemThickness * (NthRoot_max - NthRoot_min) = NthRoot_max
    // StemThickness = (NthRoot_max - NthRoot) / (NthRoot_max - NthRoot_min)
    public float StemThickness { get; set; } // 0..1

    public int CircleResolution { get; set; }


    //subnode radius has to be at least x*node.GetRadius() for a usual connection
    //the bigger the value, the less usual connections will be made
    public float MinRadiusRatioForNormalConnection { private get; set; } // 0..1

    public bool UsualConnection(float nodeRadius, float subnodeRadius) {
        return subnodeRadius > MinRadiusRatioForNormalConnection * nodeRadius;
    }


    public float PendulousBranchesIntensity { get; set; } //0..1

    public int PendulousBranchesBeginDepthMin { get; set; } //should be set to 0 or n_initial_stem_segments
    public int PendulousBranchesBeginDepthMax { get; set; } //should be set to n_initial_stem_segments + iterations
    public float PendulousBranchesBeginDepthRatio { get; set; } //0..1
    public int PendulousBranchesBeginDepth {
        get {
            //PrecisePendulousBranchesBeginDepth = PendulousBranchesBeginDepthRatio * (PendulousBranchesBeginDepthMax - PendulousBranchesBeginDepthMin);
            return (int)(PendulousBranchesBeginDepthRatio * (PendulousBranchesBeginDepthMax - PendulousBranchesBeginDepthMin));
        }
    }

    //############################
    //########## LEAVES ##########
    //############################

    public float MaxTwigRadiusForLeaves { get; set; } //make depend on tip radius?


    private Dictionary<Leaf.LeafType, float> leafSizes = new Dictionary<Leaf.LeafType, float>();

    public void SetLeafSize(Leaf.LeafType type, float leafSize) {
        this.leafSizes[type] = leafSize;
    }

    public float GetLeafSize() {
        return leafSizes[leafType];
    }

    public float GetLeafSizeValue() {
        float leafSizeStdDev = 0.2f * leafSizes[leafType];
        return Util.RandomWithStdDev(leafSizes[leafType], leafSizeStdDev);
    }


    private Leaf.LeafType leafType;
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


    private Dictionary<Leaf.LeafType, float> displayedLeavesPerNode = new Dictionary<Leaf.LeafType, float>();

    public void SetDisplayedLeavesPerNode(Leaf.LeafType type, float displayedLeavesPerNode) {
        this.displayedLeavesPerNode[type] = displayedLeavesPerNode;
    }

    public float GetDisplayedLeavesPerNode() {
        return displayedLeavesPerNode[leafType];
    }


    public int DisplayedLeafesPerNodeMaximum;


    //##############################
    //########## RENDERER ##########
    //##############################

    public List<string> StemColorStrings { get; set; }
    public int CurrentStemColorStringsIndex { get; set; }

    public List<string> LeafColorStrings { get; set; }
    public int CurrentLeafColorStringsIndex { get; set; }

}
