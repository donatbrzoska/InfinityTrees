using System;
using System.Collections.Generic;
using UnityEngine;

public class SortedNodeList : List<Node> {

    public void InsertSorted(Node node) {
        if (base.Count == 0) {
            base.Add(node);
        } else {

        }
    }

    public Node GetNearest(Vector3 position) {
        return null;
    }
}
