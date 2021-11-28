using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorageUnit : MonoBehaviour
    {

    public bool isStored;
    public int objectType;
    public bool isSelected;
    public int waitPoints;
    [SerializeField] Slider waitSlider;
    [SerializeField] GameObject fillColor;
    private Color waitColor;
    public GameObject moneySprite;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartWaiting(int secToWait)
    {
        waitSlider.gameObject.SetActive(true);
        waitPoints = secToWait;
        StartCoroutine("ObjectWaiting");     
        
    }
    IEnumerator ObjectWaiting()
    {
        
        while (waitPoints >=0)
        {
            yield return new WaitForSeconds(1f);
            waitPoints -= 1;
            
            if (waitSlider != null)
            {
                waitSlider.value = waitPoints;
                waitColor = new Color((1f- waitPoints / 100f), 0.3f, waitPoints / 100f, 0.7f);
                fillColor.GetComponent<Image>().color = waitColor;
            }
            

        }
        waitSlider.gameObject.SetActive(false);
    }


}
