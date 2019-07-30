using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StemColor : MonoBehaviour {
    Dropdown dropdown;

    // Start is called before the first frame update
    void Start() {
        dropdown = GetComponent<Dropdown>();
    }

    public void Initialize(List<string> stemColors) {
        // colors are defined here, because you cannot define them in the Core,
        // .. because then you would need to initialize the color pickers after
        // .. the Core has been initialized
        //// and then you needed to tell the color pickers to put the colors to the UI, because that cannot be done via an extra thread?
        //colors = new List<string> { "dark_brown", "brown", "greyish" };
        foreach (string c in stemColors) {
            dropdown.options.Add(new Dropdown.OptionData(c));
        }
    }

    public void OnValueChanged() {
        GameObject.Find("Core").GetComponent<Core>().OnStemColor(dropdown.value);
    }

    // Update is called once per frame
    //void Update() {

    //}
}
