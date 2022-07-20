using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class StorageUnit : MonoBehaviour
    {

    public bool isStored;
    public int objectType;
    public bool isSelected;
    public int waitPoints;
    public int nextWaitPoints;
    [SerializeField] public Slider waitSlider;
    [SerializeField] protected GameObject fillColor;
    protected Color waitColor;
    public GameObject moneySprite;
    public int loadNeed;
    protected int currentLoad;

    float boxSpawnTime = 1.7f;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SelfPuttingBox()
    {
        StartCoroutine(BoxDisapearCoroutine());
        
    }
    IEnumerator BoxDisapearCoroutine()
    {
        yield return new WaitForSeconds(boxSpawnTime); 
        if (gameObject.transform.GetChild(0) != null)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        gameObject.GetComponent<StorageUnit>().isStored = true;
        Debug.Log("Coroutine ends");

    }

}
