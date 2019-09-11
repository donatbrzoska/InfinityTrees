using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Node : IEquatable<Node> {

    private static bool debugEnabled = true;
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: Node: " + callerName + "(): " + message);
        }
    }

    private static void debug(FormatString formatString, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: Node: " + callerName + "(): " + formatString);
        }
    }
    private static System.Random random = new System.Random();

    private GeometryProperties geometryProperties;

    public Vector3 Position { get; private set; }

    public void UpdatePosition(Vector3 diff) {
        Position = Position + diff;
        foreach (Node n in Subnodes) {
            n.UpdatePosition(diff);
        }

        foreach (Leaf l in Leaves) {
            l.UpdatePosition(diff);
        }
    }

    public Vector3 GetDirection(bool normalized = false) {
        if (this.IsRoot()) {
            return Vector3.up;
        } else {
            if (normalized) {
                return (Position - supernode.Position).normalized;
            }
            return Position - supernode.Position;
        }
    }


    private Vector3 Normal { get; set; }

    private void CalculateNormal() {
        if (supernode == null) { //if this is the root node
            if (!this.HasSubnodes()) { //no subnodes
                //point upwards
                Normal = Vector3.up;
            } else if (Subnodes.Count == 1) { //one subnode
                //point to subnode
                //Normal = Subnodes[0].Position - Position; //vector from this to subnode

                Normal = Vector3.up;
            } else { //many subnodes
                Normal = Vector3.zero;
                foreach (Node subnode in Subnodes) {
                    Normal = Normal + (subnode.Position - Position) * subnode.Radius;
                }
            }
        } else {
            if (!this.HasSubnodes()) { //no subnodes
                //point in own direction
                Normal = Position - supernode.Position; //vector from supernode to this
            } else if (Subnodes.Count == 1) { //one subnode
                //find tangent between(vector pointing from super to this, vector pointing from this to sub)
                //Vector3 superToThis = position - supernode.GetPosition();
                //Vector3 thisToSub = subnodes[0].GetPosition() - position;
                //normal = thisToSub + superToThis;
                Normal = GetDirection() + Subnodes[0].GetDirection();
            } else { //many subnodes
                Normal = Vector3.zero;
                foreach (Node subnode in Subnodes) {
                    if (geometryProperties.UsualConnection(this.Radius, subnode.Radius)) {
                        Normal = Normal + (subnode.Position - Position) * subnode.Radius;// * subnode.GetRadius();
                    }
                }
            }
        }
    }

    public float Radius { get; private set; }

    public void RecalculateRadii() {
        if (this.HasSubnodes()) { // signal upwards
            foreach (Node sn in Subnodes) {
                sn.RecalculateRadii();
            }
        } else { //signal downwards begin
            Radius = geometryProperties.TipRadius;

            supernode.RecalculateRadius();
        }
    }

    private void RecalculateRadius() {
        float summedPottedSubnodeRadii = 0;
        foreach (Node subnode in Subnodes) {
            summedPottedSubnodeRadii += (float)Math.Pow(subnode.Radius, geometryProperties.NthRoot);
        }

        Radius = (float)Math.Pow(summedPottedSubnodeRadii, 1f / geometryProperties.NthRoot);

        if (!this.IsRoot()) {
            supernode.RecalculateRadius();
        }
    }


    private Node supernode;
    public bool IsRoot() {
        return supernode == null;
    }


    public List<Node> Subnodes { get; private set; } = new List<Node>();

    public bool HasSubnodes() {
        return Subnodes.Count != 0;
    }


    public List<Leaf> Leaves { get; private set; } = new List<Leaf>();



    public Node(Vector3 position, GeometryProperties geometryProperties) : this(position, null, geometryProperties) { }

    private Node(Vector3 position, Node supernode, GeometryProperties geometryProperties) {
        this.Position = position;
        this.supernode = supernode;
        this.Radius = geometryProperties.TipRadius;
        this.geometryProperties = geometryProperties;

        CalculateNormal();
        AddLeaves();
    }



    //only used by nearest node algorithm
    public Node(Vector3 position) {
        this.Position = position;
    }



    //only gets used by the method below
    private Node(Vector3 position, Vector3 normal, float radius, GeometryProperties geometryProperties) {
        this.Position = position;
        this.Normal = normal;
        this.Radius = radius;
        this.geometryProperties = geometryProperties;
    }
    //used for geometry data generation
    public Node GetGeometryCopyWithNormalAndRadius(Vector3 normal, float radius) {
        return new Node(this.Position, normal, radius, geometryProperties);
    }



    //only gets used by the method below -> deep copy
    private Node(Node supernode) {
        this.supernode = supernode;
    }
    //used for deep copy
    public Node GetDeepCopyWithSupernode(Node supernode) {
        Node copy = new Node(supernode);
        copy.Position = Position;
        copy.Normal = Normal;
        copy.Radius = Radius;
        copy.geometryProperties = geometryProperties;

        List<Node> subnodes_ = new List<Node>();
        for (int i=0; i< Subnodes.Count; i++) {
            subnodes_.Add(Subnodes[i].GetDeepCopyWithSupernode(copy));
        }
        copy.Subnodes = subnodes_;
        
        List<Leaf> leaves_ = new List<Leaf>();
        foreach (Leaf leaf in Leaves) {
            leaves_.Add(leaf.GetCopy());
        }
        copy.Leaves = leaves_;

        return copy;
    }



    //adds leavesPerNode leaves
    private void AddLeaves() {
        for (int i = 0; i < geometryProperties.DisplayedLeafesPerNodeMaximum; i++) {
            Vector3 leafPosition;
            if (supernode == null) {
                leafPosition = Position;
            } else {
                Vector3 d = Position - supernode.Position;
                leafPosition = supernode.Position + Util.RandomInRange(0, 1, random) * d;
            }
            Leaves.Add(new Leaf(leafPosition, geometryProperties));
        }
    }


    public Node Add(Vector3 position) {
        Node completeNode = new Node(position, this, geometryProperties);

        //the following needs to be one atomic step, because as soon as there is a new subnode, the current normal is not valid anymore
        // and when GetVertices() would get called in between, there would be an invalid result
        lock (this) { //the corresponding lock is located at GetVertices()
            //add the node
            Subnodes.Add(completeNode);

            //recalculate the normal
            CalculateNormal();
        }
        RecalculateRadius();

        return completeNode;
    }

    //only used for transfering the old crownRoots subodes to the new stem
    public void Add(Node node) {
        node.supernode = this; // when adding the nodes like this, their supernode reference has to be updated manually

        //the following needs to be one atomic step, because as soon as there is a new subnode, the current normal is not valid anymore
        // and when GetVertices() would get called in between, there would be an invalid result
        lock (this) { //the corresponding lock is located at GetVertices()
            //add the node
            Subnodes.Add(node);

            //recalculate the normal
            CalculateNormal();
        }
        RecalculateRadius();
    }



    public void Rotate(Vector3 byPoint, Quaternion quaternion) {
        Vector3 d = Position - byPoint;
        Vector3 direction = quaternion * d;
        Position = byPoint + direction;

        foreach (Node subnode in Subnodes) {
            subnode.Rotate(byPoint, quaternion);
            this.CalculateNormal();
        }

        foreach (Leaf l in Leaves) {
            l.Rotate(byPoint, quaternion);
        }
    }




    public void GetCircleVertices(List<Vector3> verticesResult) {
        lock (this) {
            TreeUtil.GetCircleVertices(verticesResult, Position, Normal, Radius, geometryProperties.CircleResolution);
        }
    }


    public void GetLeafMesh(List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        if (Radius < geometryProperties.MaxTwigRadiusForLeaves) {
            //if (!HasSubnodes()) {
            int n_leaves = (int)geometryProperties.DisplayedLeavesPerNode;
            float floatingRest = geometryProperties.DisplayedLeavesPerNode - n_leaves;

            float r = Util.RandomInRange(0,1);
            if (r <= floatingRest) {
                n_leaves++;
            }

            for (int i = 0; i < n_leaves; i++) {

                //TODO: this is a hotfix, 2 leaves should have been calculated, 2 leaves should be displayed at max - at the time
                if (i > Leaves.Count - 1) {
                    break;
                }

                Leaves[i].GetMesh(verticesResult, uvsResult, trianglesResult);
            }
            //}
        }
    }

    public bool Equals(Node other) {
        return (other!=null) && this.Position.Equals(other.Position);
    }
}