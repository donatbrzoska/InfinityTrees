using UnityEngine;
using UnityEngine.UI;

public class Age : MonoBehaviour {

    int sentValue=30;
    Middleware middleware;

    public Age() {
        middleware = new Middleware();
    }

    public void OnValueChanged() {
        middleware.DisableCameraMovement();
    }

    public void Update() {
        if (Input.GetMouseButtonUp(0)) {
            int value = (int) GetComponent<Slider>().value;
            if (sentValue != value) {
                GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnAge(value);
                sentValue = value;
            }

            middleware.EnableCameraMovement();
        }
    }
}