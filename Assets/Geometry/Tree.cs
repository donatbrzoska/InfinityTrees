using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class Tree : GrowerListener {

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

    private GeometryProperties geometryProperties;

    private Node root;

    private int age;


    private int ageAtPreviousCalculation = -1;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int[] triangles;


    public Tree(Vector3 position, Grower grower, GeometryProperties geometryProperties) {
        this.grower = grower;
        this.geometryProperties = geometryProperties;

        root = new Node(position, geometryProperties);
    }

    public void Grow() {

        Thread t = new Thread(() => {
            Stopwatch growingStopwatch = new Stopwatch();
            growingStopwatch.Start();

            for (int i = 0; i < grower.GetGrowthProperties().GetIterations(); i++) {
                Stopwatch iterationStopwatch = new Stopwatch();
                iterationStopwatch.Start();

                grower.Apply(root);
                age++;

                if (grower.GetGrowthProperties().GetHangingBranchesEnabled() && i > grower.GetGrowthProperties().GetIterations() * grower.GetGrowthProperties().GetHangingBranchesFromAgeRatio()) {
                    grower.GetGrowthProperties().SetTropisms(new Vector3(0, -1f, 0), true);
                    debug("updated tropisms");
                }

                iterationStopwatch.Stop();
                debug(new FormatString("iteration {0} took {1}", i, iterationStopwatch.Elapsed));
            }

            growingStopwatch.Stop();
            debug(new FormatString("grew {0} times in {1}", grower.GetGrowthProperties().GetIterations(), growingStopwatch.Elapsed));

            //Stopwatch getStopwatch = new Stopwatch();
            //getStopwatch.Start();
            //GetEverything(ref vertices, ref normals, ref uvs, ref triangles, res, -1);
            //getStopwatch.Stop();
            //debug("vertex, normal, uv and triangle calculations took " + getStopwatch.Elapsed);
            Thread.Sleep(100);

            debug(new FormatString("{0} vertices, {1} triangles", vertices.Length, triangles.Length/3));
        });
        ThreadManager.Add(t);
        t.Start();
    }

    private void Regrow() {
        age = 0;
        root = new Node(Vector3.zero, geometryProperties);
        Grow();
    }




    public void GetEverything(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, ref int[] triangles) {
        //Stopwatch sw = new Stopwatch();
        //if (age==39) {
        //    sw.Start();
        //}

        int currentAge = age; //only recalculate everything, when there where changes
        if (ageAtPreviousCalculation != currentAge) {// || (currentAge == 0)) {

            Dictionary<Node, int> nodeVerticesPositions = new Dictionary<Node, int>();

            List<Vector3> verticesTmp = new List<Vector3>();// verticesTmp.Capacity = 5000;
            List<Vector2> uvsTmp = new List<Vector2>();// uvsTmp.Capacity = 5000;
            List<int> trianglesTmp = new List<int>();// trianglesTmp.Capacity = 5000;

            CalculateEverything(root, nodeVerticesPositions, 0, verticesTmp, uvsTmp, trianglesTmp);


            //What happens, when a new node is added while iterating through the TMP List?
            // -> it won't be drawn
            this.vertices = new Vector3[verticesTmp.Count];
            verticesTmp.CopyTo(this.vertices);

            this.uvs = new Vector2[uvsTmp.Count];
            uvsTmp.CopyTo(this.uvs);

            this.triangles = new int[trianglesTmp.Count];
            trianglesTmp.CopyTo(this.triangles);

            this.normals = TreeUtil.CalculateNormals(this.vertices, this.triangles);

            ageAtPreviousCalculation = currentAge;
        }

        vertices = this.vertices;
        normals = this.normals;
        uvs = this.uvs;
        triangles = this.triangles;

        //if (age==39) {
        //    sw.Stop();
        //    debug(""+sw.Elapsed);
        //}
    }

    //looks at the current node, builds cylinders to it's subnodes and recursively calls the function for all subnodes
    private void CalculateEverything(Node node, Dictionary<Node, int> nodeVerticesPositions, float vOffset, List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        if (node.IsRoot()) {
            CalculateAndStoreCylinderVertices(node, nodeVerticesPositions, verticesResult, false);
            CalculateAndStoreCylinderUVs(vOffset, uvsResult);
            vOffset += grower.GetGrowthProperties().GetGrowthDistance(); //TODO: this is a little inaccurate
        }

        int n_subnodes = node.GetSubnodes().Count;
        for (int i=0; i<n_subnodes; i++) {
            Node subnode = node.GetSubnodes()[i];

            //calculate and store vertices
            CalculateAndStoreCylinderVertices(subnode, nodeVerticesPositions, verticesResult, false);

            //calculate and store uvs
            CalculateAndStoreCylinderUVs(vOffset, uvsResult);
            vOffset += grower.GetGrowthProperties().GetGrowthDistance(); //TODO: this is a little inaccurate

            //calculate triangles between the node's vertices and the subnode's vertices
            CalculateAndStoreCylinderTriangles(nodeVerticesPositions[node], nodeVerticesPositions[subnode], trianglesResult);

			//calculate and store leaf triangles
			subnode.CalculateAndStoreLeafData(verticesResult, uvsResult, trianglesResult);


            //recursive call
            CalculateEverything(subnode, nodeVerticesPositions, vOffset, verticesResult, uvsResult, trianglesResult);
        }
    }

    //private void CalculateAndStoreCylinderVertices(Node node, Dictionary<Node, int> nodeVerticesPostions, List<Vector3> verticesResult, bool doubled) {
    //    //calculate vertices of the current node
    //    Vector3[] vertices = node.GetCircleVertices(doubled);
    //    //store the position of the node's vertices (the position in the 'global' vertices array)
    //    nodeVerticesPostions[node] = verticesResult.Count;
    //    TreeUtil.InsertArrayIntoList(vertices, verticesResult);
    //}

    private void CalculateAndStoreCylinderVertices(Node node, Dictionary<Node, int> nodeVerticesPostions, List<Vector3> verticesResult, bool doubled) {
        ////calculate vertices of the current node
        //Vector3[] vertices = node.GetCircleVertices(doubled);
        ////store the position of the node's vertices (the position in the 'global' vertices array)
        //nodeVerticesPostions[node] = verticesResult.Count;
        //TreeUtil.InsertArrayIntoList(vertices, verticesResult);

        //store the position of the node's vertices (the position in the 'global' vertices array)
        nodeVerticesPostions[node] = verticesResult.Count;
        //calculate vertices of the current node
        node.GetCircleVertices(verticesResult, doubled);
    }

    //TODO: u in Abhängigkeit von Radius?
    //TODO: v kontinuierlich und abhängig von der growthDistance -> uPointer oder sowas, Distancen zu vorherigen Nodes benötigt...
    private void CalculateAndStoreCylinderUVs(float v, List<Vector2> uvsResult) {
        //float circle_segment_size = 0.5f / geometryProperties.GetCircleResolution();
        //float circle_segment_size = 0.4f / geometryProperties.GetCircleResolution();

        float u = 0.1f;
        for (int i = 0; i < geometryProperties.GetCircleResolution() + 1; i++) {
            Vector2 uv = new Vector2(u, v);
            uvsResult.Add(uv);

            //u += circle_segment_size;
        }
    }

    //private void CalculateAndStoreCylinderTriangles(int from, int to, List<int> trianglesResult) {
    //    int[] cylinderTriangles = TreeUtil.CalculateCylinderTriangles(from, to, geometryProperties.GetCircleResolution());

    //    //store the triangles in the 'global' temporary triangles array
    //    TreeUtil.InsertArrayIntoList(cylinderTriangles, trianglesResult);
    //}

    private void CalculateAndStoreCylinderTriangles(int from, int to, List<int> trianglesResult) {
        TreeUtil.CalculateCylinderTriangles(trianglesResult, from, to, geometryProperties.GetCircleResolution());
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


    //#######################################################################################
    //##########                     INTERFACE IMPLEMENTIATION                     ##########
    //#######################################################################################

    public void OnAttractionPointsChanged() {
        //grower.GetGrowthProperties().ResetTropisms();
        //grower.GetGrowthProperties().ResetAttractionPoints();
        Regrow();
    }

    public void OnAgeChanged() {
        //grower.GetGrowthProperties().ResetTropisms();
        Regrow();
    }
}