using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PointCloudRenderer : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;
    float triangleSize;

    // Start is called before the first frame update
    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;

        triangleSize = 0.2f;

        //Texture2D texture = Resources.Load("point_cloud_color_red") as Texture2D;
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.red);
        texture.Apply();
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
    }

    // Update is called once per frame
    void Update() {

        //always get the latest point cloud
        if (GameObject.Find("Core").GetComponent<Core>().PointCloudReady()) {
            Vector3[] pointCloud = GameObject.Find("Core").GetComponent<Core>().GetPointCloud();

            int vertexPointer = 0;

            vertices = new Vector3[pointCloud.Length * 3]; //3 vertices for every point in the cloud to make up one triangle respectively
            uvs = new Vector2[pointCloud.Length * 3];
            triangles = new int[pointCloud.Length * 3]; //1 triangle for every point in the cloud

            for (int i = 0; i < pointCloud.Length; i++) {

                Quaternion rotation = Quaternion.AngleAxis(Util.RandomInRange(0, 360), Util.RandomVector3());

                Vector3 position = pointCloud[i];
                vertices[vertexPointer++] = rotation * new Vector3(-0.5f, 0, 0) * triangleSize + position;
                vertices[vertexPointer++] = rotation * new Vector3(0, 0, 0.866f) * triangleSize + position;
                vertices[vertexPointer++] = rotation * new Vector3(0.5f, 0, 0) * triangleSize + position;

                triangles[vertexPointer - 3] = vertexPointer - 3;
                triangles[vertexPointer - 2] = vertexPointer - 2;
                triangles[vertexPointer - 1] = vertexPointer - 1;

                uvs[vertexPointer - 3] = new Vector2(0, 0);
                uvs[vertexPointer - 2] = new Vector2(0, 0);
                uvs[vertexPointer - 1] = new Vector2(0, 0);
            }
        }

        //and display it, if Enabled
        mesh.Clear();

        if (GameObject.Find("Core").GetComponent<Core>().PointCloudRenderingEnabled) {
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
        } else {
            mesh.vertices = null;
        }
    }
}