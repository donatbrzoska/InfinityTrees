using UnityEngine;

public interface MeshListener {
    void OnMeshReady(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles);
}
