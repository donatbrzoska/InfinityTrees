using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using System.Diagnostics;

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
    //int threadsLeft = 8;
    //Vector3 defaultPosition = new Vector3(0, -10000, 0);

    List<Node> nodeList;
    GrowthProperties growthProperties;
    GrowerListener growerListener;

    public void SetGrowerListener(GrowerListener growerListener) {
        this.growerListener = growerListener;
    }

    public SpaceColonization(GrowthProperties growthProperties) {
        this.growthProperties = growthProperties;
    }


    //interrupt thread nicely when growthProperties change
    private bool running;

    public void Stop() {
        if (running) {
            running = false;
            ThreadManager.Join();
        }
    }

    public void Apply(Node root, bool regrow=true) {
        running = true;

        if (regrow) {
            nodeList = new List<Node> { root };
        }

        Stopwatch growingStopwatch = new Stopwatch();
        growingStopwatch.Start();

        for (int i = 0; i < growthProperties.GetIterations(); i++) {

            if (!running) {
                return;
            }

            Dictionary<Node, List<Vector3>> nodesAttractionPoints = new Dictionary<Node, List<Vector3>>();
            //List<Node> nodeList = root.GetNodeList();
            //debug("n nodes: " + nodeList.Count);

            //iterate through all attractionPoints
            foreach (Vector3 attractionPoint in growthProperties.GetAttractionPoints()) {
                //and find the closest Node respectively
                Node closest = FindClosestNode(attractionPoint);

                //if there is a close Node
                if (closest != null) {
                    //add it to the nodesAttractionPoints
                    if (nodesAttractionPoints.ContainsKey(closest)) {
                        nodesAttractionPoints[closest].Add(attractionPoint);
                    } else {
                        nodesAttractionPoints[closest] = new List<Vector3> { attractionPoint };
                    }
                }
            }


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

                if (!IsDuplicateNode(happyNodePosition, currentNode)) {
                    //add new node to currentNode
                    currentNode.Add(happyNodePosition);
                    //add to the nodeList
                    nodeList.Add(currentNode.GetSubnodes()[currentNode.GetSubnodes().Count-1]);
                    //and to the newPositions list
                    newPositions.Add(happyNodePosition);
                }
            }

            RemoveClosePoints(newPositions);

            growerListener.OnIterationFinished();


            if (growthProperties.GetHangingBranchesEnabled() && i > growthProperties.GetIterations() * growthProperties.GetHangingBranchesFromAgeRatio()) {
                growthProperties.SetTropisms(new Vector3(0, -1f, 0), true);
                debug("updated tropisms");
            }
        }

        growingStopwatch.Stop();
        debug(new FormatString("grew {0} times in {1}", growthProperties.GetIterations(), growingStopwatch.Elapsed));

    }

    private bool IsDuplicateNode(Vector3 potentialPosition, Node node) {
        foreach (Node subnode in node.GetSubnodes()) {
            if (subnode.GetPosition().Equals(potentialPosition)) {
                return true;
            }
        }
        return false;
    }

    //returns null if there is no closest node
    private Node FindClosestNode(Vector3 attractionPoint) {
        Node closest = null;

        float currentSmallestDistance = growthProperties.GetSquaredInfluenceDistance();

        foreach (Node current in nodeList) {

            float quadraticDistanceToCurrent = GetQuadraticDistanceWithMaxValue(current.GetPosition(), attractionPoint, currentSmallestDistance);

            if (quadraticDistanceToCurrent > -1) {
                if (AttractionPointInPerceptionAngle(current, attractionPoint)) { //angle calculation is a lot slower than one distance calculation
                    currentSmallestDistance = quadraticDistanceToCurrent;
                    closest = current;
                }
            }
        }

        return closest;
    }

    private bool AttractionPointInPerceptionAngle(Node node, Vector3 attractionPoint) {
        float angle = Vector3.Angle(node.GetDirection(), attractionPoint - node.GetPosition());
        bool isInPerceptionAngle = angle <= growthProperties.GetPerceptionAngle() / 2f;
        return isInPerceptionAngle;
    }

    private void RemoveClosePoints(List<Vector3> newPositions) {
        foreach (Vector3 newPosition in newPositions) {
            List<Vector3> closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance());
            foreach (Vector3 closePoint in closePoints) {
                growthProperties.GetAttractionPoints().Remove(closePoint);
            }
        }
    }

    //for every node position, stores the distance to every attraction point
    private List<Vector3> DetermineAttractionPointsWithinQuadraticDistance(Vector3 position, float maxDistance) {
        List<Vector3> result = new List<Vector3>();
        foreach (Vector3 attractionPoint in growthProperties.GetAttractionPoints()) {

            float distance = GetQuadraticDistanceWithMaxValue(position, attractionPoint, maxDistance);
            if (distance > -1) {
                result.Add(attractionPoint);
            }
        }
        return result;
    }

    //https://stackoverflow.com/questions/1901139/closest-point-to-a-given-point
    float GetQuadraticDistanceWithMaxValue(Vector3 a, Vector3 b, float maxValue) {
        float distance = 0;

        float dx = a.x - b.x;
        distance += dx * dx;
        if (distance > maxValue) {
            return -1;
        }

        float dy = a.y - b.y;
        distance += dy * dy;
        if (distance > maxValue) {
            return -1;
        }

        float dz = a.z - b.z;
        distance += dz * dz;
        if (distance > maxValue) {
            return -1;
        }

        return distance;
    }

    //#######################################################################################
    //##########                 INTERFACE IMPLEMENTIATION : Grower                ##########
    //#######################################################################################

    public GrowthProperties GetGrowthProperties() {
        return growthProperties;
    }

    //#######################################################################################
    //##########        INTERFACE IMPLEMENTATION : GrowthPropertiesListener        ##########
    //#######################################################################################

    public void OnAttractionPointsChanged() {
        growerListener.OnAttractionPointsChanged();
    }

    public void OnAgeChanged() {
        growerListener.OnAgeChanged();
    }
}



