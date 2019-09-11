using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

public static class Util {

    //only one Random per Thread is allowed
    // Growerthread:
    // - leaf orientation
    // - leaf positioning
    // -> Leafs get own Random
    // - Stem Generation has its own AdvancedRandom
    // Mainthread:
    // - n leaves
    // - leaf size
    // - point cloud seed generation
    // - point cloud triangle orientation vector
    // - point cloud triangle orientation angle
    private static System.Random random = new System.Random();

    public static float RandomInRange(float from, float to) {
        float d = to - from;
        float f = (float)random.NextDouble() * d;
        return f + from;
    }

    public static float RandomInRange(float from, float to, System.Random random) {
        float d = to - from;
        float f = (float)random.NextDouble() * d;
        return f + from;
    }

    public static float RandomWithStdDev(float mean, float stdDev) {
        //https://stackoverflow.com/questions/218060/random-gaussian-variables
        double u1 = 1.0 - random.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal =
                     mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

        return (float) randNormal;
    }

    public static Vector3 RandomVector3() {
        float x = (float)random.NextDouble();
        float y = (float)random.NextDouble();
        float z = (float)random.NextDouble();
        return new Vector3(x, y, z);
    }

    public static Vector3 RandomVector3(System.Random random) {
        float x = (float)random.NextDouble();
        float y = (float)random.NextDouble();
        float z = (float)random.NextDouble();
        return new Vector3(x, y, z);
    }

    public static Vector3 Hadamard(Vector3 a, Vector3 b) {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static float DegreesToRadians(float degrees) {
        return (float) (degrees * Math.PI / 180);
    }

    public static bool AlmostEqual(float a, float b, float precision=0.00001f) {
        return Math.Abs(a - b) <= precision;
    }

    public static float SquaredDistance(Vector3 a, Vector3 b) {
        Vector3 d = a - b;
        return d.x * d.x + d.y * d.y + d.z * d.z;
    }

    ////https://stackoverflow.com/a/129395
    //public static T DeepCopy<T>(T obj) {
    //    using (var ms = new MemoryStream()) {
    //        var formatter = new BinaryFormatter();
    //        formatter.Serialize(ms, obj);
    //        ms.Position = 0;

    //        return (T)formatter.Deserialize(ms);
    //    }
    //}

    public static void SplitMesh(List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, List<int> triangles, ref List<Vector3[]> verticesResult, ref List<Vector3[]> normalsResult, ref List<Vector2[]> uvsResult, ref List<int[]> trianglesResult/*, object additionLock*/) {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        //List<Vector3[]> verticesResult_ = new List<Vector3[]>();
        //List<Vector3[]> normalsResult_ = new List<Vector3[]>();
        //List<Vector2[]> uvsResult_ = new List<Vector2[]>();
        //trianglesResult = new List<int[]>();

        verticesResult = new List<Vector3[]>();
        normalsResult = new List<Vector3[]>();
        uvsResult = new List<Vector2[]>();
        trianglesResult = new List<int[]>();

        List<Vector3> currentVertices = null;
        List<Vector3> currentNormals = null;
        List<Vector2> currentUVs = null;
        List<int> currentTriangles = null;

        Dictionary<int, int> globalVerticesIndizes_to_localVerticesIndizes = null;

        //copy triangles until the max amount of vertices is reached
        for (int i = 0; i < triangles.Count; i += 3) {
            // reinitialize if not initialized or currentVertices is full
            if (currentVertices == null || currentVertices.Count + 2 > 65535) {

                // initialize new arrays
                currentVertices = new List<Vector3>();
                currentNormals = new List<Vector3>();
                currentUVs = new List<Vector2>();

                currentTriangles = new List<int>();

                // reset the stored vertexIndizes, since we now have nothing stored again
                //triangles_to_vertexIndizes = new Dictionary<Vector3Int, Vector3Int>();
                globalVerticesIndizes_to_localVerticesIndizes = new Dictionary<int, int>();
            }

            // get the indizes of the vertices that the current triangle depends on
            Vector3Int globalTriangleIndizes = new Vector3Int(triangles[i], triangles[i + 1], triangles[i + 2]);

            Vector3Int localTriangleIndizes = new Vector3Int(); //will store the vertex indizes of the currently looked at triangle in the local arrays
            if (!globalVerticesIndizes_to_localVerticesIndizes.ContainsKey(globalTriangleIndizes.x)) {
                // do this if we have not stored the respective vertex locally
                currentVertices.Add(vertices[globalTriangleIndizes.x]);
                currentNormals.Add(normals[globalTriangleIndizes.x]);
                currentUVs.Add(uvs[globalTriangleIndizes.x]);

                // and point to it
                localTriangleIndizes.x = currentVertices.Count - 1;
                globalVerticesIndizes_to_localVerticesIndizes[globalTriangleIndizes.x] = currentVertices.Count - 1;
            } else {
                // or look up where we stored it and point to it
                localTriangleIndizes.x = globalVerticesIndizes_to_localVerticesIndizes[globalTriangleIndizes.x];
            }
            currentTriangles.Add(localTriangleIndizes.x);

            if (!globalVerticesIndizes_to_localVerticesIndizes.ContainsKey(globalTriangleIndizes.y)) {
                currentVertices.Add(vertices[globalTriangleIndizes.y]);
                currentNormals.Add(normals[globalTriangleIndizes.y]);
                currentUVs.Add(uvs[globalTriangleIndizes.y]);

                localTriangleIndizes.y = currentVertices.Count - 1;
                globalVerticesIndizes_to_localVerticesIndizes[globalTriangleIndizes.y] = currentVertices.Count - 1;
            } else {
                localTriangleIndizes.y = globalVerticesIndizes_to_localVerticesIndizes[globalTriangleIndizes.y];
            }
            currentTriangles.Add(localTriangleIndizes.y);

            if (!globalVerticesIndizes_to_localVerticesIndizes.ContainsKey(globalTriangleIndizes.z)) {
                currentVertices.Add(vertices[globalTriangleIndizes.z]);
                currentNormals.Add(normals[globalTriangleIndizes.z]);
                currentUVs.Add(uvs[globalTriangleIndizes.z]);

                localTriangleIndizes.z = currentVertices.Count - 1;
                globalVerticesIndizes_to_localVerticesIndizes[globalTriangleIndizes.z] = currentVertices.Count - 1;
            } else {
                localTriangleIndizes.z = globalVerticesIndizes_to_localVerticesIndizes[globalTriangleIndizes.z];
            }
            currentTriangles.Add(localTriangleIndizes.z);


            //put the data into the result lists
            if (currentVertices.Count + 2 > 65535 || i == triangles.Count - 3) {
                //lock (additionLock) {
                    verticesResult.Add(currentVertices.ToArray());
                    normalsResult.Add(currentNormals.ToArray());
                    uvsResult.Add(currentUVs.ToArray());

                    trianglesResult.Add(currentTriangles.ToArray());
                //}
            }
        }

        //debug(vertices_.Count + ((vertices_.Count == 1) ? " renderer active" : " renderer(s) active"));
        //int n_triangles = 0;
        //foreach (int[] triangleArray in triangles_) {
        //    n_triangles += triangleArray.Length / 3;
        //}
        //debug(n_triangles + " triangles");

        //SplitCheck();

        //UnityEngine.Debug.Log("Splitting mesh took: " + sw.Elapsed);
    }
}
