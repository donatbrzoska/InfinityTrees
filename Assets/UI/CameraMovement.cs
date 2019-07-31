﻿using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour {
    public EventSystem eventSystem;

    float mouse_x;
    float mouse_y;

    float distanceToTreeCenter;
    float horizontalRotation;
    float verticalRotation;

    float y_position;
    float speed = 0.1f;

    public bool Enabled { get; set; } = true;

    // Start is called before the first frame update
    void Start() {
        distanceToTreeCenter = 5;
        horizontalRotation = 270;
        verticalRotation = 90;
        y_position = 0; // this should immedeately get updated

        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update() {
        if (Enabled) {
            // find out where to look at
            Vector3 treeCenter = GameObject.Find("Core").GetComponent<Core>().GetTreeCenter();

            if (Input.GetKey(KeyCode.Space)) {
                y_position += speed;

                if (y_position > ((PseudoEllipsoid)GameObject.Find("Core").GetComponent<Core>().GetAttractionPoints()).GetHeight()) {
                    y_position -= speed;
                }
            }
            if (Input.GetKey(KeyCode.LeftShift)) {
                y_position -= speed;

                if (y_position + treeCenter.y < 0) {
                    y_position += speed;
                }
            }


            Vector3 lookAt = new Vector3(treeCenter.x, treeCenter.y + y_position, treeCenter.z);



            // 1. determine mouse difference
            if (!eventSystem.IsPointerOverGameObject()) {
                // detect mouse click
                if (Input.GetMouseButtonDown(0)) {
                    mouse_x = Input.mousePosition.x;
                    mouse_y = Input.mousePosition.y;
                }
                // detect changes of the mouse position when it has changed
                if (Input.GetMouseButton(0)) {
                    // calculate difference to last call
                    float d_x = mouse_x - Input.mousePosition.x;
                    float d_y = -(mouse_y - Input.mousePosition.y);

                    mouse_x = Input.mousePosition.x;
                    mouse_y = Input.mousePosition.y;

                    // update the rotation parameters
                    horizontalRotation = (horizontalRotation + d_x) % 360;

                    //comment this for camera demo
                    float verticalRotationBackup = verticalRotation; //make backup for restoring when flipping over
                    verticalRotation = (verticalRotation + d_y) % 360;
                    if (verticalRotation < 0 || verticalRotation > 180) { //don't flip over
                        verticalRotation = verticalRotationBackup;
                    }
                }

                // detect mouse scroll
                distanceToTreeCenter += Input.mouseScrollDelta.y;
            }


            // 2. update camera
            //// find out where to look at
            //Vector3 treeCenter = GameObject.Find("TreeMesh").GetComponent<TreeCreator>().GetAttractionPoints().GetCenter();
            //http://mathworld.wolfram.com/SphericalCoordinates.html -> y und z tauschen
            // calculate position based on rotations and distance to the center of the tree
            float x = distanceToTreeCenter * Mathf.Cos(Util.DegreesToRadians(horizontalRotation)) * Mathf.Sin(Util.DegreesToRadians(verticalRotation));
            float y = distanceToTreeCenter * Mathf.Cos(Util.DegreesToRadians(verticalRotation));
            float z = distanceToTreeCenter * Mathf.Sin(Util.DegreesToRadians(horizontalRotation)) * Mathf.Sin(Util.DegreesToRadians(verticalRotation));
            //Vector3 position = new Vector3(x, y, z) * distanceToTreeCenter + treeCenter;
            Vector3 position = new Vector3(x, y, z) * distanceToTreeCenter + lookAt;

            // update position and lookAt
            GetComponent<Transform>().position = position;
            //GetComponent<Transform>().LookAt(treeCenter);
            GetComponent<Transform>().LookAt(lookAt);


        }
    }
}