using System;
using System.Collections.Generic;
using UnityEngine;

public class StandardAlgorithm : NearestNodeAlgorithm {

    List<Node> nodeList;
    PseudoEllipsoid attractionPoints;


    public StandardAlgorithm(PseudoEllipsoid attractionPoints) {
        nodeList = new List<Node>();
        this.attractionPoints = attractionPoints;
    }

    public void Add(Node node) {
        nodeList.Add(node);
    }

    float distance=-1;

    //returns null if there is no closest node
    public Node GetNearestWithinSquaredDistance(Vector3 attractionPoint, float maxSquaredDistance, float nodePerceptionAngle) {

        float currentSmallestDistance = maxSquaredDistance;
        Node closest = null;

        foreach (Node current in nodeList) {

            float quadraticDistanceToCurrent = GetQuadraticDistanceWithMaxValue(current.GetPosition(), attractionPoint, currentSmallestDistance);

            if (quadraticDistanceToCurrent != -1) { //check if the distance is smaller than required
                if (AttractionPointInPerceptionAngle(current, attractionPoint, nodePerceptionAngle)) { //angle calculation is a lot slower than one distance calculation
                    currentSmallestDistance = quadraticDistanceToCurrent;
                    closest = current;
                }
            }
        }

        return closest;
    }

    private bool AttractionPointInPerceptionAngle(Node node, Vector3 attractionPoint, float nodePerceptionAngle) {
        float angle = Vector3.Angle(node.GetDirection(), attractionPoint - node.GetPosition());
        bool isInPerceptionAngle = angle <= nodePerceptionAngle / 2f;
        return isInPerceptionAngle;
    }

    //for every node position, stores the distance to every attraction point
    private List<Vector3> DetermineAttractionPointsWithinQuadraticDistance(Vector3 position, float maxDistance) {
        List<Vector3> result = new List<Vector3>();
        foreach (Vector3 attractionPoint in attractionPoints) {

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
}
