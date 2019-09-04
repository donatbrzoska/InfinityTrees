using System;
using UnityEngine;

public interface NearestNodeAlgorithm {
    void Add(Node node);
    Node GetNearest(Vector3 attractionPoint);
}
