using System;
using System.Collections.Generic;
using UnityEngine;

//Entwurfsentscheidungen:
// wichtig ist vor allem, dass geometryProperties geändert werden können
// >> ohne, dass der ganze Baum neu berechnet werden muss
// >> 
public class Leaf {
    Vector3 position;
    GeometryProperties geometryProperties;

    bool propertiesCalculated;

    float rotationAngle;
    Vector3 rotationAxis;
    float size;


    public Leaf(Vector3 position, GeometryProperties geometryProperties) {
        this.position = position;
        this.geometryProperties = geometryProperties;
    }

    public void CalculateAndStoreGeometry(List<Vector3> verticesResult, List<Vector2> uvsResult, List<int> trianglesResult) {

        if (!propertiesCalculated) { //calculate these here, because only the main thread can make use of Unitys Range() and Random() functions
            rotationAngle = geometryProperties.GetRandomLeafRotationAngle();
            rotationAxis = geometryProperties.GetRandomLeafRotationAxis();
            size = geometryProperties.GetLeafSize();

            propertiesCalculated = true;
        }

        int verticesOffset = verticesResult.Count;
        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, rotationAxis);

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

public enum LeafType {
    Square,
    Triangle
}