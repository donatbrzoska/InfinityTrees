using System;
using System.Collections.Generic;
using UnityEngine;

public class Leaf {

    public enum LeafType {
        Square,
        ParticleSquare,
        ParticleCrossFoil,
        CustomCrossFoil,
        Triangle
    }

    public static Dictionary<Leaf.LeafType, string> LeafTypeToString = new Dictionary<Leaf.LeafType, string> {
        { Leaf.LeafType.ParticleSquare, "Particle Square"},
        { Leaf.LeafType.ParticleCrossFoil, "Particle Cross Foil"},
        { Leaf.LeafType.CustomCrossFoil, "Custom Cross Foil"},
        { Leaf.LeafType.Triangle, "Triangle" },
        { Leaf.LeafType.Square, "Custom Square" }
    };

    public static Dictionary<Leaf.LeafType, string> LeafTypeToFilename = new Dictionary<Leaf.LeafType, string> {
        { Leaf.LeafType.ParticleSquare, "particle"},
        { Leaf.LeafType.ParticleCrossFoil, "particle"},
        { Leaf.LeafType.Triangle, "triangle" },
        { Leaf.LeafType.Square, "custom_texture.png" },
        { Leaf.LeafType.CustomCrossFoil, "custom_texture.png" }
    };

    public static Dictionary<string, Leaf.LeafType> LeafTypeStringToLeafType = new Dictionary<string, Leaf.LeafType> {
        { "Particle Square", Leaf.LeafType.ParticleSquare },
        { "Particle Cross Foil", Leaf.LeafType.ParticleCrossFoil },
        { "Custom Cross Foil", Leaf.LeafType.CustomCrossFoil },
        { "Triangle", Leaf.LeafType.Triangle },
        {  "Custom Square", Leaf.LeafType.Square}
    };

    public static List<string> LeafTypeStrings = new List<string> { "Triangle", "Particle Square", "Particle Cross Foil", "Custom Square", "Custom Cross Foil"};

	Vector3 position;
    public void UpdatePosition(Vector3 diff) {
        position = position + diff;
    }

    public void Rotate(Vector3 byPoint, Quaternion quaternion) {
        Vector3 d = this.position - byPoint;
        Vector3 direction = quaternion * d;
        this.position = byPoint + direction;
    }

    GeometryProperties geometryProperties;

    Quaternion rotation;

    // the leaf needs its own random, since randoms should only be used from one thread at a time
    private static System.Random random = new System.Random();

