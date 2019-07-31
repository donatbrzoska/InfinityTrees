using System;
using UnityEngine;

public static class Util {
    private static System.Random random = new System.Random();

    public static float RandomInRange(float from, float to) {
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
        float x = (float)(random.NextDouble() * 2) - 1;
        float y = (float)(random.NextDouble() * 2) - 1;
        float z = (float)(random.NextDouble() * 2) - 1;
        return new Vector3(x, y, z);
    }

    public static float DegreesToRadians(float degrees) {
        return (float) (degrees * Math.PI / 180);
    }

    public static bool AlmostEqual(float a, float b, float precision) {
        return Math.Abs(a - b) <= precision;
    }
}
