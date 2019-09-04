using System;
using System.Collections.Generic;
using UnityEngine;

public class StandardAlgorithm : NearestNodeAlgorithm {

    List<Node> nodeList;
    float influenceDistance;
    float perceptionAngle;

    public StandardAlgorithm(float influenceDistance, float perceptionAngle) {
        nodeList = new List<Node>();
        this.influenceDistance = influenceDistance;
        this.perceptionAngle = perceptionAngle;
    }

    public void Add(Node node) {
        nodeList.Add(node);
    }

    //returns null if there is no closest node
    public Node GetNearest(Vector3 attractionPoint) {

        float currentSmallestDistance = influenceDistance;
        Node closest = null;

        foreach (Node current in nodeList) {

            float distance = Vector3.Distance(current.Position, attractionPoint);

            if (distance <= currentSmallestDistance) { //check if the distance is smaller than required
                if (AttractionPointInPerceptionAngle(current, attractionPoint)) { //angle calculation is a lot slower than one distance calculation
                    currentSmallestDistance = distance;
                    closest = current;
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
}
