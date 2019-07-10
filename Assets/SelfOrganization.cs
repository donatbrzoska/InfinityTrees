//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;
//using System.Threading;
//using UnityEngine;

//public class SelfOrganization : Grower {

//    private static bool debugEnabled = true;
//    private static void debug(string message, [CallerMemberName]string callerName = "") {
//        if (debugEnabled) {
//            UnityEngine.Debug.Log("DEBUG: SelfOrganization: " + callerName + "(): " + message);
//        }
//    }

//    private static void debug(FormatString formatString, [CallerMemberName]string callerName = "") {
//        if (debugEnabled) {
//            UnityEngine.Debug.Log("DEBUG: SelfOrganization: " + callerName + "(): " + formatString);
//        }
//    }




//    //these are additional threads, there are always threadsLeft+1 active threads for the calculation
//    int threadsLeft = 6;
//    Vector3 defaultPosition = new Vector3(0, -10000, 0);

//    HashSet<Vector3> attractionPoints;
//    GrowthProperties growthProperties;

//    public SelfOrganization(HashSet<Vector3> attractionPoints, GrowthProperties growthProperties) {
//        this.attractionPoints = attractionPoints;
//        this.growthProperties = growthProperties;
//    }

//    public void Apply(Node root) {
//        Dictionary<Node, List<Vector3>> nodesAttractionPoints = new Dictionary<Node, List<Vector3>>();
//        List<Node> nodeList = root.GetNodeList();

//        //iterate through all attractionPoints
//        foreach (Vector3 attractionPoint in attractionPoints) {
//            //and find the closest Node respectively
//            Node closest = FindClosestNode(attractionPoint, nodeList);

//            //if there is a close Node
//            if (closest != null) {
//                //add it to the nodesAttractionPoints
//                if (nodesAttractionPoints.ContainsKey(closest)) {
//                    nodesAttractionPoints[closest].Add(attractionPoint);
//                } else {
//                    nodesAttractionPoints[closest] = new List<Vector3> { attractionPoint };
//                }
//            }
//        }

//        List<Vector3> newPositions = new List<Vector3>();
//        //iterate through all Nodes with attractionPoints associated
//        foreach (Node currentNode in nodesAttractionPoints.Keys) {
//            List<Vector3> associatedAttractionPoints = nodesAttractionPoints[currentNode];

//            //calculate direction
//            Vector3 sum = new Vector3(0, 0, 0);
//            foreach (Vector3 associatedAttractionPoint in associatedAttractionPoints) {
//                sum += (associatedAttractionPoint - currentNode.GetPosition()).normalized;
//            }
//            Vector3 direction = (sum + growthProperties.GetTropisms()).normalized * growthProperties.GetGrowthDistance();
//            //and new nodes position
//            Vector3 happyNodePosition = currentNode.GetPosition() + direction;

//            //add new node to currentNode
//            bool added = currentNode.Add(happyNodePosition);
//            //and to the newPositions list
//            if (added) {
//                newPositions.Add(happyNodePosition);
//            }
//        }

//        RemoveClosePoints(newPositions);
//    }

//    //returns null if there is no closest node
//    Node FindClosestNode(Vector3 attractionPoint, List<Node> nodeList) {
//        Node closest = null;

//        foreach (Node current in nodeList) {
//            float quadraticDistanceToCurrent = GetQuadraticDistanceWithMaxValue(current.GetPosition(), attractionPoint, growthProperties.GetSquaredInfluenceDistance());

//            if (quadraticDistanceToCurrent > -1) {
//                //float angle = Vector3.Angle(current.GetDirection(), attractionPoint-current.GetPosition());
//                //float perceptionAngle = 90f;
//                //if (angle < perceptionAngle / 2) {
//                    closest = current;
//                //}
//            }
//        }

//        return closest;
//    }

//    void RemoveClosePoints(List<Vector3> newPositions) {
//        foreach (Vector3 newPosition in newPositions) {
//            List<Vector3> closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance());
//            foreach (Vector3 closePoint in closePoints) {
//                attractionPoints.Remove(closePoint);
//            }
//        }
//    }

//    //for every node position, stores the distance to every attraction point
//    List<Vector3> DetermineAttractionPointsWithinQuadraticDistance(Vector3 position, float maxDistance) {
//        List<Vector3> result = new List<Vector3>();
//        foreach (Vector3 attractionPoint in attractionPoints) {
//            float distance = GetQuadraticDistanceWithMaxValue(position, attractionPoint, maxDistance);
//            if (distance > -1) {
//                result.Add(attractionPoint);
//            }
//        }
//        return result;
//    }

//    //https://stackoverflow.com/questions/1901139/closest-point-to-a-given-point
//    float GetQuadraticDistanceWithMaxValue(Vector3 a, Vector3 b, float maxValue) {
//        float distance = 0;

//        float dx = a.x - b.x;
//        distance += dx * dx;
//        if (distance > maxValue) {
//            return -1;
//        }

//        float dy = a.y - b.y;
//        distance += dy * dy;
//        if (distance > maxValue) {
//            return -1;
//        }

//        float dz = a.z - b.z;
//        distance += dz * dz;
//        if (distance > maxValue) {
//            return -1;
//        }

//        return distance;
//    }
//}