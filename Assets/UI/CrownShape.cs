using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrownShape : MonoBehaviour {
    Middleware middleware;

    public CrownShape() {
        middleware = new Middleware();
    }

    public void OnValueChanged_x() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnCrownRadius_x(value);

        middleware.DisableCameraMovement();
        middleware.ShowPointCloud();
    }

    public void OnValueChanged_y() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnCrownRadius_y(value);

        middleware.DisableCameraMovement();
        middleware.ShowPointCloud();
    }

    public void OnValueChanged_z() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnCrownRadius_z(value);

        middleware.DisableCameraMovement();
        middleware.ShowPointCloud();
    }


    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            middleware.EnableCameraMovement();
            middleware.HidePointCloud();
        }
    }
}
