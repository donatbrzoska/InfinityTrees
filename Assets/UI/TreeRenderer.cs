using System;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TreeRenderer : MonoBehaviour {

    Core core;


    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;

    public Texture2D texture;
    Renderer renderer_;


    // Start is called before the first frame update
    void Start() {
        Initialize();
        EnableTransparentTextures();
    }

    void Initialize() {
        Application.targetFrameRate = 60;

        core = GameObject.Find("Core").GetComponent<Core>();

        mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh; //also works, but definately use sharedMesh for reading in ObjExporter!
        GetComponent<MeshFilter>().sharedMesh = mesh;

        renderer_ = GetComponent<MeshRenderer>();
    }

    //https://docs.unity3d.com/500/Documentation/ScriptReference/ShaderVariantCollection.html
    //Important for this to work:
    //1. Run in Unity until it works
    //2. Unity -> Edit -> Project Settings ... -> Graphics -> Save to asset...
    //3. Unity -> Edit -> Project Settings ... -> Graphics -> Preloaded Shaders: Size := 1; Element 0 := saved_shader_variant_collection
    void EnableTransparentTextures() {
        //https://answers.unity.com/questions/1004666/change-material-rendering-mode-in-runtime.html
        //for cutting out empty background of png
        renderer_.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderer_.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        renderer_.material.SetInt("_ZWrite", 1);
        renderer_.material.EnableKeyword("_ALPHATEST_ON");
        renderer_.material.DisableKeyword("_ALPHABLEND_ON");
        renderer_.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        renderer_.material.renderQueue = 2450;
        //also see https://answers.unity.com/questions/1016155/standard-material-shader-ignoring-setfloat-propert.html
    }

    void SetTexture(string textureFilename) {
        texture = Resources.Load(textureFilename) as Texture2D;

        ////https://forum.unity.com/threads/possible-to-import-custom-user-textures-from-file-system-at-runtime.265862/
        //string texture_path = "/Users/donatdeva/Documents/Studium/6. Semester/Bachelorarbeit/Unity/AnimationTrees/Assets/Resources/" + textureFilename + ".png";
        //byte[] byteFile = File.ReadAllBytes(texture_path);
        //texture.LoadImage(byteFile);

        ////Tex = new Texture2D(256, 256);
        ////Tex.LoadImage(byteFile);

        renderer_.material.SetTexture("_MainTex", texture);
    }

    // Update is called once per frame
    void Update() {
        SetTexture(core.GetTexture());

        if (core.MeshReady()) {
            core.GetMesh(ref vertices, ref normals, ref uvs, ref triangles);

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        }
    }
}



//void SetNormalMap(string normalMapFileName) {
//    Texture2D normalMap = Resources.Load(normalMapFileName) as Texture2D;
//    renderer_.material.EnableKeyword("_NORMALMAP");
//    renderer_.material.SetTexture("_BumpMap", normalMap);
//}