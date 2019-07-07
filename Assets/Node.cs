using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Node : IEquatable<Node>{

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

    private GrowthProperties growthProperties;

    private Vector3 position;
    private Vector3 normal;
    private float radius;

    private Node supernode;
    private List<Node> subnodes = new List<Node>();

    public Node(Vector3 position, GrowthProperties growthProperties) : this(position, 0, null, growthProperties) { }

    private Node(Vector3 position, float radius, Node supernode, GrowthProperties growthProperties) {
        this.position = position;
        this.radius = radius;
        this.supernode = supernode;

        this.growthProperties = growthProperties;

        CalculateNormal();
    }

    private bool IsDuplicateNode(Vector3 position) {
        foreach (Node subnode in subnodes) {
            if (subnode.GetPosition().Equals(position)) {
                //debug("given node is already a subnode");
                return true;
            }
        }

        return false;
    }

    private bool IsGrowingBackwards(Vector3 position) {
        Vector3 thisNodeToNewNode = position - this.position;

        Vector3 superNodeToThisNode;
        if (supernode != null) {
            superNodeToThisNode = this.position - supernode.GetPosition();
        } else {
            superNodeToThisNode = Vector3.up;
        }

        float angle = Vector3.Angle(thisNodeToNewNode, superNodeToThisNode);

        if (angle > growthProperties.GetMaxBranchingAngle()) {
            //debug("backwards growing twig detected");
            return true;
        }

        return false;
    }

    //WRITE: this is way better than a minimum Angle to all other nodes (apart from the fact how one would calculate this)
    private bool IsTooClose(Vector3 position) {
        //lock (this) { //TODO: is this lock necessary? (no subnode should be added while looping through this)
            foreach (Node subnode in subnodes) {
                float distance = Vector3.Distance(position, subnode.GetPosition());
                if (distance < growthProperties.GetMinDistanceToExistingNodes()) {
                    //debug("node too close to an existing node");
                    return true;
                }
            }

        return false;
        //}
    }

    public bool Add(Vector3 position) {
        //make sure, that the new node does not exist yet, doesn't grow "backwards", and not too close to an existing branch
        if (!IsDuplicateNode(position)
            && !IsGrowingBackwards(position)
            && !IsTooClose(position)) {

            //the following needs to be one atomic step, because as soon as there is a new subnode, the current normal is not valid anymore
            // and when GetVertices() would get called in between, there would be an invalid result
            lock (this) { //the corresponding lock is located at GetVertices()
                //Node completeNode = new Node(position, this.radius * radiusRatio, this);
                Node completeNode = new Node(position, growthProperties.GetTipRadius(), this, growthProperties);

                //3. add the node
                subnodes.Add(completeNode);
                //debug("added node at " + position.ToString());

                //4. recalculate the normal
                CalculateNormal();


                RecalculateRadii(); //TODO: put outside lock?
            }
            return true;
        } else {
            return false;
        }
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

    private void CalculateNormal() {
        if (supernode == null) { //if this is the root node
            if (!this.HasSubnodes()) { //no subnodes
                //point upwards
                normal = Vector3.up;
            } else if (subnodes.Count == 1) { //one subnode
                //point to subnode
                normal = ((Node)subnodes[0]).position - position; //vector from this to subnode
            } else { //many subnodes
                //TODO: point to the thickest subnode the most?
                normal = Vector3.zero;
                foreach (Node subnode in subnodes) {
                    normal = normal + subnode.GetPosition() - position;
                }
            }
        } else {
            if (!this.HasSubnodes()) { //no subnodes
                //point in own direction
                normal = position - supernode.GetPosition(); //vector from supernode to this
            } else if (subnodes.Count == 1) { //one subnode
                //find tangent between(vector pointing from super to this, vector pointing from this to sub)
                Vector3 superToThis = position - supernode.GetPosition();
                Vector3 thisToSub = ((Node)subnodes[0]).GetPosition() - position;
                normal = thisToSub.normalized + superToThis.normalized;
            } else { //many subnodes
                //TODO: point to the thickest subnode the most?
                //normal = supernode.GetNormal();
                normal = Vector3.zero;
                foreach (Node subnode in subnodes) {
                    normal = normal + subnode.GetPosition() - position;
                }
            }
        }
    }

    public void RecalculateRadii() {

        float summedPottedSubnodeRadii = 0;
        foreach (Node subnode in subnodes) {
            summedPottedSubnodeRadii += (float) Math.Pow(subnode.GetRadius(), growthProperties.GetNthRoot());
        }

        radius = (float) Math.Pow(summedPottedSubnodeRadii, 1f/growthProperties.GetNthRoot());

        if (!this.IsRoot()) {
            supernode.RecalculateRadii();
        }
    }

    public float GetRadius() {
        return radius;
    }


    public List<Node> GetNodeList() {
        List<Node> nodeList = new List<Node> { this };
        ExtractNodes(this, nodeList);
        return nodeList;
    }

    //PUSHES A LOT TO THE STACK
    private void ExtractNodes(Node current, List<Node> extracted) {
        foreach (Node n in current.GetSubnodes()) {
            extracted.Add(n);
            ExtractNodes(n, extracted);
        }
    }


    public Vector3[] GetVertices(int resolution, bool doubled) {
        lock (this) {
            return TreeUtil.CalculateCircleVertices(position, normal, radius, resolution, doubled);
        }
    }

    public bool Equals(Node other) {
        return (other!=null) && this.position.Equals(other.position);
    }
}