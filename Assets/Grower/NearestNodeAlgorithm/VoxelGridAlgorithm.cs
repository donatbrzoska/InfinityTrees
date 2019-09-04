using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VoxelGridAlgorithm : NearestNodeAlgorithm {

    private static bool debugEnabled = false;
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: VoxelGridAlgorithm: " + callerName + "(): " + message);
        }
    }

    private static void debug(FormatString formatString, [CallerMemberName]string callerName = "") {
        if (debugEnabled) {
            UnityEngine.Debug.Log("DEBUG: VoxelGridAlgorithm: " + callerName + "(): " + formatString);
        }
    }

    float squaredMaxDistance;
    float perceptionAngle;

    List<Node>[,,] voxelGrid;
    int n_is;
    int n_js;
    int n_ks;

    PseudoEllipsoid attractionPoints;
    float voxelSize;

    public VoxelGridAlgorithm(PseudoEllipsoid attractionPoints, float squaredMaxDistance, float perceptionAngle) {
        this.attractionPoints = attractionPoints;

        this.squaredMaxDistance = squaredMaxDistance;
        this.perceptionAngle = perceptionAngle;
        this.voxelSize = (float)Math.Sqrt(squaredMaxDistance);
        //this.voxelSize = squaredInfluenceDistance;

        //x direction
        n_is = (int)Math.Ceiling(attractionPoints.GetWidth() / voxelSize);
        //y direction
        n_js = (int)Math.Ceiling(attractionPoints.GetHeight() / voxelSize);
        //z direction
        n_ks = (int)Math.Ceiling(attractionPoints.GetDepth() / voxelSize);

        debug("Cloud width: " + attractionPoints.GetWidth());
        debug("Cloud height: " + attractionPoints.GetHeight());
        debug("Cloud depth: " + attractionPoints.GetDepth());
        debug("Grid dimensions: " + n_is + "x" + n_js + "x" + n_ks);

        voxelGrid = new List<Node>[n_is, n_js, n_ks];

        for (int i = 0; i < n_is; i++) {
            for (int j = 0; j < n_js; j++) {
                for (int k = 0; k < n_ks; k++) {
                    voxelGrid[i, j, k] = new List<Node>();
                }
            }
        }
    }
    

    public void Add(Node node) {
        Vector3Int gridPos = PositionToGridPosition(node.Position);
        //debug("acessing: " + gridPos);
        voxelGrid[gridPos.x, gridPos.y, gridPos.z].Add(node);

        voxels_to_nodesAround.Clear();
    }


    public Node GetNearest(Vector3 position) {
        Node closest = null;

        Vector3Int gridPosition = PositionToGridPosition(position);

        List<Node> candidates = NodesAroundVoxel(gridPosition);
        //if (candidates.Count > 0) {
        //    debug("n candidates: " + candidates.Count);
        //}

        float closestDistance = float.MaxValue;

        foreach (Node n in candidates) {
            float squaredDistance = Util.SquaredDistance(position, n.Position);
            if (squaredDistance < closestDistance && squaredDistance <= squaredMaxDistance) {
                if (AttractionPointInPerceptionAngle(n, position)) {
                    closest = n;
                    closestDistance = squaredDistance;
                }
            }
        }

        return closest;
    }

    private bool AttractionPointInPerceptionAngle(Node node, Vector3 attractionPoint) {
        float angle = Vector3.Angle(node.GetDirection(), attractionPoint - node.Position);
        bool isInPerceptionAngle = angle <= perceptionAngle / 2f;
        return isInPerceptionAngle;
    }

    private int Crop(int value, int min, int max) {
        if (value < min) {
            return min;
        }

        if (value > max) {
            return max;
        }

        return value;
    }

    private Vector3Int PositionToGridPosition(Vector3 pos) {
        //crops are done, so positions outside the grid dont cause out of bounds exceptions
        int i = Crop((int)((attractionPoints.GetWidth() / 2 + pos.x) / voxelSize), 0, n_is-1);
        int j = Crop((int)(pos.y / voxelSize), 0, n_js-1);
        int k = Crop((int)((attractionPoints.GetDepth() / 2 + pos.z) / voxelSize), 0, n_ks-1);
        return new Vector3Int(i, j, k);
    }

    //public Stopwatch voxelsAroundSW = new Stopwatch();

    //cache
    private Dictionary<Vector3Int, List<Vector3Int>> voxels_to_voxelsAround = new Dictionary<Vector3Int, List<Vector3Int>>();
    private List<Vector3Int> VoxelsAroundVoxel(Vector3Int voxel) {
        //voxelsAroundSW.Start();
        List<Vector3Int> result = new List<Vector3Int>();

        if (voxels_to_voxelsAround.ContainsKey(voxel)) {
            result = voxels_to_voxelsAround[voxel];
        } else {
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    for (int k = -1; k <= 1; k++) {
                        Vector3Int pos = voxel + new Vector3Int(i, j, k);
                        if (pos.x > -1 && pos.x < n_is
                            && pos.y > -1 && pos.y < n_js
                            && pos.z > -1 && pos.z < n_ks) {

                            result.Add(pos);
                        }
                    }
                }
            }

            voxels_to_voxelsAround[voxel] = result;
        }
        //voxelsAroundSW.Stop();

        return result;
    }

    //public Stopwatch nodesAroundSW = new Stopwatch();

    //cache, needs to be reset in every iteration though because then new nodes are present in the tree,
    // .. the latter is done in the Add() method
    private Dictionary<Vector3Int, List<Node>> voxels_to_nodesAround = new Dictionary<Vector3Int, List<Node>>();
    private List<Node> NodesAroundVoxel(Vector3Int voxel) {
        //nodesAroundSW.Start();
        List<Node> result = new List<Node>();

        if (voxels_to_nodesAround.ContainsKey(voxel)) {
            result = voxels_to_nodesAround[voxel];
        } else {
            List<Vector3Int> voxelsAroundVoxel = VoxelsAroundVoxel(voxel);

            foreach (Vector3Int v in voxelsAroundVoxel) {
                foreach (Node n in voxelGrid[v.x, v.y, v.z]) {
                    result.Add(n);
                }
            }

            voxels_to_nodesAround[voxel] = result;
        }

        //nodesAroundSW.Stop();

        return result;
    }
}
