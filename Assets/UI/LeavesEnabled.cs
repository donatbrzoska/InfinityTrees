using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeavesEnabled : MonoBehaviour {
    Toggle toggle;

    // Start is called before the first frame update
    void Start() {
        toggle = GetComponent<Toggle>();
    }

    public void OnValueChanged() {
        GameObject.Find("Core").GetComponent<Core>().OnLeavesEnabled(toggle.isOn);
    }

    // Update is called once per frame
    void Update() {

    }
}
