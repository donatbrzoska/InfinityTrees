using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CrownShape : MonoBehaviour {
    Middleware middleware;

    public CrownShape() {
        middleware = new Middleware();
    }


    public void OnValueChanged_Thickness() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnThickness(value);
    }

    public void OnValueChanged_Length() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnLength(value);
    }



    public void OnValueChanged_Width() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownWidth(value);

        //middleware.DisableCameraMovement();
        //middleware.EnablePointCloudRenderer();
    }

    public void OnValueChanged_Height() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownHeight(value);

        //middleware.DisableCameraMovement();
        //middleware.EnablePointCloudRenderer();
    }

    public void OnValueChanged_Depth() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownDepth(value);

        //middleware.DisableCameraMovement();
        //middleware.EnablePointCloudRenderer();
    }

    public void OnValueChanged_TopCutoff() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownTopCutoff(value);

        //middleware.DisableCameraMovement();
        //middleware.EnablePointCloudRenderer();
    }

    public void OnValueChanged_BottomCutoff() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownBottomCutoff(value);

        //middleware.DisableCameraMovement();
        //middleware.EnablePointCloudRenderer();
    }




    public void OnValueChanged_ClearDistanceBegin_clearDistanceEnd_Ratio() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnClearDistanceBegin_clearDistanceEnd_Ratio(value);
    }

    public void OnValueChanged_BranchDensityBegin() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnBranchDensityBegin(value);
    }

    public void OnValueChanged_BranchDensityEnd() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnBranchDensityEnd(value);
    }




    public void OnValueChanged_HangingBranches() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnHangingBranches(value);
    }




    public void OnValueChanged_Density() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnDensity(value);
        //GameObject.Find("Density Text").GetComponent<Text>().text = "Density " + value;
        //GameObject.Find("Density Text").GetComponent<Text>().resizeTextForBestFit = true;
    }

    public void OnValueChanged_ClearDistanceBegin() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnClearDistanceBegin(value);
        //GameObject.Find("Clear Distance Begin Text").GetComponent<Text>().text = "Begin " + value;
        //GameObject.Find("Clear Distance Begin Text").GetComponent<Text>().resizeTextForBestFit = true;
    }

    public void OnValueChanged_ClearDistanceEnd() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnClearDistanceEnd(value);
        //GameObject.Find("Clear Distance End Text").GetComponent<Text>().text = "End " + value;
        //GameObject.Find("Clear Distance End Text").GetComponent<Text>().resizeTextForBestFit = true;
    }

    public void OnValueChanged_InfluenceDistance() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnInfluenceDistance(value);
        //GameObject.Find("Influence Distance Text").GetComponent<Text>().text = "Influence Distance " + value;
        //GameObject.Find("Influence Distance Text").GetComponent<Text>().resizeTextForBestFit = true;
    }

    public void OnValueChanged_GrowthDistance() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnGrowthDistance(value);
        //GameObject.Find("Growth Distance Text").GetComponent<Text>().text = "GrowthDistance " + value;
        //GameObject.Find("Growth Distance Text").GetComponent<Text>().resizeTextForBestFit = true;
    }

    public void OnValueChanged_PerceptionAngle() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnPerceptionAngle(value);
        //GameObject.Find("Perception Angle Text").GetComponent<Text>().text = "PerceptionAngle " + value;
        //GameObject.Find("Perception Angle Text").GetComponent<Text>().resizeTextForBestFit = true;
    }


    void Update() {
        if (Input.GetMouseButtonDown(0) && GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject == gameObject) {
            middleware.DisableCameraMovement();
            middleware.EnablePointCloudRenderer();
        }

        if (Input.GetMouseButtonUp(0)) {
            middleware.EnableCameraMovement();
            middleware.DisablePointCloudRenderer();
            GameObject.Find("Core").GetComponent<Core>().OnCrownShapeDone();
        }
    }
}
