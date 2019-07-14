using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(MeshFilter))]
public class TreeCreator : MonoBehaviour {
    private static void debug(string message, [CallerMemberName]string callerName = "") {
        if (true) {
            UnityEngine.Debug.Log("DEBUG: CylinderGenerator: " + callerName + "(): " + message);
        }
    }

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;
    //public Vector3[] vertices;
    //public Vector3[] normals;
    //public Vector2[] uvs;
    //public int[] triangles;
    bool meshReady;

    Renderer renderer_;

    PseudoEllipsoid attractionPoints;

    public AttractionPoints GetAttractionPoints() {
        return attractionPoints;
    }

    GrowthProperties growthProperties;
    Grower grower;
    GeometryProperties geometryProperties;
    Tree tree;

    // Start is called before the first frame update
    void Start() {


        //SortedList l = new SortedList();
        //for (int i=0; i<40000; i++) {
        //    int e = UnityEngine.Random.Range(0, 4000);
        //    l.InsertSorted(e);
        //}

        //int j = 32;
        //Debug.Log("Nearest to " + j + " is " + l.GetNearest(j));

        //Debug.Log("Count: " + l.Count);
        //foreach (int k in l) {
        //    Debug.Log(k);
        //}

        Initialize();

        //SetTexture("potentialOak_png_alpha");
        //SetTexture("texture");
        //SetTexture("brown_green");
        //SetTexture("orange_green");
        //SetTexture("dark_brown_green");
        SetTexture("dark_brown_light_blue");
        //SetTexture("dark_brown_red");

        //EnableTransparentTextures();

        //SetNormalMap("potentialOak_normal");




        //smallTree_hangingBranches(); //TODO
        //normalTree();
        //testTree();
        //bigTree();


        int initialAge = 30;
        float initialRadius_x = 3;
        float initialRadius_y = 5;
        float initialRadius_z = 3.5f;
        //float initialRadius_x = 4;
        //float initialRadius_y = 10;
        //float initialRadius_z = 4;

        GameObject.Find("Age Slider").GetComponent<Slider>().SetValueWithoutNotify(30);
        GameObject.Find("Width X Slider").GetComponent<Slider>().SetValueWithoutNotify(initialRadius_x);
        GameObject.Find("Width Y Slider").GetComponent<Slider>().SetValueWithoutNotify(initialRadius_y);
        GameObject.Find("Width Z Slider").GetComponent<Slider>().SetValueWithoutNotify(initialRadius_z);

        //load_normalTree_hangingBranches();
        load_normalTree(initialAge, initialRadius_x, initialRadius_y, initialRadius_z);

        tree.Grow();
    }

    void Initialize() {
        Application.targetFrameRate = 60;

        mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh; //also works, but definately use sharedMesh for reading in ObjExporter!
        GetComponent<MeshFilter>().sharedMesh = mesh;
        renderer_ = GetComponent<MeshRenderer>();
    }

    void SetTexture(string textureFileName) {
        Texture2D texture = Resources.Load(textureFileName) as Texture2D;
        renderer_.material.SetTexture("_MainTex", texture);
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

    void SetNormalMap(string normalMapFileName) {
        Texture2D normalMap = Resources.Load(normalMapFileName) as Texture2D;
        renderer_.material.EnableKeyword("_NORMALMAP");
        renderer_.material.SetTexture("_BumpMap", normalMap);
    }


    public void OnAge(int value) {
        //debug("received");
        grower.Stop();

        growthProperties.UpdateIterations(value);
    }

    public void OnCrownRadius_x(float value) {
        //debug("received");
        grower.Stop();

        ((PseudoEllipsoid)attractionPoints).UpdateRadius_x(value);
    }

    public void OnCrownRadius_y(float value) {
        //debug("received");
        grower.Stop();

        ((PseudoEllipsoid)attractionPoints).UpdateRadius_y(value);
    }

    public void OnCrownRadius_z(float value) {
        //debug("received");
        grower.Stop();

        ((PseudoEllipsoid)attractionPoints).UpdateRadius_z(value);
    }

    public void OnNewSeed() {
        //debug("received");
        grower.Stop();

        attractionPoints.NewSeed();
    }

    //TODO: specify location &| unique naming
    public void OnSave() {
        ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "tree_" + mesh.vertices.Length + "_" + mesh.triangles.Length + ".obj");
        //ObjExporter.MeshToFile(GetComponent<MeshFilter>().name, mesh, "tree_" + mesh.vertices.Length + "_" + mesh.triangles.Length + ".obj");
    }

