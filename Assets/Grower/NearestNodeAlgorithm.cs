using System.Collections.Generic;
using UnityEngine;

public class NearestNodeAlgorithm {

    SortedNodeList x = new SortedNodeList(CoordinateType.x);
    SortedNodeList y = new SortedNodeList(CoordinateType.y);
    SortedNodeList z = new SortedNodeList(CoordinateType.z);

    public void Add(Node node) {
        x.InsertSorted(node);
        y.InsertSorted(node);
        z.InsertSorted(node);
    }

    public Node GetNearestWithinSquaredDistance(Vector3 position, float maxSquaredDistance, float nodePerceptionAngle) {
        SortedCandidateNodeList candidates = new SortedCandidateNodeList();

        //find index of nearest Node concerning the x coordinate
        int nearest_x = x.GetNearestIndex(new Node(position));
        if (nearest_x == -1) {
            return null;
        }
        //look at left neighbours
        for (int i = nearest_x; i >= 0; i--) {
            Vector3 currentPosition = x[i].GetPosition();
            Vector3 d = currentPosition - position;

            //find out whether we are still in the x range
            float dx = d.x * d.x;
            if (dx <= maxSquaredDistance) {
                float distance = dx + d.y * d.y + d.z * d.z;
                if (distance <= maxSquaredDistance) {
                    candidates.InsertSorted(new CandidateNode(x[i], distance));
                }
            } else {
                break;
            }
        }
        //look at right neighbours
        for (int i = nearest_x + 1; i < x.Count; i++) {
            Vector3 currentPosition = x[i].GetPosition();
            Vector3 d = currentPosition - position;

            //find out whether we are still in the x range
            float dx = d.x * d.x; 
            if (dx <= maxSquaredDistance) {
                float distance = dx + d.y * d.y + d.z * d.z;
                if (distance <= maxSquaredDistance) {
                    candidates.InsertSorted(new CandidateNode(x[i], distance));
                }
            } else {
                break;
            }
        }


        int nearest_y = y.GetNearestIndex(new Node(position));
        for (int i = nearest_y; i >= 0; i--) {
            Vector3 currentPosition = y[i].GetPosition();
            Vector3 d = currentPosition - position;

            float dy = d.y * d.y;
            if (dy <= maxSquaredDistance) { //find out whether we are still in the y range
                float distance = d.x * d.x + dy + d.z * d.z;
                if (distance <= maxSquaredDistance) {
                    candidates.InsertSorted(new CandidateNode(y[i], distance));
                }
            } else {
                break;
            }
        }
        for (int i = nearest_y + 1; i < y.Count; i++) {
            Vector3 currentPosition = y[i].GetPosition();
            Vector3 d = currentPosition - position;

            float dy = d.y * d.y;
            if (dy <= maxSquaredDistance) { //find out whether we are still in the y range
                float distance = d.x * d.x + dy + d.z * d.z;
                if (distance <= maxSquaredDistance) {
                    candidates.InsertSorted(new CandidateNode(y[i], distance));
                }
            } else {
                break;
            }
        }


        int nearest_z = z.GetNearestIndex(new Node(position));
        for (int i = nearest_z; i >= 0; i--) {
            Vector3 currentPosition = z[i].GetPosition();
            Vector3 d = currentPosition - position;

            float dz = d.z * d.z;
            if (dz <= maxSquaredDistance) { //find out whether we are still in the z range
                float distance = d.x * d.x + d.y * d.y + dz;
                if (distance <= maxSquaredDistance) {
                    candidates.InsertSorted(new CandidateNode(z[i], distance));
                }
            } else {
                break;
            }
        }
        for (int i = nearest_z + 1; i < z.Count; i++) {
            Vector3 currentPosition = z[i].GetPosition();
            Vector3 d = currentPosition - position;

            float dz = d.z * d.z;
            if (dz <= maxSquaredDistance) { //find out whether we are still in the z range
                float distance = d.x * d.x + d.y * d.y + dz;
                if (distance <= maxSquaredDistance) {
                    candidates.InsertSorted(new CandidateNode(z[i], distance));
                }
            } else {
                break;
            }
        }

        //eventually determine closest node in perceptionAngle
        if (candidates.Count > 0) {
            for (int i = 0; i < candidates.Count; i++) {
                CandidateNode current = candidates[i];
                if (AttractionPointInPerceptionAngle(current.node, position, nodePerceptionAngle)) {
                    return current.node;
                }
            }
            return null;
        } else {
            return null;
        }
    }

