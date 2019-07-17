using System;
using System.Collections.Generic;
using UnityEngine;

public enum CoordinateType {
    x,
    y,
    z
}

public class SortedNodeList : List<Node> {

    CoordinateType coordinateType;

    public SortedNodeList(CoordinateType coordinateType) {
        this.coordinateType = coordinateType;
    }

    public void InsertSorted(Node e) {
        if (base.Count == 0) {
            base.Add(e);
        }

        int i = GetNearestIndex(e);

        if (LessThan(e, base[i]) || Equal(e, base[i])) { // if e is less or equal to the found index, the new element can be placed at that index
            base.Insert(i, e);
        } else { // otherwise e is greater, so the new element is placed right next to it
            base.Insert(i + 1, e);
        }
    }

    public Node GetNearest(Node e) {
        return base[GetNearestIndex(e)];
    }

    public int GetNearestIndex(Node e) {
        if (base.Count == 0) {
            throw new System.Exception("List is empty...");
        } else if (base.Count == 1) {
            return 0;
        } else {
            int l = 0;
            int r = base.Count - 1;

            int d = r - l;
            int i = l + d / 2;
            bool found = false;

            while (!found && l < r) {
                // l<r is important because it may happen, that for example r gets smaller than l, ({ 2, 2, 4, 9, 12, 12, 34, 34} insert 13)
                // in this case we just need to look at the neighbours and are done

                d = r - l;
                if (d == 0) {
                    break;
                }
                i = l + d / 2;
                Node current = base[i];

                if (Equals(e, current)) {
                    found = true;
                    break;
                }

                if (LessThan(e, current)) {
                    if (i == 0) { // when the element is smaller than the current and we already look at the smallest, the result is obvious
                        found = true;
                        break;
                    } // otherwise look at the left
                    r = i - 1;
                } else {
                    if (i == base.Count - 1) { //when the element is greater than the current and we already look at the greatest, the result is obvious
                        found = true;
                        break;
                    } // otherwise look at the right
                    l = i + 1;
                }
            }

            if (found) {
                return i;
            } else {
                if (i == 0) {
                    Node mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    Node right = base[i + 1];
                    float d_right = Math.Abs(Minus(right, e));

                    if (d_mid <= d_right) {
                        return i;
                    } else {
                        return i + 1;
                    }
                } else if (i == base.Count - 1) {
                    Node left = base[i - 1];
                    float d_left = Math.Abs(Minus(left, e));

                    Node mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    if (d_left <= d_mid) {
                        return i - 1;
                    } else {
                        return i;
                    }
                } else {
                    // when d==0, it is:
                    // - base[i-1]<e -> because of that l was increased
                    // - base[i]! =e -> because of that the Equals condition didn't match
                    // - base[i+1]>e -> because of that r was decreased
                    // so we need to find out which one is the closest
                    Node left = base[i - 1];
                    float d_left = Math.Abs(Minus(left, e));

                    Node mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    Node right = base[i + 1];
                    float d_right = Math.Abs(Minus(right, e));

                    if (d_left <= d_right && d_left <= d_mid) { //when they are equally distanced, return the left one
                        return i - 1;
                    } else if (d_mid < d_right && d_mid < d_left) {
                        return i;
                    } else /*if (d_right < d_mid && d_right < d_left)*/ {
                        return i + 1;
                    }
                }
            }
        }
    }

    private bool Equal(Node a, Node b) {
        switch (coordinateType) {
            case CoordinateType.x:
                return Util.AlmostEqual(a.GetPosition().x, b.GetPosition().x, 0.0001f);
            case CoordinateType.y:
                return Util.AlmostEqual(a.GetPosition().y, b.GetPosition().y, 0.0001f);
            case CoordinateType.z:
                return Util.AlmostEqual(a.GetPosition().z, b.GetPosition().z, 0.0001f);
            default:
                return false;
        }
    }

    private bool LessThan(Node a, Node b) {
        switch (coordinateType) {
            case CoordinateType.x:
                return a.GetPosition().x < b.GetPosition().x;
            case CoordinateType.y:
                return a.GetPosition().y < b.GetPosition().y;
            case CoordinateType.z:
                return a.GetPosition().z < b.GetPosition().z;
            default: //impossible but neccessary
                return false;
        }
    }

