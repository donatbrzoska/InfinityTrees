using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomize : MonoBehaviour
{
    //// Start is called before the first frame update
    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void onClick() {
        GameObject.Find("TreeMesh").GetComponent<TreeCreator>().OnRandomize();
    }
}
