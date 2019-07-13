using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttractionPoints : List<Vector3> {
    protected Vector3 position;

    //backup points, so the tree can be regrown with any amount of iterations
    protected List<Vector3> backup = new List<Vector3>();

    protected System.Random random = new System.Random();

    protected AttractionPointsListener attractionPointsListener;

    public void SetAttractionPointsListener(AttractionPointsListener attractionPointsListener) {
        this.attractionPointsListener = attractionPointsListener;
    }

    //generates a new set of points
    public abstract void NewSeed();

    //"copies" all points in backup to the base
    public void Reset() {
        base.Clear();
        foreach (Vector3 p in backup) {
            base.Add(p);
        }

        attractionPointsListener.OnAttractionPointsChanged();
    }

    protected float RandomInRange(float from, float to) {
        float difference = to - from;
        if (difference<0) {
            throw new Exception("Random: from needs to be smaller or equal than to");
        }
        return (float) random.NextDouble() * difference + from;
    }

}