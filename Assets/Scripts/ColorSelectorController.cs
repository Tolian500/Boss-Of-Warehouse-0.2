using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ColorSelectorController : MonoBehaviour
{
    [SerializeField] Material hairColor;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }
       
           
    


    public void ColorChange()
    {
        //hairColor.color = new Color(gameObject.GetComponent<Slider>().value, 1, 1, 1);
        hairColor.color = Color.HSVToRGB(gameObject.GetComponent<Slider>().value, gameObject.GetComponent<Slider>().value, gameObject.GetComponent<Slider>().value);


    }
   
}
