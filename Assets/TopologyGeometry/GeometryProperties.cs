using System.Collections.Generic;
using UnityEngine;

public class GeometryProperties {

    System.Random random = new System.Random();

    List<GeometryPropertiesObserver> geometryPropertiesObservers = new List<GeometryPropertiesObserver>();

    public void Subscribe(GeometryPropertiesObserver geometryPropertiesObserver) {
        this.geometryPropertiesObservers.Add(geometryPropertiesObserver);
        //Debug.Log(geometryPropertiesObservers.Count);
    }

    public void Unsubsribe(GeometryPropertiesObserver gpo) {
        geometryPropertiesObservers.Remove(gpo);
    }

    public void UnsubscribeAll() {
        geometryPropertiesObservers.Clear();
    }

    //GeometryPropertiesListener geometryPropertiesListener;
    //public void SetGeometryPropertiesListener(GeometryPropertiesListener geometryPropertiesListener) {
    //    this.geometryPropertiesListener = geometryPropertiesListener;
    //}

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
    private float leavesPerNode; //TODO: move to growthProperties
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

    public void UpdateLeafSize(float leafSize) {
        SetLeafSize(leafSize);
        foreach (GeometryPropertiesObserver gpo in geometryPropertiesObservers) {
            gpo.OnLeafSizeChanged();
        }
    }

    public float GetLeafSize() {
        return Util.RandomInRangeNormal(leafSize, leafSizeStdDev);
    }



    public void SetLeavesPerNode(float leavesPerNode) {
        this.leavesPerNode = leavesPerNode;
    }

    public void UpdateLeavesPerNode(float leavesPerNode) {
        this.leavesPerNode = leavesPerNode;
        //geometryPropertiesListener.OnLeavesPerNodeChanged();
        for (int i=0; i<geometryPropertiesObservers.Count; i++) {
            geometryPropertiesObservers[i].OnLeavesPerNodeChanged();
        }
    }

    public float GetLeavesPerNode() {
        return leavesPerNode;
    }



    public List<string> LeafTypeStrings { get; set; }
    public int CurrentLeafTypeStringsIndex { get; set; }

    public void SetLeafType(Leaf.LeafType leafType) {
        this.leafType = leafType;
    }

    public void UpdateLeafType(int leafTypeStringsIndex) {
        this.CurrentLeafTypeStringsIndex = leafTypeStringsIndex;
        this.leafType = Leaf.LeafTypeStringToLeafType[LeafTypeStrings[leafTypeStringsIndex]];
        //geometryPropertiesListener.OnLeafTypeChanged();
        foreach (GeometryPropertiesObserver gpo in geometryPropertiesObservers) {
            gpo.OnLeafTypeChanged();
        }
    }

    public Leaf.LeafType GetLeafType() {
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

    public void UpdateLeavesEnabled(bool leavesEnabled) {
        this.leavesEnabled = leavesEnabled;
        //geometryPropertiesListener.OnLeavesEnabledChanged();
        foreach (GeometryPropertiesObserver gpo in geometryPropertiesObservers) {
            gpo.OnLeavesEnabledChanged();
        }
    }

	public bool GetLeavesEnabled() {
        return leavesEnabled;
    }



    //##############################
    //########## RENDERER ##########
    //##############################

    public List<string> StemColorStrings { get; set; }
    public int CurrentStemColorStringsIndex { get; set; }

    public List<string> LeafColorStrings { get; set; }
    public int CurrentLeafColorStringsIndex { get; set; }

    //public void SetStemColors(List<string> stemColors) {
    //    this.stemColors = stemColors;
    //}

    //public void SetCurrentStemColor(int currentStemColor) {
    //    this.currentStemColor = currentStemColor;
    //}

    //public void SetLeafColors(List<string> leafColors) {
    //    this.leafColors = leafColors;
    //}

    //public void SetCurrentLeafColor(int currentLeafColor) {
    //    this.currentLeafColor = currentLeafColor;
    //}





}
