using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{
    public void OnClick() {
        Debug.Log("UI: Button: Save");

        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnSave();
    }
}