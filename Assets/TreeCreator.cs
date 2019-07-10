using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
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

    Texture2D texture;
    Texture2D normalMap;
    Renderer renderer_;

    Tree tree;

    // Start is called before the first frame update
    void Start() {
        Application.targetFrameRate = 60;

        mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh; //also works, but definately use sharedMesh for reading in ObjExporter!
        GetComponent<MeshFilter>().sharedMesh = mesh;
        renderer_ = GetComponent<MeshRenderer>();

        //texture = Resources.Load("potentialOak_png_alpha") as Texture2D;
        //texture = Resources.Load("texture") as Texture2D;
        //texture = Resources.Load("brown_green") as Texture2D;
        //texture = Resources.Load("orange_green") as Texture2D;
        //texture = Resources.Load("dark_brown_green") as Texture2D;
        //texture = Resources.Load("dark_brown_light_blue") as Texture2D;
        texture = Resources.Load("dark_brown_red") as Texture2D;
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
        //also see https://answers.unity.com/questions/1016155/standard-material-shader-ignoring-setfloat-propert.html


        //Normalmapping
        //normalMap = Resources.Load("potentialOak_normal") as Texture2D;
        //renderer_.material.EnableKeyword("_NORMALMAP");
        //renderer_.material.SetTexture("_BumpMap", normalMap);


        //squeezedTree();
        //smallTree_hangingBranches(); //TODO
        //normalTree();
        normalTree_hangingBranches();
        //testTree();
        //hollowNormalTree();
        //demoTree();
        //bigTree();

        //triangleTree();

    }

    public void OnRandomize() {
        normalTree_hangingBranches();
        //tree.Regrow();
    }

    public void OnSave() {
        ObjExporter.MeshToFile(GetComponent<MeshFilter>(), "tree_" + mesh.vertices.Length + "_" + mesh.triangles.Length + ".obj");
        //ObjExporter.MeshToFile(GetComponent<MeshFilter>().name, mesh, "tree_" + mesh.vertices.Length + "_" + mesh.triangles.Length + ".obj");
    }

    //void triangleTree() {
    //    Vector3 center = new Vector3(0, 2.5f, 0);
    //    float maxDistance = 2.5f;
    //    int amount = 2000;
    //    HashSet<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

    //    GrowthProperties growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(0.8f);
    //    growthProperties.SetGrowthDistance(0.15f);
    //    growthProperties.SetClearDistance(0.4f);
    //    growthProperties.SetTropisms(new Vector3(0, 0, 0));

    //    growthProperties.SetMaxBranchingAngle(80);

    //    SpaceColonization grower = new SpaceColonization(attractionPoints, growthProperties);


    //    Vector3 position = Vector3.zero;
    //    tree = new Tree(position, grower, growthProperties);

    //    cylinderResolution = 3;

    //    tree.Grow(30, cylinderResolution);
    //}

    //void squeezedTree() {
    //    Vector3 center = new Vector3(0, 5f, 0);
    //    float sideMaxDistance = 1.5f;
    //    //float sideMaxDistance = 2f;
    //    float upperMaxDistance = 5f;
    //    int amount = 2500;
    //    HashSet<Vector3> attractionPoints = GenerateAttractionPoints(center, sideMaxDistance, upperMaxDistance, amount);


    //    GrowthProperties growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(0.5f);
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetClearDistance(0.4f);
    //    growthProperties.SetTropisms(new Vector3(0, 1, 0));

    //    growthProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    growthProperties.SetMinLeafSize(0.1f);
    //    growthProperties.SetMaxLeafSize(0.5f);
    //    growthProperties.SetLeavesPerNode(3);

    //    growthProperties.SetMaxBranchingAngle(80);
    //    SpaceColonization grower = new SpaceColonization(attractionPoints, growthProperties);
    //    //SpaceColonizationSimple grower = new SpaceColonizationSimple(attractionPoints, growthProperties);


    //    Vector3 position = Vector3.zero;
    //    tree = new Tree(position, grower, growthProperties);

    //    cylinderResolution = 7;

    //    tree.Grow(40, cylinderResolution);
    //}

    void smallTree_hangingBranches() {
        Vector3 center = new Vector3(0, 5f, 0);
        float maxDistance = 5f;
        int amount = 8000;
        //int amount = 16000;
        List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(0.9f);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.8f);
        growthProperties.SetTropisms(new Vector3(-0.25f, 1f, 0));
        growthProperties.SetHangingBranchesEnabled(true);
        growthProperties.SetHangingBranchesFromAgeRatio(0.6f);
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);

        SpaceColonization grower = new SpaceColonization(growthProperties);


        Vector3 position = Vector3.zero;

        GeometryProperties geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.6f);
        geometryProperties.SetCircleResolution(3);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetMinLeafSize(0.1f);
        geometryProperties.SetMaxLeafSize(0.3f);
        geometryProperties.SetLeafType(LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);

        tree = new Tree(position, grower, geometryProperties);

        tree.Grow(20);
    } //kleinere ClearDistance macht Tropismen hinfällig, viele AttractionPoints auch
    // -> vermutlich weil es einfach unten keine AttractionPoints mehr gibt

    void normalTree() {
        Vector3 center = new Vector3(0, 5f, 0);
        float maxDistance = 5f;
        int amount = 8000;
        //int amount = 16000;
        List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(0.9f);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.8f);
        growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);

        SpaceColonization grower = new SpaceColonization(growthProperties);


        Vector3 position = Vector3.zero;

        GeometryProperties geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.6f);
        geometryProperties.SetCircleResolution(3);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetMinLeafSize(0.1f);
        geometryProperties.SetMaxLeafSize(0.3f);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(false);
        geometryProperties.SetLeafType(LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);

        tree = new Tree(position, grower, geometryProperties);

        tree.Grow(40);
    }

    void normalTree_hangingBranches() {
        Vector3 center = new Vector3(0, 5f, 0);
        float maxDistance = 5f;
        int amount = 8000;
        //int amount = 30000;
        List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(0.9f);
        growthProperties.SetPerceptionAngle(160f);
        growthProperties.SetClearDistance(0.8f);
        growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        growthProperties.SetHangingBranchesEnabled(true);
        growthProperties.SetHangingBranchesFromAgeRatio(0.6f);
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);

        SpaceColonization grower = new SpaceColonization(growthProperties);


        Vector3 position = Vector3.zero;

        GeometryProperties geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        geometryProperties.SetNthRoot(1.3f);
        geometryProperties.SetCircleResolution(3);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetMinLeafSize(0.1f);
        geometryProperties.SetMaxLeafSize(0.5f);
        geometryProperties.SetLeafType(LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties);

        tree.Grow(40);
        //tree.Grow(30);
    }

    void testTree() {
        Vector3 center = new Vector3(0, 5f, 0);
        float maxDistance = 5f;
        int amount = 8000;
        //int amount = 30000;
        List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(0.9f);
        //growthProperties.SetInfluenceDistance(1.5f);
        growthProperties.SetPerceptionAngle(160f);
        //growthProperties.SetPerceptionAngle(100f);
        growthProperties.SetClearDistance(0.8f);
        //growthProperties.SetClearDistance(0.5f);
        growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        growthProperties.SetGrowthDistance(0.25f);
        growthProperties.SetAttractionPoints(attractionPoints);

        SpaceColonization grower = new SpaceColonization(growthProperties);


        Vector3 position = Vector3.zero;

        GeometryProperties geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        //geometryProperties.SetNthRoot(1.6f);
        geometryProperties.SetNthRoot(1.3f);
        geometryProperties.SetCircleResolution(3);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetMinLeafSize(0.1f);
        geometryProperties.SetMaxLeafSize(0.5f);
        geometryProperties.SetLeafType(LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties);

        tree.Grow(50);
        //tree.Grow(30);
    }


    void highRes() {
        Vector3 center = new Vector3(0, 5f, 0);
        float maxDistance = 5f;
        int amount = 8000;
        //int amount = 30000;
        List<Vector3> attractionPoints = GenerateAttractionPoints(center, maxDistance, amount);

        GrowthProperties growthProperties = new GrowthProperties();
        growthProperties.SetInfluenceDistance(0.9f);
        //growthProperties.SetInfluenceDistance(1.5f);
        growthProperties.SetPerceptionAngle(160f);
        //growthProperties.SetPerceptionAngle(100f);
        growthProperties.SetClearDistance(0.8f);
        //growthProperties.SetClearDistance(0.5f);
        growthProperties.SetTropisms(new Vector3(-0.25f, 0.5f, 0));
        growthProperties.SetGrowthDistance(0.125f);
        growthProperties.SetAttractionPoints(attractionPoints);

        SpaceColonization grower = new SpaceColonization(growthProperties);


        Vector3 position = Vector3.zero;

        GeometryProperties geometryProperties = new GeometryProperties();
        geometryProperties.SetTipRadius(0.007f);
        //geometryProperties.SetNthRoot(1.6f);
        geometryProperties.SetNthRoot(1.3f);
        geometryProperties.SetCircleResolution(3);

        geometryProperties.SetMaxTwigRadiusForLeaves(0.02f);
        geometryProperties.SetMinLeafSize(0.1f);
        geometryProperties.SetMaxLeafSize(0.65f);
        geometryProperties.SetLeafType(LeafType.Triangle);
        geometryProperties.SetLeavesPerNode(2);
        geometryProperties.SetLeavesEnabled(true);


        tree = new Tree(position, grower, geometryProperties);

        tree.Grow(100);
        //tree.Grow(30);
    }

    //void hollowNormalTree() {
    //    Vector3 center = new Vector3(0, 5f, 0);
    //    float minDistance = 3.5f;
    //    float maxDistance = 5f;
    //    int amount = 2000;
    //    HashSet<Vector3> attractionPoints = GenerateAttractionPoints_Outer(center, minDistance, maxDistance, amount);

    //    GrowthProperties growthProperties = new GrowthProperties();
    //    growthProperties.SetInfluenceDistance(2*maxDistance);
    //    growthProperties.SetGrowthDistance(0.25f);
    //    growthProperties.SetClearDistance(0.4f);
    //    growthProperties.SetTropisms(new Vector3(0, 1f, 0));

    //    growthProperties.SetMaxTwigRadiusForLeaves(0.02f);
    //    growthProperties.SetMinLeafSize(0.1f);
    //    growthProperties.SetMaxLeafSize(0.5f);
    //    growthProperties.SetLeavesPerNode(2);

    //    growthProperties.SetMaxBranchingAngle(80);
    //    //growthProperties.SetNthRoot(1.f);
    //    SpaceColonization grower = new SpaceColonization(attractionPoints, growthProperties);
    //    //SpaceColonizationSimple grower = new SpaceColonizationSimple(attractionPoints, growthProperties);


    //    Vector3 position = Vector3.zero;
    //    tree = new Tree(position, grower, growthProperties);

    //    cylinderResolution = 4;

    //    tree.Grow(40, cylinderResolution);
    //}

    //void demoTree() {
    //    Vector3 center = new Vector3(0, 5f, 0);
    //    float maxDistance = 5f;
    //    int amount = 5000;
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
    //    growthProperties.SetNthRoot(1.2f);
    //    SpaceColonization grower = new SpaceColonization(attractionPoints, growthProperties);


    //    Vector3 position = Vector3.zero;
    //    tree = new Tree(position, grower, growthProperties);

    //    cylinderResolution = 6;

    //    tree.Grow(40, cylinderResolution);
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
        tree.GetEverything(ref vertices, ref normals, ref uvs, ref triangles);
        UpdateMesh();
    }

    List<Vector3> GenerateAttractionPoints(Vector3 center, float radius, int amount) {
        List<Vector3> result = new List<Vector3>();
        while (result.Count < amount) {
            float x = UnityEngine.Random.Range(-radius, radius);
            float y = UnityEngine.Random.Range(-radius, radius);
            float z = UnityEngine.Random.Range(-radius, radius);
            Vector3 point = new Vector3(x, y, z) + center;

            float distance = (point - center).magnitude;
            if (distance <= radius) {
                result.Add(point);
            }
        }

        return result;
    }


    HashSet<Vector3> GenerateAttractionPoints_Outer(Vector3 center, float minRadius, float maxRadius, int amount) {
        HashSet<Vector3> result = new HashSet<Vector3>();
        while (result.Count < amount) {
            float x = UnityEngine.Random.Range(-maxRadius, maxRadius);
            float y = UnityEngine.Random.Range(-maxRadius, maxRadius);
            float z = UnityEngine.Random.Range(-maxRadius, maxRadius);
            Vector3 point = new Vector3(x, y, z) + center;

            float distance = (point - center).magnitude;
            if (distance <= maxRadius && distance >= minRadius) {
                result.Add(point);
            }
        }

        return result;
    }


    HashSet<Vector3> GenerateAttractionPoints(Vector3 center, float sideRadius, float upperRadius, int amount) {
        HashSet<Vector3> result = new HashSet<Vector3>();
        while (result.Count < amount) {
            float x = UnityEngine.Random.Range(-sideRadius, sideRadius);
            float y = UnityEngine.Random.Range(-upperRadius, upperRadius);
            float z = UnityEngine.Random.Range(-sideRadius, sideRadius);
            Vector3 point = new Vector3(x, y, z) + center;

            //float distance = (point - center).magnitude;
            //if (distance <= radius) {
                result.Add(point);
            //}
        }

        return result;
    }

    void UpdateMesh() {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.normals = normals;
    }

    /* NOTIZEN
     * nächste Schritte:
     * - Blätter dreieckig machen / austauschbar machen
     * >> Anzahl der Leaves im Nachhinein änderbar machen
     * - Texturen -> Farben
     * - UI
     * - - Alter
     * - - Speichern
     * - - Randomize
     * - Punktewolkengenerator
     * - - was überlegen für die Form der Basis-Baumkrone
     * - - in die Höhe ziehen
     * - - in die Breite ziehen
     * - - Punkte überwiegend am Rand
     * - - Punkte während des Wachstums hinzufügen
     *
     * 
     * - bessere Geometrie
     * - - dünne Äste an dicken Ästen fixen
     * - - Verdrehung fixen
     * - Konifären mittels Apical Control?
     * - hängende Äste über Änderung des PerceptionVolumes -> Achse in Abhängigkeit der Noderichtung
     *
     *
     * - Blätter als Tetraeder oder ein paar senkrecht zueinander stehende Bilder?
     * - Blätter besser verteilen an Nodes?
     * - mehrere Farben auswählbar machen für Blätter
     * - Farbenverläufe
     *
     *
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
     *
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
