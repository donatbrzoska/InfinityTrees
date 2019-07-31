using System;
using UnityEngine;

//doing this via a listener makes sense, because this way you have one snippet of code (content of OnMeshReady())
// where the locking takes place, instead of multiple ones, at each place where we need to recalculate the data
public interface MeshListener {
    void OnMeshReady(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles);
}
