using UnityEngine;
using UnityEngine.UI;

public class Age : MonoBehaviour {
    public void OnValueChanged() {
        Debug.Log("UI: Slider: Age");

        int value = (int) GetComponent<Slider>().value;
        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnAge(value);
    }
}