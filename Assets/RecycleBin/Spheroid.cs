using System;
using System.Collections.Generic;
using UnityEngine;

public class Spheroid : AttractionPoints
{
	float radius_y;
	float radius_x_z;
	float cutoffRatio;//how many "percent" of the sphere in y direction are cut off

	float density;
	float densityRatio; //TODO

	//density says: how many points per 1x1x1 voxel
	public Spheroid(Vector3 position, float radius_y, float radius_x_z, float density)
	{
		this.position = position;
		this.radius_y = radius_y;
		this.radius_x_z = radius_x_z;
		this.density = density;

		Generate();
	}

    override protected void Generate()	{
        //https://planetcalc.com/149/
        float volume = (float)((4f / 3f) * Math.PI * radius_x_z * radius_x_z * radius_y);

		int n_points = (int)(volume * density);

		Vector3 targetCenter = new Vector3(position.x, position.y + radius_y, position.z);// Vector3.up*radius + position;

		while (base.Count < n_points) {
            float y = RandomInRange(-radius_y, radius_y) + targetCenter.y;

            //float x_z = RandomInRange(-radius_x_z, radius_x_z);
            //float x = x_z + targetCenter.x;
            //float z = x_z + targetCenter.z;

            float x = RandomInRange(-radius_x_z, radius_x_z) + targetCenter.x;
            float z = RandomInRange(-radius_x_z, radius_x_z) + targetCenter.z;

            if ((x*x)/(radius_x_z*radius_x_z) + (z * z) / (radius_x_z * radius_x_z) + ((y*y)/radius_y*radius_y) <= 120) {
                Vector3 point = new Vector3(x, y, z);
                base.Add(point);
				backup.Add(point);
			}
		}
	}
}