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

    private GeometryProperties geometryProperties;

    private Vector3 position;
    private Vector3 normal;
    private float radius;
    public bool Active { get; set; }
    

    private Node supernode;
    private List<Node> subnodes = new List<Node>();
    private List<Leaf> leaves = new List<Leaf>();



    public Node(Vector3 position, GeometryProperties geometryProperties) : this(position, null, geometryProperties) { }

    private Node(Vector3 position, Node supernode, GeometryProperties geometryProperties) {
        this.position = position;
        this.supernode = supernode;
        this.radius = geometryProperties.GetTipRadius();
        this.geometryProperties = geometryProperties;

        Active = true;

        CalculateNormal();
        AddLeaves();
    }



    //only used be nearest node algorithm
    public Node(Vector3 position) {
        this.position = position;
    }



    //only gets used by the method below
    private Node(Vector3 position, Vector3 normal, float radius, GeometryProperties geometryProperties) {
        this.position = position;
        this.normal = normal;
        this.radius = radius;
        this.geometryProperties = geometryProperties;
    }
    //used for geometry data generation
    public Node GetGeometryCopyWithNormalAndRadius(Vector3 normal, float radius) {
        return new Node(this.position, normal, radius, geometryProperties);
    }



    //only gets used by the method below -> deep copy
    private Node(Node supernode) {
        this.supernode = supernode;
    }
    private void SetPosition(Vector3 position) {
        this.position = position;
    }
    private void SetNormal(Vector3 normal) {
        this.normal = normal;
    }
    private void SetRadius(float radius) {
        this.radius = radius;
    }
    private void SetGeometryProperties(GeometryProperties geometryProperties) {
        this.geometryProperties = geometryProperties;
    }
    private void SetSubnodes(List<Node> subnodes) {
        this.subnodes = subnodes;
    }
    private void SetLeaves(List<Leaf> leaves) {
        this.leaves = leaves;
    }
    //used for deep copy
    public Node GetCopyWithSupernode(Node supernode) {
        Node copy = new Node(supernode);
        copy.SetPosition(position);
        copy.SetNormal(normal);
        copy.SetRadius(radius);
        copy.SetGeometryProperties(geometryProperties);

        List<Node> subnodes_ = new List<Node>();
        for (int i=0; i<subnodes.Count; i++) {
            subnodes_.Add(subnodes[i].GetCopyWithSupernode(copy));
        }
        copy.SetSubnodes(subnodes_);
        
        List<Leaf> leaves_ = new List<Leaf>();
        foreach (Leaf leaf in leaves) {
            leaves_.Add(leaf.GetCopy());
        }
        copy.SetLeaves(leaves_);

        return copy;
    }



    //adds leavesPerNode leaves
    private void AddLeaves() {
        for (int i = 0; i < geometryProperties.DisplayedLeafesPerNodeMaximum; i++) {
            Vector3 leafPosition;
            if (supernode == null) {
                leafPosition = position;
            } else {
                Vector3 d = this.position - supernode.GetPosition();
                leafPosition = supernode.GetPosition() + Util.RandomInRange(0, 1) * d;
            }
            leaves.Add(new Leaf(leafPosition, geometryProperties));
        }
    }


    public Node Add(Vector3 position) {
        Node completeNode = new Node(position, this, geometryProperties);

        //the following needs to be one atomic step, because as soon as there is a new subnode, the current normal is not valid anymore
        // and when GetVertices() would get called in between, there would be an invalid result
        lock (this) { //the corresponding lock is located at GetVertices()
            //add the node
            subnodes.Add(completeNode);

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
            subnodes.Add(node);

            //recalculate the normal
            CalculateNormal();
        }
        RecalculateRadius();
    }

    public bool IsRoot() {
        return supernode == null;
    }

    public bool HasSubnodes() {
        return subnodes.Count != 0;
    }

    public List<Node> GetSubnodes() {
        return subnodes;
    }

    public Vector3 GetPosition() {
        return position;
    }

    public void UpdatePosition(Vector3 diff) {
        position = position + diff;
        foreach (Node n in subnodes) {
            n.UpdatePosition(diff);
        }

        foreach (Leaf l in leaves) {
            l.UpdatePosition(diff);
        }
    }

    public void Rotate(Vector3 byPoint, Quaternion quaternion) {
        Vector3 d = this.position - byPoint;
        Vector3 direction = quaternion * d;
        this.position = byPoint + direction;

        foreach (Node subnode in subnodes) {
            subnode.Rotate(byPoint, quaternion);
            this.CalculateNormal();
        }

        foreach (Leaf l in leaves) {
            l.Rotate(byPoint, quaternion);
        }
    }

    public Vector3 GetNormal() {
        return normal;
    }

    public Vector3 GetDirection(bool normalized=false) {
        if (this.IsRoot()) {
            return Vector3.up;
        } else {
            if (normalized) {
                return (position - supernode.position).normalized;
            }
            return position - supernode.position;
            //return supernode.position - position;
        }
    }

    private void CalculateNormal() {
        if (supernode == null) { //if this is the root node
            if (!this.HasSubnodes()) { //no subnodes
                //point upwards
                normal = Vector3.up;
            } else if (subnodes.Count == 1) { //one subnode
                //point to subnode
                normal = subnodes[0].position - position; //vector from this to subnode

                //normal = Vector3.up;
            } else { //many subnodes
                normal = Vector3.zero;
                foreach (Node subnode in subnodes) {
                    normal = normal + (subnode.GetPosition() - position) * subnode.GetRadius();
                }
            }
        } else {
            if (!this.HasSubnodes()) { //no subnodes
                //point in own direction
                normal = position - supernode.GetPosition(); //vector from supernode to this
            } else if (subnodes.Count == 1) { //one subnode
                //find tangent between(vector pointing from super to this, vector pointing from this to sub)
                //Vector3 superToThis = position - supernode.GetPosition();
                //Vector3 thisToSub = subnodes[0].GetPosition() - position;
                //normal = thisToSub + superToThis;
                normal = GetDirection() + subnodes[0].GetDirection();
            } else { //many subnodes
                normal = Vector3.zero;
                foreach (Node subnode in subnodes) {
                    normal = normal + (subnode.GetPosition() - position) * subnode.GetRadius();// * subnode.GetRadius();
                }
            }
        }
    }

    public void RecalculateRadii() {
        if (this.HasSubnodes()) { // signal upwards
            foreach (Node sn in subnodes) {
				sn.RecalculateRadii();
			}
		} else { //signal downwards begin
            radius = geometryProperties.GetTipRadius();

            supernode.RecalculateRadius();
		}
	}

	private void RecalculateRadius() {
        float summedPottedSubnodeRadii = 0;
        foreach (Node subnode in subnodes) {
            summedPottedSubnodeRadii += (float) Math.Pow(subnode.GetRadius(), geometryProperties.GetNthRoot());
        }

        radius = (float) Math.Pow(summedPottedSubnodeRadii, 1f/geometryProperties.GetNthRoot());

        if (!this.IsRoot()) {
            supernode.RecalculateRadius();
        }
    }

    public float GetRadius() {
        return radius;
    }

    public void GetCircleVertices(List<Vector3> verticesResult) {
        lock (this) {
            TreeUtil.CalculateAndStoreCircleVertices(verticesResult, position, normal, radius, geometryProperties.GetCircleResolution());
        }
    }

    private static System.Random random = new System.Random();

    public void CalculateAndStoreLeafData(List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        //if (geometryProperties.GetLeavesEnabled()
        if (radius < geometryProperties.GetMaxTwigRadiusForLeaves()) {
            //if (!HasSubnodes()) {
            int n_leaves = (int)geometryProperties.GetDisplayedLeavesPerNode();
            float floatingRest = geometryProperties.GetDisplayedLeavesPerNode() - n_leaves;

            float r = (float)random.NextDouble();//Util.RandomInRange(0, 1);
            if (r <= floatingRest) {
                n_leaves++;
            }

            //debug("displayed leaves per node: " + n_leaves);

            for (int i = 0; i < n_leaves; i++) {

                //TODO: this is a hotfix, 2 leaves should have been calculated, 2 leaves should be displayed at max - at the time
                if (i > leaves.Count - 1) {
                    break;
                }

                leaves[i].CalculateAndStoreGeometry(verticesResult, uvsResult, trianglesResult);
            }
        }
        //}
    }

    public bool Equals(Node other) {
        return (other!=null) && this.position.Equals(other.position);
    }
}