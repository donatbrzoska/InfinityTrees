using System;

public static class Util {
    private static Random random = new Random();
    public static float RandomInRange(float from, float to) {
        float d = to - from;
        float f = (float) random.NextDouble() * d;
        return f + from;
    }

    public static float DegreesToRadians(float degrees) {
        return (float) (degrees * Math.PI / 180);
    }

    public static bool AlmostEqual(float a, float b, float precision) {
        return Math.Abs(a - b) <= precision;
    }
}