    public Leaf(Vector3 position, GeometryProperties geometryProperties) {
        this.position = position;
        this.geometryProperties = geometryProperties;

        float rotationAngle = Util.RandomInRange(0, 360, random);
        Vector3 rotationAxis = Util.RandomVector3(random);
        rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);
    }



    //only used for deep copy
    private Leaf(Vector3 position, GeometryProperties geometryProperties, Quaternion rotation) {
        this.position = position;
        this.geometryProperties = geometryProperties;
        this.rotation = rotation;
    }
    public Leaf GetCopy() {
        return new Leaf(position, geometryProperties, rotation);
    }



    public void GetMesh(List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {

        //Vector3 swooshVector = new Vector3(0, 0.0000f, 0);

        float size = geometryProperties.GetLeafSizeValue();

        int verticesOffset = verticesResult.Count;

        if (geometryProperties.GetLeafType() == LeafType.Square) {

            //Quaternion y_rotation = Quaternion.AngleAxis(Util.RandomInRange(0, 360), Vector3.up);
            //Quaternion x_z_rotation = Quaternion.AngleAxis(Util.RandomInRange(0, 90), Vector3.right * Util.RandomWithStdDev(0, 1) + Vector3.forward * Util.RandomWithStdDev(0, 1));
            ////Quaternion x_z_rotation = Quaternion.AngleAxis(Util.RandomInRange(0, 90), Vector3.right * Util.RandomInRange(-1, 1) + Vector3.forward * Util.RandomInRange(-1, 1));
            //Quaternion rotation = y_rotation * x_z_rotation;


			//top side of leaf
			verticesResult.Add(rotation * new Vector3(-0.5f, 0, 0) * size + position);
            verticesResult.Add(rotation * new Vector3(-0.5f, 0, 1) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0, 1) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0, 0) * size + position);

            //bottom side of leaf
            verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, 0) * size + position);
            verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, 1) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0.0001f, 1) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0.0001f, 0) * size + position);


            //top side of leaf
            uvsResult.Add(new Vector2(0.5f, 0));
            uvsResult.Add(new Vector2(0.5f, 1));
            uvsResult.Add(new Vector2(1, 1));
            uvsResult.Add(new Vector2(1, 0));

            //bottom side of leaf
            uvsResult.Add(new Vector2(0.5f, 0));
            uvsResult.Add(new Vector2(0.5f, 1));
            uvsResult.Add(new Vector2(1, 1));
            uvsResult.Add(new Vector2(1, 0));


            //top side of leaf
            //triangle 1
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 1);
            trianglesResult.Add(verticesOffset + 2);
            //triangle 2
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 3);

            verticesOffset += 4; //TODO: ADJUST THIS WHEN ADDING VERTICES FOR LEAVES

            //bottom side of leaf
            //triangle 1
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 1);
            //triangle 2
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 3);
            trianglesResult.Add(verticesOffset + 2);

        } else if (geometryProperties.GetLeafType() == LeafType.ParticleSquare) {

            //top side of leaf
            verticesResult.Add(rotation * new Vector3(-0.5f, 0, -0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(-0.5f, 0, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0, -0.5f) * size + position);

            //bottom side of leaf
            verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, -0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0.0001f, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0.0001f, -0.5f) * size + position);


            //top side of leaf
            uvsResult.Add(new Vector2(0.5f, 0));
            uvsResult.Add(new Vector2(0.5f, 1));
            uvsResult.Add(new Vector2(1, 1));
            uvsResult.Add(new Vector2(1, 0));

            //bottom side of leaf
            uvsResult.Add(new Vector2(0.5f, 0));
            uvsResult.Add(new Vector2(0.5f, 1));
            uvsResult.Add(new Vector2(1, 1));
            uvsResult.Add(new Vector2(1, 0));


            //top side of leaf
            //triangle 1
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 1);
            trianglesResult.Add(verticesOffset + 2);
            //triangle 2
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 3);

            verticesOffset += 4; //TODO: ADJUST THIS WHEN ADDING VERTICES FOR LEAVES

            //bottom side of leaf
            //triangle 1
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 1);
            //triangle 2
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 3);
            trianglesResult.Add(verticesOffset + 2);

        } else if (geometryProperties.GetLeafType() == LeafType.ParticleCrossFoil || geometryProperties.GetLeafType() == LeafType.CustomCrossFoil) {

            //top side of leaf square 1
            verticesResult.Add(rotation * new Vector3(-0.5f, 0, -0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(-0.5f, 0, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0, -0.5f) * size + position);

            //bottom side of leaf square 1
            verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, -0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0.0001f, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0.0001f, -0.5f) * size + position);


            //top side of leaf square 1
            uvsResult.Add(new Vector2(0.5f, 0));
            uvsResult.Add(new Vector2(0.5f, 1));
            uvsResult.Add(new Vector2(1, 1));
            uvsResult.Add(new Vector2(1, 0));

            //bottom side of leaf square 1
            uvsResult.Add(new Vector2(0.5f, 0));
            uvsResult.Add(new Vector2(0.5f, 1));
            uvsResult.Add(new Vector2(1, 1));
            uvsResult.Add(new Vector2(1, 0));


            //top side of leaf square 1
            //triangle 1
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 1);
            trianglesResult.Add(verticesOffset + 2);
            //triangle 2
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 3);

            verticesOffset += 4; //TODO: ADJUST THIS WHEN ADDING VERTICES FOR LEAVES

            //bottom side of leaf square 1
            //triangle 1
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 1);
            //triangle 2
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 3);
            trianglesResult.Add(verticesOffset + 2);



            verticesOffset += 4;



            //top side of leaf square 2
            verticesResult.Add(rotation * new Vector3(-0.5f, 0, -0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.0f, 0.707f, 0.0f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.0f, -0.707f, 0.0f) * size + position);

            //bottom side of leaf square 2
            verticesResult.Add(rotation * new Vector3(-0.5f, 0.000001f, -0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.0f, 0.707001f, 0.0f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.5f, 0.000001f, 0.5f) * size + position);
            verticesResult.Add(rotation * new Vector3(0.0f, -0.707001f, 0.0f) * size + position);


            //top side of leaf square 2
            uvsResult.Add(new Vector2(0.5f, 0));
            uvsResult.Add(new Vector2(0.5f, 1));
            uvsResult.Add(new Vector2(1, 1));
            uvsResult.Add(new Vector2(1, 0));

            //bottom side of leaf square 2
            uvsResult.Add(new Vector2(0.5f, 0));
            uvsResult.Add(new Vector2(0.5f, 1));
            uvsResult.Add(new Vector2(1, 1));
            uvsResult.Add(new Vector2(1, 0));


            //top side of leaf square 2
            //triangle 1
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 1);
            trianglesResult.Add(verticesOffset + 2);
            //triangle 2
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 3);

            verticesOffset += 4; //TODO: ADJUST THIS WHEN ADDING VERTICES FOR LEAVES

            //bottom side of leaf square 2
            //triangle 1
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 1);
            //triangle 2
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 3);
            trianglesResult.Add(verticesOffset + 2);

        } else if (geometryProperties.GetLeafType() == LeafType.Triangle) {

            //top side of leaf
            verticesResult.Add(rotation * new Vector3(-0.5f, 0, 0) * size + position);
            verticesResult.Add(rotation * new Vector3( 0,    0, 0.866f) * size + position);
            verticesResult.Add(rotation * new Vector3( 0.5f, 0, 0) * size + position);

            //bottom side of leaf
            verticesResult.Add(rotation * new Vector3(-0.5f, 0.0001f, 0) * size + position);
            verticesResult.Add(rotation * new Vector3( 0,    0.0001f, 0.866f) * size + position);
            verticesResult.Add(rotation * new Vector3( 0.5f, 0.0001f, 0) * size + position);


            //top side of leaf
            uvsResult.Add(new Vector2(0.6f, 0));
            uvsResult.Add(new Vector2(0.75f, 1));
            uvsResult.Add(new Vector2(0.9f, 0));

            //bottom side of leaf
            uvsResult.Add(new Vector2(0.6f, 0));
            uvsResult.Add(new Vector2(0.75f, 1));
            uvsResult.Add(new Vector2(0.9f, 0));


            //top side of leaf
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 1);
            trianglesResult.Add(verticesOffset + 2);

            verticesOffset += 3; //TODO: ADJUST THIS WHEN ADDING VERTICES FOR LEAVES

            //bottom side of leaf
            trianglesResult.Add(verticesOffset + 0);
            trianglesResult.Add(verticesOffset + 2);
            trianglesResult.Add(verticesOffset + 1);
        }
    }
}