using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class SpaceColonization : Grower {

    private static bool debugEnabled = true;
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: SpaceColonization: " + callerName + "(): " + message);
        }
    }

    private static void debug(FormatString formatString, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: SpaceColonization: " + callerName + "(): " + formatString);
        }
    }




    //these are additional threads, there are always threadsLeft+1 active threads for the calculation
    int threadsLeft = 6;
    Vector3 defaultPosition = new Vector3(0, -10000, 0);

    HashSet<Vector3> attractionPoints;
    GrowthProperties growthProperties;

    public SpaceColonization(HashSet<Vector3> attractionPoints, GrowthProperties growthProperties) {
        this.attractionPoints = attractionPoints;
        this.growthProperties = growthProperties;
    }

    public void Apply(Node root) {
        Dictionary<Node, List<Vector3>> nodesAttractionPoints = new Dictionary<Node, List<Vector3>>();
        List<Node> nodeList = root.GetNodeList();

        //List<Thread> threads = new List<Thread>();

        //iterate through all attractionPoints
        foreach (Vector3 attractionPoint in attractionPoints) {
            //if (threadsLeft > 0) {
            //    threadsLeft--;
            //    Thread t = new Thread(() => {
            //        //and find the closest Node respectively
            //        Node closest = FindClosestNode(nodeList, attractionPoint);

            //        //if there is a close Node
            //        if (closest != null) {
            //            lock (nodesAttractionPoints) { //make sure, multiple threads don't access the dictionary while modifying it
            //                //add it to the nodesAttractionPoints
            //                if (nodesAttractionPoints.ContainsKey(closest)) {
            //                    nodesAttractionPoints[closest].Add(attractionPoint);
            //                } else {
            //                    nodesAttractionPoints[closest] = new List<Vector3> { attractionPoint };
            //                }
            //            }
            //        }
            //        threadsLeft++; //TODO: synchronize this?
            //    });
            //    threads.Add(t);
            //    t.Start();
            //} else {
                //and find the closest Node respectively
                Node closest = FindClosestNode(attractionPoint, nodeList);

                //if there is a close Node
                if (closest != null) {
                    //lock (nodesAttractionPoints) { //make sure, multiple threads don't access the dictionary while modifying it
                        //add it to the nodesAttractionPoints
                        if (nodesAttractionPoints.ContainsKey(closest)) {
                            nodesAttractionPoints[closest].Add(attractionPoint);
                        } else {
                            nodesAttractionPoints[closest] = new List<Vector3> { attractionPoint };
                        }
                    //}
                }
            //}
        }

        //foreach (Thread t in threads) {
        //    t.Join();
        //}

        List<Vector3> newPositions = new List<Vector3>();
        //iterate through all Nodes with attractionPoints associated
        foreach (Node currentNode in nodesAttractionPoints.Keys) {
            List<Vector3> associatedAttractionPoints = nodesAttractionPoints[currentNode];

            //calculate direction
            Vector3 sum = new Vector3(0, 0, 0);
            foreach (Vector3 associatedAttractionPoint in associatedAttractionPoints) {
                sum += (associatedAttractionPoint - currentNode.GetPosition()).normalized;
            }
            Vector3 direction = (sum + growthProperties.GetTropisms()).normalized * growthProperties.GetGrowthDistance();
            //and new nodes position
            Vector3 happyNodePosition = currentNode.GetPosition() + direction;

            //add new node to currentNode
            bool added = currentNode.Add(happyNodePosition);
            //and to the newPositions list
            if (added) {
                newPositions.Add(happyNodePosition);
            }
        }

        RemoveClosePoints(newPositions);
    }

    //returns null if there is no closest node
    Node FindClosestNode(Vector3 attractionPoint, List<Node> nodeList) {
        Node closest = null;

        float minDistance = growthProperties.GetSquaredInfluenceDistance();

        foreach (Node current in nodeList) {
            float quadraticDistanceToCurrent = 0;

            float dx = current.GetPosition().x - attractionPoint.x;
            quadraticDistanceToCurrent += dx * dx;
            if (quadraticDistanceToCurrent > minDistance) {
                continue;
            }

            float dy = current.GetPosition().y - attractionPoint.y;
            quadraticDistanceToCurrent += dy * dy;
            if (quadraticDistanceToCurrent > minDistance) {
                continue;
            }

            float dz = current.GetPosition().z - attractionPoint.z;
            quadraticDistanceToCurrent += dz * dz;
            if (quadraticDistanceToCurrent > minDistance) {
                continue;
            }

            minDistance = quadraticDistanceToCurrent;
            closest = current;
        }

        //foreach (Node current in nodeList) {
        //    float quadraticDistanceToCurrent = GetQuadraticDistance(current.GetPosition(), attractionPoint);

        //    if (quadraticDistanceToCurrent <= minDistance) {
        //        minDistance = quadraticDistanceToCurrent;
        //        closest = current;
        //    }
        //}

        return closest;
    }

    void RemoveClosePoints(List<Vector3> newPositions) {
        foreach (Vector3 newPosition in newPositions) {
            List<Vector3> closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance());
            foreach (Vector3 closePoint in closePoints) {
                attractionPoints.Remove(closePoint);
            }
        }
    }

    //for every node position, stores the distance to every attraction point
    List<Vector3> DetermineAttractionPointsWithinQuadraticDistance(Vector3 position, float maxDistance) {
        List<Vector3> result = new List<Vector3>();
        foreach (Vector3 attractionPoint in attractionPoints) {
            float distance = GetQuadraticDistance(position, attractionPoint);
            if (distance <= maxDistance) {
                result.Add(attractionPoint);
            }
        }
        return result;
    }

    //https://stackoverflow.com/questions/1901139/closest-point-to-a-given-point
    float GetQuadraticDistance(Vector3 a, Vector3 b) {
        float dx = a.x - b.x;
        float dy = a.y - b.y;
        float dz = a.z - b.z;
        return dx * dx + dy * dy + dz * dz;
    }
}
