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


    bool advanced_algorithm = true;
    List<Node> nodeList;
    NearestNodeAlgorithm nearestNodeAlgorithm;

    private float treeHeight;
    public float GetTreeHeight() {
        return treeHeight;
    }

    GrowthProperties growthProperties;
    GrowerListener growerListener;

    public SpaceColonization(GrowthProperties growthProperties, GrowerListener growerListener) {
        this.growthProperties = growthProperties;
        this.growerListener = growerListener;
    }


    Thread growerThread;

    //interrupt thread nicely when growthProperties change
    private bool running;

    public void Stop() {
        if (running) {
            running = false;
            growerThread.Join();
        }
    }


    public void Grow(Tree tree) {
        if (!advanced_algorithm) {
            nodeList = new List<Node> { tree.StemRoot };
        } else {
            nearestNodeAlgorithm = new NearestNodeAlgorithm();
            nearestNodeAlgorithm.Add(tree.StemRoot);
            //nearestNodeAlgorithm.Add(initialNode);
        }


        growerThread = new Thread(() => {
            Stopwatch growingStopwatch = new Stopwatch();
            growingStopwatch.Start();

            GrowStem(tree);
            GrowCrown(tree);

            growingStopwatch.Stop();
            debug(new FormatString("grew {0} times in {1}", growthProperties.GetIterations(), growingStopwatch.Elapsed));
        });
        growerThread.IsBackground = true;
        growerThread.Start();
    }

    private AdvancedRandom stemRandom;

    public void RegrowStem(Tree tree) {
        //save the current crown structure
        Node CurrentCrownRoot = tree.CrownRoot;

        //delete everything from the tree
        tree.Reset();

        //grow a new stem
        GrowStem(tree);

        //put the current crown structure to the new stem
        Vector3 pos_diff = tree.CrownRoot.GetPosition() - CurrentCrownRoot.GetPosition();
        CurrentCrownRoot.UpdatePosition(pos_diff);
        foreach (Node n in CurrentCrownRoot.GetSubnodes()) {
            tree.CrownRoot.Add(n);
            //have a look at the Add method again, if you want the move the attraction points towards the stem too
        }

        growerListener.OnUpdate();
    }

    private void GrowStem(Tree tree) {
        if (Util.AlmostEqual(growthProperties.StemLength, 0)) {
            tree.CrownRoot = tree.StemRoot;
            //tree.StemTip = tree.StemRoot;
        } else {
            stemRandom = new AdvancedRandom(((PseudoEllipsoid)growthProperties.GetAttractionPoints()).Seed);

            float angleRange = 5;

            int iterations = (int)(growthProperties.StemLength / growthProperties.GetGrowthDistance());
            for (int i = 0; i < iterations; i++) {
                float angle = stemRandom.RandomInRange(-angleRange, angleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                if (tree.StemRoot.HasSubnodes()) {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.CrownRoot.GetDirection(true);
                    Node newNode = tree.CrownRoot.Add(tree.CrownRoot.GetPosition() + direction * growthProperties.GetGrowthDistance());
                    //nearestNodeAlgorithm.Add(newNode);
                    //nodeList.Add(newNode);
                    tree.CrownRoot = newNode;
                    //tree.StemTip = newNode;
                } else {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.StemRoot.GetDirection(true);
                    Node newNode = tree.StemRoot.Add(tree.StemRoot.GetPosition() + direction * growthProperties.GetGrowthDistance());
                    //nearestNodeAlgorithm.Add(newNode);
                    //nodeList.Add(newNode);
                    tree.CrownRoot = newNode;
                    //tree.StemTip = newNode;
                }
            }

            float rest = growthProperties.StemLength % growthProperties.GetGrowthDistance();
            if (!Util.AlmostEqual(rest, 0)) {
                float angle = stemRandom.RandomInRange(-angleRange, angleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                if (tree.StemRoot.HasSubnodes()) {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.CrownRoot.GetDirection(true);
                    Node newNode = tree.CrownRoot.Add(tree.CrownRoot.GetPosition() + direction * rest);
                    //nearestNodeAlgorithm.Add(newNode);
                    //nodeList.Add(newNode);
                    tree.CrownRoot = newNode;
                    //tree.StemTip = newNode;
                } else {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.StemRoot.GetDirection(true);
                    Node newNode = tree.StemRoot.Add(tree.StemRoot.GetPosition() + direction * rest);
                    //nearestNodeAlgorithm.Add(newNode);
                    //nodeList.Add(newNode);
                    tree.CrownRoot = newNode;
                    //tree.StemTip = newNode;
                }
            }
        }

        nearestNodeAlgorithm.Add(tree.CrownRoot);
        //nodeList.Add(newNode);
        ((PseudoEllipsoid)growthProperties.GetAttractionPoints()).UpdatePosition(tree.CrownRoot.GetPosition());
    }

    private void GrowCrown(Tree tree) {
        treeHeight = 0;
        running = true;

        Stopwatch findClosePointStopwatch = new Stopwatch();
        Stopwatch removeClosePointsStopwatch = new Stopwatch();

        for (int i = 0; i < growthProperties.GetIterations(); i++) {

            if (!running) {
                return;
            }

            Dictionary<Node, List<Vector3>> nodesAttractionPoints = new Dictionary<Node, List<Vector3>>();
            //List<Node> nodeList = root.GetNodeList();
            //debug("n nodes: " + nodeList.Count);

            findClosePointStopwatch.Start();
            //iterate through all attractionPoints
            //foreach (Vector3 attractionPoint in growthProperties.GetAttractionPoints()) { //there is some threading problem with the enumeration foreach loop, usual for should fix it
            for (int j = 0; j < growthProperties.GetAttractionPoints().Count; j++) {
                Vector3 attractionPoint = growthProperties.GetAttractionPoints()[j];

                //and find the closest Node respectively
                Node closest;
                if (!advanced_algorithm) {
                    closest = FindClosestNode(attractionPoint);
                } else {
                    closest = nearestNodeAlgorithm.GetNearestWithinSquaredDistance(attractionPoint, growthProperties.GetSquaredInfluenceDistance(), growthProperties.GetPerceptionAngle());
                }


                //if there is a close Node
                if (closest != null) {
                    //add it to the nodesAttractionPoints
                    if (nodesAttractionPoints.ContainsKey(closest)) {
                        nodesAttractionPoints[closest].Add(attractionPoint);
                    } else {
                        nodesAttractionPoints[closest] = new List<Vector3> { attractionPoint };
                    }
                }

                if (!running) {
                    break;
                }
            }
            findClosePointStopwatch.Stop();

            if (!running) {
                return;
            }

            List<Vector3> newPositions = new List<Vector3>();
            //iterate through all Nodes with attractionPoints associated
            foreach (Node currentNode in nodesAttractionPoints.Keys) {
                List<Vector3> associatedAttractionPoints = nodesAttractionPoints[currentNode];

                //calculate direction
                Vector3 sum = new Vector3(0, 0, 0);
                foreach (Vector3 associatedAttractionPoint in associatedAttractionPoints) {
                    sum += (associatedAttractionPoint - currentNode.GetPosition()).normalized;
                    //sum += (associatedAttractionPoint - currentNode.GetPosition()) * (associatedAttractionPoint - currentNode.GetPosition()).magnitude;
                    //sum += (associatedAttractionPoint - currentNode.GetPosition()) * (1/((associatedAttractionPoint - currentNode.GetPosition()).magnitude* (associatedAttractionPoint - currentNode.GetPosition()).magnitude));
                }

                Vector3 direction = (sum + Util.Hadamard(growthProperties.GetTropisms(i), growthProperties.GetTropismsWeights())).normalized * growthProperties.GetGrowthDistance();

                //Vector3 direction;
                //if (i < 0.3 * growthProperties.GetIterations()) {
                //    direction = (sum + Util.Hadamard(growthProperties.GetTropisms(i), new Vector3(0, 10, 0))).normalized * growthProperties.GetGrowthDistance();
                //} else if (i < 0.5 * growthProperties.GetIterations()) {
                //    direction = (sum + Util.Hadamard(growthProperties.GetTropisms(i), new Vector3(0, 0, 0))).normalized * growthProperties.GetGrowthDistance();
                //} else if (i < 0.8 * growthProperties.GetIterations()) {
                //    direction = (sum + Util.Hadamard(growthProperties.GetTropisms(i), new Vector3(0, -5, 0))).normalized * growthProperties.GetGrowthDistance();
                //} else {
                //    direction = (sum + Util.Hadamard(growthProperties.GetTropisms(i), new Vector3(0, -5, 0))).normalized * growthProperties.GetGrowthDistance();
                //}

                //Vector3 direction;
                //if (i < 0.5 * growthProperties.GetIterations()) {
                //    direction = (sum + Util.Hadamard(growthProperties.GetTropisms(i), new Vector3(0, 10, 0))).normalized * growthProperties.GetGrowthDistance();
                //} else {
                //    direction = (sum + Util.Hadamard(growthProperties.GetTropisms(i), new Vector3(0, -1, 0))).normalized * growthProperties.GetGrowthDistance();
                //}

                //Vector3 direction = (sum + growthProperties.GetTropisms()).normalized * growthProperties.GetGrowthDistance();
                //debug("direction is " + direction);
                ////hanging branches:
                //// the smaller the angle(newNode_direction, Vec3.down)
                //// the more the gravitation gets applied (with lower bound of the branch pointing downwards!)
                //// .. rotation by axis Cross(Vec3.down, newNode_direction)
                //// ..        and f(angle) = k*(180-angle)
                //float angleToDown = Vector3.Angle(direction, Vector3.down);
                //Vector3 rotationAxis = Vector3.Cross(direction, Vector3.down);
                //direction = Quaternion.AngleAxis(1f * (180 - angleToDown), rotationAxis) * direction;

                //and new nodes position
                Vector3 happyNodePosition = currentNode.GetPosition() + direction;

                if (!IsDuplicateNode(happyNodePosition, currentNode)) {
                    //add new node to currentNode
                    currentNode.Add(happyNodePosition).AddLeaves(growthProperties.GetLeavesPerNode());

                    //add to the nodeList
                    if (!advanced_algorithm) {
                        nodeList.Add(currentNode.GetSubnodes()[currentNode.GetSubnodes().Count - 1]);
                    } else {
                        nearestNodeAlgorithm.Add(currentNode.GetSubnodes()[currentNode.GetSubnodes().Count - 1]);
                    }

                    //and to the newPositions list
                    newPositions.Add(happyNodePosition);

                    // used by Core -> CameraMovement
                    if (treeHeight < happyNodePosition.y) {
                        treeHeight = happyNodePosition.y;
                    }
                }
            }

            removeClosePointsStopwatch.Start();
            RemoveClosePoints(newPositions, i);
            removeClosePointsStopwatch.Stop();

            growerListener.OnUpdate();


            Prune(tree);
        }

        debug(new FormatString("finding close points took {0}", findClosePointStopwatch.Elapsed));
        debug(new FormatString("removing close points took {0}", removeClosePointsStopwatch.Elapsed));

        running = false;
    }

    private bool IsDuplicateNode(Vector3 potentialPosition, Node node) {
        foreach (Node subnode in node.GetSubnodes()) {
            if (subnode.GetPosition().Equals(potentialPosition)) {
                return true;
            }
        }
        return false;
    }

	private int maxConsecutiveNonBranchingNodes = 8;

	private void Prune(Tree tree)
	{
		PruneHelper(tree.CrownRoot, 0);
	}

	private void PruneHelper(Node currentNode, int consecutiveNonBranchingNodes)
	{
		if (consecutiveNonBranchingNodes == maxConsecutiveNonBranchingNodes)
		{
			currentNode.Active = false;
		}
		else
		{

			if (currentNode.HasSubnodes())
			{
				if (currentNode.GetSubnodes().Count == 1)
				{
					PruneHelper(currentNode.GetSubnodes()[0], consecutiveNonBranchingNodes + 1);
				}
				else
				{ //(currentNode.GetSubnodes().Count > 1)
					foreach (Node sn in currentNode.GetSubnodes())
					{
						PruneHelper(sn, 0);
					}
				}
			}
		}
	}


	//private int minBranching = 2; //how often in perNodes the tree as to branch
	//private int perNodes = 8;

	//private void Prune(Tree tree) {
	//    PruneHelper(tree.CrownRoot, 0, perNodes);
	//}

	//private void PruneHelper(Node currentNode, int branches, int nodesLeft) {
	//    if (nodesLeft == 0) {
	//        if (branches < minBranching) {
	//            currentNode.Active = false;
	//        } else {
	//            foreach (Node sn in currentNode.GetSubnodes()) {
	//                PruneHelper(sn, 0, perNodes);
	//            }
	//        }
	//    } else {
	//        if (currentNode.HasSubnodes()) {
	//            foreach (Node sn in currentNode.GetSubnodes()) {
	//                PruneHelper(sn, branches+currentNode.GetSubnodes().Count, nodesLeft - 1);
	//            }
	//        }
	//    }
	//}

	//returns null if there is no closest node
	private Node FindClosestNode(Vector3 attractionPoint) {
        Node closest = null;

        float currentSmallestDistance = growthProperties.GetSquaredInfluenceDistance();

        foreach (Node current in nodeList) {

            float quadraticDistanceToCurrent = GetQuadraticDistanceWithMaxValue(current.GetPosition(), attractionPoint, currentSmallestDistance);


            if (!Util.AlmostEqual(quadraticDistanceToCurrent, -1, 0.0001f)) {
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

    private void RemoveClosePoints(List<Vector3> newPositions, int iteration) {
        foreach (Vector3 newPosition in newPositions) {
            List<Vector3> closePoints;

            //debug("squared cleardistance is " + growthProperties.GetSquaredClearDistance(iteration));
            closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance(iteration));
            //closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance()); //near the envelope test
            //debug("removing " + closePoints.Count + " close points");
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

    //public void OnAttractionPointsChanged() {
    //    growerListener.OnAttractionPointsChanged();
    //}

    //public void OnAgeChanged() {
    //    growerListener.OnAgeChanged();
    //}
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