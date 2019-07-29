using System;
using UnityEngine;

public class Middleware {

    public void EnablePointCloudRenderer() {
        GameObject.Find("PointCloudRenderer").GetComponent<PointCloudRenderer>().Enabled = true;
    }

    public void DisablePointCloudRenderer() {
        GameObject.Find("PointCloudRenderer").GetComponent<PointCloudRenderer>().Enabled = false;
    }

    public void DisableCameraMovement() {
        GameObject.Find("Main Camera").GetComponent<CameraMovement>().Enabled = false;
    }

    public void EnableCameraMovement() {
        GameObject.Find("Main Camera").GetComponent<CameraMovement>().Enabled = true;
    }
}