    void OnApplicationQuit() {
        //UnityEngine.Debug.Log("Application ending after " + Time.time + " seconds");
        grower.Stop();
        ThreadManager.Join();
        //ThreadManager.Reset();
    }

    //void smallTree_hangingBranches() {
    //    Vector3 center = new Vector3(0, 5f, 0);
    //    float maxDistance = 5f;
    //    int amount = 8000;
    //    //int amount = 16000;
    //    List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

    //    growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(0.9f);
    //    growthProperties.SetPerceptionAngle(160f);
    //    growthProperties.SetClearDistance(0.8f);
    //    growthProperties.SetTropisms(new Vector3(-0.25f, 1f, 0));
    //    growthProperties.SetHangingBranchesEnabled(true);
    //    growthProperties.SetHangingBranchesFromAgeRatio(0.6f);
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);

    //    SpaceColonization grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    geometryProperties.SetNthRoot(1.6f);
    //    geometryProperties.SetCircleResolution(3);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetMinLeafSize(0.1f);
    //    geometryProperties.SetMaxLeafSize(0.3f);
    //    geometryProperties.SetLeafType(LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(2);
    //    geometryProperties.SetLeavesEnabled(true);

    //    tree = new Tree(position, grower, geometryProperties);

    //    tree.Grow(20);
    //} //kleinere ClearDistance macht Tropismen hinfällig, viele AttractionPoints auch
    //// -> vermutlich weil es einfach unten keine AttractionPoints mehr gibt

    void load_normalTree(int age, float radius_x, float radius_y, float radius_z) {
        //Vector3 center = new Vector3(0, 5f, 0);
        //float maxDistance = 5f;
        //int amount = 8000;
        ////int amount = 30000;
        //List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);
        //AttractionPoints attractionPoints = new AttractionPoints(Vector3.zero, 0, 5, 8000, 0.2f);

        //AttractionPoints attractionPoints = new AttractionPoints(new Vector3(0, 0, 0), 5, 8000, 0.15f);
        //AttractionPoints attractionPoints = new SphereCut(new Vector3(0, 0, 0), 5, 15, 0.2f);

        //also not working yet:
        //AttractionPoints attractionPoints = new SpheroidAttractionPoints(new Vector3(0, 0, 0), 5, 2, 20);

        //not working yet:
        //AttractionPoints attractionPoints = new EllipsoidAttractionPoints(new Vector3(0, 0, 0), 1.5f, 5, 1.5f, 15);

        //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 4f, 2f, 15, 0.2f);
        //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 4f, 2f, 15, 0.0f);

        //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 2f, 8f, 4f, 15, 0.3f);

        //AttractionPoints attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), 3f, 10f, 3f, 15, 0.15f, 0.1f);


        //influence distance und cleardistance müssen auf jeden Fall von density / Radien abhängen

        attractionPoints = new PseudoEllipsoid(new Vector3(0, 0, 0), radius_x, radius_y, radius_z, 15, 0.15f, 0.05f);


        growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(1f);
        //growthProperties.SetInfluenceDistance(1.1f);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.9f);
        //growthProperties.SetClearDistance(1f);
        //growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        growthProperties.SetTropisms(new Vector3(0, 0.5f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);
        growthProperties.SetIterations(age);
        //SET LISTENER
        attractionPoints.SetAttractionPointsListener(growthProperties);


        grower = new SpaceColonization(growthProperties);
        //SET LISTENER
        growthProperties.SetGrowthPropertiesListener(grower);


        Vector3 position = Vector3.zero;

        geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.3f);
        geometryProperties.SetCircleResolution(3);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetMinLeafSize(0.1f);
        geometryProperties.SetMaxLeafSize(0.5f);
        geometryProperties.SetLeafType(LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties, this);
        //SET LISTENER
        grower.SetGrowerListener(tree);
    }

    //void load_normalTree_hangingBranches(int age) {
    //    Vector3 center = new Vector3(0, 5f, 0);
    //    float maxDistance = 5f;
    //    int amount = 8000;
    //    //int amount = 30000;
    //    List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

    //    growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(0.9f);
    //    growthProperties.SetPerceptionAngle(160f);
    //    growthProperties.SetClearDistance(0.8f);
    //    growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
    //    growthProperties.SetHangingBranchesEnabled(true);
    //    growthProperties.SetHangingBranchesFromAgeRatio(0.6f);
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);
    //    growthProperties.SetIterations(age);

    //    Grower grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    geometryProperties.SetNthRoot(1.3f);
    //    geometryProperties.SetCircleResolution(3);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetMinLeafSize(0.1f);
    //    geometryProperties.SetMaxLeafSize(0.5f);
    //    geometryProperties.SetLeafType(LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(2);
    //    geometryProperties.SetLeavesEnabled(true);


    //    tree = new Tree(position, grower, geometryProperties);

    //    tree.Grow();
    //    //tree.Grow(30);
    //}

    //void testTree() {
    //    Vector3 center = new Vector3(0, 5f, 0);
    //    float maxDistance = 5f;
    //    int amount = 8000;
    //    //int amount = 30000;
    //    List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

    //    GrowthProperties growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(0.9f);
    //    //growthProperties.SetInfluenceDistance(1.5f);
    //    growthProperties.SetPerceptionAngle(160f);
    //    //growthProperties.SetPerceptionAngle(100f);
    //    growthProperties.SetClearDistance(0.8f);
    //    //growthProperties.SetClearDistance(0.5f);
    //    growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetAttractionPoints(attractionPoints);

    //    SpaceColonization grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    GeometryProperties geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    //geometryProperties.SetNthRoot(1.6f);
    //    geometryProperties.SetNthRoot(1.3f);
    //    geometryProperties.SetCircleResolution(3);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetMinLeafSize(0.1f);
    //    geometryProperties.SetMaxLeafSize(0.5f);
    //    geometryProperties.SetLeafType(LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(2);
    //    geometryProperties.SetLeavesEnabled(true);


    //    tree = new Tree(position, grower, geometryProperties);

    //    tree.Grow(50);
    //    //tree.Grow(30);
    //}


    //void highRes() {
    //    Vector3 center = new Vector3(0, 5f, 0);
    //    float maxDistance = 5f;
    //    int amount = 8000;
    //    //int amount = 30000;
    //    List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

    //    GrowthProperties growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(0.9f);
    //    //growthProperties.SetInfluenceDistance(1.5f);
    //    growthProperties.SetPerceptionAngle(160f);
    //    //growthProperties.SetPerceptionAngle(100f);
    //    growthProperties.SetClearDistance(0.8f);
    //    //growthProperties.SetClearDistance(0.5f);
    //    growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
    //    growthProperties.SetGrowthDistance(0.125f);
    //    growthProperties.SetAttractionPoints(attractionPoints);

    //    SpaceColonization grower = new SpaceColonization(growthProperties);


    //    Vector3 position = Vector3.zero;

    //    GeometryProperties geometryProperties = new GeometryProperties();
    //    geometryProperties.SetTipRadius(0.007f);
    //    //geometryProperties.SetNthRoot(1.6f);
    //    geometryProperties.SetNthRoot(1.3f);
    //    geometryProperties.SetCircleResolution(3);

    //    geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    geometryProperties.SetMinLeafSize(0.1f);
    //    geometryProperties.SetMaxLeafSize(0.65f);
    //    geometryProperties.SetLeafType(LeafType.Triangle);
    //    geometryProperties.SetLeavesPerNode(2);
    //    geometryProperties.SetLeavesEnabled(true);


    //    tree = new Tree(position, grower, geometryProperties);

    //    tree.Grow(100);
    //    //tree.Grow(30);
    //}


    //void bigTree() {
    //    Vector3 center = new Vector3(0, 10f, 0);
    //    float maxDistance = 10f;
    //    int amount = 50000;
    //    HashSet<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

    //    GrowthProperties growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(0.9f);
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetClearDistance(0.8f);
    //    growthProperties.SetTropisms(new Vector3(0, 1, 0));

    //    growthProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    growthProperties.SetMinLeafSize(0.1f);
    //    growthProperties.SetMaxLeafSize(0.5f);
    //    growthProperties.SetLeavesPerNode(2);

    //    growthProperties.SetMaxBranchingAngle(80);
    //    SpaceColonization grower = new SpaceColonization(attractionPoints, growthProperties);
    //    //SimpleSpaceColonization grower = new SimpleSpaceColonization(attractionPoints, growthProperties);


    //    Vector3 position = Vector3.zero;
    //    tree = new Tree(position, grower, growthProperties);


    //    cylinderResolution = 7;

    //    tree.Grow(65, cylinderResolution);
    //}


    // Update is called once per frame
    void Update() {
        //tree.GetEverything(ref vertices, ref normals, ref uvs, ref triangles);

        if (meshReady) {
            UpdateMesh();
            meshReady = false;
        }

    }

    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawSphere(Vector3.up, 3);
    //}

    public void OnUpdateMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles) {
        lock (mesh) {
            this.vertices = vertices;
            this.normals = normals;
            this.uvs = uvs;
            this.triangles = triangles;

            meshReady = true;
        }
    }

    void UpdateMesh() {
        lock (mesh) {
            mesh.Clear();

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.normals = normals;
        }
    }

    /* NOTIZEN
     * nächste Schritte:
     * - Blätter dreieckig machen / austauschbar machen
     * >>!! Anzahl der Leaves im Nachhinein änderbar machen
     * - Texturen -> Farben
     * 
     * - UI
     * - - Alter
     * >> Settings in zentralem Objekt speichern
     * - - Speichern
     * - - Randomize
     * - Vertices und Triangles anzeigen
     *
     * - automatisches Camera-Movement
     * 
     * >> EventSystem für Änderungen einzelner Werte
     * 
     * 
     * - Punktewolkengenerator
     * - - was überlegen für die Form der Basis-Baumkrone
     * - - in die Höhe ziehen
     * - - in die Breite ziehen
     * - - Punkte überwiegend am Rand
     * - - Punkte während des Wachstums hinzufügen
     * - - density, influenceDistance und ClearDistance davon abhängig machen
     *
     * 
     * - bessere Geometrie
     * - - dünne Äste an dicken Ästen fixen
     * - - Verdrehung fixen
     * - Konifären mittels Apical Control?
     * - hängende Äste
     * - - über Änderung des PerceptionVolumes -> Achse in Abhängigkeit der Noderichtung
     * - - über neue AttractionPoints, Persistenz durch Seed?
     *
     * - 
     * 
     *
     * - middleware für Buttons?
     *
     * - nächsten Punkt finden über Octree oder Sortierung und dann Suchen
     * 
     *
     * - Blätter als Tetraeder oder ein paar senkrecht zueinander stehende Bilder?
     * - Blätter besser verteilen an Nodes?
     * - mehrere Farben auswählbar machen für Blätter
     * - Farbenverläufe
     *
     *
     * - neuer Seed <-> Würfeln
     * - Smooth-Slider Age
     *
     *
     * 
     * - Verformung der Krone zur Laufzeit bei schon schöner Form
     * -> die alten Punkte sollten möglichst behalten werden, vermutlich nicht so einfach
     *
     * - genau so: Änderungen der cutoffRatio zur Laufzeit?
     *
     * - Sinnhaftigkeit des density Parameters überdenken
     *
     * 
     * Parameter die definitiv während des Wachstums gebraucht werden
     * - position vom jeweils vorherigen Node
     * - growthDistance
     * - minBranchinAngle
     * - maxBranchingAngle
     * - 
     * - normal?
     * - stemRadius, stemRadiusRatio?
     * > Was, wenn position vom obersten Node geändert wird?
     *
     * Parameter die nur für die Berechnung der Dreiecke gebraucht werden
     * - cylinderResolution
     * - curveResolution
     *
     * 
     *
     */

    /* TODO
     * Distanzberechnungen:
     * - nur der nächste Node wird associated zum Attraction Point
     * - Nodes die nicht mehr associated werden, können "deaktiviert" werden
     * - Eingrenzung der Distanzberechnungen, in dem geschaut wird ob die Differenz z.B. der x-Koordinate schon zu groß ist (https://www.codeproject.com/articles/882739/simple-approach-to-voronoi-diagrams)
     * 
     * 
     * Stammtextur hat Spalt
     * Stammdicke interpolieren für flüssigere Übergänge bei kleinen Werten für nth_root
     * 
     * Stamm
     * 
     * Curve-Resolution
     *
     * Distanzberechnungen parallelisieren VS Berechnungen für Subnodes parallelisieren
     *
     * sinnvolle Werte finden
     * - tipRadius
     * - nth_root
     * 
     * Occupancy Zone (Node::minDistanceToExistingNodes) muss eigentlich über alle existierenden Nodes gerechnet werden
     * (und nicht nur über die direkten subnodes)
     * 
     *
     * AttractionPoints: HashSet -> List?
     * 
     * Algorithmus für Dreiecke an Gabelungen überlegen, maximale Anzahl an Gabelungen zulassen? (auch in Anlehnung an Realität)
     *
     *
     * Vector3 newNodePosition = new Vector3(1000, 1000, 1000);
     * -> sinnvollen Default-Wert finden?
     * 
     * abbrechen, wenn die neuen Nodes immer wieder entfernt werden
     * 
     */

    /* GELÖST
     *
     * zu viele Dreiecke werden buggy -> max 2^16 -1 Dreiecke erlaubt...
     *
     * uvs out of bounds fixen
     *
     *!! über Thread.Abort nachdenken, in Besitz genommene Locks werden nicht freigegeben!
     */

    /* BEACHTEN
     * Resolution muss fuer alle Zylinder gleich sein
     * 
     * 
     */

    /* OPTIMIEREN
     * Kreiskoordinaten direkt in Vertex-Array schreiben
     * vertices in Node werden immer erst bei Abruf berechnet, Übergabe des resolution-Parameters problematisch...
     *
     *
     * ansonsten: OPTIMIERBAR
     */

}