    private bool AttractionPointInPerceptionAngle(Node node, Vector3 attractionPoint, float nodePerceptionAngle) {
        float angle = Vector3.Angle(node.GetDirection(), attractionPoint - node.GetPosition());
        bool isInPerceptionAngle = angle <= nodePerceptionAngle / 2f;
        return isInPerceptionAngle;
    }

    //public Node GetNearestWithinSquaredDistance(Vector3 position, float maxSquaredDistance, float nodePerceptionAngle) {
    //    SortedCandidateNodeList candidates = new SortedCandidateNodeList();
    //    //HashSet<Node> alreadyCandidate = new HashSet<Node>();

    //    int nearest_x = x.GetNearestIndex(new Node(position));
    //    for (int i = nearest_x; i >= 0; i--) {
    //        //if (!alreadyCandidate.Contains(x[i]) && x[i].Active) {
    //        if (x[i].Active) {
    //            Vector3 currentPosition = x[i].GetPosition();
    //            Vector3 d = currentPosition - position;

    //            float dx = d.x * d.x;
    //            if (dx <= maxSquaredDistance) { //find out whether we are still in the x range
    //                float distance = dx + d.y * d.y + d.z * d.z;
    //                if (distance <= maxSquaredDistance) {
    //                    candidates.InsertSorted(new CandidateNode(x[i], distance));
    //                    //alreadyCandidate.Add(x[i]);
    //                }
    //            } else {
    //                break;
    //            }
    //        }
    //    }
    //    for (int i = nearest_x + 1; i < x.Count; i++) {
    //        //if (!alreadyCandidate.Contains(x[i]) && x[i].Active) {
    //        if (x[i].Active) {
    //            Vector3 currentPosition = x[i].GetPosition();
    //            Vector3 d = currentPosition - position;

    //            float dx = d.x * d.x;
    //            if (dx <= maxSquaredDistance) { //find out whether we are still in the x range
    //                float distance = dx + d.y * d.y + d.z * d.z;
    //                if (distance <= maxSquaredDistance) {
    //                    candidates.InsertSorted(new CandidateNode(x[i], distance));
    //                    //alreadyCandidate.Add(x[i]);
    //                }
    //            } else {
    //                break;
    //            }
    //        }
    //    }


    //    int nearest_y = y.GetNearestIndex(new Node(position));
    //    for (int i = nearest_y; i >= 0; i--) {
    //        //if (!alreadyCandidate.Contains(y[i]) && y[i].Active) {
    //        if (y[i].Active) {
    //            Vector3 currentPosition = y[i].GetPosition();
    //            Vector3 d = currentPosition - position;

    //            float dy = d.y * d.y;
    //            if (dy <= maxSquaredDistance) { //find out whether we are still in the y range
    //                float distance = d.x * d.x + dy + d.z * d.z;
    //                if (distance <= maxSquaredDistance) {
    //                    candidates.InsertSorted(new CandidateNode(y[i], distance));
    //                    //alreadyCandidate.Add(y[i]);
    //                }
    //            } else {
    //                break;
    //            }
    //        }
    //    }
    //    for (int i = nearest_y + 1; i < y.Count; i++) {
    //        //if (!alreadyCandidate.Contains(y[i]) && y[i].Active) {
    //        if (y[i].Active) {
    //            Vector3 currentPosition = y[i].GetPosition();
    //            Vector3 d = currentPosition - position;

    //            float dy = d.y * d.y;
    //            if (dy <= maxSquaredDistance) { //find out whether we are still in the y range
    //                float distance = d.x * d.x + dy + d.z * d.z;
    //                if (distance <= maxSquaredDistance) {
    //                    candidates.InsertSorted(new CandidateNode(y[i], distance));
    //                    //alreadyCandidate.Add(y[i]);
    //                }
    //            } else {
    //                break;
    //            }
    //        }
    //    }


