using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class SceneView : MonoBehaviour {
    public EventSystem eventSystem;

    float mouse_x;
    float mouse_y;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update() {
        if (!eventSystem.IsPointerOverGameObject()) {
            if (Input.GetMouseButtonDown(0)) {
                mouse_x = Input.mousePosition.x;
                mouse_y = Input.mousePosition.y;
            }

            if (Input.GetMouseButton(0)) {
                float d_x = mouse_x - Input.mousePosition.x;
                float d_y = mouse_y - Input.mousePosition.y;

                GameObject.Find("TreeMesh").GetComponent<Transform>().RotateAround(Vector3.zero, Vector3.up, d_x);

                mouse_x = Input.mousePosition.x;
                mouse_y = Input.mousePosition.y;
            }
        }
    }
}
