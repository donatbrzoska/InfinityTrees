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

    //private int age;


    //private int ageAtPreviousCalculation = -1;
    //private Vector3[][] vertices = new Vector3[2][];
    //private Vector3[][] normals = new Vector3[2][];
    //private Vector2[][] uvs = new Vector2[2][];
    //private int[][] triangles = new int[2][];
    //private FinishedPointer finishedPointer = new FinishedPointer();

    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector2[] uvs;
    private int[] triangles;

    Core treeCreator;

    public Tree(Vector3 position, Grower grower, GeometryProperties geometryProperties, Core treeCreator) {
        this.grower = grower;
        this.geometryProperties = geometryProperties;
        this.treeCreator = treeCreator;

        root = new Node(position, geometryProperties);
    }

    private int twigVertices;
    private int twigTriangles;
    private int leafVertices;
    private int leafTriangles;

    public void Grow() {
        Thread t = new Thread(() => {
            grower.Apply(root);
            Thread.Sleep(100);
            debug(new FormatString("{0} vertices, {1} triangles", vertices.Length, triangles.Length / 3));
            debug(new FormatString("{0} twig vertices, {1} twig triangles", twigVertices, twigTriangles / 3));
            debug(new FormatString("{0} leaf vertices, {1} leaf triangles", leafVertices, leafTriangles / 3));
        });
        ThreadManager.Add(t);
        t.Start();
    }

    private void Regrow() {
        //age = 0;
        root = new Node(Vector3.zero, geometryProperties);
        Grow();
    }




    //public void GetEverything(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, ref int[] triangles) {
    //    int index = finishedPointer.GetRead();

    //    if (index == -1) { //return not null arrays, when there weren't any calculations yet
    //        vertices = new Vector3[0];
    //        normals = new Vector3[0];
    //        uvs = new Vector2[0];
    //        triangles = new int[0];
    //    } else {
    //        vertices = this.vertices[index];
    //        normals = this.normals[index];
    //        uvs = this.uvs[index];
    //        triangles = this.triangles[index];
    //    }
    //}

    private void CalculateEverything() {
        twigVertices = 0;
        twigTriangles = 0;
        leafVertices = 0;
        leafTriangles = 0;

        //stores at which index the vertices of a node are stored
        Dictionary<Node, int> nodeVerticesPositions = new Dictionary<Node, int>();

        List<Vector3> verticesTmp = new List<Vector3>();// verticesTmp.Capacity = 5000;
        List<Vector2> uvsTmp = new List<Vector2>();// uvsTmp.Capacity = 5000;
        List<int> trianglesTmp = new List<int>();// trianglesTmp.Capacity = 5000;

        CalculateEverythingHelper(root, nodeVerticesPositions, 0, verticesTmp, uvsTmp, trianglesTmp);


        //int write = finishedPointer.GetWrite();
        ////What happens, when a new node is added while iterating through the TMP List?
        //// -> it won't be drawn
        //this.vertices[write] = new Vector3[verticesTmp.Count];
        //verticesTmp.CopyTo(this.vertices[write]);

        //this.uvs[write] = new Vector2[uvsTmp.Count];
        //uvsTmp.CopyTo(this.uvs[write]);

        //this.triangles[write] = new int[trianglesTmp.Count];
        //trianglesTmp.CopyTo(this.triangles[write]);

        //int read = finishedPointer.GetRead();
        //this.normals[write] = TreeUtil.CalculateNormals(this.vertices[read], this.triangles[read]);

        this.vertices = new Vector3[verticesTmp.Count];
        verticesTmp.CopyTo(this.vertices);

        this.uvs = new Vector2[uvsTmp.Count];
        uvsTmp.CopyTo(this.uvs);

        this.triangles = new int[trianglesTmp.Count];
        trianglesTmp.CopyTo(this.triangles);

        this.normals = TreeUtil.CalculateNormals(this.vertices, this.triangles);
    }

    //looks at the current node, builds cylinders to it's subnodes and recursively calls the function for all subnodes
    private void CalculateEverythingHelper(Node node, Dictionary<Node, int> nodeVerticesPositions, float vOffset, List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        if (node.IsRoot()) {
            CalculateAndStoreCircleVertices(node, nodeVerticesPositions, verticesResult);
            CalculateAndStoreCircleUVs(vOffset, uvsResult);
            vOffset += grower.GetGrowthProperties().GetGrowthDistance(); //TODO: this is a little inaccurate
        }

        int n_subnodes = node.GetSubnodes().Count;
        for (int i=0; i<n_subnodes; i++) {
            Node subnode = node.GetSubnodes()[i];

            //TODO: 0.75 leads to an error
            if (subnode.GetRadius() > geometryProperties.GetMinRadiusRatioForNormalConnection() * node.GetRadius()) { //subnode radius has to be at least x*node.GetRadius() for a usual connection
                //calculate and store vertices
                CalculateAndStoreCircleVertices(subnode, nodeVerticesPositions, verticesResult);

                //calculate and store uvs
                CalculateAndStoreCircleUVs(vOffset, uvsResult);
                vOffset += grower.GetGrowthProperties().GetGrowthDistance(); //TODO: this is a little inaccurate

                //calculate triangles between the node's vertices and the subnode's vertices
                CalculateAndStoreCylinderTriangles(nodeVerticesPositions[node], nodeVerticesPositions[subnode], trianglesResult);

            } else {
                // if this hits and the current node is the root, having all of the children too little of a radius, the root's vertices are stored once too much and must be removed
                // -> this is only a problem for the normal calculations, so it can also be solved there, which is probably easier than to figure out, whether all subnodes of the root are having too little of a radius

                //calculate and store vertices of node oriented towards the subnode and with a smaller radius
                //Node node_ = node.GetGeometryCopyWithNormalAndRadius(subnode.GetPosition() - node.GetPosition(), subnode.GetRadius());
                Node node_ = node.GetGeometryCopyWithNormalAndRadius(subnode.GetDirection(), subnode.GetRadius());
                //Node node_ = new Node(node.GetPosition(), subnode.GetDirection(), subnode.GetRadius(), geometryProperties);
                CalculateAndStoreCircleVertices(node_, nodeVerticesPositions, verticesResult);
                //calculate and store uvs
                vOffset -= grower.GetGrowthProperties().GetGrowthDistance(); //TODO: this is a little inaccurate
                CalculateAndStoreCircleUVs(vOffset, uvsResult);
                vOffset += grower.GetGrowthProperties().GetGrowthDistance(); //TODO: this is a little inaccurate

                //calculate and store vertices
                CalculateAndStoreCircleVertices(subnode, nodeVerticesPositions, verticesResult);
                //calculate and store uvs
                CalculateAndStoreCircleUVs(vOffset, uvsResult);
                vOffset += grower.GetGrowthProperties().GetGrowthDistance(); //TODO: this is a little inaccurate

                //calculate triangles between the node's vertices and the subnode's vertices
                CalculateAndStoreCylinderTriangles(nodeVerticesPositions[node_], nodeVerticesPositions[subnode], trianglesResult);
            }


            //statistics
            int leaf_vertices = verticesResult.Count;
            int leaf_triangles = trianglesResult.Count;

            //calculate and store leaf triangles
            subnode.CalculateAndStoreLeafData(verticesResult, uvsResult, trianglesResult);

            //statistics
            leafVertices += verticesResult.Count - leaf_vertices;
            leafTriangles += trianglesResult.Count - leaf_triangles;

            //recursive call
            CalculateEverythingHelper(subnode, nodeVerticesPositions, vOffset, verticesResult, uvsResult, trianglesResult);
        }
    }

    private void CalculateAndStoreCircleVertices(Node node, Dictionary<Node, int> nodeVerticesPostions, List<Vector3> verticesResult) {
        //store the position of the node's vertices (the position in the 'global' vertices array)
        nodeVerticesPostions[node] = verticesResult.Count;
        //calculate vertices of the current node
        node.GetCircleVertices(verticesResult);

        //statistics
        twigVertices += verticesResult.Count - nodeVerticesPostions[node];
    }

    //private void CalculateAndStoreCylinderVertices(Node node, Dictionary<Node, int> nodeVerticesPostions, List<Vector3> verticesResult, float radius) {

    //    //store the position of the node's vertices (the position in the 'global' vertices array)
    //    nodeVerticesPostions[node] = verticesResult.Count;
    //    //calculate vertices of the current node
    //    node.GetCircleVertices(verticesResult, radius);
    //}

    //TODO: u in Abhängigkeit von Radius?
    //TODO: v kontinuierlich und abhängig von der growthDistance -> uPointer oder sowas, Distancen zu vorherigen Nodes benötigt...
    private void CalculateAndStoreCircleUVs(float v, List<Vector2> uvsResult) {
        //float circle_segment_size = 0.5f / geometryProperties.GetCircleResolution();
        //float circle_segment_size = 0.4f / geometryProperties.GetCircleResolution();

        float u = 0.1f;
        for (int i = 0; i < geometryProperties.GetCircleResolution() + 1; i++) {
            Vector2 uv = new Vector2(u, v);
            uvsResult.Add(uv);

            //u += circle_segment_size;
        }
    }

    private void CalculateAndStoreCylinderTriangles(int from, int to, List<int> trianglesResult) {
        //statistics
        int triangles_ = trianglesResult.Count;

        TreeUtil.CalculateCylinderTriangles(trianglesResult, from, to, geometryProperties.GetCircleResolution());

        //statistics
        twigTriangles += trianglesResult.Count - triangles_;
    }



    //#######################################################################################
    //##########                     INTERFACE IMPLEMENTIATION                     ##########
    //#######################################################################################

    public void OnAttractionPointsChanged() {
        //grower.GetGrowthProperties().ResetTropisms();
        //grower.GetGrowthProperties().ResetAttractionPoints();
        treeCreator.OnAttractionPointsChanged();
        Regrow();
    }

    public void OnAgeChanged() {
        //grower.GetGrowthProperties().ResetTropisms();
        Regrow();
    }

    public void OnIterationFinished() {
        CalculateEverything();
        //finishedPointer.Done();
        treeCreator.OnMeshReady(this.vertices, this.normals, this.uvs, this.triangles);
    }
}