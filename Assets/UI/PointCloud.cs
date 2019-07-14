using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PointCloud : MonoBehaviour
{
    AttractionPoints attractionPoints;

    Mesh mesh;

    public Vector3[] vertices;
    public int[] indizes;

    public bool Enabled { get; set; } = false;

    // Start is called before the first frame update
    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update() {
        attractionPoints = GameObject.Find("TreeMesh").GetComponent<TreeCreator>().GetAttractionPoints();

        if (attractionPoints != null) {
            vertices = new Vector3[attractionPoints.GetBackup().Count]; //TODO: THIS IS NOT THREADSAFE
            indizes = new int[attractionPoints.GetBackup().Count];
                                                            //attractionPoints.CopyTo(vertices);
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = attractionPoints.GetBackup()[i];
                indizes[i] = i;
            }

            mesh.Clear();

            if (Enabled) {
                mesh.vertices = vertices;
                mesh.SetIndices(indizes, MeshTopology.Points, 0);
            }
        }
    }
}
