using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class SimpleSpaceColonization : Grower {

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

    public SimpleSpaceColonization(HashSet<Vector3> attractionPoints, GrowthProperties growthProperties) {
        this.attractionPoints = attractionPoints;
        this.growthProperties = growthProperties;
    }

    public void Apply(Node node) {
        ArrayList newPositions = new ArrayList();
        IterateThroughNodes(node, newPositions);
        RemoveClosePoints(newPositions);
    }

    void IterateThroughNodes(Node node, ArrayList newPositions) {
        //determine closest points
        HashSet<Vector3> closePoints = DetermineAttractionPointsWithinQuadraticDistance(node.GetPosition(), growthProperties.GetSquaredInfluenceDistance());

        //calculate new node's position (if there will be one)
        Vector3 happyNodePosition = defaultPosition;
        if (closePoints.Count > 0) {
            Vector3 sum = new Vector3(0, 0, 0);
            foreach (Vector3 closePoint in closePoints) {
                sum += (closePoint - node.GetPosition()).normalized;
            }

            Vector3 direction = (sum + growthProperties.GetTropisms()).normalized * growthProperties.GetGrowthDistance();
            happyNodePosition = node.GetPosition() + direction;
        }

        //do everything for all the subnodes
        List<Node> subnodes = node.GetSubnodes();
        List<Thread> threads = new List<Thread>();
        for (int i = 0; i < subnodes.Count; i++) {
            Node subnode = (Node)subnodes[i];
            if (threadsLeft > 0) {
                threadsLeft--;
                Thread t = new Thread(() => {
                    IterateThroughNodes(subnode, newPositions);
                    threadsLeft++; //move this to cleaner line?
                });
                threads.Add(t);
                t.Start();
            } else {
                IterateThroughNodes(subnode, newPositions);
            }
        }
        foreach (Thread t in threads) {
            t.Join();
        }

        //add new Node to the current node's subnodes
        if (!happyNodePosition.Equals(defaultPosition)) {
            //debug(new FormatString("adding Node at {0}", happyNodePosition.ToString()));
            node.Add(happyNodePosition);
            lock (newPositions) { //multiple threads could write to newPositions
                newPositions.Add(happyNodePosition);
            }
        }
    }

    void RemoveClosePoints(ArrayList newPositions) {
        foreach (Vector3 newPosition in newPositions) {
            HashSet<Vector3> closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance());
            foreach (Vector3 closePoint in closePoints) {
                attractionPoints.Remove(closePoint);
            }
        }
    }

    //for every node position, stores the distance to every attraction point
    Dictionary<Vector3, Dictionary<float, HashSet<Vector3>>> results = new Dictionary<Vector3, Dictionary<float, HashSet<Vector3>>>();
    HashSet<Vector3> DetermineAttractionPointsWithinQuadraticDistance(Vector3 position, float maxDistance) {

        ////THIS WORKS:
        //HashSet<Vector3> result = new HashSet<Vector3>();
        //foreach (Vector3 attractionPoint in attractionPoints) {
        //    float distance = (position - attractionPoint).magnitude;
        //    if (distance <= maxDistance) {
        //        result.Add(attractionPoint);
        //    }
        //}
        //return result;


        //THIS WORKS BETTER, distance parameters have to be squared though!
        HashSet<Vector3> result = new HashSet<Vector3>();
        foreach (Vector3 attractionPoint in attractionPoints) {
            float dx = position.x - attractionPoint.x;
            float dy = position.y - attractionPoint.y;
            float dz = position.z - attractionPoint.z;
            float distance = dx * dx + dy * dy + dz * dz;
            if (distance <= maxDistance) {
                result.Add(attractionPoint);
            }
        }
        return result;



        ////DOESN'T WORK, ATTRACTION POINTS ARE REMOVED DURING THE PROCESS!
        ////  +++ distance parameters have to be squared though!
        ////  +++ Only works if there won't be new attraction points
        //bool nodeKnown = results.ContainsKey(position);
        //bool resultForDistanceKnown = false;
        //if (nodeKnown) {
        //    resultForDistanceKnown = results[position].ContainsKey(maxDistance);
        //}
        //if (resultForDistanceKnown) {
        //    return results[position][maxDistance];
        //}

        //HashSet<Vector3> result = new HashSet<Vector3>();
        //foreach (Vector3 attractionPoint in attractionPoints) {
        //    float dx = position.x - attractionPoint.x;
        //    float dy = position.y - attractionPoint.y;
        //    float dz = position.z - attractionPoint.z;
        //    float distance = dx * dx + dy * dy + dz * dz;
        //    if (distance <= maxDistance) {
        //        result.Add(attractionPoint);
        //    }
        //}

        //if (!nodeKnown) {
        //    results[position] = new Dictionary<float, HashSet<Vector3>>();
        //}
        //results[position][maxDistance] = result;

        //return result;






        //foreach (Vector3 current in attractionPoints) {
        //    Vector3 diff = position - current;
        //    Vector3 diffSquared = Vector3.Scale(diff, diff);
        //    float distance = diffSquared.x + diffSquared.y + diffSquared.z;
        //    if (distance <= maxDistance) {
        //        result.Add(current);
        //    }
        //}

        //foreach (Vector3 current in attractionPoints) {
        //    float distance = (position.x - current.x) * (position.x - current.x)
        //        + (position.y - current.y) * (position.y - current.y)
        //        + (position.z - current.z) * (position.z - current.z);
        //    if (distance <= maxDistance) {
        //        result.Add(current);
        //    }
        //}

    }
}