//List<Thread> threads = new List<Thread>();

//int rangeSize = attractionPoints.Count / threadsLeft;
//int fromIndex = 0;
//int toIndex = rangeSize;
//for (int i = 0; i < threadsLeft; i++) {
//    Thread t = new Thread(() => {
//        for (int j = fromIndex; j < toIndex; j++) {
//            Node closest = FindClosestNode(attractionPoints[j], nodeList);

//            //if there is a close Node
//            if (closest != null) {
//                lock (nodesAttractionPoints) { //make sure, multiple threads don't access the dictionary while modifying it
//                    //add it to the nodesAttractionPoints
//                    if (nodesAttractionPoints.ContainsKey(closest)) {
//                        nodesAttractionPoints[closest].Add(attractionPoints[j]);
//                    } else {
//                        nodesAttractionPoints[closest] = new List<Vector3> { attractionPoints[j] };
//                    }
//                }
//            }
//        }
//    });
//    threads.Add(t);
//    t.Start();

//    fromIndex += rangeSize;
//    toIndex += rangeSize;

//    if (i == threadsLeft - 1) {
//        toIndex = attractionPoints.Count;
//    }
//}

//foreach (Thread t in threads) {
//    t.Join();
//}




//    float quadraticDistanceToCurrent = GetQuadraticDistance(current.GetPosition(), attractionPoint);

//    if (quadraticDistanceToCurrent <= smallestDistance) {
//        smallestDistance = quadraticDistanceToCurrent;
//        closest = current;
//    }


//float quadraticDistanceToCurrent = 0;

//float dx = current.GetPosition().x - attractionPoint.x;
//quadraticDistanceToCurrent += dx * dx;
//if (quadraticDistanceToCurrent > smallestDistance) {
//    continue;
//}

//float dy = current.GetPosition().y - attractionPoint.y;
//quadraticDistanceToCurrent += dy * dy;
//if (quadraticDistanceToCurrent > smallestDistance) {
//    continue;
//}

//float dz = current.GetPosition().z - attractionPoint.z;
//quadraticDistanceToCurrent += dz * dz;
//if (quadraticDistanceToCurrent > smallestDistance) {
//    continue;
//}

//smallestDistance = quadraticDistanceToCurrent;
//closest = current;




//float distance = GetQuadraticDistance(position, attractionPoint);
//if (distance <= maxDistance) {
//    result.Add(attractionPoint);
//}


//float quadraticDistance = 0;

//float dx = position.x - attractionPoint.x;
//quadraticDistance += dx * dx;
//if (quadraticDistance > maxDistance) {
//    continue;
//}

//float dy = position.y - attractionPoint.y;
//quadraticDistance += dy * dy;
//if (quadraticDistance > maxDistance) {
//    continue;
//}

//float dz = position.z - attractionPoint.z;
//quadraticDistance += dz * dz;
//if (quadraticDistance > maxDistance) {
//    continue;
//}

//result.Add(attractionPoint);







////https://stackoverflow.com/questions/1901139/closest-point-to-a-given-point
//float GetQuadraticDistance(Vector3 a, Vector3 b) {
//    float dx = a.x - b.x;
//    float dy = a.y - b.y;
//    float dz = a.z - b.z;
//    return dx * dx + dy * dy + dz * dz;
//}