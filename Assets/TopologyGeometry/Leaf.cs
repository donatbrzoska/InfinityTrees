using System;
using System.Collections.Generic;
using UnityEngine;

//Entwurfsentscheidungen:
// wichtig ist vor allem, dass geometryProperties geändert werden können
// >> ohne, dass der ganze Baum neu berechnet werden muss
public class Leaf {

    public enum LeafType {
        Square,
        ParticleSquare,
        ParticleCrossFoil,
        Triangle
    }

    public static Dictionary<Leaf.LeafType, string> LeafTypeToString = new Dictionary<Leaf.LeafType, string> {
        { Leaf.LeafType.ParticleSquare, "Particle"},
        { Leaf.LeafType.ParticleCrossFoil, "Particle Cross Foil"},
        { Leaf.LeafType.Triangle, "Triangle" }
    };

    public static Dictionary<Leaf.LeafType, string> LeafTypeToFilename = new Dictionary<Leaf.LeafType, string> {
        { Leaf.LeafType.ParticleSquare, "particle"},
        { Leaf.LeafType.ParticleCrossFoil, "particle"},
        { Leaf.LeafType.Triangle, "triangle" }
    };

    public static Dictionary<string, Leaf.LeafType> LeafTypeStringToLeafType = new Dictionary<string, Leaf.LeafType> {
        { "Particle", Leaf.LeafType.ParticleSquare },
        { "Particle Cross Foil", Leaf.LeafType.ParticleCrossFoil },
        { "Triangle", Leaf.LeafType.Triangle }
    };

    public static List<string> LeafTypeStrings = new List<string> { "Particle", "Particle Cross Foil", "Triangle" };

	Vector3 position;
    public void UpdatePosition(Vector3 diff) {
        position = position + diff;
    }

    //float size;
    //public void UpdateSize() {
    //    size = geometryProperties.GetLeafSize();
    //}

    GeometryProperties geometryProperties;

    Quaternion rotation;


    public Leaf(Vector3 position, GeometryProperties geometryProperties) {
        this.position = position;
        this.geometryProperties = geometryProperties;

        float rotationAngle = Util.RandomInRange(0, 360);
        Vector3 rotationAxis = Util.RandomVector3();
        rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);

        //size = geometryProperties.GetLeafSize();
    }

    public void CalculateAndStoreGeometry(List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {

        //Vector3 swooshVector = new Vector3(0, 0.0000f, 0);

        float size = geometryProperties.GetLeafSize();

        int verticesOffset = verticesResult.Count;

        if (geometryProperties.GetLeafType() == LeafType.Square) {

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

        } else if (geometryProperties.GetLeafType() == LeafType.ParticleCrossFoil) {

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