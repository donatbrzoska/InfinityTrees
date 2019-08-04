﻿using System.Collections.Generic;
using UnityEngine;

public class GeometryProperties {

    //############################
    //########## TWIGS  ##########
    //############################

    private float tipRadius; //SET //0.007f

	public void SetTipRadius(float tipRadius)
	{
		this.tipRadius = tipRadius;
	}

	public float GetTipRadius()
	{
		return tipRadius;
	}



	private float nth_root; //smaller values make bigger radii   //1.6f

	public void SetNthRoot(float nth_root)
	{
		this.nth_root = nth_root;
	}

	public float GetNthRoot()
	{
		return nth_root;
	}

	public float nth_root_min { get; set; }
	public float nth_root_max { get; set; }
	public float StemThickness {
		get{
			//nth_root = nth_root_max - value * (nth_root_max - nth_root_min);
			//nth_root + value * (nth_root_max - nth_root_min) = nth_root_max;
			//value * (nth_root_max - nth_root_min) = nth_root_max - nth_root;
			//value = (nth_root_max - nth_root) / (nth_root_max - nth_root_min);
			return (nth_root_max - nth_root) / (nth_root_max - nth_root_min);
		}
		set {
			nth_root = nth_root_max - value * (nth_root_max - nth_root_min);
		}
	}


	private int circleResolution;

    public void SetCircleResolution(int circleResolution) {
        this.circleResolution = circleResolution;
    }

    public int GetCircleResolution() {
        return circleResolution;
    }


	private float minRadiusRatioForNormalConnection;

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
    private Dictionary<Leaf.LeafType, float> leafSizes = new Dictionary<Leaf.LeafType, float>();
    private Leaf.LeafType leafType;
    private Dictionary<Leaf.LeafType, float> displayedLeavesPerNode = new Dictionary<Leaf.LeafType, float>();
    private bool leavesEnabled;



    //FIXED?
    public void SetMaxTwigRadiusForLeaves(float maxTwigRadiusForLeaves) {
        this.maxTwigRadiusForLeaves = maxTwigRadiusForLeaves;
    }

    public float GetMaxTwigRadiusForLeaves() {
        return maxTwigRadiusForLeaves;
    }



    public void SetLeafSize(Leaf.LeafType type, float leafSize) {
        this.leafSizes[type] = leafSize;
    }

    public float GetLeafSize() {
        float leafSizeStdDev = 0.2f * leafSizes[leafType];
        return Util.RandomWithStdDev(leafSizes[leafType], leafSizeStdDev);
    }



    public void SetLeavesEnabled(bool leavesEnabled) {
        this.leavesEnabled = leavesEnabled;
    }

    public bool GetLeavesEnabled() {
        return leavesEnabled;
    }



    public void SetDisplayedLeavesPerNode(Leaf.LeafType type, float displayedLeavesPerNode) {
        this.displayedLeavesPerNode[type] = displayedLeavesPerNode;
    }

    public float GetDisplayedLeavesPerNode() {
        return displayedLeavesPerNode[leafType];
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
