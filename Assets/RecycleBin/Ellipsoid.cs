using System;
using UnityEngine;

public class Ellipsoid : AttractionPoints
{
	float radius_x;
	float radius_y;
	float radius_z;

	float density;
	float densityRatio; //TODO

    //density says: how many points per 1x1x1 voxel
    public Ellipsoid(Vector3 position, float radius_x, float radius_y, float radius_z, float density)
	{
		this.position = position;
		this.radius_x = radius_x;
		this.radius_y = radius_y;
		this.radius_z = radius_z;
		this.density = density;


        Generate();
	}

	private void Generate()
	{
        //https://planetcalc.com/149/
        //(https://math.stackexchange.com/questions/1145267/volume-of-the-smaller-region-of-ellipsoid-cut-by-plane)
        float volume = (float)((4f / 3f) * Math.PI * radius_x * radius_y * radius_z);

		int n_points = (int)(volume * density);


        float smallest_y = 50;
        float biggest_y = 0;

        while (base.Count < n_points)
		{
            float x = RandomInRange(-radius_x, radius_x);
			float y = RandomInRange(-radius_y, radius_y);
			float z = RandomInRange(-radius_z, radius_z);

			if (((x * x) / (radius_x * radius_x)) + ((y * y) / radius_y * radius_y) + ((z * z) / (radius_z * radius_z)) <= 1) {
                Vector3 targetCenter = new Vector3(position.x, position.y + radius_y, position.z);// Vector3.up*radius + position;
                Vector3 point = new Vector3(x, y, z) + targetCenter;

                if (point.y < smallest_y) {
                    smallest_y = point.y;
                }
                if (point.y > biggest_y) {
                    biggest_y = point.y;
                }

                if (point.y < 0.5) {
                    Debug.Log("Point near 0.5");
                }
				base.Add(point);
				backup.Add(point);
			}
        }

        Debug.Log("s_y" + smallest_y);
        Debug.Log("b_y" + biggest_y);
        Debug.Log(base[0]);
        Debug.Log(base[1]);
        Debug.Log(base[2]);
    }

	//generates a new set of points
	override public void NewSeed()
	{
		base.Clear();
		backup.Clear();
		Generate();
	}
}