    //    int nearest_z = z.GetNearestIndex(new Node(position));
    //    for (int i = nearest_z; i >= 0; i--) {
    //        if (z[i].Active) {
    //            //if (!alreadyCandidate.Contains(z[i]) && z[i].Active) {
    //            Vector3 currentPosition = z[i].GetPosition();
    //            Vector3 d = currentPosition - position;

    //            float dz = d.z * d.z;
    //            if (dz <= maxSquaredDistance) { //find out whether we are still in the z range
    //                float distance = d.x * d.x + d.y * d.y + dz;
    //                if (distance <= maxSquaredDistance) {
    //                    candidates.InsertSorted(new CandidateNode(z[i], distance));
    //                    //alreadyCandidate.Add(z[i]);
    //                }
    //            } else {
    //                break;
    //            }
    //        }
    //    }
    //    for (int i = nearest_z + 1; i < z.Count; i++) {
    //        //if (!alreadyCandidate.Contains(z[i]) && z[i].Active) {
    //        if (z[i].Active) {
    //            Vector3 currentPosition = z[i].GetPosition();
    //            Vector3 d = currentPosition - position;

    //            float dz = d.z * d.z;
    //            if (dz <= maxSquaredDistance) { //find out whether we are still in the z range
    //                float distance = d.x * d.x + d.y * d.y + dz;
    //                if (distance <= maxSquaredDistance) {
    //                    candidates.InsertSorted(new CandidateNode(z[i], distance));
    //                    //alreadyCandidate.Add(z[i]);
    //                }
    //            } else {
    //                break;
    //            }
    //        }
    //    }

    //    if (candidates.Count > 0) {
    //        for (int i = 0; i < candidates.Count; i++) {
    //            CandidateNode current = candidates[i];
    //            if (AttractionPointInPerceptionAngle(current.node, position, nodePerceptionAngle)) {
    //                return current.node;
    //            }
    //        }
    //        return null;
    //    } else {
    //        return null;
    //    }
    //}

    //public Node GetNearestWithinSquaredDistance(Vector3 position, float maxSquaredDistance, float nodePerceptionAngle) {
    //    //SortedCandidateNodeList candidates = new SortedCandidateNodeList();
    //    CandidateNode closest = new CandidateNode(new Node(new Vector3(0, 0, 0)), float.MaxValue);

    //    int nearest_x = x.GetNearestIndex(new Node(position));
    //    for (int i = nearest_x; i >= 0; i--) {
    //        Node currentNode = x[i];
    //        Vector3 d = currentNode.GetPosition() - position;

