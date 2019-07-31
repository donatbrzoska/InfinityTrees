using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttractionPoints : List<Vector3> {
    protected Vector3 position;

    protected Vector3 center;

	//backup points, so the tree can be regrown with any amount of iterations
	protected List<Vector3> backup = new List<Vector3>();

    protected int seed;
    protected System.Random random;

    protected AttractionPointsListener attractionPointsListener;

    public void SetAttractionPointsListener(AttractionPointsListener attractionPointsListener) {
        this.attractionPointsListener = attractionPointsListener;
    }

    protected abstract void Generate();

    //generates a new set of points
    public void NewSeed() {
        seed = (int) Util.RandomInRange(0, 65335);// (int)(new System.Random()).NextDouble() * 65335;
        random = new System.Random(seed);

        Generate();
        attractionPointsListener.OnAttractionPointsChanged();
    }

    //"copies" all points in backup to the base
    public void Reset() {
        base.Clear();
        foreach (Vector3 p in backup) {
            base.Add(p);
        }

        //attractionPointsListener.OnAttractionPointsChanged();
    }

    public Vector3 GetCenter() {
        return center;
    }

    public List<Vector3> GetBackup() {
        return backup;
    }

    protected float RandomInRange(float from, float to) {
        float difference = to - from;
        if (difference<0) {
            throw new Exception("Random: from needs to be smaller or equal than to");
        }
        return (float) random.NextDouble() * difference + from;
    }

}