    private float Minus(Node a, Node b) {
        switch (coordinateType) {
            case CoordinateType.x:
                return a.GetPosition().x - b.GetPosition().x;
            case CoordinateType.y:
                return a.GetPosition().y - b.GetPosition().y;
            case CoordinateType.z:
                return a.GetPosition().z - b.GetPosition().z;
            default: //impossible but neccessary
                return 0;
        }
    }
}

public class CandidateNode {
    public Node node;
    public float distance;

    public CandidateNode(Node node, float distance) {
        this.node = node;
        this.distance = distance;
    }
}

public class SortedCandidateNodeList : List<CandidateNode> {

    public void InsertSorted(CandidateNode e) {
        if (base.Count == 0) {
            base.Add(e);
        }

        int i = GetNearestIndex(e);

        if (LessThan(e, base[i]) || Equal(e, base[i])) { // if e is less or equal to the found index, the new element can be placed at that index
            base.Insert(i, e);
        } else { // otherwise e is greater, so the new element is placed right next to it
            base.Insert(i + 1, e);
        }
    }

    public CandidateNode GetNearest(CandidateNode e) {
        return base[GetNearestIndex(e)];
    }

    public int GetNearestIndex(CandidateNode e) {
        if (base.Count == 0) {
            throw new System.Exception("List is empty...");
        } else if (base.Count == 1) {
            return 0;
        } else {
            int l = 0;
            int r = base.Count - 1;

            int d = r - l;
            int i = l + d / 2;
            bool found = false;

            while (!found && l < r) {
                // l<r is important because it may happen, that for example r gets smaller than l, ({ 2, 2, 4, 9, 12, 12, 34, 34} insert 13)
                // in this case we just need to look at the neighbours and are done

                d = r - l;
                if (d == 0) {
                    break;
                }
                i = l + d / 2;
                CandidateNode current = base[i];

                if (Equals(e, current)) {
                    found = true;
                    break;
                }

                if (LessThan(e, current)) {
                    if (i == 0) { // when the element is smaller than the current and we already look at the smallest, the result is obvious
                        found = true;
                        break;
                    } // otherwise look at the left
                    r = i - 1;
                } else {
                    if (i == base.Count - 1) { //when the element is greater than the current and we already look at the greatest, the result is obvious
                        found = true;
                        break;
                    } // otherwise look at the right
                    l = i + 1;
                }
            }

            if (found) {
                return i;
            } else {
                if (i == 0) {
                    CandidateNode mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    CandidateNode right = base[i + 1];
                    float d_right = Math.Abs(Minus(right, e));

                    if (d_mid <= d_right) {
                        return i;
                    } else {
                        return i + 1;
                    }
                } else if (i == base.Count - 1) {
                    CandidateNode left = base[i - 1];
                    float d_left = Math.Abs(Minus(left, e));

                    CandidateNode mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    if (d_left <= d_mid) {
                        return i - 1;
                    } else {
                        return i;
                    }
                } else {
                    // when d==0, it is:
                    // - base[i-1]<e -> because of that l was increased
                    // - base[i]! =e -> because of that the Equals condition didn't match
                    // - base[i+1]>e -> because of that r was decreased
                    // so we need to find out which one is the closest
                    CandidateNode left = base[i - 1];
                    float d_left = Math.Abs(Minus(left, e));

                    CandidateNode mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    CandidateNode right = base[i + 1];
                    float d_right = Math.Abs(Minus(right, e));

                    if (d_left <= d_right && d_left <= d_mid) { //when they are equally distanced, return the left one
                        return i - 1;
                    } else if (d_mid < d_right && d_mid < d_left) {
                        return i;
                    } else /*if (d_right < d_mid && d_right < d_left)*/ {
                        return i + 1;
                    }
                }
            }
        }
    }

    private bool Equal(CandidateNode a, CandidateNode b) {
        return Util.AlmostEqual(a.distance, b.distance, 0.0001f);
    }

    private bool LessThan(CandidateNode a, CandidateNode b) {
        return a.distance < b.distance;
    }

    private float Minus(CandidateNode a, CandidateNode b) {
        return a.distance - b.distance;
    }
}



















public class SortedVector3List : List<Vector3> {

    CoordinateType coordinateType;

