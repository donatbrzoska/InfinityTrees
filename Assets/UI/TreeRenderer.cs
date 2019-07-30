using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TreeRenderer : MonoBehaviour {

    Core core;


    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;

    Renderer renderer_;

    // Start is called before the first frame update
    void Start() {
        Initialize();
        EnableTransparentTextures();
        //SetTexture("potentialOak_png_alpha");
        //SetTexture("texture");
        //SetTexture("brown_green");
        //SetTexture("orange_green");
        //SetTexture("dark_brown_green");
        //SetTexture("dark_brown_light_blue");
        //SetTexture("dark_brown_green_particles");
        //SetTexture("particle/dark_brown_green");
        //SetTexture("particle/dark_brown_red_tiny");
        SetTexture("particle/dark_brown_red_small");
        //SetTexture("particle/dark_brown_red_medium");
        //SetTexture("particle/dark_brown_red_big");
        //SetTexture("particle/dark_brown_red_huge");
        //SetTexture("dark_brown_red");
    }

    void Initialize() {
        Application.targetFrameRate = 60;

        core = GameObject.Find("Core").GetComponent<Core>();

        mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh; //also works, but definately use sharedMesh for reading in ObjExporter!
        GetComponent<MeshFilter>().sharedMesh = mesh;

        renderer_ = GetComponent<MeshRenderer>();
    }

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

    void SetTexture(string textureFileName) {
        Texture2D texture = Resources.Load(textureFileName) as Texture2D;
        renderer_.material.SetTexture("_MainTex", texture);
    }

    // Update is called once per frame
    void Update() {
        if (core.MeshReady()) {
            core.GetMesh(ref vertices, ref normals, ref uvs, ref triangles);

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;
        }

        SetTexture(core.GetTexture());
    }
}



//void SetNormalMap(string normalMapFileName) {
//    Texture2D normalMap = Resources.Load(normalMapFileName) as Texture2D;
//    renderer_.material.EnableKeyword("_NORMALMAP");
//    renderer_.material.SetTexture("_BumpMap", normalMap);
//}