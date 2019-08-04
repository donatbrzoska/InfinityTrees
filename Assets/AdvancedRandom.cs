using System;
using UnityEngine;

public class AdvancedRandom : System.Random {

    public AdvancedRandom(int seed) : base(seed) { }

    public float RandomInRange(float from, float to) {
        float d = to - from;
        float f = (float)base.NextDouble() * d;
        return f + from;
    }

    public float RandomWithStdDev(float mean, float stdDev) {
        //https://stackoverflow.com/questions/218060/random-gaussian-variables
        double u1 = 1.0 - base.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - base.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal =
                     mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

        return (float) randNormal;
    }

    public Vector3 RandomVector3() {
        float x = (float)(base.NextDouble() * 2) - 1;
        float y = (float)(base.NextDouble() * 2) - 1;
        float z = (float)(base.NextDouble() * 2) - 1;
        return new Vector3(x, y, z);
    }
}
