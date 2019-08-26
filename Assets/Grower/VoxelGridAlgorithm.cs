using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VoxelGridAlgorithm {

    private static bool debugEnabled = true;
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

    List<Node>[,,] voxelGrid;
    int n_is;
    int n_js;
    int n_ks;

    PseudoEllipsoid attractionPoints;
    float voxelSize;

    //Dictionary<Vector3, Vector3Int> attractionPoints_to_voxelCoordinates;

    public VoxelGridAlgorithm(PseudoEllipsoid attractionPoints, float voxelSize) {
        this.attractionPoints = attractionPoints;
        this.voxelSize = voxelSize;

        //for every attraction point, cache its voxel
        //attractionPoints_to_voxelCoordinates = new Dictionary<Vector3, Vector3Int>();
        //foreach (Vector3 a in attractionPoints) {
        //    Vector3Int gridPos = PositionToGridPosition(a);
        //    attractionPoints_to_voxelCoordinates.Add(a, gridPos);
        //}


        //x direction
        n_is = (int)Math.Ceiling(attractionPoints.GetWidth() / voxelSize);
        //y direction
        n_js = (int)Math.Ceiling(attractionPoints.GetHeight() / voxelSize);
        //z direction
        n_ks = (int)Math.Ceiling(attractionPoints.GetDepth() / voxelSize);

        voxelGrid = new List<Node>[n_is, n_js, n_ks];
        debug("Cloud width: " + attractionPoints.GetWidth());
        debug("Cloud height: " + attractionPoints.GetHeight());
        debug("Cloud depth: " + attractionPoints.GetDepth());
        debug("Grid dimensions: " + n_is + "x" + n_js + "x" + n_ks);

        for (int i = 0; i < n_is; i++) {
            for (int j = 0; j < n_js; j++) {
                for (int k = 0; k < n_ks; k++) {
                    voxelGrid[i, j, k] = new List<Node>();
                }
            }
        }
    }

    public void Add(Node node) {
        Vector3Int gridPos = PositionToGridPosition(node.GetPosition());
        //debug("acessing: " + gridPos);
        voxelGrid[gridPos.x, gridPos.y, gridPos.z].Add(node);
    }


    public Node GetNearestWithinSquaredDistance(Vector3 position, float maxSquaredDistance, float nodePerceptionAngle) {
        Vector3Int gridPosition = PositionToGridPosition(position);
        List<Node> candidates = NodesAroundVoxel(gridPosition);
        //if (candidates.Count > 0) {
        //    debug("n candidates: " + candidates.Count);
        //}

        Node closest = null;
        float closestDistance = float.MaxValue;

        foreach (Node n in candidates) {
            Vector3 d = n.GetPosition() - position;
            float squaredDistance = d.x * d.x + d.y * d.y + d.z * d.z;
            if (squaredDistance < closestDistance && squaredDistance < maxSquaredDistance) {
                if (AttractionPointInPerceptionAngle(n, position, nodePerceptionAngle)) {
                    closest = n;
                    closestDistance = squaredDistance;
                }
            }
        }

        return closest;
    }

    //public void RemoveClosePoints(List<Vector3> newPositions, float squaredClearDistance) {
    //    foreach (Vector3 newPosition in newPositions) {
    //        List<Vector3> closePoints;

    //        //PositionToGridPosition

    //        //debug("squared cleardistance is " + growthProperties.GetSquaredClearDistance(iteration));
    //        closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance(iteration));
    //        //closePoints = DetermineAttractionPointsWithinQuadraticDistance(newPosition, growthProperties.GetSquaredClearDistance()); //near the envelope test
    //        //debug("removing " + closePoints.Count + " close points");
    //        foreach (Vector3 closePoint in closePoints) {
    //            growthProperties.GetAttractionPoints().Remove(closePoint);
    //            //if (voxelGridAlgorithm != null) {
    //            //    voxelGridAlgorithm.RemoveAttractionPoint(closePoint);
    //            //}
    //        }
    //    }
    //}

    private bool AttractionPointInPerceptionAngle(Node node, Vector3 attractionPoint, float nodePerceptionAngle) {
        float angle = Vector3.Angle(node.GetDirection(), attractionPoint - node.GetPosition());
        bool isInPerceptionAngle = angle <= nodePerceptionAngle / 2f;
        return isInPerceptionAngle;
    }

    private int Crop(int v, int lo, int hi) {
        if (v < lo) {
            return lo;
        }
        if (v > hi) {
            return hi;
        } else {
            return v;
        }
        //return Math.Max(Math.Min(v, hi), lo);
    }

    private Vector3Int PositionToGridPosition(Vector3 pos/*, float gridWidth, float gridDepth, float gridHeight*/) {
        int i = Crop((int)((attractionPoints.GetWidth() / 2 + pos.x) / voxelSize), 0, n_is-1);
        int j = Crop((int)(pos.y / voxelSize), 0, n_js-1);
        int k = Crop((int)((attractionPoints.GetDepth() / 2 + pos.z) / voxelSize), 0, n_ks-1);

        return new Vector3Int(i, j, k);
    }

    private List<Node> NodesAroundVoxel(Vector3Int voxel) {
        List<Node> result = new List<Node>();

        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                for (int k = -1; k <= 1; k++) {
                    Vector3Int pos = voxel + new Vector3Int(i, j, k);
                    if (pos.x > -1 && pos.x < n_is
                        && pos.y > -1 && pos.y < n_js
                        && pos.z > -1 && pos.z < n_ks) {

                        foreach (Node n in voxelGrid[pos.x, pos.y, pos.z]) {
                            result.Add(n);
                        }
                    }
                }
            }
        }

        return result;
    }

    //private List<Vector3> PositionsAroundVoxel(Vector3Int voxel) {
    //    List<Vector3> result = new List<Vector3>();

    //    for (int i = -1; i <= 1; i++) {
    //        for (int j = -1; j <= 1; j++) {
    //            for (int k = -1; k <= 1; k++) {
    //                Vector3Int pos = voxel + new Vector3Int(i, j, k);
    //                if (pos.x > -1 && pos.x < n_is
    //                    && pos.y > -1 && pos.y < n_js
    //                    && pos.z > -1 && pos.z < n_ks) {

    //                    foreach (Node n in voxelGrid[pos.x, pos.y, pos.z]) {
    //                        result.Add(n);
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return result;
    //}
}
