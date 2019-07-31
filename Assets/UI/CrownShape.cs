using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrownShape : MonoBehaviour {
    Middleware middleware;

    public CrownShape() {
        middleware = new Middleware();
    }

    public void OnValueChanged_Width() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownWidth(value);

        middleware.DisableCameraMovement();
        middleware.EnablePointCloudRenderer();
    }

    public void OnValueChanged_Height() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownHeight(value);

        middleware.DisableCameraMovement();
        middleware.EnablePointCloudRenderer();
    }

    public void OnValueChanged_Depth() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownDepth(value);

        middleware.DisableCameraMovement();
        middleware.EnablePointCloudRenderer();
    }

    public void OnValueChanged_TopCutoff() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownTopCutoff(value);

        middleware.DisableCameraMovement();
        middleware.EnablePointCloudRenderer();
    }

    public void OnValueChanged_BottomCutoff() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownBottomCutoff(value);

        middleware.DisableCameraMovement();
        middleware.EnablePointCloudRenderer();
    }


    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            middleware.EnableCameraMovement();
            middleware.DisablePointCloudRenderer();
            GameObject.Find("Core").GetComponent<Core>().OnCrownShapeDone();
        }
    }
}
