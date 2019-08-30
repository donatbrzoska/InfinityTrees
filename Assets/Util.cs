using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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
}
