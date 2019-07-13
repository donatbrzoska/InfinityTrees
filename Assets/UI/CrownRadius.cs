using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrownRadius : MonoBehaviour {
    public void OnValueChanged_x() {
        Debug.Log("UI: Slider: Age");

        int value = (int)GetComponent<Slider>().value;
        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnCrownRadius_x(value);
    }

    public void OnValueChanged_y() {
        Debug.Log("UI: Slider: Age");

        int value = (int)GetComponent<Slider>().value;
        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnCrownRadius_y(value);
    }

    public void OnValueChanged_z() {
        Debug.Log("UI: Slider: Age");

        int value = (int)GetComponent<Slider>().value;
        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnCrownRadius_z(value);
    }
}
