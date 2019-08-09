using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetToDefaults : MonoBehaviour
{
    public void OnClick() {
        GameObject.Find("Core").GetComponent<Core>().OnResetToDefaults();
    }
}
