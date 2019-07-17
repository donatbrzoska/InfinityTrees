using System;
using UnityEngine;

public sealed class PseudoEllipsoid : AttractionPoints {
    float radius_x;
    float radius_y;
    float radius_z;
    float cutoffRatio_bottom; //how many "percent" of the sphere in y direction are cut off at the bottom
    float cutoffRatio_top; //how many "percent" of the sphere in y direction are cut off at the top

    float density;
    float densityRatio; //TODO, what did I want to do here?

    float height;
    public float GetHeight() {
        return height;
    }

    //density says: how many points per 1x1x1 voxel
    public PseudoEllipsoid(Vector3 position, float radius_x, float radius_y, float radius_z, float density, float cutoffRatio_bottom, float cutoffRatio_top) {
        //seed = (int)(new System.Random()).NextDouble() * 65335;
        seed = 0;// (int)Util.RandomInRange(0, 65335);
        random = new System.Random(seed);

        this.position = position;
        this.radius_x = radius_x;
        this.radius_y = radius_y;
        this.radius_z = radius_z;
        this.density = density;
        this.cutoffRatio_bottom = cutoffRatio_bottom;
        this.cutoffRatio_top = cutoffRatio_top;

        Generate();
    }

    override protected void Generate() {
        base.Clear();
        base.backup.Clear();

        //1. Calculate volume of sphere with radius 1
        float radius = 1f;

        float cutoffThreshhold_top = 2f * radius * cutoffRatio_top;
        float h_top = radius - cutoffThreshhold_top;
        float roh_squared_top = radius * radius - h_top * h_top;

        float cutoffThreshhold_bottom = 2f * radius * cutoffRatio_bottom;
        float h_bottom = radius - cutoffThreshhold_bottom;
        float roh_squared_bottom = radius * radius - h_bottom * h_bottom;

        float h = h_top + h_bottom;

        float volume = (float) (Math.PI * h / 6f) * (3f * roh_squared_top + 3f * roh_squared_bottom + h * h);

        //2. Calculate volume of sphere with given radii
        //generate transformation matrix
        Matrix4x4 transformation = Matrix4x4.Scale(new Vector3(radius_x, radius_y, radius_z));
        //determine the volume of the target ellipsoid
        volume = volume * transformation.determinant; //https://youtu.be/Ip3X9LOh2dk?list=PLZHQObOWTQDPD3MizzM2xVFitgF8hE_ab

        //3. Calculate the amount of points for the given density
        int n_points = (int)(volume * density);

        //float smallestx = float.MaxValue;
        //float biggestx = float.MinValue;
        //float smallestz = float.MaxValue;
        //float biggestz = float.MinValue;

        //4. Generate n_points attraction points
        while (base.Count < n_points) {
            //4.1 generate points within the sphere with radius 1
            float y = RandomInRange(-1, 1);
            if ((y < 0 - radius + cutoffThreshhold_bottom) | y > 0 + radius - cutoffThreshhold_top) {
                continue;
            }
            if (y > height) {
                height = y;
            }

            float x = RandomInRange(-1, 1);
            //if (x < smallestx) {
            //    smallestx = x;
            //}
            //if (x > biggestx) {
            //    biggestx = x;
            //}
            float z = RandomInRange(-1, 1);
            //if (z < smallestz) {
            //    smallestz = z;
            //}
            //if (z > biggestz) {
            //    biggestz = z;
            //}

            Vector3 point = new Vector3(x, y, z);

            float distance = (point - Vector3.zero).magnitude;
            if (distance <= radius) {
                //4.2 Scale the points with the transformation matrix
                point = transformation.MultiplyVector(point);

                //4.3 Translate the points based on the real cutoff threshhold at the bottom (it is a different one than the one for the sphere with radius 1!)
                float real_cutoffThreshhold_bottom = 2f * radius_y * cutoffRatio_bottom;
                Vector3 targetCenter = new Vector3(position.x, position.y + radius_y - real_cutoffThreshhold_bottom, position.z);// Vector3.up*radius + position;
                base.center = targetCenter;

                base.Add(point + targetCenter);
                backup.Add(point + targetCenter);
            }
        }

        //Debug.Log("smallest x: " + smallestx);
        //Debug.Log("biggest x: " + biggestx);
        //Debug.Log("smallest z: " + smallestz);
        //Debug.Log("biggest z: " + biggestz);
    }

    //private void Generate() {
    //    //1. Calculate volume of sphere with radius 1
    //    float radius = 1;

    //    float cutoffThreshhold = 2 * radius * cutoffRatio;

    //    float h = 2 * radius - cutoffThreshhold;
    //    float roh = (float)Math.Sqrt(h * (2 * radius - h));
    //    float volume = (float)((1f / 6f) * Math.PI * h * (3f * roh * roh + h * h));

    //    //2. Calculate volume of sphere with given radii
    //    //generate transformation matrix
    //    Matrix4x4 transformation = Matrix4x4.Scale(new Vector3(radius_x, radius_y, radius_z));
    //    //determine the volume of the target ellipsoid
    //    volume = volume * transformation.determinant; //https://youtu.be/Ip3X9LOh2dk?list=PLZHQObOWTQDPD3MizzM2xVFitgF8hE_ab

    //    //3. Calculate the amount of points for the given density
    //    int n_points = (int)(volume * density);

    //    //4. Generate n_points attraction points
    //    while (base.Count < n_points) {
    //        //4.1 generate points within the sphere with radius 1
    //        float y = RandomInRange(-1, 1);
    //        if (y<0-radius+cutoffThreshhold) {
    //            continue;
    //        }
    //        float x = RandomInRange(-1, 1);
    //        float z = RandomInRange(-1, 1);

    //        Vector3 point = new Vector3(x, y, z);

    //        float distance = (point - Vector3.zero).magnitude;
    //        if (distance <= radius) {
    //            //4.2 Scale the points with the transformation matrix
    //            point = transformation.MultiplyVector(point);

    //            //4.3 Translate the points based on the real cutoff threshhold (it is a different one than the one for the sphere with radius 1!)
    //            float real_cutoffThreshhold = 2 * radius_y * cutoffRatio;
    //            Vector3 targetCenter = new Vector3(position.x, position.y + radius_y - real_cutoffThreshhold, position.z);// Vector3.up*radius + position;

    //            base.Add(point + targetCenter);
    //            backup.Add(point + targetCenter);
    //        }
    //    }
    //}

    public void UpdateRadius_x(float radius_x) {
        this.radius_x = radius_x;
        base.random = new System.Random(base.seed);
        Generate();

        attractionPointsListener.OnAttractionPointsChanged();
    }

    public void UpdateRadius_y(float radius_y) {
        this.radius_y = radius_y;
        base.random = new System.Random(base.seed);
        Generate();

        attractionPointsListener.OnAttractionPointsChanged();
    }

    public void UpdateRadius_z(float radius_z) {
        this.radius_z = radius_z;
        base.random = new System.Random(base.seed);
        Generate();

        attractionPointsListener.OnAttractionPointsChanged();
    }
}