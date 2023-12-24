using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ContainerData : MonoBehaviour
{
    public int FOOD_CONTAINER=20;
    public int WATER_CONTAINER=20;
    public int maxCards=10;
    public List<string> cards=new List<string>();
    public int foodValue=24;
    public int waterValue=24;

    public TextMeshProUGUI foodText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI cardText;
    public Tile dirtTile;
    public Tile stoneTile;
    public Tile diggingDirtTile1;
    public Tile diggingDirtTile2;
    public Tile diggingDirtTile3;
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
