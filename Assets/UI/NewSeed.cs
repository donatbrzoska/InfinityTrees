using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSeed : MonoBehaviour
{
    public void OnClick() {
        Debug.Log("UI: Button: NewSeed");

		GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnNewSeed();
    }
}