public class SortedList : List<int> {
    public void InsertSorted(int e) {
        if (base.Count == 0) {
            base.Add(e);
        } else {
            //InsertSorted(e, 0, base.Count - 1);
            for (int i = 0; i < base.Count; i++) {
                if (e <= base[i]) {
                    base.Insert(i, e);
                    break;
                }

                if (i == base.Count - 1 && e > base[i]) {
                    base.Add(e);
                    break;
                }
            }
        }

        //base.Add(e);
        //base.Sort();
    }

    public int GetNearest(int e) {
        if (base.Count == 0) {
            throw new System.Exception("List is empty...");
        } else if (base.Count==1) {
            return base[0];
        } else {
            return GetNearestH(e);
        }
    }

    private int GetNearestH(int e) {
        int steps = 0;

        int l = 0;
        int r = base.Count - 1;

        int d = r - l;
        int i = 0;

        while (d>0) {
            steps++;

            d = r - l;
            i = l + d / 2;

            int number = base[i];
            if (Equals(e,number)) {
                Debug.Log(steps + " steps");
                return base[i];
            }
            if (LessThan(e,number)) {
                r = i - 1;
            }
            //if (GreaterThan(e, number)) {
                l = i + 1;
            //}
        }

        Debug.Log(steps + " steps");

        return CheckNeighbours(e, i);
    }

