using System;
using UnityEngine;

public interface NearestNodeAlgorithm {
    void Add(Node node);
    Node GetNearestWithinSquaredDistance(Vector3 attractionPoint, float maxSquaredDistance, float nodePerceptionAngle);
}
