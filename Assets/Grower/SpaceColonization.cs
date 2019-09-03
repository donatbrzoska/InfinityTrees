﻿using System;
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

    NearestNodeAlgorithm nearestNodeAlgorithm;
    //VoxelGridAlgorithm nearestNodeAlgorithm;

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

    Node crownRoot;

    public void Grow(Tree tree) {
        //nearestNodeAlgorithm = new StandardAlgorithm(growthProperties.GetInfluenceDistance(), growthProperties.GetPerceptionAngle());
        //nearestNodeAlgorithm = new SquaredDistanceAlgorithm(growthProperties.GetSquaredInfluenceDistance(), growthProperties.GetPerceptionAngle());
        //nearestNodeAlgorithm = new BinarySearchAlgorithm(growthProperties.GetSquaredInfluenceDistance(), growthProperties.GetPerceptionAngle()); //little bug somewhere
        nearestNodeAlgorithm = new VoxelGridAlgorithm(growthProperties.GetAttractionPoints(), growthProperties.GetSquaredInfluenceDistance(), growthProperties.GetPerceptionAngle());

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
            //debug(new FormatString("finding voxels around took {0}", nearestNodeAlgorithm.voxelsAround.Elapsed));
            //debug(new FormatString("finding nodes in voxels took {0}", nearestNodeAlgorithm.nodesAround.Elapsed));
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
        Node CurrentCrownRoot = crownRoot;

        //delete everything from the tree
        tree.Reset();

        //grow a new stem
        GrowStem(tree);

        //put the current crown structure to the new stem
        Vector3 pos_diff = crownRoot.GetPosition() - CurrentCrownRoot.GetPosition();
        CurrentCrownRoot.UpdatePosition(pos_diff);
        foreach (Node n in CurrentCrownRoot.GetSubnodes()) {
            crownRoot.Add(n);
            //have a look at the Add method again, if you want the move the attraction points towards the stem too
        }

        growerListener.OnIterationFinished();
    }

    private void GrowStem(Tree tree) {
        if (Util.AlmostEqual(growthProperties.StemLength, 0)) {
            crownRoot = tree.StemRoot;
        } else {
            stemRandom = new AdvancedRandom(growthProperties.GetAttractionPoints().Seed);

            int iterations = (int)(growthProperties.StemLength / growthProperties.GetGrowthDistance());
            for (int i = 0; i < iterations; i++) {
                float angle = stemRandom.RandomInRange(-growthProperties.StemAngleRange, growthProperties.StemAngleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                if (tree.StemRoot.HasSubnodes()) {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * crownRoot.GetDirection(true);
                    Node newNode = crownRoot.Add(crownRoot.GetPosition() + direction * growthProperties.GetGrowthDistance());
                    crownRoot = newNode;
                } else {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.StemRoot.GetDirection(true);
                    Node newNode = tree.StemRoot.Add(tree.StemRoot.GetPosition() + direction * growthProperties.GetGrowthDistance());
                    crownRoot = newNode;
                }
            }

            float rest = growthProperties.StemLength % growthProperties.GetGrowthDistance();
            if (!Util.AlmostEqual(rest, 0)) {
                float angle = stemRandom.RandomInRange(-growthProperties.StemAngleRange, growthProperties.StemAngleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                if (tree.StemRoot.HasSubnodes()) {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * crownRoot.GetDirection(true);
                    Node newNode = crownRoot.Add(crownRoot.GetPosition() + direction * rest);
                    crownRoot = newNode;
                } else {
                    Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.StemRoot.GetDirection(true);
                    Node newNode = tree.StemRoot.Add(tree.StemRoot.GetPosition() + direction * rest);
                    crownRoot = newNode;
                }
            }
        }

        nearestNodeAlgorithm.Add(crownRoot);
        growthProperties.GetAttractionPoints().UpdatePosition(crownRoot.GetPosition());
        growerListener.OnIterationFinished();
    }



    private void GrowCrownStem(Tree tree) {
        if (!Util.AlmostEqual(growthProperties.CrownStemLengthRatio, 0)) {
            Node crownStemTip = crownRoot;

            stemRandom = new AdvancedRandom(growthProperties.GetAttractionPoints().Seed);

            int iterations = (int)(growthProperties.CrownStemLengthRatio * growthProperties.GetAttractionPoints().GetHeight() / growthProperties.GetGrowthDistance());
            for (int i = 0; i < iterations; i++) {
                float angle = stemRandom.RandomInRange(-growthProperties.StemAngleRange, growthProperties.StemAngleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                Vector3 direction = Quaternion.AngleAxis(angle, axis) * crownRoot.GetDirection(true);
                crownStemTip = crownStemTip.Add(crownStemTip.GetPosition() + direction * growthProperties.GetGrowthDistance());

                nearestNodeAlgorithm.Add(crownStemTip);
            }

            float rest = growthProperties.StemLength % growthProperties.GetGrowthDistance();
            if (!Util.AlmostEqual(rest, 0)) {
                float angle = stemRandom.RandomInRange(-growthProperties.StemAngleRange, growthProperties.StemAngleRange);
                Vector3 axis = stemRandom.RandomVector3();
                axis.y = 0;

                Vector3 direction = Quaternion.AngleAxis(angle, axis) * crownRoot.GetDirection(true);
                crownStemTip = crownStemTip.Add(crownStemTip.GetPosition() + direction * growthProperties.GetGrowthDistance());

                nearestNodeAlgorithm.Add(crownStemTip);
            }
        }
        growerListener.OnIterationFinished();
    }


    private void GrowCrown(Tree tree) {
        float smallest_x = crownRoot.GetPosition().x;
        float biggest_x = crownRoot.GetPosition().x;
        float smallest_y = crownRoot.GetPosition().y;
        float biggest_y = crownRoot.GetPosition().y;
        float smallest_z = crownRoot.GetPosition().z;
        float biggest_z = crownRoot.GetPosition().z;

        int n_nodes = 0;

        treeHeight = 0;

        Stopwatch findClosePointStopwatch = new Stopwatch();
        Stopwatch removeClosePointsStopwatch = new Stopwatch();


        for (int i = 0; i < growthProperties.GetIterations(); i++) {

            if (!running) {
                return;
            }

            float influenceDistance = growthProperties.GetInfluenceDistance();
            float squaredClearDistance = growthProperties.GetSquaredClearDistance(i);

            Dictionary<Node, List<Vector3>> nodes_to_attractionPoints = new Dictionary<Node, List<Vector3>>();

            //iterate through all attractionPoints
            //foreach (Vector3 attractionPoint in growthProperties.GetAttractionPoints()) { //there is some threading problem with the enumeration foreach loop, usual for should fix it
            for (int j = 0; j < growthProperties.GetAttractionPoints().Points.Length; j++) {

                if (!running) {
                    return;
                }

                Vector3 attractionPoint = growthProperties.GetAttractionPoints().Points[j];
                if (growthProperties.GetAttractionPoints().ActivePoints[j]) {

                    if (attractionPoint.x < smallest_x - influenceDistance
                        || attractionPoint.x > biggest_x + influenceDistance
                        || attractionPoint.y < smallest_y - influenceDistance
                        || attractionPoint.y > biggest_y + influenceDistance
                        || attractionPoint.z < smallest_z - influenceDistance
                        || attractionPoint.z > biggest_z + influenceDistance
                        ) {
                        continue;
                    }

                    //and find the closest Node respectively
                    findClosePointStopwatch.Start();
                    Node closest = nearestNodeAlgorithm.GetNearestWithinSquaredDistance(attractionPoint);
                    findClosePointStopwatch.Stop();

                    //if there is a close Node
                    if (closest != null) {
                        // Rudis ultimate plan to make the removal in the next iteration
                        removeClosePointsStopwatch.Start();
                        if (i > 0) { //in the first iteration, the attraction points shall not get deleted
                            if (Util.SquaredDistance(attractionPoint, closest.GetPosition()) <= squaredClearDistance) {
                                growthProperties.GetAttractionPoints().ActivePoints[j] = false;
                                growthProperties.GetAttractionPoints().ActiveCount--;
                                removeClosePointsStopwatch.Stop();
                                continue;
                            } else {
                                removeClosePointsStopwatch.Stop();
                            }
                        }

                        //add it to the nodesAttractionPoints
                        if (nodes_to_attractionPoints.ContainsKey(closest)) {
                            nodes_to_attractionPoints[closest].Add(attractionPoint);
                        } else {
                            nodes_to_attractionPoints[closest] = new List<Vector3> { attractionPoint };
                        }
                    }
                }
            }

            if (!running) {
                return;
            }

            int n_newNodes = 0;

            //iterate through all Nodes with attractionPoints associated
            foreach (Node currentNode in nodes_to_attractionPoints.Keys) {
                List<Vector3> associatedAttractionPoints = nodes_to_attractionPoints[currentNode];

                //calculate direction
                Vector3 sum = new Vector3(0, 0, 0);
                foreach (Vector3 associatedAttractionPoint in associatedAttractionPoints) {
                    sum += (associatedAttractionPoint - currentNode.GetPosition()).normalized;
                    //sum += (associatedAttractionPoint - currentNode.GetPosition()) * (associatedAttractionPoint - currentNode.GetPosition()).magnitude;
                    //sum += (associatedAttractionPoint - currentNode.GetPosition()) * (1/((associatedAttractionPoint - currentNode.GetPosition()).magnitude* (associatedAttractionPoint - currentNode.GetPosition()).magnitude));
                }

                Vector3 direction = (sum + Util.Hadamard(growthProperties.GetTropisms(), growthProperties.TropismsWeights)).normalized * growthProperties.GetGrowthDistance();

                //sometimes tropisms make the nodes grow "backwards" again, this fixes that:
                float d_angle = Vector3.Angle(currentNode.GetDirection(), direction);
                if (d_angle > growthProperties.GetPerceptionAngle() / 2) {
                    float unallowedDiff = d_angle - growthProperties.GetPerceptionAngle() / 2;
                    Vector3 axis = Vector3.Cross(direction, currentNode.GetDirection());
                    direction = Quaternion.AngleAxis(unallowedDiff, axis) * direction;
                }

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
                    n_newNodes++;
                    n_nodes++;

                    //add to the nodeList
                    nearestNodeAlgorithm.Add(newNode);

                    // used by Core -> CameraMovement
                    if (treeHeight < happyNodePosition.y) {
                        treeHeight = happyNodePosition.y;
                    }
                    if (happyNodePosition.x < smallest_x) {
                        smallest_x = happyNodePosition.x;
                    }
                    if (happyNodePosition.x > biggest_x) {
                        biggest_x = happyNodePosition.x;
                    }
                    if (happyNodePosition.y < smallest_y) {
                        smallest_y = happyNodePosition.y;
                    }
                    if (happyNodePosition.y > biggest_y) {
                        biggest_y = happyNodePosition.y;
                    }
                    if (happyNodePosition.z < smallest_z) {
                        smallest_z = happyNodePosition.z;
                    }
                    if (happyNodePosition.z > biggest_z) {
                        biggest_z = happyNodePosition.z;
                    }
                }
            }

            //removeClosePointsStopwatch.Start();
            //RemoveClosePoints(newPositions, i);
            //removeClosePointsStopwatch.Stop();

            growerListener.OnIterationFinished();
            debug("finished iteration " + i);

            if (n_newNodes == 0) {
                growerListener.OnGrowthStopped();
                break;
            }
        }

        debug(n_nodes + " nodes");

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


    //#######################################################################################
    //##########                 INTERFACE IMPLEMENTIATION : Grower                ##########
    //#######################################################################################

    public GrowthProperties GetGrowthProperties() {
        return growthProperties;
    }
}
