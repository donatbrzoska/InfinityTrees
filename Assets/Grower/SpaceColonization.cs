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

    NearestNodeAlgorithm nearestNodeAlgorithm;
    //VoxelGridAlgorithm nearestNodeAlgorithm;

    // used by Core -> CameraMovement
    public float GetTreeHeight() {
        return biggest_y - smallest_y;
    }

    private float smallest_x;
    private float biggest_x;
    private float smallest_y;
    private float biggest_y;
    private float smallest_z;
    private float biggest_z;

    private void ResetBounds() {
        smallest_x = float.MaxValue;
        biggest_x = float.MinValue;
        smallest_y = float.MaxValue;
        biggest_y = float.MinValue;
        smallest_z = float.MaxValue;
        biggest_z = float.MinValue;
    }

    private void UpdateBoundStorage(Vector3 happyNodePosition) {
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

    private bool OutOfBounds(Vector3 position, float influenceDistance) {
        bool result = position.x < smallest_x - influenceDistance
                    || position.x > biggest_x + influenceDistance
                    || position.y < smallest_y - influenceDistance
                    || position.y > biggest_y + influenceDistance
                    || position.z < smallest_z - influenceDistance
                    || position.z > biggest_z + influenceDistance;
        return result;
    }

    public GrowthProperties GrowthProperties { get; private set; }
    GrowerListener growerListener;

    public SpaceColonization(GrowthProperties growthProperties, GrowerListener growerListener) {
        this.GrowthProperties = growthProperties;
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
        ResetBounds();

        //nearestNodeAlgorithm = new StandardAlgorithm(growthProperties.GetInfluenceDistance(), growthProperties.GetPerceptionAngle());
        //nearestNodeAlgorithm = new SquaredDistanceAlgorithm(growthProperties.GetSquaredInfluenceDistance(), growthProperties.GetPerceptionAngle());
        //nearestNodeAlgorithm = new BinarySearchAlgorithm(growthProperties.GetSquaredInfluenceDistance(), growthProperties.GetPerceptionAngle()); //little bug somewhere
        nearestNodeAlgorithm = new VoxelGridAlgorithm(GrowthProperties.AttractionPoints, GrowthProperties.GetSquaredInfluenceDistance(), GrowthProperties.PerceptionAngle);

        //if (growerThread == null) {
        growerThread = new Thread(() => {
            running = true;

            Stopwatch growingStopwatch = new Stopwatch();
            growingStopwatch.Start();

            GrowStem(tree);
            GrowCrownStem(tree);
            GrowCrown(tree);

            growingStopwatch.Stop();
            debug(new FormatString("grew {0} times in {1}", GrowthProperties.Iterations, growingStopwatch.Elapsed));
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
        Vector3 pos_diff = crownRoot.Position - CurrentCrownRoot.Position;
        CurrentCrownRoot.UpdatePosition(pos_diff);
        foreach (Node n in CurrentCrownRoot.Subnodes) {
            crownRoot.Add(n);
            //have a look at the Add method again, if you want the move the attraction points towards the stem too
        }

        growerListener.OnIterationFinished();
    }

    // grows a stem on the given node
    // uses the tree.StemRoot for max angle difference
    // adds the nodes to the nearest node optimization stuff if specified
    // returns the tip (last node created)
    private Node ProceduralStem(Tree tree, Node node, float length, bool addToNearestNodeAlgorithmAndBounds) {
        float left = length;

        stemRandom = new AdvancedRandom(GrowthProperties.AttractionPoints.Seed);

        while (left > 0) {
            float growthDistance;
            if (left > GrowthProperties.GrowthDistance) {
                growthDistance = GrowthProperties.GrowthDistance;
            } else { //this automatically hits in the last iteration
                growthDistance = left;
            }
            left = left - growthDistance;

            float angle = stemRandom.RandomInRange(-GrowthProperties.StemAngleRange, GrowthProperties.StemAngleRange);
            Vector3 axis = stemRandom.RandomVector3();
            axis.y = 0;

            Vector3 direction = Quaternion.AngleAxis(angle, axis) * tree.StemRoot.GetDirection(true);
            node = node.Add(node.Position + direction * growthDistance);

            if (addToNearestNodeAlgorithmAndBounds) {
                nearestNodeAlgorithm.Add(node);
                UpdateBoundStorage(node.Position);
            }
        }

        return node;
    }

    private void GrowStem(Tree tree) {
        crownRoot = tree.StemRoot;
        crownRoot = ProceduralStem(tree, crownRoot, GrowthProperties.StemLength, false);

        nearestNodeAlgorithm.Add(crownRoot);
        GrowthProperties.AttractionPoints.UpdatePosition(crownRoot.Position);
        growerListener.OnIterationFinished();
    }



    private void GrowCrownStem(Tree tree) {
        ProceduralStem(tree, crownRoot, GrowthProperties.CrownStemLengthRatio * GrowthProperties.AttractionPoints.GetHeight(), true);
        growerListener.OnIterationFinished();
    }


    private void GrowCrown(Tree tree) {
        UpdateBoundStorage(crownRoot.Position);
        int n_nodes = 0;

        Stopwatch findClosePointStopwatch = new Stopwatch();
        Stopwatch removeClosePointsStopwatch = new Stopwatch();


        for (int i = 0; i < GrowthProperties.Iterations; i++) {

            if (!running) {
                return;
            }

            float influenceDistance = GrowthProperties.GetInfluenceDistance();
            float squaredClearDistance = GrowthProperties.GetSquaredClearDistance(i);

            Dictionary<Node, List<Vector3>> nodes_to_attractionPoints = new Dictionary<Node, List<Vector3>>();

            //iterate through all attractionPoints
            //foreach (Vector3 attractionPoint in growthProperties.AttractionPoints) { //there is some threading problem with the enumeration foreach loop, usual for should fix it
            for (int j = 0; j < GrowthProperties.AttractionPoints.Points.Length; j++) {

                if (!running) {
                    return;
                }

                Vector3 attractionPoint = GrowthProperties.AttractionPoints.Points[j];
                if (GrowthProperties.AttractionPoints.ActivePoints[j]) {

                    if (OutOfBounds(attractionPoint, influenceDistance)) {
                        continue;
                    }

                    //and find the closest Node respectively
                    findClosePointStopwatch.Start();
                    Node closest = nearestNodeAlgorithm.GetNearest(attractionPoint);
                    findClosePointStopwatch.Stop();

                    //if there is a close Node
                    if (closest != null) {
                        // Rudis ultimate plan to make the removal in the next iteration
                        removeClosePointsStopwatch.Start();
                        if (i > 0) { //in the first iteration, the attraction points shall not get deleted
                            if (Util.SquaredDistance(attractionPoint, closest.Position) <= squaredClearDistance) {
                                GrowthProperties.AttractionPoints.ActivePoints[j] = false;
                                GrowthProperties.AttractionPoints.ActiveCount--;
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
                    sum += (associatedAttractionPoint - currentNode.Position).normalized;
                }

                Vector3 direction = (sum + Util.Hadamard(GrowthProperties.Tropisms, GrowthProperties.TropismsWeights)).normalized * GrowthProperties.GrowthDistance;

                //sometimes tropisms make the nodes grow "backwards" again, this fixes that:
                float d_angle = Vector3.Angle(currentNode.GetDirection(), direction);
                if (d_angle > GrowthProperties.PerceptionAngle / 2) {
                    float unallowedDiff = d_angle - GrowthProperties.PerceptionAngle / 2;
                    Vector3 axis = Vector3.Cross(direction, currentNode.GetDirection());
                    direction = Quaternion.AngleAxis(unallowedDiff, axis) * direction;
                }

                //and new nodes position
                Vector3 happyNodePosition = currentNode.Position + direction;


                if (!IsDuplicateNode(happyNodePosition, currentNode)) {
                    //add new node to currentNode
                    Node newNode = currentNode.Add(happyNodePosition);
                    n_newNodes++;
                    n_nodes++;

                    //add to the nodeList
                    nearestNodeAlgorithm.Add(newNode);

                    UpdateBoundStorage(happyNodePosition);
                }
            }

            //removeClosePointsStopwatch.Start();
            //RemoveClosePoints(newPositions, i);
            //removeClosePointsStopwatch.Stop();

            growerListener.OnIterationFinished();
            //debug("finished iteration " + i);

            if (n_newNodes == 0) {
                growerListener.OnGrowthStopped();
                break;
            }
        }

        debug(n_nodes + " nodes");

        debug(new FormatString("finding close points took {0}", findClosePointStopwatch.Elapsed));
        debug(new FormatString("removing close points took {0}", removeClosePointsStopwatch.Elapsed));

        running = false;
    }

    private bool IsDuplicateNode(Vector3 potentialPosition, Node node) {
        foreach (Node subnode in node.Subnodes) {
            if (subnode.Position.Equals(potentialPosition)) {
                return true;
            }
        }
        return false;
    }
}
