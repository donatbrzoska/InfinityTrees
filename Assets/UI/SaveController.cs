using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using UnityEngine.EventSystems;

public class SaveController : MonoBehaviour {

    Thread displayDoneThread;
    bool displayDoneThreadRunning;
    bool displayDone;

    public void OnClick() {
        GameObject.Find("Core").GetComponent<Core>().OnSave();

        if (displayDoneThread != null){
            displayDoneThreadRunning = false;
            displayDoneThread.Join();
        }

        displayDone = true;
        displayDoneThread = new Thread(() => {
            int slept = 0;
            while (slept < 700) {
                if (displayDoneThreadRunning) {
                    Thread.Sleep(50);
                    slept += 50;
                } else {
                    break;
                }
            }
            displayDone = false;
        });
        displayDoneThreadRunning = true;
        displayDoneThread.Start();
    }

    void Update() {
        if (displayDone) {
            GetComponentInChildren<Text>().text = "Done :)";
        } else {
            GetComponentInChildren<Text>().text = "Save";
        }

        if (Input.GetMouseButtonDown(0) && GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject == gameObject) {
            GameObject.Find("Core").GetComponent<Core>().DisableCameraMovement();
            GameObject.Find("Core").GetComponent<Core>().EnablePointCloudRenderer();
        }

        if (Input.GetMouseButtonUp(0)) {
            GameObject.Find("Core").GetComponent<Core>().EnableCameraMovement();
            GameObject.Find("Core").GetComponent<Core>().DisablePointCloudRenderer();
        }
    }

    //public void OnGUI() {
    //    GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200f, 200f), "Hello World");
    //}
}