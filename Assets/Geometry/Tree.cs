using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tree {

    private static bool debugEnabled = false ;
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

    private GeometryProperties geometryProperties;

    public Node StemRoot { get; set; }

    public Tree(GeometryProperties geometryProperties) {
        this.geometryProperties = geometryProperties;
        Initialize();
    }
    private void Initialize() {
        StemRoot = new Node(Vector3.zero, geometryProperties);
	}

    public void Reset() {
        Initialize();
    }

    private void MakePendulous(Node node, int depth) {
        foreach (Node subnode in node.Subnodes) {
            if (depth >= geometryProperties.PendulousBranchesBeginDepth) {
                Vector3 d_pos = subnode.Position - node.Position;
                float d_angle = Vector3.Angle(d_pos, Vector3.down);

                // d_angle should have a max value of 180
                // .. in this case, gravity is not having an impact (hypothesis) -> rotation_angle should be 0
                // .. otherwise HangingBranchesIntensity says, how much of an impact the gravity has


                // it should be rotated by a _fraction_ of d_angle
                // - this _fraction_ is defined through someFactor * d_angle
                // - - someFactor is 1.0, when d_angle is 0
                // - - someFactor is 0.5, when d_angle is 90
                // - - someFactor is 0.0, when d_angle is 180
                // - > someFactor = 1 - (d_angle/180)
                float someFactor = 1 - (d_angle / 180);

                // - HaningBranchesIntensity just says how much of this _fraction_ will actually be applied as rotation angle
                float rotation_angle =  geometryProperties.PendulousBranchesIntensity * someFactor * d_angle;
                // rotation angle is also 0, if d_angle is 0 (when no more hang is needed)
                // -> because no matter how big of a fraction you will take (of d_angle) no rotation will be performed

                //rotate d_pos towards the (0, -1, 0) Vector3
                Quaternion rotation = Quaternion.AngleAxis(rotation_angle, Vector3.Cross(d_pos, Vector3.down));

                //rotate the subnodes inclusive all its subnodes
                subnode.Rotate(node.Position, rotation);
            }

            MakePendulous(subnode, depth+1);
        }
    }

    public void GetMesh(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> uvs, ref List<int> triangles) {
        //stores at which index the vertices of a node are stored
        Dictionary<Node, int> nodeVerticesPositions = new Dictionary<Node, int>();

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        triangles = new List<int>();

        if (geometryProperties.PendulousBranchesIntensity > 0) {
            Node copy = StemRoot.GetDeepCopyWithSupernode(null);
            MakePendulous(copy, 0);
            CalculateEverything(copy, nodeVerticesPositions, 0, vertices, uvs, triangles);
        } else {
            CalculateEverything(StemRoot, nodeVerticesPositions, 0, vertices, uvs, triangles);
        }

        normals = TreeUtil.CalculateNormals(vertices, triangles);
    }

    //looks at the current node, builds cylinders to it's subnodes and recursively calls the function for all subnodes
    private void CalculateEverything(Node node, Dictionary<Node, int> nodeVerticesPositions, float vOffset, List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        if (node.IsRoot() && node.HasSubnodes()) { //if the tree is empty (latter condition) the calculation is senseless
            GetCircleVertices(node, nodeVerticesPositions, verticesResult);
            GetCircleUVs(vOffset, uvsResult);
            vOffset += Vector3.Distance(node.Position, node.Subnodes[0].Position); //TODO: this is a little inaccurate
        }

        int n_subnodes = node.Subnodes.Count;
        for (int i=0; i<n_subnodes; i++) {
            Node subnode = node.Subnodes[i];

            if (geometryProperties.UsualConnection(node.Radius, subnode.Radius)) { //subnode radius has to be at least x*node.GetRadius() for a usual connection
                //calculate and store vertices
                GetCircleVertices(subnode, nodeVerticesPositions, verticesResult);

                //calculate and store uvs
                //bool tipNode = !subnode.HasSubnodes(); //close tips
                //CalculateAndStoreCircleUVs(vOffset, uvsResult, tipNode);
                GetCircleUVs(vOffset, uvsResult);
                vOffset += Vector3.Distance(node.Position, subnode.Position); //TODO: this is a little inaccurate

                //calculate triangles between the node's vertices and the subnode's vertices
                ////bool tipNode = !subnode.HasSubnodes(); //close tips
                //CalculateAndStoreCylinderTriangles(nodeVerticesPositions[node], nodeVerticesPositions[subnode], trianglesResult, tipNode);
                TreeUtil.CalculateCylinderTriangles(trianglesResult, nodeVerticesPositions[node], nodeVerticesPositions[subnode], geometryProperties.CircleResolution, false);

            } else {
                // if this hits and the current node is the root, having all of the children too little of a radius, the root's vertices are stored once too much and must be removed
                // -> this is only a problem for the normal calculations, so it can also be solved there, which is probably easier than to figure out, whether all subnodes of the root are having too little of a radius

                //calculate and store vertices of node oriented towards the subnode and with a smaller radius
                Node node_ = node.GetGeometryCopyWithNormalAndRadius(subnode.GetDirection(), subnode.Radius);
                //Node node_ = new Node(node.GetPosition(), subnode.GetDirection(), subnode.GetRadius(), geometryProperties);
                GetCircleVertices(node_, nodeVerticesPositions, verticesResult);

                //calculate and store uvs
                vOffset -= Vector3.Distance(node.Position, subnode.Position); //TODO: this is a little inaccurate
                GetCircleUVs(vOffset, uvsResult);
                vOffset += Vector3.Distance(node.Position, subnode.Position); //TODO: this is a little inaccurate



                //calculate and store vertices
                GetCircleVertices(subnode, nodeVerticesPositions, verticesResult);

                //calculate and store uvs
                //bool tipNode = !subnode.HasSubnodes(); //close tips
                //CalculateAndStoreCircleUVs(vOffset, uvsResult, tipNode);
                GetCircleUVs(vOffset, uvsResult);
                vOffset += Vector3.Distance(node.Position, subnode.Position); //TODO: this is a little inaccurate

                //calculate triangles between the node's vertices and the subnode's vertices
                ////bool tipNode = !subnode.HasSubnodes(); //close tips
                //CalculateAndStoreCylinderTriangles(nodeVerticesPositions[node_], nodeVerticesPositions[subnode], trianglesResult, tipNode);
                TreeUtil.CalculateCylinderTriangles(trianglesResult, nodeVerticesPositions[node_], nodeVerticesPositions[subnode], geometryProperties.CircleResolution, false);
            }


            //calculate and store leaf triangles
            subnode.GetLeafMesh(verticesResult, uvsResult, trianglesResult);


            //recursive call
            CalculateEverything(subnode, nodeVerticesPositions, vOffset, verticesResult, uvsResult, trianglesResult);
        }
    }

    private void GetCircleVertices(Node node, Dictionary<Node, int> nodeVerticesPostions, List<Vector3> verticesResult) {
        //store the position of the node's vertices (the position in the 'global' vertices array)
        nodeVerticesPostions[node] = verticesResult.Count;
        //calculate vertices of the current node
        node.GetCircleVertices(verticesResult);
    }

    //TODO: u in Abhängigkeit von Radius?
    //TODO: v kontinuierlich und abhängig von der growthDistance -> uPointer oder sowas, Distancen zu vorherigen Nodes benötigt...
    private void GetCircleUVs(float v, List<Vector2> uvsResult, bool tipNode=false) {
        if (tipNode) {
            uvsResult.Add(new Vector2(v, 0.25f));
        } else {
            float circle_segment_size = 0.49f / geometryProperties.CircleResolution;
            //float circle_segment_size = 0.4f / geometryProperties.GetCircleResolution();

            //float u = 0.1f; //this way the color is picked from the mid of the stem texture and not from the leaf part
            float u = 0.01f;
            for (int i = 0; i < geometryProperties.CircleResolution + 1; i++) {
                Vector2 uv = new Vector2(u, v);
                uvsResult.Add(uv);

                u += circle_segment_size;
            }
        }
    }
}
