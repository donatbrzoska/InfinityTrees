using System;
using System.Collections.Generic;
using UnityEngine;

public class BinarySearchAlgorithm : NearestNodeAlgorithm{

    SortedNodeList x = new SortedNodeList(CoordinateType.x);
    SortedNodeList y = new SortedNodeList(CoordinateType.y);
    SortedNodeList z = new SortedNodeList(CoordinateType.z);

    float squaredInfluenceDistance;
    float perceptionAngle;

    public BinarySearchAlgorithm(float squaredInfluenceDistance, float perceptionAngle) {
        this.squaredInfluenceDistance = squaredInfluenceDistance;
        this.perceptionAngle = perceptionAngle;
    }

    //private void CheckOrder() {
    //    for (int i=0; i<x.Count-1; i++) {
    //        if (x[i].GetPosition().x > x[i + 1].GetPosition().x) {
    //            for (int j = 0; j<x.Count; j++) {
    //                Debug.Log(j + ": " + x[j].GetPosition().x);
    //            }
    //            throw new Exception("x sorting function is buggy at " + i);
    //        }
    //        if (y[i].GetPosition().y > y[i + 1].GetPosition().y) {
    //            for (int j = 0; j < y.Count; j++) {
    //                Debug.Log(j + ": " + y[j].GetPosition().y);
    //            }
    //            throw new Exception("y sorting function is buggy at " + i);
    //        }
    //        if (z[i].GetPosition().z > z[i + 1].GetPosition().z) {
    //            for (int j = 0; j < z.Count; j++) {
    //                Debug.Log(j + ": " + z[j].GetPosition().z);
    //            }
    //            throw new Exception("z sorting function is buggy at " + i);
    //        }
    //    }
    //}

    public void Add(Node node) {
        x.InsertSorted(node);
        y.InsertSorted(node);
        z.InsertSorted(node);
        //CheckOrder();
    }

    public Node GetNearest(Vector3 position) {
        SortedCandidateNodeList candidates = new SortedCandidateNodeList();

        //find index of nearest Node concerning the x coordinate
        int nearest_x = x.GetNearestIndex(new Node(position));
        if (nearest_x == -1) {
            return null;
        }
        //look at left neighbours
        for (int i = nearest_x; i >= 0; i--) {
            Vector3 currentPosition = x[i].Position;
            Vector3 d = currentPosition - position;

            //find out whether we are still in the x range
            float dx = d.x * d.x;
            if (dx <= squaredInfluenceDistance) {
                float distance = dx + d.y * d.y + d.z * d.z;
                if (distance <= squaredInfluenceDistance) {
                    candidates.InsertSorted(new CandidateNode(x[i], distance));
                }
            } else {
                break;
            }
        }
        //look at right neighbours
        for (int i = nearest_x + 1; i < x.Count; i++) {
            Vector3 currentPosition = x[i].Position;
            Vector3 d = currentPosition - position;

            //find out whether we are still in the x range
            float dx = d.x * d.x; 
            if (dx <= squaredInfluenceDistance) {
                float distance = dx + d.y * d.y + d.z * d.z;
                if (distance <= squaredInfluenceDistance) {
                    candidates.InsertSorted(new CandidateNode(x[i], distance));
                }
            } else {
                break;
            }
        }


        int nearest_y = y.GetNearestIndex(new Node(position));
        for (int i = nearest_y; i >= 0; i--) {
            Vector3 currentPosition = y[i].Position;
            Vector3 d = currentPosition - position;

            float dy = d.y * d.y;
            if (dy <= squaredInfluenceDistance) { //find out whether we are still in the y range
                float distance = d.x * d.x + dy + d.z * d.z;
                if (distance <= squaredInfluenceDistance) {
                    candidates.InsertSorted(new CandidateNode(y[i], distance));
                }
            } else {
                break;
            }
        }
        for (int i = nearest_y + 1; i < y.Count; i++) {
            Vector3 currentPosition = y[i].Position;
            Vector3 d = currentPosition - position;

            float dy = d.y * d.y;
            if (dy <= squaredInfluenceDistance) { //find out whether we are still in the y range
                float distance = d.x * d.x + dy + d.z * d.z;
                if (distance <= squaredInfluenceDistance) {
                    candidates.InsertSorted(new CandidateNode(y[i], distance));
                }
            } else {
                break;
            }
        }


        int nearest_z = z.GetNearestIndex(new Node(position));
        for (int i = nearest_z; i >= 0; i--) {
            Vector3 currentPosition = z[i].Position;
            Vector3 d = currentPosition - position;

            float dz = d.z * d.z;
            if (dz <= squaredInfluenceDistance) { //find out whether we are still in the z range
                float distance = d.x * d.x + d.y * d.y + dz;
                if (distance <= squaredInfluenceDistance) {
                    candidates.InsertSorted(new CandidateNode(z[i], distance));
                }
            } else {
                break;
            }
        }
        for (int i = nearest_z + 1; i < z.Count; i++) {
            Vector3 currentPosition = z[i].Position;
            Vector3 d = currentPosition - position;

            float dz = d.z * d.z;
            if (dz <= squaredInfluenceDistance) { //find out whether we are still in the z range
                float distance = d.x * d.x + d.y * d.y + dz;
                if (distance <= squaredInfluenceDistance) {
                    candidates.InsertSorted(new CandidateNode(z[i], distance));
                }
            } else {
                break;
            }
        }

        //eventually determine closest node in perceptionAngle
        if (candidates.Count > 0) {
            for (int i = 0; i < candidates.Count; i++) {
                CandidateNode current = candidates[i];
                if (AttractionPointInPerceptionAngle(current.node, position)) {
                    return current.node;
                }
            }
            return null;
        } else {
            return null;
        }
    }

    private bool AttractionPointInPerceptionAngle(Node node, Vector3 attractionPoint) {
        float angle = Vector3.Angle(node.GetDirection(), attractionPoint - node.Position);
        bool isInPerceptionAngle = angle <= perceptionAngle / 2f;
        return isInPerceptionAngle;
    }
}