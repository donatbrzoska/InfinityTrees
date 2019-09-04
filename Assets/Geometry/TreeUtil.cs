using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TreeUtil
{

	private static bool debugEnabled = false;
	private static void debug(string message, [CallerMemberName]string callerName = "")
	{
		if (debugEnabled)
		{
			UnityEngine.Debug.Log("DEBUG: TreeUtil: " + callerName + "(): " + message);
		}
	}

	private static void debug(FormatString formatString, [CallerMemberName]string callerName = "")
	{
		if (debugEnabled)
		{
			UnityEngine.Debug.Log("DEBUG: TreeUtil: " + callerName + "(): " + formatString);
		}
	}


	public static void printTriangles(int[] triangles)
	{
		int pointer = 0;
		while (pointer < triangles.Length)
		{
			UnityEngine.Debug.Log("triangle: " + triangles[pointer++] + ", " + triangles[pointer++] + ", " + triangles[pointer++]);
		}
	}

	public static void printTriangles(List<int> triangles)
	{
		int pointer = 0;
		while (pointer < triangles.Count)
		{
			UnityEngine.Debug.Log("triangle: " + triangles[pointer++] + ", " + triangles[pointer++] + ", " + triangles[pointer++]);
		}
	}

	public static void InsertArrayIntoList(Vector3[] source, List<Vector3> target)
	{
		for (int i = 0; i < source.Length; i++)
		{
			target.Add(source[i]);
		}
	}

	public static void InsertArrayIntoList(int[] source, List<int> target)
	{
		for (int i = 0; i < source.Length; i++)
		{
			target.Add(source[i]);
		}
	}

	public static void InsertArrayIntoArray(Vector3[] source, int sourceIndex, Vector3[] target, int targetIndex)
	{
		while (targetIndex < target.Length && sourceIndex < source.Length)
		{
			target[targetIndex] = source[sourceIndex];
			sourceIndex++;
			targetIndex++;
		}
	}

	public static void GetCircleVertices(List<Vector3> verticesResult, Vector3 position, Vector3 targetNormal, float radius, int resolution)
	{
		float angle = 360f / resolution;
		float currentAngle = 0;

		Vector3 firstVertex = new Vector3(0, 0, 0); //just setting some default value


		//calculate rotation Quaternion if necessary
		Quaternion rotation = Quaternion.AngleAxis(0, Vector3.zero);
		if (!targetNormal.Equals(Vector3.up))
		{
			// 1. calculate the angle between the current normal (0, 1, 0) and the targetNormal
			float _angle = Vector3.Angle(Vector3.up, targetNormal);

			// 2. rotate all coordinates by that angle (the axis to rotate by is calculated by cross(normal, targetNormal))
			//WRITE: order of Cross() parameters is important, probably determines in which direction the rotation takes place (right hand rule)
			//Vector3 axis = Vector3.Cross(targetNormal, normal);
			Vector3 axis = Vector3.Cross(Vector3.up, targetNormal);
			rotation = Quaternion.AngleAxis(_angle, axis);
		}

		for (int i = 0; i < resolution; i++)
		{
			//calculate coordinates
			float x1 = Mathf.Cos(Util.DegreesToRadians(currentAngle));
			float y1 = 0;
			float z1 = Mathf.Sin(Util.DegreesToRadians(currentAngle));

			//create vertex
			Vector3 vertex = new Vector3(x1, y1, z1);


			//apply target normal
			vertex = rotation * vertex;

			//apply radius parameter
			vertex = vertex * radius;

			//apply position (after rotating!)
			vertex = vertex + position;


			//store in result
			verticesResult.Add(vertex);

			//store first vertex
			if (i == 0)
			{
				firstVertex = vertex;
			}

			currentAngle = currentAngle + angle;
		}

		//store first vertex once more for texturing
		verticesResult.Add(firstVertex);

    }

    public static void CalculateCylinderTriangles(List<int> trianglesResult, int fromVerticesOffset, int toVerticesOffset, int resolution, bool tipNode) {
        //initialize a VertexPointer for both circles
        //the VertexPointers indicate, where the next vertex is going to be read from
        int fromVertexPointer = fromVerticesOffset;
        int toVertexPointer = toVerticesOffset;

        //for every resolution, two triangles are made
        for (int i = 0; i < resolution; i++) {
            //first triangle
            trianglesResult.Add(fromVertexPointer); fromVertexPointer++;
            trianglesResult.Add(toVertexPointer);
            trianglesResult.Add(fromVertexPointer);

            //second triangle
            trianglesResult.Add(fromVertexPointer);
            trianglesResult.Add(toVertexPointer); toVertexPointer++;
            if (tipNode) {
                toVertexPointer--;
            }
            trianglesResult.Add(toVertexPointer);
        }
    }

    public static Vector3[] CalculateNormals(Vector3[] vertices, int[] triangles) {
        debug("Calculating normals for " + vertices.Length + " vertices and " + triangles.Length + " triangles");
        if (triangles.Length == 0) {
            debug("No triangles ...");
            Vector3[] normals = new Vector3[vertices.Length];
            return normals;
        } else {
            Dictionary<Vector3, Vector3> verticesToSummedNormals = new Dictionary<Vector3, Vector3>();

            //https://stackoverflow.com/questions/16340931/calculating-vertex-normals-of-a-mesh?noredirect=1&lq=1
            //iterate through all triangles
            int triangle_vertexPointer = 0;
            while (triangle_vertexPointer < triangles.Length) {
                // and calculate their normals
                Vector3 a = vertices[triangles[triangle_vertexPointer++]];
                Vector3 b = vertices[triangles[triangle_vertexPointer++]];
                Vector3 c = vertices[triangles[triangle_vertexPointer++]];

                Vector3 ab = b - a;
                Vector3 ac = c - a;
                Vector3 currentNormal = Vector3.Cross(ab, ac);

                // then, add the normal in the map to the respective vertex
                if (verticesToSummedNormals.ContainsKey(a)) {
                    verticesToSummedNormals[a] += currentNormal;
                } else {
                    verticesToSummedNormals[a] = currentNormal;
                }

                if (verticesToSummedNormals.ContainsKey(b)) {
                    verticesToSummedNormals[b] += currentNormal;
                } else {
                    verticesToSummedNormals[b] = currentNormal;
                }

                if (verticesToSummedNormals.ContainsKey(c)) {
                    verticesToSummedNormals[c] += currentNormal;
                } else {
                    verticesToSummedNormals[c] = currentNormal;
                }
            }

            //normalize all the summed normals
            foreach (Vector3 normal in verticesToSummedNormals.Values) {
                normal.Normalize();
            }

            //put the calculated normals in an array, retrieving the respective normal for each vertex
            Vector3[] normals = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) {
                Vector3 associatedVertex = vertices[i];
                try {
                    normals[i] = verticesToSummedNormals[associatedVertex];
                } catch (KeyNotFoundException) {
                    debug("Root's vertices are stored once too much. If this occurrs more circleResolution+1 times per normal calculation, there is a bug in the code!");
                }
            }

            return normals;
        }
    }

    public static List<Vector3> CalculateNormals(List<Vector3> vertices, List<int> triangles) {
        debug("Calculating normals for " + vertices.Count + " vertices and " + triangles.Count + " triangles");
        if (triangles.Count == 0) {
            debug("No triangles ...");
            List<Vector3> normals = new List<Vector3>();
            return normals;
        } else {
            Dictionary<Vector3, Vector3> verticesToSummedNormals = new Dictionary<Vector3, Vector3>();

            //https://stackoverflow.com/questions/16340931/calculating-vertex-normals-of-a-mesh?noredirect=1&lq=1
            //iterate through all triangles
            int triangle_vertexPointer = 0;
            while (triangle_vertexPointer < triangles.Count) {
                // and calculate their normals
                Vector3 a = vertices[triangles[triangle_vertexPointer++]];
                Vector3 b = vertices[triangles[triangle_vertexPointer++]];
                Vector3 c = vertices[triangles[triangle_vertexPointer++]];

                Vector3 ab = b - a;
                Vector3 ac = c - a;
                Vector3 currentNormal = Vector3.Cross(ab, ac);

                // then, add the normal in the map to the respective vertex
                if (verticesToSummedNormals.ContainsKey(a)) {
                    verticesToSummedNormals[a] += currentNormal;
                } else {
                    verticesToSummedNormals[a] = currentNormal;
                }

                if (verticesToSummedNormals.ContainsKey(b)) {
                    verticesToSummedNormals[b] += currentNormal;
                } else {
                    verticesToSummedNormals[b] = currentNormal;
                }

                if (verticesToSummedNormals.ContainsKey(c)) {
                    verticesToSummedNormals[c] += currentNormal;
                } else {
                    verticesToSummedNormals[c] = currentNormal;
                }
            }

            //normalize all the summed normals
            foreach (Vector3 normal in verticesToSummedNormals.Values) {
                normal.Normalize();
            }

            //put the calculated normals in an array, retrieving the respective normal for each vertex
            List<Vector3> normals = new List<Vector3>();
            for (int i = 0; i < vertices.Count; i++) {
                Vector3 associatedVertex = vertices[i];
                try {
                    normals.Add(verticesToSummedNormals[associatedVertex]);
                } catch (KeyNotFoundException) {
                    debug("Root's vertices are stored once too much. If this occurrs more circleResolution+1 times per normal calculation, there is a bug in the code!");
                }
            }

            return normals;
        }
    }
}