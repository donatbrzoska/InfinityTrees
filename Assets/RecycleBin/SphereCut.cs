using System;
using System.Collections.Generic;
using UnityEngine;

public class SphereCut : AttractionPoints
{
	float radius;
	//int n_points;
	float cutoffRatio; //how many "percent" of the sphere in y direction are cut off

	float density;
	float densityRatio; //TODO

	//public AttractionPoints(Vector3 position, float radius, int n_points, float cutoffRatio) {
	//    this.position = position;
	//    this.radius = radius;
	//    this.n_points = n_points;
	//    this.cutoffRatio = cutoffRatio;

	//    Generate();
	//}

	//density says: how many points per 1x1x1 voxel
	public SphereCut(Vector3 position, float radius, float density, float cutoffRatio)
	{
		this.position = position;
		this.radius = radius;
		this.density = density;
		this.cutoffRatio = cutoffRatio;

		Generate();
	}

	private void Generate()
	{
		float cutoffThreshhold = 2 * radius * cutoffRatio;

		float h = 2 * radius - cutoffThreshhold;
		float roh = (float)Math.Sqrt(h * (2 * radius - h));
		float volume = (float)((1f / 6f) * Math.PI * h * (3f * roh * roh + h * h));

		int n_points = (int)(volume * density);

		Vector3 targetCenter = new Vector3(position.x, position.y + radius - cutoffThreshhold, position.z);// Vector3.up*radius + position;

		while (base.Count < n_points)
		{
			float y = RandomInRange(-radius, radius) + targetCenter.y;
			if (y < 0)
			{
				continue;
			}

			float x = RandomInRange(-radius, radius) + targetCenter.x;
			float z = RandomInRange(-radius, radius) + targetCenter.z;


			Vector3 point = new Vector3(x, y, z);

			float distance = (point - targetCenter).magnitude;
			if (distance <= radius)
			{
				base.Add(point);
				backup.Add(point);
			}
		}
	}

	//generates a new set of points
	override public void NewSeed()
	{
		base.Clear();
		backup.Clear();
		Generate();
	}
}