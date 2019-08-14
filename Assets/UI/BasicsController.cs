using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasicsController : MonoBehaviour {

    Dropdown dropdown; // a reference to the dropdown, if there is one in the parent GameObject

    private void Start() {
        dropdown = GetComponent<Dropdown>();
    }

    // DROPDOWNS

    public void Initialize_StemColors(List<string> stemColors) {
        dropdown.options.Clear();
        foreach (string c in stemColors) {
            dropdown.options.Add(new Dropdown.OptionData(c));
        }
    }

    public void OnValueChanged_StemColor() {
        GameObject.Find("Core").GetComponent<Core>().OnStemColor(dropdown.value);
    }


    public void Initialize_LeafColors(List<string> leafColors) {
        dropdown.options.Clear();
        foreach (string c in leafColors) {
            dropdown.options.Add(new Dropdown.OptionData(c));
        }
    }

    public void OnValueChanged_LeafColor() {
        GameObject.Find("Core").GetComponent<Core>().OnLeafColor(dropdown.value);
    }

    ////https://answers.unity.com/questions/1303416/how-to-add-color-to-dropdown-item-in-runtime.html
    ////Color color = new Color(0.2f, 0, 0.4f);
    //Color color = Color.yellow;

    //Texture2D texture = new Texture2D(1, 1); // creating texture with 1 pixel
    //texture.SetPixel(0, 0, color); // setting to this pixel some color
    //texture.Apply(); //applying texture. necessarily

    //Dropdown.OptionData item = new Dropdown.OptionData();
    //item.image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)); // creating dropdown item and converting texture to sprite
    ////item.image = 
    ////Dropdown.OptionData item = new Dropdown.OptionData("weird item"); // creating dropdown item and converting texture to sprite
    //dropdown.options.Add(item); // adding this item to dropdown options


    public void Initialize_LeafTypes(List<string> leafTypes) {
        dropdown.options.Clear();
        foreach (string t in leafTypes) {
            dropdown.options.Add(new Dropdown.OptionData(t));
        }
    }

    public void OnValueChanged_LeafType() {
        GameObject.Find("Core").GetComponent<Core>().OnLeafType(dropdown.value);
    }

    // SLIDERS

    public void OnValueChanged_Age() {
        int value = (int)GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnAge(value);
    }

    public void OnValueChanged_Thickness() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnThickness(value);
    }

    public void OnValueChanged_Length() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnLength(value);
    }




    public void OnValueChanged_FoliageDensity() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnFoliageDensity(value);
    }

    public void OnValueChanged_FoliageLobeSize() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnFoliageLobeSize(value);
    }





    public void OnValueChanged_Width() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownWidth(value);
    }

    public void OnValueChanged_Height() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownHeight(value);
    }

    public void OnValueChanged_Depth() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownDepth(value);
    }

    public void OnValueChanged_TopCutoff() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownTopCutoff(value);
    }

    public void OnValueChanged_BottomCutoff() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnCrownBottomCutoff(value);
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

    public void OnValueChanged_GrowTowardsLight() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnGrowTowardsLight(value);
    }




    public void OnValueChanged_Density() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnDensity(value);
    }

    public void OnValueChanged_ClearDistanceBegin() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnClearDistanceBegin(value);
    }

    public void OnValueChanged_ClearDistanceEnd() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnClearDistanceEnd(value);
    }

    public void OnValueChanged_InfluenceDistance() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnInfluenceDistance(value);
    }

    public void OnValueChanged_GrowthDistance() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnGrowthDistance(value);
    }

    public void OnValueChanged_PerceptionAngle() {
        float value = GetComponent<Slider>().value;
        GameObject.Find("Core").GetComponent<Core>().OnPerceptionAngle(value);
    }

    // BUTTONS


    public void OnClick_NewSeed() {
        GameObject.Find("Core").GetComponent<Core>().OnNewSeed();
    }


    public void OnClick_Quit() {
        Application.Quit();
    }


    public void OnClick_ResetToDefaults() {
        GameObject.Find("Core").GetComponent<Core>().OnResetToDefaults();
    }

    // SIDE EFFECTS

    void Update() {

        if (Input.GetMouseButtonDown(0) && GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject == gameObject) {
            GameObject.Find("Core").GetComponent<Core>().DisableCameraMovement();
            GameObject.Find("Core").GetComponent<Core>().EnablePointCloudRenderer();
        }

        if (Input.GetMouseButtonUp(0)) {
            GameObject.Find("Core").GetComponent<Core>().EnableCameraMovement();
            GameObject.Find("Core").GetComponent<Core>().DisablePointCloudRenderer();
            //GameObject.Find("Core").GetComponent<Core>().OnCrownShapeDone(); //set boolean flag for this
        }



        if (Input.GetKey("escape")) {
            Application.Quit();
        }
    }
}