    private bool Equals(int a, int b) {
        return a == b;
    }

    private bool LessThan(int a, int b) {
        return a < b;
    }

    //private bool GreaterThan(int a, int b) {
    //    return a > b;
    //}

    private int CheckNeighbours(int e, int i) {
        int d_left = Math.Abs(base[i - 1] - e);
        int d_index = Math.Abs(base[i] - e);
        int d_right = Math.Abs(base[i + 1] - e);

        int min = int.MaxValue;
        if (d_left < min) {
            min = d_left;
        }
        if (d_index < min) {
            min = d_index;
        }
        if (d_right < min) {
            min = d_right;
        }

        if (d_left == min) {
            return base[i - 1];
        }
        if (d_index == min) {
            return base[i];
        }
        //if (d_right == min) {
            return base[i+1];
        //}
    }

    private void InsertSorted(int e, int l, int r) {
        int d = r - l;
        int i = l + d / 2;

        if (d == 0) {
            base.Insert(i, e);
        } else {
            int number = base[i];
            if (e == number) {
                base.Insert(i, e);
            } else if (e < number) {
                if (i == 0) {
                    base.Insert(i, e);
                } else {
                    r = i - 1;
                    InsertSorted(e, l, r);
                }
            } else if (e > number) {
                if (i == base.Count - 1) {
                    base.Insert(i, e);
                } else {
                    l = i + 1;
                    InsertSorted(e, l, r);
                }
            }
        }
    }
}