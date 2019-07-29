using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PointCloudRenderer : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    int[] indizes;

    public bool Enabled { get; set; } = false;

    // Start is called before the first frame update
    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update() {

        //always get the latest point cloud
        if (GameObject.Find("Core").GetComponent<Core>().PointCloudReady()) {
            List<Vector3> pointCloud = GameObject.Find("Core").GetComponent<Core>().GetPointCloud();

            vertices = new Vector3[pointCloud.Count]; 
            indizes = new int[pointCloud.Count];
            for (int i = 0; i < vertices.Length; i++) {
                vertices[i] = pointCloud[i];
                indizes[i] = i;
            }

            //Threadsafety is given, because the backup object never gets changed
        }

        //and display it, if Enabled
        mesh.Clear();

        if (Enabled) {
            mesh.vertices = vertices;
            mesh.SetIndices(indizes, MeshTopology.Points, 0);
        } else {
            mesh.vertices = null;
        }

        //if (attractionPoints != null) {
        //    vertices = new Vector3[attractionPoints.GetBackup().Count]; //TODO: THIS IS NOT THREADSAFE
        //    indizes = new int[attractionPoints.GetBackup().Count];
        //                                                    //attractionPoints.CopyTo(vertices);
        //    for (int i = 0; i < vertices.Length; i++) {
        //        vertices[i] = attractionPoints.GetBackup()[i];
        //        indizes[i] = i;
        //    }

        //    mesh.Clear();

        //    if (Enabled) {
        //        mesh.vertices = vertices;
        //        mesh.SetIndices(indizes, MeshTopology.Points, 0);
        //    }
        //}
    }
}