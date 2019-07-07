using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class Tree {

    private static bool debugEnabled = true;
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: Tree: " + callerName + "(): " + message);
        }
    }

    private static void debug(FormatString formatString, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: Tree: " + callerName + "(): " + formatString);
        }
    }





    //private Vector3 position;
    //private float stemRadius;
    //private float steamRadiusRatio; //calculated when GetVertices() ... is called

    private Grower grower;

    private GrowthProperties growthProperties;

    private Node root;

    private int age;


    private int ageAtPreviousCalculation;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int[] triangles;


    public Tree(Vector3 position, Grower grower, GrowthProperties growthProperties) {
        this.grower = grower;
        this.growthProperties = growthProperties;

        root = new Node(position, growthProperties);
    }

    public void Grow(int iterations, int res) {

        Thread t = new Thread(() => {
            Stopwatch growingStopwatch = new Stopwatch();
            growingStopwatch.Start();

            for (int i = 0; i < iterations; i++) {
                Stopwatch iterationStopwatch = new Stopwatch();
                iterationStopwatch.Start();

                grower.Apply(root);
                age++;

                iterationStopwatch.Stop();
                debug(new FormatString("iteration {0} took {1}", i, iterationStopwatch.Elapsed));
            }

            growingStopwatch.Stop();
            debug(new FormatString("grew {0} times in {1}", iterations, growingStopwatch.Elapsed));

            //Stopwatch getStopwatch = new Stopwatch();
            //getStopwatch.Start();
            //GetEverything(ref vertices, ref normals, ref uvs, ref triangles, res, -1);
            //getStopwatch.Stop();
            //debug("vertex, normal, uv and triangle calculations took " + getStopwatch.Elapsed);

            debug(new FormatString("{0} vertices, {1} triangles", vertices.Length, triangles.Length/3));
        });
        t.Start();
    }






    public void GetEverything(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, ref int[] triangles, int cylinderResolution, int curveResolution) {
        int currentAge = age; //only recalculate everything, when there where changes
        if ((ageAtPreviousCalculation < currentAge) || (currentAge == 0)) {

            Dictionary<Node, int> nodeVerticesPositions = new Dictionary<Node, int>();

            List<Vector3> verticesTmp = new List<Vector3>();
            List<Vector2> uvsTmp = new List<Vector2>();
            List<int> trianglesTmp = new List<int>();

            CalculateEverythingHelper(root, nodeVerticesPositions, 0, verticesTmp, uvsTmp, trianglesTmp, cylinderResolution, curveResolution);


            //What happens, when a new node is added while iterating through the TMP List?
            // -> it won't be drawn
            this.vertices = new Vector3[verticesTmp.Count];
            for (int i = 0; i < verticesTmp.Count; i++) {
                this.vertices[i] = verticesTmp[i];
            }

            this.uvs = new Vector2[uvsTmp.Count];
            for (int i = 0; i < uvsTmp.Count; i++) {
                this.uvs[i] = uvsTmp[i];
            }

            this.triangles = new int[trianglesTmp.Count];
            for (int i = 0; i < trianglesTmp.Count; i++) {
                this.triangles[i] = trianglesTmp[i];
            }

            this.normals = TreeUtil.CalculateNormals(this.vertices, this.triangles);

            ageAtPreviousCalculation = currentAge;
        }

        vertices = this.vertices;
        normals = this.normals;
        uvs = this.uvs;
        triangles = this.triangles;
    }

    //private void CalculateEverything() {

    //}

    //looks at the current node, builds cylinders to it's subnodes and recursively calls the function for all subnodes
    private void CalculateEverythingHelper(Node node, Dictionary<Node, int> nodeVerticesPositions, float vOffset, List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult, int cylinderResolution, int curveResolution) {
        if (node.IsRoot()) {
            CalculateAndStoreVertices(node, nodeVerticesPositions, verticesResult, cylinderResolution, false);
            CalculateAndStoreUVs(vOffset, uvsResult, cylinderResolution);
            vOffset += growthProperties.GetGrowthDistance();
        }

        int n_subnodes = node.GetSubnodes().Count;
        for (int i=0; i<n_subnodes; i++) {
            Node subnode = node.GetSubnodes()[i];

            //calculate and store vertices
            CalculateAndStoreVertices(subnode, nodeVerticesPositions, verticesResult, cylinderResolution, false);

            //calculate and store uvs
            CalculateAndStoreUVs(vOffset, uvsResult, cylinderResolution);
            vOffset += growthProperties.GetGrowthDistance();

            //calculate triangles between the node's vertices and the subnode's vertices
            CalculateAndStoreCylinderTriangles(nodeVerticesPositions[node], nodeVerticesPositions[subnode], trianglesResult, cylinderResolution);

            //TODO: parametrisieren
            //calculate and store leaf triangles
            //if (subnode.GetRadius() < growthProperties.GetMaxTwigRadiusForLeaves()) {
            //    for (int j = 0; j < growthProperties.GetLeavesPerNode(); j++) {
            //        CalculateAndStoreLeafTriangles(
            //            subnode.GetPosition(),
            //            growthProperties.GetLeafRotationAxis(),
            //            growthProperties.GetLeafRotationAngle(),
            //            growthProperties.GetLeafSize(),
            //            verticesResult,
            //            uvsResult,
            //            trianglesResult);
            //    }
            //}

            //recursive call
            CalculateEverythingHelper(subnode, nodeVerticesPositions, vOffset, verticesResult, uvsResult, trianglesResult, cylinderResolution, curveResolution);
        }
    }

    private void CalculateAndStoreVertices(Node node, Dictionary<Node, int> nodeVerticesPostions, List<Vector3> verticesResult, int cylinderResolution, bool doubled) {
        //calculate vertices of the current node
        Vector3[] vertices = node.GetVertices(cylinderResolution, doubled);
        //store the position of the node's vertices (the position in the 'global' vertices array)
        nodeVerticesPostions[node] = verticesResult.Count;
        TreeUtil.InsertArrayIntoList(vertices, verticesResult);
    }

    //TODO: u in Abhängigkeit von Radius?
    //TODO: v kontinuierlich und abhängig von der growthDistance -> uPointer oder sowas, Distancen zu vorherigen Nodes benötigt...
    private void CalculateAndStoreUVs(float v, List<Vector2> uvsResult, int cylinderResolution) {
        float circle_segment_size = 0.5f / cylinderResolution;

        float u = 0;
        for (int i = 0; i < cylinderResolution + 1; i++) {
            Vector2 uv = new Vector2(u, v);
            uvsResult.Add(uv);

            u += circle_segment_size;
        }
    }

    private void CalculateAndStoreCylinderTriangles(int from, int to, List<int> trianglesResult, int cylinderResolution) {
        int[] cylinderTriangles = TreeUtil.CalculateCylinderTriangles(from, to, cylinderResolution);

        //store the triangles in the 'global' temporary triangles array
        TreeUtil.InsertArrayIntoList(cylinderTriangles, trianglesResult);
    }

    private void CalculateAndStoreLeafTriangles(Vector3 position, Vector3 rotationAxis, float rotationAngle, float size, List<Vector3> verticesResult,/* List<Vector3> normalsResult,*/ List<Vector2> uvsResult, List<int> trianglesResult) {

        int verticesOffset = verticesResult.Count;

        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);

        //top side of leaf
        verticesResult.Add(rotation * new Vector3(-0.5f, 0, 0) * size + position);
        verticesResult.Add(rotation * new Vector3(-0.5f, 0, 1) * size + position);
        verticesResult.Add(rotation * new Vector3(0.5f, 0, 1) * size + position);
        verticesResult.Add(rotation * new Vector3(0.5f, 0, 0) * size + position);

        //bottom side of leaf
        verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, 0) * size + position);
        verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, 1) * size + position);
        verticesResult.Add(rotation * new Vector3(0.5f, 0.0001f, 1) * size + position);
        verticesResult.Add(rotation * new Vector3(0.5f, 0.0001f, 0) * size + position);

        //for (int i = 0; i < 4; i++) {
        //    normalsResult.Add(Vector3.up); //TODO
        //}

        //top side of leaf
        uvsResult.Add(new Vector2(0.5f, 0));
        uvsResult.Add(new Vector2(0.5f, 1));
        uvsResult.Add(new Vector2(1, 1));
        uvsResult.Add(new Vector2(1, 0));

        //bottom side of leaf
        uvsResult.Add(new Vector2(0.5f, 0));
        uvsResult.Add(new Vector2(0.5f, 1));
        uvsResult.Add(new Vector2(1, 1));
        uvsResult.Add(new Vector2(1, 0));


        //top side of leaf
        //triangle 1
        trianglesResult.Add(verticesOffset + 0);
        trianglesResult.Add(verticesOffset + 1);
        trianglesResult.Add(verticesOffset + 2);
        //triangle 2
        trianglesResult.Add(verticesOffset + 0);
        trianglesResult.Add(verticesOffset + 2);
        trianglesResult.Add(verticesOffset + 3);

        verticesOffset += 4; //TODO: ADJUST THIS WHEN ADDING VERTICES FOR LEAVES

        //bottom side of leaf
        //triangle 1
        trianglesResult.Add(verticesOffset + 0);
        trianglesResult.Add(verticesOffset + 2);
        trianglesResult.Add(verticesOffset + 1);
        //triangle 2
        trianglesResult.Add(verticesOffset + 0);
        trianglesResult.Add(verticesOffset + 3);
        trianglesResult.Add(verticesOffset + 2);

        //trianglesResult.Add(verticesOffset + 4);
        //trianglesResult.Add(verticesOffset + 6);
        //trianglesResult.Add(verticesOffset + 5);
        //trianglesResult.Add(verticesOffset + 4);
        //trianglesResult.Add(verticesOffset + 7);
        //trianglesResult.Add(verticesOffset + 6);
    }

    ////looks at the current node, builds cylinders to it's subnodes and recursively calls the function for all subnodes
    //private void CalculateVerticesAndTriangles(Node node, List< verticesResult, List< trianglesResult, int cylinderResolution, int curveResolution) {

    //    //(if not already and stored)
    //    if (!node.VerticesAreStored()) {
    //        //calculate vertices of the current node
    //        Vector3[] nodeVertices = node.GetVertices(cylinderResolution);

    //        //store the position of the node's vertices (in the 'global' vertices array)
    //        node.SetVerticesPosition(verticesResult.Count);

    //        //store the vertices in the 'global' temporary vertices array
    //        TreeUtil.InsertArrayIntoList<(nodeVertices, verticesResult);
    //    }

    //    foreach (Node subnode in node.GetSubnodes()) {
    //        //calculate the subnode's vertices
    //        Vector3[] subnodeVertices = subnode.GetVertices(cylinderResolution);

    //        //store the position of the subnode's vertices (in the 'global' vertices array)
    //        subnode.SetVerticesPosition(verticesResult.Count);

    //        //store the subnode's vertices in the 'global' temporary vertices array
    //        TreeUtil.InsertArrayIntoList<(subnodeVertices, verticesResult);



    //        //calculate triangles between the node's vertices and the subnode's vertices
    //        int[] cylinderTriangles = TreeUtil.CalculateCylinderTriangles(node.GetVerticesPosition(), subnode.GetVerticesPosition(), cylinderResolution);
    //        //node.ResetVerticesPosition();
    //        //store the triangles in the 'global' temporary triangles array
    //        TreeUtil.InsertArrayIntoList<(cylinderTriangles, trianglesResult);



    //        //recursive call
    //        CalculateVerticesAndTriangles(subnode, verticesResult, trianglesResult, cylinderResolution, curveResolution);
    //    }
    //}
}













