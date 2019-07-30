using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeafColor : MonoBehaviour {
    Dropdown dropdown;

    // Start is called before the first frame update
    void Start() {
        dropdown = GetComponent<Dropdown>();
    }

    public void Initialize(List<string> leafColors) {
        // colors are defined here, because you cannot define them in the Core,
        // .. because then you would need to initialize the color pickers after
        // .. the Core has been initialized
        //// and then you needed to tell the color pickers to put the colors to the UI, because that cannot be done via an extra thread?
        //colors = new List<string> { "dark_brown", "brown", "greyish" };
        foreach (string c in leafColors) {
            dropdown.options.Add(new Dropdown.OptionData(c));
        }
    }

    public void OnValueChanged() {
        GameObject.Find("Core").GetComponent<Core>().OnLeafColor(dropdown.value);
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}




////https://answers.unity.com/questions/1303416/how-to-add-color-to-dropdown-item-in-runtime.html
////Color color = new Color(0.2f, 0, 0.4f);
//Color color = Color.yellow;

//Texture2D texture = new Texture2D(1, 1); // creating texture with 1 pixel
//texture.SetPixel(0, 0, color); // setting to this pixel some color
//texture.Apply(); //applying texture. necessarily

//Dropdown.OptionData item = new Dropdown.OptionData();
//item.image = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)); // creating dropdown item and converting texture to sprite
////item.image = 
////Dropdown.OptionData item = new Dropdown.OptionData("weird item"); // creating dropdown item and converting texture to sprite
//dropdown.options.Add(item); // adding this item to dropdown options
