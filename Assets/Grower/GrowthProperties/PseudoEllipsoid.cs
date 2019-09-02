using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PseudoEllipsoid {

    private Vector3 center;
    public Vector3 GetCenter() {
        return center;
    }


    public float smallest_x { get; private set; }
    float biggest_x;
    public float GetWidth() {
        //Debug.Log("width: " + (biggest_x - smallest_x));
        return biggest_x - smallest_x;
    }
    public float smallest_y { get; private set; }
    float biggest_y;
    public float GetHeight() {
        //Debug.Log("height: " + (biggest_y - smallest_y));
        return biggest_y - smallest_y;
        //return height;
    }
    public float GetHighestPoint() {
        return biggest_y;
    }

    public float smallest_z { get; private set; }
    float biggest_z;
    public float GetDepth() {
        //Debug.Log("depth: " + (biggest_z - smallest_z));
        return biggest_z - smallest_z;
    }

    public bool[] ActivePoints { get; private set; }
    public int ActiveCount { get; set; }
    public Vector3[] Points { get; set; }


    public int Seed { private set; get; }
    private System.Random random;
    private float RandomInRange(float from, float to) {
        float difference = to - from;
        if (difference < 0) {
            throw new Exception("Random: from needs to be smaller or equal than to");
        }
        return (float)random.NextDouble() * difference + from;
    }

    //generates a new set of points
    public void NewSeed() {
        Seed = (int)Util.RandomInRange(0, 65335);// (int)(new System.Random()).NextDouble() * 65335;
        random = new System.Random(Seed);

        Generate();
    }

    //"copies" all points in backup to the base
    public void Reset() {
        for (int i=0; i<ActivePoints.Length; i++) {
            ActivePoints[i] = true;
        }
    }

    public Vector3 Position { get; private set; }
    public void UpdatePosition(Vector3 value) {
        this.Position = value;
        random = new System.Random(Seed);
        Generate();
    }

    //public void UpdatePosition(Vector3 position) {
    //    Vector3 diff = position - base[0];
    //    for (int i = 0; i < base.Count; i++) {
    //        base[i] = base[i] + diff;
    //    }
    //}

    //public float radius_x { get; private set; }
    //public float radius_y { get; private set; }
    //public float radius_z { get; private set; }
    //public float cutoffRatio_bottom { get; private set; } //how many "percent" of the sphere in y direction are cut off at the bottom
    //public float cutoffRatio_top { get; private set; } //how many "percent" of the sphere in y direction are cut off at the top
    float radius_x;
    float radius_y;
    float radius_z;
    float cutoffRatio_bottom; //how many "percent" of the sphere in y direction are cut off at the bottom
    float cutoffRatio_top; //how many "percent" of the sphere in y direction are cut off at the top

    float density;
    //float densityRatio; //TODO, what did I want to do here?


    //density says: how many points per 1x1x1 voxel
    public PseudoEllipsoid(/*Vector3 position, */float radius_x, float radius_y, float radius_z, float density, float cutoffRatio_bottom, float cutoffRatio_top) {
        //seed = (int)(new System.Random()).NextDouble() * 65335;
        Seed = 0;// (int)Util.RandomInRange(0, 65335); //when deleting this seed, also hand the old seed over at LoadGnarlyBranches!
        random = new System.Random(Seed);

        //this.position = position;
        this.radius_x = radius_x;
        this.radius_y = radius_y;
        this.radius_z = radius_z;
        this.density = density;
        this.cutoffRatio_bottom = cutoffRatio_bottom;
        this.cutoffRatio_top = cutoffRatio_top;

        Initialize();
    }

    //density says: how many points per 1x1x1 voxel
    public PseudoEllipsoid(Vector3 position, float radius_x, float radius_y, float radius_z, float density, float cutoffRatio_bottom, float cutoffRatio_top) {
        //seed = (int)(new System.Random()).NextDouble() * 65335;
        Seed = 0;// (int)Util.RandomInRange(0, 65335); //when deleting this seed, also hand the old seed over at LoadGnarlyBranches!
        random = new System.Random(Seed);

        this.Position = position;
        this.radius_x = radius_x;
        this.radius_y = radius_y;
        this.radius_z = radius_z;
        this.density = density;
        this.cutoffRatio_bottom = cutoffRatio_bottom;
        this.cutoffRatio_top = cutoffRatio_top;

        Initialize();
    }

    //density says: how many points per 1x1x1 voxel
    public PseudoEllipsoid(Vector3 position, float radius_x, float radius_y, float radius_z, float density, float cutoffRatio_bottom, float cutoffRatio_top, int seed) {
        //seed = (int)(new System.Random()).NextDouble() * 65335;
        Seed = seed;// (int)Util.RandomInRange(0, 65335); //when deleting this seed, also hand the old seed over at LoadGnarlyBranches!
        random = new System.Random(Seed);
        this.Position = position;
        this.radius_x = radius_x;
        this.radius_y = radius_y;
        this.radius_z = radius_z;
        this.density = density;
        this.cutoffRatio_bottom = cutoffRatio_bottom;
        this.cutoffRatio_top = cutoffRatio_top;

        Initialize();
    }

    private void Initialize() {
        Generate();
    }

    private void Generate() {
        ActiveCount = 0;

        smallest_x = float.MaxValue;
        biggest_x = float.MinValue;
        smallest_y = float.MaxValue;
        biggest_y = float.MinValue;
        smallest_z = float.MaxValue;
        biggest_z = float.MinValue;

        center = new Vector3(0, 0, 0);
        //Center = new Vector3(0, 0, 0);

        //1. Calculate volume of sphere layer with radius 1
        float radius = 1f;

        float cutoffThreshhold_top = 2f * radius * cutoffRatio_top;
        float h_top = radius - cutoffThreshhold_top;
        float roh_squared_top = radius * radius - h_top * h_top;

        float cutoffThreshhold_bottom = 2f * radius * cutoffRatio_bottom;
        float h_bottom = radius - cutoffThreshhold_bottom;
        float roh_squared_bottom = radius * radius - h_bottom * h_bottom;

        float h = h_top + h_bottom;

        float volume = (float)(Math.PI * h / 6f) * (3f * roh_squared_top + 3f * roh_squared_bottom + h * h);

        //2. Calculate volume of sphere cut with given radii
        //generate transformation matrix
        Matrix4x4 transformation = Matrix4x4.Scale(new Vector3(radius_x, radius_y, radius_z));
        //determine the volume of the target ellipsoid
        volume = volume * transformation.determinant; //https://youtu.be/Ip3X9LOh2dk?list=PLZHQObOWTQDPD3MizzM2xVFitgF8hE_ab

        //3. Calculate the amount of points for the given density
        int n_points = (int)Math.Ceiling(volume * density);
        Points = new Vector3[n_points];
        ActivePoints = new bool[n_points];

        //4. Generate n_points attraction points
        while (ActiveCount < n_points) {
            //4.1 generate points within the sphere with radius 1
            float y = RandomInRange(-1, 1);
            if ((y < 0 - radius + cutoffThreshhold_bottom) | y > 0 + radius - cutoffThreshhold_top) {
                continue;
            }

            float x = RandomInRange(-1, 1);

            float z = RandomInRange(-1, 1);

            Vector3 point = new Vector3(x, y, z);

            //float distance = (point - Vector3.zero).magnitude;
            //if (distance <= radius) {
            float squaredDistance = Util.SquaredDistance(point, Vector3.zero);
            if (squaredDistance <= radius) {
                //float ran = Util.RandomInRange(0, 1);
                //if (ran < distance*distance) {

                //if (distance <= radius && distance > 0.5f) { //near the envelope test
                //4.2 Scale the points with the transformation matrix
                point = transformation.MultiplyVector(point);

                //4.3 Translate the points based on the real cutoff threshhold at the bottom (it is a different one than the one for the sphere with radius 1!)
                float real_cutoffThreshhold_bottom = 2f * radius_y * cutoffRatio_bottom;
                Vector3 targetCenter = new Vector3(Position.x, Position.y + radius_y - real_cutoffThreshhold_bottom, Position.z);// Vector3.up*radius + position;

                Vector3 calculatedPoint = point + targetCenter;
                
                Points[ActiveCount] = calculatedPoint; //ActiveCount is used as an indexer here
                ActivePoints[ActiveCount] = true;
                ActiveCount++;

                // for Core -> CameraMovement and UpTropismDamping
                if (smallest_x > calculatedPoint.x) {
                    smallest_x = calculatedPoint.x;
                }
                if (biggest_x < calculatedPoint.x) {
                    biggest_x = calculatedPoint.x;
                }
                if (smallest_y > calculatedPoint.y) {
                    smallest_y = calculatedPoint.y;
                }
                if (biggest_y < calculatedPoint.y) {
                    biggest_y = calculatedPoint.y;
                }
                if (smallest_z > calculatedPoint.z) {
                    smallest_z = calculatedPoint.z;
                }
                if (biggest_z < calculatedPoint.z) {
                    biggest_z = calculatedPoint.z;
                }
                // for Core -> CameraMovement
                if (center.y < calculatedPoint.y / 2) {
                    center.y = calculatedPoint.y / 2;
                }
            }
        }
    }

    public float GetRadius_x() {
        return radius_x;
    }

    public float GetRadius_y() {
        return radius_y;
    }

    public float GetRadius_z() {
        return radius_z;
    }


    public void UpdateCutoffRatio_bottom(float value) {
        this.cutoffRatio_bottom = value;
        random = new System.Random(Seed);
        Generate();
    }

    public float GetCutoffRatio_bottom() {
        return cutoffRatio_bottom;
    }


    public void UpdateCutoffRatio_top(float value) {
        this.cutoffRatio_top = value;
        random = new System.Random(Seed);
        Generate();
    }

    public float GetCutoffRatio_top() {
        return cutoffRatio_top;
    }


    public void UpdateRadius_x(float radius_x) {
        this.radius_x = radius_x;
        random = new System.Random(Seed);
        Generate();
	}

    public void UpdateRadius_y(float radius_y) {
        this.radius_y = radius_y;
        random = new System.Random(Seed);
        Generate();
	}

    public void UpdateRadius_z(float radius_z) {
        this.radius_z = radius_z;
        random = new System.Random(Seed);
        Generate();
	}

    public float GetDensity() {
        return density;
    }

    public void UpdateDensity(float density) {
        this.density = density;
        random = new System.Random(Seed);
        Generate();
    }
}