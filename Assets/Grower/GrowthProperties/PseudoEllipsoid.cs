using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class PseudoEllipsoid {

    private static bool debugEnabled = true;
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: PseudoEllipsoid: " + callerName + "(): " + message);
        }
    }

    private static void debug(FormatString formatString, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: PseudoEllipsoid: " + callerName + "(): " + formatString);
        }
    }

    private Vector3 center;
    public Vector3 Center {
        get {
            return center;
        }
        private set {
            center = value;
        }
    }

    private float smallest_x;
    private float biggest_x;
    public float GetWidth() {
        return biggest_x - smallest_x;
    }

    private float smallest_y;
    private float biggest_y;
    public float GetHeight() {
        return biggest_y - smallest_y;
    }
    public float GetHighestPoint() {
        return biggest_y;
    }

    private float smallest_z;
    private float biggest_z;
    public float GetDepth() {
        return biggest_z - smallest_z;
    }



    public Vector3[] Points { get; set; }

    private bool[] activePoints;
    public int ActiveCount { get;  private set; }
    public bool IsActive(int pointIndex) {
        return activePoints[pointIndex];
    }
    public void Deactivate(int pointIndex) {
        activePoints[pointIndex] = false;
        ActiveCount--;
    }
    //"copies" all points in backup to the base
    public void Reset() {
        for (int i = 0; i < activePoints.Length; i++) {
            activePoints[i] = true;
        }
    }


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


    public Vector3 Position { get; private set; }
    public void UpdatePosition(Vector3 value) {
        this.Position = value;
        random = new System.Random(Seed);
        Generate();
    }


    public float Radius_x { get; private set; }
    public void UpdateRadius_x(float radius_x) {
        this.Radius_x = radius_x;
        random = new System.Random(Seed);
        Generate();
    }

    public float Radius_y { get; private set; }
    public void UpdateRadius_y(float radius_y) {
        this.Radius_y = radius_y;
        random = new System.Random(Seed);
        Generate();
    }

    public float Radius_z { get; private set; }
    public void UpdateRadius_z(float radius_z) {
        this.Radius_z = radius_z;
        random = new System.Random(Seed);
        Generate();
    }

    public float CutoffRatio_bottom { get; private set; } //how many "percent" of the sphere in y direction are cut off at the bottom
    public void UpdateCutoffRatio_bottom(float value) {
        this.CutoffRatio_bottom = value;
        random = new System.Random(Seed);
        Generate();
    }

    public float CutoffRatio_top { get; private set; } //how many "percent" of the sphere in y direction are cut off at the top
    public void UpdateCutoffRatio_top(float value) {
        this.CutoffRatio_top = value;
        random = new System.Random(Seed);
        Generate();
    }

    public float Density { get; private set; }
    public void UpdateDensity(float density) {
        Density = density;
        random = new System.Random(Seed);
        Generate();
    }


    //used for initial point cloud creation
    //density says: how many points per 1x1x1 voxel
    public PseudoEllipsoid(float radius_x, float radius_y, float radius_z, float density, float cutoffRatio_bottom, float cutoffRatio_top) {
        //seed = (int)(new System.Random()).NextDouble() * 65335;
        Seed = 0;// (int)Util.RandomInRange(0, 65335); //when deleting this seed, also hand the old seed over at LoadGnarlyBranches!
        random = new System.Random(Seed);

        this.Position = Vector3.zero;
        this.Radius_x = radius_x;
        this.Radius_y = radius_y;
        this.Radius_z = radius_z;
        this.Density = density;
        this.CutoffRatio_bottom = cutoffRatio_bottom;
        this.CutoffRatio_top = cutoffRatio_top;

        Initialize();
    }

    ////had some usage
    ////density says: how many points per 1x1x1 voxel
    //public PseudoEllipsoid(Vector3 position, float radius_x, float radius_y, float radius_z, float density, float cutoffRatio_bottom, float cutoffRatio_top, int seed) {
    //    //seed = (int)(new System.Random()).NextDouble() * 65335;
    //    Seed = seed;// (int)Util.RandomInRange(0, 65335); //when deleting this seed, also hand the old seed over at LoadGnarlyBranches!
    //    random = new System.Random(Seed);
    //    this.Position = position;
    //    this.radius_x = radius_x;
    //    this.radius_y = radius_y;
    //    this.radius_z = radius_z;
    //    this.density = density;
    //    this.cutoffRatio_bottom = cutoffRatio_bottom;
    //    this.cutoffRatio_top = cutoffRatio_top;

    //    Initialize();
    //}

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

        float cutoffThreshhold_top = 2f * radius * CutoffRatio_top;
        float h_top = radius - cutoffThreshhold_top;
        float roh_squared_top = radius * radius - h_top * h_top;

        float cutoffThreshhold_bottom = 2f * radius * CutoffRatio_bottom;
        float h_bottom = radius - cutoffThreshhold_bottom;
        float roh_squared_bottom = radius * radius - h_bottom * h_bottom;

        float h = h_top + h_bottom;

        float volume = (float)(Math.PI * h / 6f) * (3f * roh_squared_top + 3f * roh_squared_bottom + h * h);

        //2. Calculate volume of sphere cut with given radii
        //generate transformation matrix
        Matrix4x4 transformation = Matrix4x4.Scale(new Vector3(Radius_x, Radius_y, Radius_z));
        //determine the volume of the target ellipsoid
        volume = volume * transformation.determinant; //https://youtu.be/Ip3X9LOh2dk?list=PLZHQObOWTQDPD3MizzM2xVFitgF8hE_ab

        //3. Calculate the amount of points for the given density
        int n_points = (int)Math.Ceiling(volume * Density);
        debug(n_points + " attraction points");
        Points = new Vector3[n_points];
        activePoints = new bool[n_points];

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
                float real_cutoffThreshhold_bottom = 2f * Radius_y * CutoffRatio_bottom;
                Vector3 targetCenter = new Vector3(Position.x, Position.y + Radius_y - real_cutoffThreshhold_bottom, Position.z);// Vector3.up*radius + position;

                Vector3 calculatedPoint = point + targetCenter;
                
                Points[ActiveCount] = calculatedPoint; //ActiveCount is used as an indexer here
                activePoints[ActiveCount] = true;
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

}