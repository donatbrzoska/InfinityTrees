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
    

    private Node supernode;
    private List<Node> subnodes = new List<Node>();
    private List<Leaf> leaves = new List<Leaf>();


    //THIS SHOULD ONLY BE USED BY THE NEAREST NODE ALGORITHM
    public Node(Vector3 position) {
        this.position = position;
    }

    //public Node(Vector3 position, Vector3 normal, float radius, GeometryProperties geometryProperties) {
    //    this.position = new Vector3(position.x, position.y, position.z);
    //    this.normal = new Vector3(normal.x, normal.y, normal.z);
    //    this.radius = radius;
    //    this.geometryProperties = geometryProperties;
    //}

    public Node(Vector3 position, GeometryProperties geometryProperties) : this(position, null, geometryProperties) { }

    private Node(Vector3 position, Node supernode, GeometryProperties geometryProperties) {
        this.position = position;
        this.supernode = supernode;
        this.radius = geometryProperties.GetTipRadius();
        this.geometryProperties = geometryProperties;

        CalculateNormal();
    }

    //private void UpdateLeaves() {
    //    if (leaves.Count < geometryProperties.GetLeavesPerNode()) {
    //        AddLeaves(geometryProperties.GetLeavesPerNode() - leaves.Count);
    //    } else if (leaves.Count > geometryProperties.GetLeavesPerNode()) {
    //        RemoveLeaves(leaves.Count - geometryProperties.GetLeavesPerNode());
    //    }
    //}

    //adds leavesPerNode leaves
    public void AddLeaves(float leavesPerNode) {
        int integer_part = (int)leavesPerNode;
        float floating_part = leavesPerNode - integer_part;

        float r = Util.RandomInRange(0, 1);
        if (r <= floating_part) {
            integer_part++;
        }

        for (int i = 0; i < integer_part; i++) {
            leaves.Add(new Leaf(position, geometryProperties));
        }
    }

    //removes leavesPerNode leaves
    //private void RemoveLeaves(float leavesPerNode) {
    //    int integer_part = (int)leavesPerNode;
    //    float floating_part = leavesPerNode - integer_part;

    //    float r = Util.RandomInRange(0, 1);
    //    if (r <= floating_part) {
    //        integer_part++;
    //    }

    //    for (int i = 0; i < integer_part; i++) {
    //        leaves.RemoveAt(leaves.Count-1);
    //    }
    //}

    private Node(Vector3 position, Vector3 normal, float radius, GeometryProperties geometryProperties) {
        this.position = position;
        this.normal = normal;
        this.radius = radius;
        this.geometryProperties = geometryProperties;
    }

    public Node GetGeometryCopyWithNormalAndRadius(Vector3 normal, float radius) {
        return new Node(this.position, normal, radius, geometryProperties);
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
        RecalculateRadii();

        return completeNode;
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

    public Vector3 GetNormal() {
        return normal;
    }

    public Vector3 GetDirection() {
        if (this.IsRoot()) {
            return Vector3.up;
        } else {
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
                //normal = subnodes[0].position - position; //vector from this to subnode

                normal = Vector3.up;
            } else { //many subnodes

                //normal = Vector3.zero;
                //foreach (Node subnode in subnodes) {
                //    normal = normal + subnode.GetPosition() - position;
                //}

                //normal = Vector3.up;

                normal = Vector3.up * this.radius;
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
                Vector3 superToThis = position - supernode.GetPosition();
                Vector3 thisToSub = subnodes[0].GetPosition() - position;
                //normal = thisToSub.normalized + superToThis.normalized;
                normal = thisToSub + superToThis;
            } else { //many subnodes
                // #####
                // TODO: SOMETHING IS WRONG HERE MAKING THE NORMALS FLIP OVER SOMETIMES
                // #####

                //normal = supernode.GetNormal();

                //normal = Vector3.zero;

                //normal = subnodes[0].GetPosition() - position;
                //foreach (Node subnode in subnodes) {
                //    normal = normal + subnode.GetPosition() - position;
                //}

                //normal = position - supernode.GetPosition();

                normal = (position - supernode.GetPosition()) * this.radius;
                foreach (Node subnode in subnodes) {
                    normal = normal + (subnode.GetPosition() - position) * subnode.GetRadius();
                }
            }
        }
    }

    public void RecalculateRadii() {
        float summedPottedSubnodeRadii = 0;
        foreach (Node subnode in subnodes) {
            summedPottedSubnodeRadii += (float) Math.Pow(subnode.GetRadius(), geometryProperties.GetNthRoot());
        }

        radius = (float) Math.Pow(summedPottedSubnodeRadii, 1f/geometryProperties.GetNthRoot());

        if (!this.IsRoot()) {
            supernode.RecalculateRadii();
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

    public void CalculateAndStoreLeafData(List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        if (geometryProperties.GetLeavesEnabled()) {
            if (radius < geometryProperties.GetMaxTwigRadiusForLeaves()) {
                float displayedLeavesPerNode = geometryProperties.GetDisplayedLeavesPerNode();
                int integerPart = (int)displayedLeavesPerNode;
                float floatingPart = displayedLeavesPerNode - integerPart;

                float r = Util.RandomInRange(0, 1);
                if (r <= floatingPart) {
                    integerPart++;
                }

                //debug("adding " + integerPart + " leaves");

                for (int i=0; i<integerPart; i++) {

                    //TODO: this is a hotfix, 2 leaves should have been calculated, 2 leaves should be displayed at max - at the time
                    if (i > leaves.Count - 1) {
                        break;
                    }

                    leaves[i].CalculateAndStoreGeometry(verticesResult, uvsResult, trianglesResult);
                }
            }
        }
    }

    public bool Equals(Node other) {
        return (other!=null) && this.position.Equals(other.position);
    }

    //#######################################################################################
    //##########                     GEOMETRY PROPERTIES OBSERVER                  ##########
    //#######################################################################################

    public void OnLeafTypeChanged() {
        // do nothing
    }

    public void OnLeavesPerNodeChanged() {
        //UpdateLeaves();
    }

    public void OnLeavesEnabledChanged() {
        // do nothing
    }

    public void OnLeafSizeChanged() {
        // do nothing
    }
}