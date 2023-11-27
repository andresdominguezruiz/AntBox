using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContainerData : MonoBehaviour
{
    public int FOOD_CONTAINER=20;
    public int WATER_CONTAINER=20;
    public int foodValue=24;
    public int waterValue=24;

    public TextMeshProUGUI foodText;
    public TextMeshProUGUI waterText;
    // Start is called before the first frame update
    void Start()
    {
        foodText.text="F:"+FOOD_CONTAINER;
        waterText.text="W:"+WATER_CONTAINER;
        
    }

    // Update is called once per frame
    void Update()
    {
        foodText.text="F:"+FOOD_CONTAINER;
        waterText.text="W:"+WATER_CONTAINER;
    }

    public void AddResources(int value,Type type){
        Debug.Log("añadidio");
        if(type.Equals(Type.FOOD)) FOOD_CONTAINER+=value;
        else WATER_CONTAINER+=value;
    }
}
