using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ExcavationTest : MonoBehaviour
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    public Tilemap tilemap;


    void Awake()
    {
        tilemap.RefreshAllTiles();
    }
    void Start(){
        
    }

    void Update(){
        OnMouseEnter();

    }

    private void OnMouseEnter() {
        Vector3 mousePosition=Input.mousePosition;
        if(tilemap.WorldToCell(mousePosition)!=null){
            Debug.Log(tilemap.GetTile(tilemap.WorldToCell(mousePosition)));
           tilemap.SetTile(tilemap.WorldToCell(mousePosition),null);
           Debug.Log(tilemap.GetTile(tilemap.WorldToCell(mousePosition)));
        } 
    }
}
