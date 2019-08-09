using System.Collections.Generic;
using System.Runtime.CompilerServices;
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


    // Stammlänge ändern:
    // - Stamm muss auf Basis der neuen Stammlänge neu generiert werden (möglichst gleicher Seed)
    // - alle Nodes müssen verschoben werden
    //
    // alte CrownRoot muss letzter Node des Stammes sein
    //public float StemLength { get; private set; }
    //public void UpdateStemLength(float value) {
    //    StemLength = value;
    //    GrowStem();
    //}
    //private float stemStepSize;
    private GeometryProperties geometryProperties;

    public Node CrownRoot { get; set; }
    public Node StemRoot { get; set; }
    //public Node StemTip { get; set; }

    public Tree(/*float stemLength, float stemStepSize, */GeometryProperties geometryProperties) {
        //this.StemLength = stemLength;
        //this.stemStepSize = stemStepSize;
        this.geometryProperties = geometryProperties;
        Initialize();
    }
    private void Initialize() {
        StemRoot = new Node(Vector3.zero, geometryProperties);
        //GrowStem();
	}

    public void Reset() {
        Initialize();
    }

    //// wo die AttractionPoints positioniert sind, hängt vom Endpunkt des Stammes ab
    //private void GrowStem() {
    //    float angleRange = 10;

    //    int iterations = (int) (StemLength / stemStepSize);
    //    for (int i=0; i<iterations; i++) {
    //        float angle = Util.RandomInRange(-angleRange, angleRange);
    //        Vector3 axis = Util.RandomVector3();
    //        axis.y = 0;

    //        if (StemRoot.HasSubnodes()) {
    //            Vector3 direction = Quaternion.AngleAxis(angle, axis) * StemTip.GetDirection(true);
    //            Node newNode = StemTip.Add(StemTip.GetPosition() + direction * stemStepSize);
    //            StemTip = newNode;
    //        } else {
    //            Vector3 direction = Quaternion.AngleAxis(angle, axis) * StemRoot.GetDirection(true);
    //            Node newNode = StemRoot.Add(StemRoot.GetPosition() + direction * stemStepSize);
    //            StemTip = newNode;
    //        }
    //    }

    //    float rest = StemLength % stemStepSize;
    //    if (!Util.AlmostEqual(rest, 0)) {
    //        float angle = Util.RandomInRange(-angleRange, angleRange);
    //        Vector3 axis = Util.RandomVector3();
    //        axis.y = 0;

    //        if (StemRoot.HasSubnodes()) {
    //            Vector3 direction = Quaternion.AngleAxis(angle, axis) * StemTip.GetDirection(true);
    //            Node newNode = StemTip.Add(StemTip.GetPosition() + direction * rest);
    //            StemTip = newNode;
    //        } else {
    //            Vector3 direction = Quaternion.AngleAxis(angle, axis) * StemRoot.GetDirection(true);
    //            Node newNode = StemRoot.Add(StemRoot.GetPosition() + direction * rest);
    //            StemTip = newNode;
    //        }
    //    }

    //    CrownRoot = StemTip;
    //}

	private int twigVertices;
	private int twigTriangles;
	private int leafVertices;
	private int leafTriangles;

	public void GetMesh(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, ref int[] triangles) {
        twigVertices = 0;
        twigTriangles = 0;
        leafVertices = 0;
        leafTriangles = 0;

        //stores at which index the vertices of a node are stored
        Dictionary<Node, int> nodeVerticesPositions = new Dictionary<Node, int>();

        List<Vector3> verticesTmp = new List<Vector3>();// verticesTmp.Capacity = 5000;
        List<Vector2> uvsTmp = new List<Vector2>();// uvsTmp.Capacity = 5000;
        List<int> trianglesTmp = new List<int>();// trianglesTmp.Capacity = 5000;

        CalculateEverything(StemRoot, nodeVerticesPositions, 0, verticesTmp, uvsTmp, trianglesTmp);
        //CalculateEverything(CrownRoot, nodeVerticesPositions, 0, verticesTmp, uvsTmp, trianglesTmp);


        vertices = new Vector3[verticesTmp.Count];
        verticesTmp.CopyTo(vertices);

        uvs = new Vector2[uvsTmp.Count];
        uvsTmp.CopyTo(uvs);

        triangles = new int[trianglesTmp.Count];
        trianglesTmp.CopyTo(triangles);

        normals = TreeUtil.CalculateNormals(vertices, triangles);



        debug(new FormatString("{0} vertices, {1} triangles", vertices.Length, triangles.Length / 3));
        debug(new FormatString("{0} twig vertices, {1} twig triangles", twigVertices, twigTriangles / 3));
        debug(new FormatString("{0} leaf vertices, {1} leaf triangles", leafVertices, leafTriangles / 3));
    }

    private float segmentLength;

    //looks at the current node, builds cylinders to it's subnodes and recursively calls the function for all subnodes
    private void CalculateEverything(Node node, Dictionary<Node, int> nodeVerticesPositions, float vOffset, List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        if (node.IsRoot()) {
            CalculateAndStoreCircleVertices(node, nodeVerticesPositions, verticesResult);
            CalculateAndStoreCircleUVs(vOffset, uvsResult);
            if (node.HasSubnodes()) {
                segmentLength = Vector3.Distance(node.GetPosition(), node.GetSubnodes()[0].GetPosition());
            }
            vOffset += segmentLength; //TODO: this is a little inaccurate
        }

        int n_subnodes = node.GetSubnodes().Count;
        for (int i=0; i<n_subnodes; i++) {
            Node subnode = node.GetSubnodes()[i];

            if (subnode.GetRadius() > geometryProperties.GetMinRadiusRatioForNormalConnection() * node.GetRadius()) { //subnode radius has to be at least x*node.GetRadius() for a usual connection
                //calculate and store vertices
                CalculateAndStoreCircleVertices(subnode, nodeVerticesPositions, verticesResult);

                //calculate and store uvs
                CalculateAndStoreCircleUVs(vOffset, uvsResult);
                vOffset += segmentLength; //TODO: this is a little inaccurate

                //calculate triangles between the node's vertices and the subnode's vertices
                CalculateAndStoreCylinderTriangles(nodeVerticesPositions[node], nodeVerticesPositions[subnode], trianglesResult);

            } else {
                // if this hits and the current node is the root, having all of the children too little of a radius, the root's vertices are stored once too much and must be removed
                // -> this is only a problem for the normal calculations, so it can also be solved there, which is probably easier than to figure out, whether all subnodes of the root are having too little of a radius

                //calculate and store vertices of node oriented towards the subnode and with a smaller radius
                Node node_ = node.GetGeometryCopyWithNormalAndRadius(subnode.GetDirection(), subnode.GetRadius());
                //Node node_ = new Node(node.GetPosition(), subnode.GetDirection(), subnode.GetRadius(), geometryProperties);
                CalculateAndStoreCircleVertices(node_, nodeVerticesPositions, verticesResult);
                //calculate and store uvs
                vOffset -= segmentLength; //TODO: this is a little inaccurate
                CalculateAndStoreCircleUVs(vOffset, uvsResult);
                vOffset += segmentLength; //TODO: this is a little inaccurate

                //calculate and store vertices
                CalculateAndStoreCircleVertices(subnode, nodeVerticesPositions, verticesResult);
                //calculate and store uvs
                CalculateAndStoreCircleUVs(vOffset, uvsResult);
                vOffset += segmentLength; //TODO: this is a little inaccurate

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
            CalculateEverything(subnode, nodeVerticesPositions, vOffset, verticesResult, uvsResult, trianglesResult);
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

    //TODO: u in Abhängigkeit von Radius?
    //TODO: v kontinuierlich und abhängig von der growthDistance -> uPointer oder sowas, Distancen zu vorherigen Nodes benötigt...
    private void CalculateAndStoreCircleUVs(float v, List<Vector2> uvsResult) {
        //float circle_segment_size = 0.5f / geometryProperties.GetCircleResolution();
        //float circle_segment_size = 0.4f / geometryProperties.GetCircleResolution();

        float u = 0.1f; //this way the color is picked from the mid of the stem texture and not from the leaf part
        //float u = 0f;
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
}