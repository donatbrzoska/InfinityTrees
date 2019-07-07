using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafCreator : MonoBehaviour
{


    Mesh mesh;

    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;


    // Start is called before the first frame update
    void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        Renderer renderer_ = GetComponent<MeshRenderer>();


        Texture2D texture = Resources.Load("texture") as Texture2D;
        renderer_.material.SetTexture("_MainTex", texture);


        //https://answers.unity.com/questions/1004666/change-material-rendering-mode-in-runtime.html
        //for cutting out empty background of png
        renderer_.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderer_.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        renderer_.material.SetInt("_ZWrite", 1);
        renderer_.material.EnableKeyword("_ALPHATEST_ON");
        renderer_.material.DisableKeyword("_ALPHABLEND_ON");
        renderer_.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer_.material.renderQueue = 2450;


        List<Vector3> verticesTmp = new List<Vector3>();
        List<Vector3> normalsTmp = new List<Vector3>();
        List<Vector2> uvsTmp = new List<Vector2>();
        List<int> trianglesTmp = new List<int>();

        Vector3 position = new Vector3(0, 0, 0);
        Vector3 direction = new Vector3(-1, 0, 1);
        float size = 1.4f;

        GenerateLeaf(position, direction, size, verticesTmp, normalsTmp, uvsTmp, trianglesTmp);

        //vertices = new Vector3[verticesTmp.Count];
        //normals = new Vector3[normalsTmp.Count];
        //uvs = new Vector2[uvsTmp.Count];
        //triangles = new int[trianglesTmp.Count];

        vertices = verticesTmp.ToArray();
        normals = normalsTmp.ToArray();
        uvs = uvsTmp.ToArray();
        triangles = trianglesTmp.ToArray();

    }

    void GenerateLeaf(Vector3 position, Vector3 direction, float size, List<Vector3> verticesResult, List<Vector3> normalsResult, List<Vector2> uvsResult, List<int> trianglesResult) {
        Vector3 directionAxis = Vector3.Cross(new Vector3(0, 0, 1), direction);
        float directionAngle = Vector3.Angle(new Vector3(0, 0, 1), direction);
        Quaternion directionRotation = Quaternion.AngleAxis(directionAngle, directionAxis);

        verticesResult.Add(directionRotation * new Vector3(-0.5f, 0, 0) * size + position);
        verticesResult.Add(directionRotation * new Vector3(-0.5f, 0, 1) * size + position);
        verticesResult.Add(directionRotation * new Vector3( 0.5f, 0, 1) * size + position);
        verticesResult.Add(directionRotation * new Vector3( 0.5f, 0, 0) * size + position);

        for (int i = 0; i < 4; i++) {
            normalsResult.Add(Vector3.up); //TODO
        }

        uvsResult.Add(new Vector2(0.5f, 0));
        uvsResult.Add(new Vector2(0.5f, 1));
        uvsResult.Add(new Vector2(1, 1));
        uvsResult.Add(new Vector2(1, 0));

        trianglesResult.Add(0);
        trianglesResult.Add(1);
        trianglesResult.Add(2);
        trianglesResult.Add(0);
        trianglesResult.Add(2);
        trianglesResult.Add(3);
        trianglesResult.Add(0);
        trianglesResult.Add(2);
        trianglesResult.Add(1);
        trianglesResult.Add(0);
        trianglesResult.Add(3);
        trianglesResult.Add(2);
    }

    // Update is called once per frame
    void Update()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;
    }

    //private System.Drawing.Bitmap MergeImages(IEnumerable<Bitmap> images) {
    //    var enumerable = images as IList<Bitmap> ?? images.ToList();

    //    var width = 0;
    //    var height = 0;

    //    foreach (var image in enumerable) {
    //        width += image.Width;
    //        height = image.Height > height
    //            ? image.Height
    //            : height;
    //    }

    //    var bitmap = new Bitmap(width, height);
    //    using (var g = Graphics.FromImage(bitmap)) {
    //        var localWidth = 0;
    //        foreach (var image in enumerable) {
    //            g.DrawImage(image, localWidth, 0);
    //            localWidth += image.Width;
    //        }
    //    }
    //    return bitmap;
    //}

    //public void Image() {
    //    var bitmap = GetBitmap(); // The method that returns List<Bitmap>
    //    var width = 0;
    //    var height = 0;
    //    foreach (var image in bitmap) {
    //        width += image.Width;
    //        height = image.Height > height
    //            ? image.Height
    //            : height;
    //    }
    //    var bitmap2 = new Bitmap(width, height);
    //    var g = Graphics.FromImage(bitmap2);
    //    var localWidth = 0;
    //    foreach (var image in bitmap) {
    //        g.DrawImage(image, localWidth, 0);
    //        localWidth += image.Width;
    //    }

    //    var ms = new MemoryStream();

    //    bitmap2.Save(ms, ImageFormat.Png);
    //    var result = ms.ToArray();
    //    //string base64String = Convert.ToBase64String(result); 
    //    //return File(result, "image/jpeg"); //Return as file result
    //    //return base64String;
    //}

    ////this method returns List<Bitmap>
    //public List<Bitmap> GetBitmap() {
    //    var lstbitmap = new List<Bitmap>();
    //    var bitmap = new Bitmap(@"E:\My project\ProjectImage\ProjectImage\BmImage\1525244892128.JPEG");
    //    var bitmap2 = new Bitmap(@"E:\My project\ProjectImage\ProjectImage\BmImage\1525244892204.JPEG");
    //    var bitmap3 = new Bitmap(@"E:\My project\ProjectImage\ProjectImage\BmImage\3.jpg");
    //    lstbitmap.Add(bitmap);
    //    lstbitmap.Add(bitmap2);
    //    lstbitmap.Add(bitmap3);
    //    return lstbitmap;
    //}
}

public class Leaf {
    Vector3 position;
    Vector3 normal;
    Vector3 direction;

    float size;

    public Leaf(Vector3 position, Vector3 normal, Vector3 direction, float size) {
        this.position = position;
        this.normal = normal;
        this.direction = direction;
        this.size = size;
    }

    public void GetEverything(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uvs, ref int[] triangles) {
        //Vector3 normalAxis = Vector3.Cross(Vector3.up, normal);
        //float normalAngle = Vector3.Angle(Vector3.up, normal);
        //Quaternion normalRotation = Quaternion.AngleAxis(normalAngle, normalAxis);

        Vector3 directionAxis = Vector3.Cross(new Vector3(0, 0, 1), direction);
        float directionAngle = Vector3.Angle(new Vector3(0, 0, 1), direction);
        Quaternion directionRotation = Quaternion.AngleAxis(directionAngle, directionAxis);

        vertices = new Vector3[4];
        vertices[0] = directionRotation * new Vector3(-0.5f, 0, 0)*size + position;
        vertices[1] = directionRotation * new Vector3(-0.5f, 0, 1)*size + position;
        vertices[2] = directionRotation * new Vector3(0.5f, 0, 1)*size + position;
        vertices[3] = directionRotation * new Vector3(0.5f, 0, 0)*size + position;

        normals = new Vector3[4];
        for (int i=0; i<normals.Length; i++) {
            normals[i] = normal;
        }

        uvs = new Vector2[4];
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(0, 1);
        uvs[2] = new Vector2(1, 1);
        uvs[3] = new Vector2(1, 0);

        int n_triangles = 4;
        triangles = new int[n_triangles*3];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;
        triangles[6] = 0;
        triangles[7] = 2;
        triangles[8] = 1;
        triangles[9] = 0;
        triangles[10] = 3;
        triangles[11] = 2;
    }
}