    public SortedVector3List(CoordinateType coordinateType) {
        this.coordinateType = coordinateType;
    }

    public void InsertSorted(Vector3 e) {
        if (base.Count == 0) {
            base.Add(e);
        }

        int i = GetNearestIndex(e);

        if (LessThan(e, base[i]) || Equal(e, base[i])) { // if e is less or equal to the found index, the new element can be placed at that index
            base.Insert(i, e);
        } else { // otherwise e is greater, so the new element is placed right next to it
            base.Insert(i + 1, e);
        }
    }

    public int GetNearestIndex(Vector3 e) {
        if (base.Count == 0) {
            throw new System.Exception("List is empty...");
        } else if (base.Count == 1) {
            return 0;
        } else {
            int l = 0;
            int r = base.Count - 1;

            int d = r - l;
            int i = l + d / 2;
            bool found = false;

            while (!found && l < r) {
                // l<r is important because it may happen, that for example r gets smaller than l, ({ 2, 2, 4, 9, 12, 12, 34, 34} insert 13)
                // in this case we just need to look at the neighbours and are done

                d = r - l;
                if (d == 0) {
                    break;
                }
                i = l + d / 2;
                Vector3 current = base[i];

                if (Equals(e, current)) {
                    found = true;
                    break;
                }

                if (LessThan(e, current)) {
                    if (i == 0) { // when the element is smaller than the current and we already look at the smallest, the result is obvious
                        found = true;
                        break;
                    } // otherwise look at the left
                    r = i - 1;
                } else {
                    if (i == base.Count - 1) { //when the element is greater than the current and we already look at the greatest, the result is obvious
                        found = true;
                        break;
                    } // otherwise look at the right
                    l = i + 1;
                }
            }

            if (found) {
                return i;
            } else {
                if (i == 0) {
                    Vector3 mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    Vector3 right = base[i + 1];
                    float d_right = Math.Abs(Minus(right, e));

                    if (d_mid <= d_right) {
                        return i;
                    } else {
                        return i + 1;
                    }
                } else if (i == base.Count - 1) {
                    Vector3 left = base[i - 1];
                    float d_left = Math.Abs(Minus(left, e));

                    Vector3 mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    if (d_left <= d_mid) {
                        return i - 1;
                    } else {
                        return i;
                    }
                } else {
                    // when d==0, it is:
                    // - base[i-1]<e -> because of that l was increased
                    // - base[i]! =e -> because of that the Equals condition didn't match
                    // - base[i+1]>e -> because of that r was decreased
                    // so we need to find out which one is the closest
                    Vector3 left = base[i - 1];
                    float d_left = Math.Abs(Minus(left, e));

                    Vector3 mid = base[i];
                    float d_mid = Math.Abs(Minus(mid, e));

                    Vector3 right = base[i + 1];
                    float d_right = Math.Abs(Minus(right, e));

                    if (d_left <= d_right && d_left <= d_mid) { //when they are equally distanced, return the left one
                        return i - 1;
                    } else if (d_mid < d_right && d_mid < d_left) {
                        return i;
                    } else /*if (d_right < d_mid && d_right < d_left)*/ {
                        return i + 1;
                    }
                }
            }
        }
    }

    private bool Equal(Vector3 a, Vector3 b) {
        switch (coordinateType) {
            case CoordinateType.x:
                return Util.AlmostEqual(a.x, b.x, 0.0001f);
            case CoordinateType.y:
                return Util.AlmostEqual(a.y, b.y, 0.0001f);
            case CoordinateType.z:
                return Util.AlmostEqual(a.z, b.z, 0.0001f);
            default:
                return false;
        }
    }

    private bool LessThan(Vector3 a, Vector3 b) {
        switch (coordinateType) {
            case CoordinateType.x:
                return a.x < b.x;
            case CoordinateType.y:
                return a.y < b.y;
            case CoordinateType.z:
                return a.z < b.z;
            default: //impossible but neccessary
                return false;
        }
    }

    private float Minus(Vector3 a, Vector3 b) {
        switch (coordinateType) {
            case CoordinateType.x:
                return a.x - b.x;
            case CoordinateType.y:
                return a.y - b.y;
            case CoordinateType.z:
                return a.z - b.z;
            default: //impossible but neccessary
                return 0;
        }
    }
}