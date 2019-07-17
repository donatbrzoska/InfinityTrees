﻿using System;
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

    public Node(Vector3 position, GeometryProperties geometryProperties) : this(position, null, geometryProperties) { }

    private Node(Vector3 position, Node supernode, GeometryProperties geometryProperties) {
        this.position = position;
        this.supernode = supernode;
        this.radius = geometryProperties.GetTipRadius();

        this.geometryProperties = geometryProperties;

        CalculateNormal();


        //add leaves
        for (int i = 0; i < geometryProperties.GetLeavesPerNode(); i++) {
            leaves.Add(new Leaf(position, geometryProperties));
        }
    }

    public void Add(Vector3 position) {
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


    //public List<Node> GetNodeList() {
    //    List<Node> nodeList = new List<Node> { this };
    //    ExtractNodes(this, nodeList);
    //    return nodeList;
    //}

    ////PUSHES A LOT TO THE STACK
    //private void ExtractNodes(Node current, List<Node> extracted) {
    //    foreach (Node n in current.GetSubnodes()) {
    //        extracted.Add(n);
    //        ExtractNodes(n, extracted);
    //    }
    //}


    //public Vector3[] GetCircleVertices(bool doubled) {
    //    lock (this) {
    //        return TreeUtil.CalculateCircleVertices(position, normal, radius, geometryProperties.GetCircleResolution(), doubled);
    //    }
    //}


    public void GetCircleVertices(List<Vector3> verticesResult, bool doubled) {
        lock (this) {
            TreeUtil.CalculateAndStoreCircleVertices(verticesResult, position, normal, radius, geometryProperties.GetCircleResolution(), doubled);
        }
    }

    public void CalculateAndStoreLeafData(List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        if (geometryProperties.GetLeavesEnabled()) {
            if (radius < geometryProperties.GetMaxTwigRadiusForLeaves()) {
                foreach (Leaf leaf in leaves) {
                    leaf.CalculateAndStoreGeometry(verticesResult, uvsResult, trianglesResult);
                }
            }
        }
    }

    public bool Equals(Node other) {
        return (other!=null) && this.position.Equals(other.position);
    }
}