using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSeed : MonoBehaviour {
    //Middleware middleware;

    //public NewSeed() {
    //    middleware = new Middleware();
    //}

    public void OnClick() {
        //Debug.Log("UI: Button: NewSeed");

		GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnNewSeed();
    }
}