    //        float dx = d.x * d.x;
    //        if (dx <= maxSquaredDistance) { //find out whether we are still in the x range
    //            float distance = dx + d.y * d.y + d.z * d.z;
    //            if (distance <= maxSquaredDistance && distance < closest.distance && AttractionPointInPerceptionAngle(currentNode, position, nodePerceptionAngle)) {
    //                closest = new CandidateNode(x[i], distance);
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_x + 1; i < x.Count; i++) {
    //        Node currentNode = x[i];
    //        Vector3 d = currentNode.GetPosition() - position;

    //        float dx = d.x * d.x;
    //        if (dx <= maxSquaredDistance) { //find out whether we are still in the x range
    //            float distance = dx + d.y * d.y + d.z * d.z;
    //            if (distance <= maxSquaredDistance && distance < closest.distance && AttractionPointInPerceptionAngle(currentNode, position, nodePerceptionAngle)) {
    //                closest = new CandidateNode(x[i], distance);
    //            }
    //        } else {
    //            break;
    //        }
    //    }


    //    int nearest_y = y.GetNearestIndex(new Node(position));
    //    for (int i = nearest_y; i >= 0; i--) {
    //        Node currentNode = y[i];
    //        Vector3 d = currentNode.GetPosition() - position;

    //        float dy = d.y * d.y;
    //        if (dy <= maxSquaredDistance) { //find out whether we are still in the y range
    //            float distance = d.x * d.x + dy + d.z * d.z;
    //            if (distance <= maxSquaredDistance && distance < closest.distance && AttractionPointInPerceptionAngle(currentNode, position, nodePerceptionAngle)) {
    //                closest = new CandidateNode(y[i], distance);
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_y + 1; i < y.Count; i++) {
    //        Node currentNode = y[i];
    //        Vector3 d = currentNode.GetPosition() - position;

    //        float dy = d.y * d.y;
    //        if (dy <= maxSquaredDistance) { //find out whether we are still in the y range
    //            float distance = d.x * d.x + dy + d.z * d.z;
    //            if (distance <= maxSquaredDistance && distance < closest.distance && AttractionPointInPerceptionAngle(currentNode, position, nodePerceptionAngle)) {
    //                closest = new CandidateNode(y[i], distance);
    //            }
    //        } else {
    //            break;
    //        }
    //    }


    //    int nearest_z = z.GetNearestIndex(new Node(position));
    //    for (int i = nearest_z; i >= 0; i--) {
    //        Node currentNode = z[i];
    //        Vector3 d = currentNode.GetPosition() - position;

    //        float dz = d.z * d.z;
    //        if (dz <= maxSquaredDistance) { //find out whether we are still in the z range
    //            float distance = d.x * d.x + d.y * d.y + dz;
    //            if (distance <= maxSquaredDistance && distance < closest.distance && AttractionPointInPerceptionAngle(currentNode, position, nodePerceptionAngle)) {
    //                closest = new CandidateNode(z[i], distance);
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_z + 1; i < z.Count; i++) {
    //        Node currentNode = z[i];
    //        Vector3 d = currentNode.GetPosition() - position;

    //        float dz = d.z * d.z;
    //        if (dz <= maxSquaredDistance) { //find out whether we are still in the z range
    //            float distance = d.x * d.x + d.y * d.y + dz;
    //            if (distance <= maxSquaredDistance && distance < closest.distance && AttractionPointInPerceptionAngle(currentNode, position, nodePerceptionAngle)) {
    //                closest = new CandidateNode(currentNode, distance);
    //            }
    //        } else {
    //            break;
    //        }
    //    }

    //    if (!Util.AlmostEqual(closest.distance, float.MaxValue, 0.0001f)) {
    //        return closest.node;
    //    } else {
    //        return null;
    //    }
    //}

    //### ORIGINAL ###
    //public Node GetNearestWithinSquaredDistance(Vector3 position, float maxSquaredDistance, float nodePerceptionAngle) {
    //    SortedCandidateNodeList candidates = new SortedCandidateNodeList();

    //    int nearest_x = x.GetNearestIndex(new Node(position));
    //    for (int i = nearest_x; i >= 0; i--) {
    //        Vector3 currentPosition = x[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float dx = d.x * d.x;
    //        if (dx <= maxSquaredDistance) { //find out whether we are still in the x range
    //            float distance = dx + d.y * d.y + d.z * d.z;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(x[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_x + 1; i < x.Count; i++) {
    //        Vector3 currentPosition = x[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float dx = d.x * d.x;
    //        if (dx <= maxSquaredDistance) { //find out whether we are still in the x range
    //            float distance = dx + d.y * d.y + d.z * d.z;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(x[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }


    //    int nearest_y = y.GetNearestIndex(new Node(position));
    //    for (int i = nearest_y; i >= 0; i--) {
    //        Vector3 currentPosition = y[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float dy = d.y * d.y;
    //        if (dy <= maxSquaredDistance) { //find out whether we are still in the y range
    //            float distance = d.x * d.x + dy + d.z * d.z;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(y[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_y + 1; i < y.Count; i++) {
    //        Vector3 currentPosition = y[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float dy = d.y * d.y;
    //        if (dy <= maxSquaredDistance) { //find out whether we are still in the y range
    //            float distance = d.x * d.x + dy + d.z * d.z;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(y[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }


    //    int nearest_z = z.GetNearestIndex(new Node(position));
    //    for (int i = nearest_z; i >= 0; i--) {
    //        Vector3 currentPosition = z[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float dz = d.z * d.z;
    //        if (dz <= maxSquaredDistance) { //find out whether we are still in the z range
    //            float distance = d.x * d.x + d.y * d.y + dz;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(z[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_z + 1; i < z.Count; i++) {
    //        Vector3 currentPosition = z[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float dz = d.z * d.z;
    //        if (dz <= maxSquaredDistance) { //find out whether we are still in the z range
    //            float distance = d.x * d.x + d.y * d.y + dz;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(z[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }

    //    if (candidates.Count > 0) {
    //        for (int i = 0; i < candidates.Count; i++) {
    //            CandidateNode current = candidates[i];
    //            if (AttractionPointInPerceptionAngle(current.node, position, nodePerceptionAngle)) {
    //                return current.node;
    //            }
    //        }
    //        return null;
    //    } else {
    //        return null;
    //    }
    //}




    //### EQUALLY FAST ###
    //public Node GetNearestWithinSquaredDistance(Vector3 position, float maxSquaredDistance, float nodePerceptionAngle) {
    //    SortedCandidateNodeList candidates = new SortedCandidateNodeList();

    //    int nearest_x = x.GetNearestIndex(new Node(position));
    //    for (int i = nearest_x; i >= 0; i--) {
    //        Vector3 currentPosition = x[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float distance = d.x * d.x;
    //        //float dx = d.x * d.x;
    //        if (distance <= maxSquaredDistance) { //find out whether we are still in the x range
    //            distance += d.y * d.y;
    //            if (distance > maxSquaredDistance) {
    //                continue;
    //            }

    //            distance += d.z * d.z;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(x[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_x + 1; i < x.Count; i++) {
    //        Vector3 currentPosition = x[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float distance = d.x * d.x;
    //        //float dx = d.x * d.x;
    //        if (distance <= maxSquaredDistance) { //find out whether we are still in the x range
    //            distance += d.y * d.y;
    //            if (distance > maxSquaredDistance) {
    //                continue;
    //            }

    //            distance += d.z * d.z;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(x[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }


    //    int nearest_y = y.GetNearestIndex(new Node(position));
    //    for (int i = nearest_y; i >= 0; i--) {
    //        Vector3 currentPosition = y[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float distance = d.y * d.y;
    //        if (distance <= maxSquaredDistance) { //find out whether we are still in the y range
    //            distance += d.x * d.x;
    //            if (distance > maxSquaredDistance) {
    //                continue;
    //            }

    //            distance += d.z * d.z;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(y[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_y + 1; i < y.Count; i++) {
    //        Vector3 currentPosition = y[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float distance = d.y * d.y;
    //        if (distance <= maxSquaredDistance) { //find out whether we are still in the y range
    //            distance += d.x * d.x;
    //            if (distance > maxSquaredDistance) {
    //                continue;
    //            }

    //            distance += d.z * d.z;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(y[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }


    //    int nearest_z = z.GetNearestIndex(new Node(position));
    //    for (int i = nearest_z; i >= 0; i--) {
    //        Vector3 currentPosition = z[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float distance = d.z * d.z;
    //        if (distance <= maxSquaredDistance) { //find out whether we are still in the y range
    //            distance += d.x * d.x;
    //            if (distance > maxSquaredDistance) {
    //                continue;
    //            }

    //            distance += d.y * d.y;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(z[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }
    //    for (int i = nearest_z + 1; i < z.Count; i++) {
    //        Vector3 currentPosition = z[i].GetPosition();
    //        Vector3 d = currentPosition - position;

    //        float distance = d.z * d.z;
    //        if (distance <= maxSquaredDistance) { //find out whether we are still in the y range
    //            distance += d.x * d.x;
    //            if (distance > maxSquaredDistance) {
    //                continue;
    //            }

    //            distance += d.y * d.y;
    //            if (distance <= maxSquaredDistance) {
    //                candidates.InsertSorted(new CandidateNode(z[i], distance));
    //            }
    //        } else {
    //            break;
    //        }
    //    }

    //    if (candidates.Count > 0) {
    //        for (int i = 0; i < candidates.Count; i++) {
    //            CandidateNode current = candidates[i];
    //            if (AttractionPointInPerceptionAngle(current.node, position, nodePerceptionAngle)) {
    //                return current.node;
    //            }
    //        }
    //        return null;
    //    } else {
    //        return null;
    //    }
    //}
}