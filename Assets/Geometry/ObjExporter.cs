using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
//http://wiki.unity3d.com/index.php/ObjExporter
public static class ObjExporter {

    //Calling mf.mesh causes the original mesh (that has been created in the TreeCreator Class) to get unaccessable, because a copy is made and only this copy will be returned then.
    //This is a problem, because any changes made afterwards won't affect the queryable mesh (after calling mf.mesh, mf.sharedMesh does the same as mf.mesh)
    //Therefore the actual mesh of the MeshFilter cannot be changed anymore, which we don't want
    public static string MeshToString(MeshFilter mf) {
        Mesh m = mf.sharedMesh;
        //Mesh m = mf.mesh;
        //Material[] mats = mf.renderer.sharedMaterials;

        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mf.name).Append("\n");
        foreach (Vector3 v in m.vertices) {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in m.normals) {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in m.uv) {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < m.subMeshCount; material++) {
            sb.Append("\n");
            //sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            //sb.Append("usemap ").Append(mats[material].name).Append("\n");

            int[] triangles = m.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3) {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString();
    }

    public static void MeshToFile(MeshFilter mf, string filename) {
        using (StreamWriter sw = new StreamWriter(filename)) {
            sw.Write(MeshToString(mf));
        }
        Debug.Log("Saved mesh to " + filename);
    }




    public static string MeshToString(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles) {
        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append("TreeMesh").Append("\n");
        foreach (Vector3 v in vertices) {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in normals) {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in uvs) {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int i = 0; i < triangles.Length; i += 3) {
            sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
        }
        return sb.ToString();
    }

    public static void MeshToFile(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles, string filename) {
        using (StreamWriter sw = new StreamWriter(filename)) {
            sw.Write(MeshToString(vertices, normals, uvs, triangles));
        }
    }





    public static string MeshToString(List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, List<int> triangles) {
        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append("TreeMesh").Append("\n");
        foreach (Vector3 v in vertices) {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in normals) {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in uvs) {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int i = 0; i < triangles.Count; i += 3) {
            sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
        }
        return sb.ToString();
    }

    public static void MeshToFile(List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, List<int> triangles, string filename) {
        using (StreamWriter sw = new StreamWriter(filename)) {
            sw.Write(MeshToString(vertices, normals, uvs, triangles));
        }
    }
}