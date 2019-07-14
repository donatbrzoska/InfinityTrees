using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    Dictionary<GameObject, float> sliders = new Dictionary<GameObject, float>();
    TreeCreator listener;

    // Start is called before the first frame update
    void Start() {
        listener = GameObject.Find("TreeMesh").GetComponent<TreeCreator>();
        sliders[GameObject.Find("Width X Slider")] = -1;
        sliders[GameObject.Find("Width Y Slider")] = -1;
        sliders[GameObject.Find("Width Z Slider")] = -1;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject o in sliders.Keys) {
            float sliderValue = o.GetComponent<Slider>().value;

            if (!AlmostEqual(sliderValue, sliders[o], 0.1f)) {
                if (o.name == "Width X Slider") {
                    listener.OnCrownRadius_x(sliderValue);
                    sliders[o] = sliderValue;
                } else if (o.name == "Width Y Slider") {
                    listener.OnCrownRadius_y(sliderValue);
                    sliders[o] = sliderValue;
                } else if (o.name == "Width Z Slider") {
                    listener.OnCrownRadius_z(sliderValue);
                    sliders[o] = sliderValue;
                }
            }
        }
    }

    bool AlmostEqual(float a, float b, float max_d) {
        return System.Math.Abs(a - b) < max_d;
    }
}

            //int[] indizes = new int[vertices.Length];
            //for (int i = 0; i<indizes.Length; i++) {
            //    indizes[i] = i;
            //}

            //mesh.SetIndices(indizes, MeshTopology.Quads, 0);