public static class TreeUtil {

    private static bool debugEnabled = true;
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: TreeUtil: " + callerName + "(): " + message);
        }
    }

    private static void debug(FormatString formatString, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: TreeUtil: " + callerName + "(): " + formatString);
        }
    }



    public static float ToRadians(float degrees) {
        return degrees * Mathf.PI / 180;
    }

    public static void printTriangles(int[] triangles) {
        int pointer = 0;
        while (pointer < triangles.Length) {
            UnityEngine.Debug.Log("triangle: " + triangles[pointer++] + ", " + triangles[pointer++] + ", " + triangles[pointer++]);
        }
    }

    public static void printTriangles(List<int> triangles) {
        int pointer = 0;
        while (pointer < triangles.Count) {
            UnityEngine.Debug.Log("triangle: " + triangles[pointer++] + ", " + triangles[pointer++] + ", " + triangles[pointer++]);
        }
    }

    public static void InsertArrayIntoList(Vector3[] source, List<Vector3> target) {
        for (int i = 0; i < source.Length; i++) {
            target.Add(source[i]);
        }
    }

    public static void InsertArrayIntoList(int[] source, List<int> target) {
        for (int i = 0; i < source.Length; i++) {
            target.Add(source[i]);
        }
    }

    public static void InsertArrayIntoArray(Vector3[] source, int sourceIndex, Vector3[] target, int targetIndex) {
        while (targetIndex < target.Length && sourceIndex < source.Length) {
            target[targetIndex] = source[sourceIndex];
            sourceIndex++;
            targetIndex++;
        }
    }

    //public static Vector3[] Translate(Vector3[] vectors, Vector3 by) {
    //    for (int i = 0; i < vectors.Length; i++) {
    //        vectors[i].x += by.x;
    //        vectors[i].y += by.y;
    //        vectors[i].z += by.z;
    //    }
    //    return vectors;
    //}

    // resolution must be >= 3
    public static Vector3[] CalculateCircleVertices(Vector3 position, Vector3 targetNormal, float radius, int resolution, bool doubled) {
        float angle = 360f / resolution;
        float currentAngle = 0;

        //Vector3[] result = new Vector3[resolution];
        //for (int i = 0; i < resolution; i++) {
        //    //calculate coordinates
        //    float x1 = Mathf.Cos(ToRadians(currentAngle));
        //    float y1 = 0;
        //    float z1 = Mathf.Sin(ToRadians(currentAngle));

        //    //create vertex
        //    Vector3 vertex = new Vector3(x1, y1, z1);

        //    //apply radius parameter
        //    vertex = vertex * radius;

        //    //store in result
        //    result[i] = vertex;

        //    currentAngle = currentAngle + angle;
        //}

        Vector3[] result;
        if (doubled) {
            result = new Vector3[resolution * 2 + 2];
        } else {
            result = new Vector3[resolution + 1];
        }
        int vertexPointer = 0;

        Vector3 firstVertex = new Vector3(00000000, 00000000,0000000); //just setting some default value


        //calculate rotation Quaternion if necessary
        Quaternion rotation = Quaternion.AngleAxis(0, Vector3.zero);
        if (!targetNormal.Equals(Vector3.up)) {
            // 1. calculate the angle between the current normal (0, 1, 0) and the targetNormal
            float _angle = Vector3.Angle(Vector3.up, targetNormal);

            // 2. rotate all coordinates by that angle (the axis to rotate by is calculated by cross(normal, targetNormal))
            //WRITE: order of Cross() parameters is important, probably determines in which direction the rotation takes place (right hand rule)
            //Vector3 axis = Vector3.Cross(targetNormal, normal);
            Vector3 axis = Vector3.Cross(Vector3.up, targetNormal);
            rotation = Quaternion.AngleAxis(_angle, axis);
        }

        for (int i = 0; i < resolution; i++) {
            //calculate coordinates
            float x1 = Mathf.Cos(ToRadians(currentAngle));
            float y1 = 0;
            float z1 = Mathf.Sin(ToRadians(currentAngle));

            //create vertex
            Vector3 vertex = new Vector3(x1, y1, z1);


            //apply target normal
            vertex = rotation * vertex;

            //apply radius parameter
            vertex = vertex * radius;

            //apply position (after rotating!)
            vertex = vertex + position;


            //store in result
            result[vertexPointer] = vertex;
            if (doubled) {
                result[resolution + vertexPointer + 1] = vertex; //second circle
            }
            vertexPointer++;

            //store first vertex
            if (i==0) {
                firstVertex = vertex;
            }

            currentAngle = currentAngle + angle;
        }

        //store first vertexPointer two times more
        result[resolution] = firstVertex;
        if (doubled) {
            result[resolution * 2 + 1] = firstVertex;
        }

        return result;
    }

    //public static int[] CalculateCylinderTriangles(int fromVerticesOffset, int toVerticesOffset, int resolution) {
    //    //every resolution results in 2 triangles which consist of 3 vertices
    //    int[] triangles = new int[resolution * 2 * 3];
    //    int trianglePointer = 0;

    //    //initialize a VertexPointer for both circles
    //    //the VertexPointers indicate, where the next vertex is going to be read from
    //    VertexPointer fromVertexPointer = new VertexPointer(resolution, fromVerticesOffset);
    //    VertexPointer toVertexPointer = new VertexPointer(resolution, toVerticesOffset);

    //    //for every resolution, two triangles are made
    //    for (int i = 0; i < resolution; i++) {
    //        //first triangle
    //        triangles[trianglePointer++] = fromVertexPointer.Current(); fromVertexPointer.Increment();
    //        triangles[trianglePointer++] = toVertexPointer.Current();
    //        triangles[trianglePointer++] = fromVertexPointer.Current();

    //        //second triangle
    //        triangles[trianglePointer++] = fromVertexPointer.Current();
    //        triangles[trianglePointer++] = toVertexPointer.Current(); toVertexPointer.Increment();
    //        triangles[trianglePointer++] = toVertexPointer.Current();
    //    }

    //    return triangles;
    //}


    public static int[] CalculateCylinderTriangles(int fromVerticesOffset, int toVerticesOffset, int resolution) {
        //every resolution results in 2 triangles which consist of 3 vertices
        int[] triangles = new int[resolution * 2 * 3];
        int trianglePointer = 0;

        //initialize a VertexPointer for both circles
        //the VertexPointers indicate, where the next vertex is going to be read from
        int fromVertexPointer = fromVerticesOffset;
        int toVertexPointer = toVerticesOffset;

        //for every resolution, two triangles are made
        for (int i = 0; i < resolution; i++) {
            //first triangle
            triangles[trianglePointer++] = fromVertexPointer; fromVertexPointer++;
            triangles[trianglePointer++] = toVertexPointer;
            triangles[trianglePointer++] = fromVertexPointer;

            //second triangle
            triangles[trianglePointer++] = fromVertexPointer;
            triangles[trianglePointer++] = toVertexPointer; toVertexPointer++;
            triangles[trianglePointer++] = toVertexPointer;
        }

        return triangles;
    }

    public static Vector3[] CalculateNormals(Vector3[] vertices, int[] triangles) {
        Dictionary<Vector3, Vector3> verticesToSummedNormals = new Dictionary<Vector3, Vector3>();

        //https://stackoverflow.com/questions/16340931/calculating-vertex-normals-of-a-mesh?noredirect=1&lq=1
        //iterate through all triangles
        int triangle_vertexPointer = 0;
        while (triangle_vertexPointer<triangles.Length) {
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

        //for (int i = 0; i < triangles.Length-2; i += 3) {
        //    // and calculate their normals
        //    Vector3 a = vertices[triangles[i]];
        //    Vector3 b = vertices[triangles[i + 1]];
        //    Vector3 c = vertices[triangles[i + 2]];

        //    Vector3 ab = b - a;
        //    Vector3 ac = c - a;
        //    Vector3 currentNormal = Vector3.Cross(ab, ac);

        //    // then, add the normal in the map to the respective vertex
        //    if (verticesToSummedNormals.ContainsKey(a)) {
        //        verticesToSummedNormals[a] += currentNormal;
        //    } else {
        //        verticesToSummedNormals[a] = currentNormal;
        //    }

        //    if (verticesToSummedNormals.ContainsKey(b)) {
        //        verticesToSummedNormals[b] += currentNormal;
        //    } else {
        //        verticesToSummedNormals[b] = currentNormal;
        //    }

        //    if (verticesToSummedNormals.ContainsKey(c)) {
        //        verticesToSummedNormals[c] += currentNormal;
        //    } else {
        //        verticesToSummedNormals[c] = currentNormal;
        //    }
        //}

        //normalize all the summed normals
        foreach (Vector3 normal in verticesToSummedNormals.Values) {
            normal.Normalize();
        }

        //put the calculated normals in an array, retrieving the respective normal for each vertex
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 associatedVertex = vertices[i];
            normals[i] = verticesToSummedNormals[associatedVertex];
        }

        return normals;
    }
}
