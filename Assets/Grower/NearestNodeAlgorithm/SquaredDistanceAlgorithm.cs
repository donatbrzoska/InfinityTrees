using System;
using System.Collections.Generic;
using UnityEngine;

public class SquaredDistanceAlgorithm : NearestNodeAlgorithm{

    List<Node> nodeList;
    float squaredInfluenceDistance;
    float perceptionAngle;

    public SquaredDistanceAlgorithm(float squaredInfluenceDistance, float perceptionAngle) {
        nodeList = new List<Node>();
        this.squaredInfluenceDistance = squaredInfluenceDistance;
        this.perceptionAngle = perceptionAngle;
    }

    public void Add(Node node) {
        nodeList.Add(node);
    }


    //returns null if there is no closest node
    public Node GetNearest(Vector3 attractionPoint) {

        float currentSmallestDistance = squaredInfluenceDistance;
        Node closest = null;

        foreach (Node current in nodeList) {

            float quadraticDistanceToCurrent = GetQuadraticDistanceWithMaxValue(current.Position, attractionPoint, currentSmallestDistance);

            if (quadraticDistanceToCurrent!=-1) { //check if the distance is smaller than required
                if (AttractionPointInPerceptionAngle(current, attractionPoint, perceptionAngle)) { //angle calculation is a lot slower than one distance calculation
                    currentSmallestDistance = quadraticDistanceToCurrent;
                    closest = current;
                }
            }
        }

        return closest;
    }

    private bool AttractionPointInPerceptionAngle(Node node, Vector3 attractionPoint, float nodePerceptionAngle) {
        float angle = Vector3.Angle(node.GetDirection(), attractionPoint - node.Position);
        bool isInPerceptionAngle = angle <= nodePerceptionAngle / 2f;
        return isInPerceptionAngle;
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
