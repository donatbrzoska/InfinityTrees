using System;
using UnityEngine;

public class Middleware {

    public void ShowPointCloud() {
        GameObject.Find("PointCloud").GetComponent<PointCloud>().Enabled = true;
    }

    public void HidePointCloud() {
        GameObject.Find("PointCloud").GetComponent<PointCloud>().Enabled = false;
    }

    public void DisableCameraMovement() {
        GameObject.Find("Main Camera").GetComponent<CameraMovement>().Enabled = false;
    }

    public void EnableCameraMovement() {
        GameObject.Find("Main Camera").GetComponent<CameraMovement>().Enabled = true;
    }
}
