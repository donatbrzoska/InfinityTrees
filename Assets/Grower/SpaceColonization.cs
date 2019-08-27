using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using System.Diagnostics;

public class SpaceColonization {

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

    enum DistanceCalculationAlgorithm {
        Standard,
        BinarySearch,
        VoxelGrid
    }

    DistanceCalculationAlgorithm distanceCalculationAlgorithm = DistanceCalculationAlgorithm.VoxelGrid;

    List<Node> nodeList;
    BinarySearchAlgorithm binarySearchAlgorithm;
    VoxelGridAlgorithm voxelGridAlgorithm;

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
        running = false;
        growerThread.Join();
    }

    private void InsertToAlgorithm(Node node) {
        if (distanceCalculationAlgorithm == DistanceCalculationAlgorithm.Standard) {
            nodeList.Add(node);
        } else if (distanceCalculationAlgorithm == DistanceCalculationAlgorithm.BinarySearch) {
            binarySearchAlgorithm.Add(node);
        } else {
            voxelGridAlgorithm.Add(node);
        }
    }

    public void Grow(Tree tree) {
        if (distanceCalculationAlgorithm == DistanceCalculationAlgorithm.Standard) {
            nodeList = new List<Node>();// { tree.StemRoot };
        } else if (distanceCalculationAlgorithm == DistanceCalculationAlgorithm.BinarySearch) {
            binarySearchAlgorithm = new BinarySearchAlgorithm();
        } else { //if(voxelGrid_algorithm)
            voxelGridAlgorithm = new VoxelGridAlgorithm(growthProperties.GetAttractionPoints(), growthProperties.GetInfluenceDistance());
        }


        //if (growerThread == null) {
        growerThread = new Thread(() => {
                running = true;

                Stopwatch growingStopwatch = new Stopwatch();
                growingStopwatch.Start();

                GrowStem(tree);
                GrowCrownStem(tree);
                GrowCrown(tree);

                growingStopwatch.Stop();
                debug(new FormatString("grew {0} times in {1}", growthProperties.GetIterations(), growingStopwatch.Elapsed));
            });
            growerThread.IsBackground = true;
            growerThread.Start();
        //} else {
            //throw new Exception("attempted to loose reference to runnning thread");
        //}
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

        growerListener.OnIterationFinished();
    }

    private void GrowStem(Tree tree) {
        if (Util.AlmostEqual(growthProperties.StemLength, 0)) {
            tree.CrownRoot = tree.StemRoot;
        } else {
            stemRandom = new AdvancedRandom(growthProperties.GetAttractionPoints().Seed);

            int iterations = (int)(growthProperties.StemLength / growthProperties.GetGrowthDistance());
            for (int i = 0; i < iterations; i++) {
                float angle = stemRandom.RandomInRange(-growthProperties.StemAngleRange, growthProperties.StemAngleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                if (tree.StemRoot.HasSubnodes()) {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.CrownRoot.GetDirection(true);
                    Node newNode = tree.CrownRoot.Add(tree.CrownRoot.GetPosition() + direction * growthProperties.GetGrowthDistance());
                    tree.CrownRoot = newNode;
                } else {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.StemRoot.GetDirection(true);
                    Node newNode = tree.StemRoot.Add(tree.StemRoot.GetPosition() + direction * growthProperties.GetGrowthDistance());
                    tree.CrownRoot = newNode;
                }
            }

            float rest = growthProperties.StemLength % growthProperties.GetGrowthDistance();
            if (!Util.AlmostEqual(rest, 0)) {
                float angle = stemRandom.RandomInRange(-growthProperties.StemAngleRange, growthProperties.StemAngleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                if (tree.StemRoot.HasSubnodes()) {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.CrownRoot.GetDirection(true);
                    Node newNode = tree.CrownRoot.Add(tree.CrownRoot.GetPosition() + direction * rest);
                    tree.CrownRoot = newNode;
                } else {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.StemRoot.GetDirection(true);
                    Node newNode = tree.StemRoot.Add(tree.StemRoot.GetPosition() + direction * rest);
                    tree.CrownRoot = newNode;
                }
            }
        }

        InsertToAlgorithm(tree.CrownRoot);
        growthProperties.GetAttractionPoints().UpdatePosition(tree.CrownRoot.GetPosition());
    }



    private void GrowCrownStem(Tree tree) {
        if (!Util.AlmostEqual(growthProperties.CrownStemLengthRatio, 0)) {
            Node crownStemTip = tree.CrownRoot;

            stemRandom = new AdvancedRandom(growthProperties.GetAttractionPoints().Seed);

            int iterations = (int)(growthProperties.CrownStemLengthRatio * growthProperties.GetAttractionPoints().GetHeight() / growthProperties.GetGrowthDistance());
            for (int i = 0; i < iterations; i++) {
                float angle = stemRandom.RandomInRange(-growthProperties.StemAngleRange, growthProperties.StemAngleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.CrownRoot.GetDirection(true);
                crownStemTip = crownStemTip.Add(crownStemTip.GetPosition() + direction * growthProperties.GetGrowthDistance());

                InsertToAlgorithm(crownStemTip);
            }

            float rest = growthProperties.StemLength % growthProperties.GetGrowthDistance();
            if (!Util.AlmostEqual(rest, 0)) {
                float angle = stemRandom.RandomInRange(-growthProperties.StemAngleRange, growthProperties.StemAngleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.CrownRoot.GetDirection(true);
                crownStemTip = crownStemTip.Add(crownStemTip.GetPosition() + direction * growthProperties.GetGrowthDistance());

                InsertToAlgorithm(crownStemTip);
            }
        }
    }

    private float SquaredDistance(Vector3 a, Vector3 b) {
        Vector3 d = a - b;
        return d.x * d.x + d.y * d.y + d.z * d.z;
    }

    private void GrowCrown(Tree tree) {
        treeHeight = 0;

        Stopwatch findClosePointStopwatch = new Stopwatch();
        Stopwatch removeClosePointsStopwatch = new Stopwatch();

        for (int i = 0; i < growthProperties.GetIterations(); i++) {

            if (!running) {
                return;
            }

            Dictionary<Node, List<Vector3>> nodesAttractionPoints = new Dictionary<Node, List<Vector3>>();

            findClosePointStopwatch.Start();
            //iterate through all attractionPoints
            //foreach (Vector3 attractionPoint in growthProperties.GetAttractionPoints()) { //there is some threading problem with the enumeration foreach loop, usual for should fix it
            for (int j = 0; j < growthProperties.GetAttractionPoints().Count; j++) {
                Vector3 attractionPoint = growthProperties.GetAttractionPoints()[j];

                //and find the closest Node respectively
                Node closest;// = FindClosestNode(attractionPoint);
                if (distanceCalculationAlgorithm == DistanceCalculationAlgorithm.Standard) {
                    closest = FindClosestNode(attractionPoint);
                } else if (distanceCalculationAlgorithm == DistanceCalculationAlgorithm.BinarySearch) {
                    closest = binarySearchAlgorithm.GetNearestWithinSquaredDistance(attractionPoint, growthProperties.GetSquaredInfluenceDistance(), growthProperties.GetPerceptionAngle());
                } else {
                    closest = voxelGridAlgorithm.GetNearestWithinSquaredDistance(attractionPoint, growthProperties.GetSquaredInfluenceDistance(), growthProperties.GetPerceptionAngle());

                    // Rudis ultimate plan to make the removal in the next iteration
                    if (i > 0) {
                        if (closest != null) {
                            if (SquaredDistance(attractionPoint, closest.GetPosition()) < growthProperties.GetSquaredClearDistance(i)) {
                                j--;
                                growthProperties.GetAttractionPoints().Remove(attractionPoint);
                                continue;
                            }
                        }
                    }
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
                    return;
                    //break;
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
                    Node newNode = currentNode.Add(happyNodePosition);
                    newNode.AddLeaves(growthProperties.GetLeavesPerNode());

                    //add to the nodeList
                    InsertToAlgorithm(newNode);

                    //and to the newPositions list
                    newPositions.Add(happyNodePosition);

                    // used by Core -> CameraMovement
                    if (treeHeight < happyNodePosition.y) {
                        treeHeight = happyNodePosition.y;
                    }
                }
            }

            removeClosePointsStopwatch.Start();
            //RemoveClosePoints(newPositions, i);
            removeClosePointsStopwatch.Stop();

            growerListener.OnIterationFinished();
            debug("finished iteration " + i);

        }

        debug(new FormatString("finding close points took {0}", findClosePointStopwatch.Elapsed));
        debug(new FormatString("removing close points took {0}", removeClosePointsStopwatch.Elapsed));

        //Prune(tree);
        //tree.StemRoot.RecalculateRadii();

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


    //PRUNING FOR ONCE IN THE END
    //private int maxConsecutiveNonBranchingNodes = 7;

    //private void Prune(Tree tree) {
    //    PruneHelper(tree.CrownRoot, 0);
    //}

    //private void PruneHelper(Node currentNode, int consecutiveNonBranchingNodes) {
    //    if (consecutiveNonBranchingNodes == maxConsecutiveNonBranchingNodes) {
    //        currentNode.GetSubnodes().Clear();
    //    } else {

    //        if (currentNode.HasSubnodes()) {
    //            if (currentNode.GetSubnodes().Count == 1) {
    //                PruneHelper(currentNode.GetSubnodes()[0], consecutiveNonBranchingNodes + 1);
    //            } else { //(currentNode.GetSubnodes().Count > 1)
    //                foreach (Node sn in currentNode.GetSubnodes()) {
    //                    PruneHelper(sn, 0);
    //                }
    //            }
    //        }
    //    }
    //}



    //private int maxConsecutiveNonBranchingNodes = 8;

    //private void Prune(Tree tree)
    //{
    //	PruneHelper(tree.CrownRoot, 0);
    //}

    //private void PruneHelper(Node currentNode, int consecutiveNonBranchingNodes)
    //{
    //	if (consecutiveNonBranchingNodes == maxConsecutiveNonBranchingNodes)
    //	{
    //		currentNode.Active = false;
    //	}
    //	else
    //	{

    //		if (currentNode.HasSubnodes())
    //		{
    //			if (currentNode.GetSubnodes().Count == 1)
    //			{
    //				PruneHelper(currentNode.GetSubnodes()[0], consecutiveNonBranchingNodes + 1);
    //			}
    //			else
    //			{ //(currentNode.GetSubnodes().Count > 1)
    //				foreach (Node sn in currentNode.GetSubnodes())
    //				{
    //					PruneHelper(sn, 0);
    //				}
    //			}
    //		}
    //	}
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

            closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance(iteration));
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
}