using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class Save : MonoBehaviour {

    Thread displayDoneThread;
    bool displayDoneThreadRunning;
    bool displayDone;

    public void OnClick() {
        //Debug.Log("UI: Button: Save");

        GameObject.Find("Core").GetComponent<Core>().OnSave();

        if (displayDoneThread != null){
            displayDoneThreadRunning = false;
            displayDoneThread.Join();
        }

        displayDone = true;
        displayDoneThread = new Thread(() => {
            int slept = 0;
            while (slept < 600) {
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
    }

    //public void OnGUI() {
    //    GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200f, 200f), "Hello World");
    //}
}