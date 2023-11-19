using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class SelectableItem : MonoBehaviour
{
    public bool isSelected=false;
    public bool canBeSelected=true;

    public GameObject moveMenu;

    public static List<SelectableItem> selectableItems=new List<SelectableItem>();
    public static HashSet<Vector3> availablePath=new HashSet<Vector3>();

    private System.Random random = new System.Random();
    
    public UIManager itemUI;

    public void AddPath(List<Vector3Int> path,Tilemap destructablePath){
        foreach(Vector3Int localPosition in path){
            Vector3 worldPosition=destructablePath.CellToWorld(localPosition);
            availablePath.Add(worldPosition);
        }
    }
    void Start(){
        selectableItems.Add(this);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Ant") || collision.gameObject.CompareTag("Queen"))
        {
            // Ignora la colisión
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
    }

    public void MakeEveryoneUnselectable(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=false;
        }
    }

    public void MakeEveryoneSelectable(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=true;
        }
    }

    void Update(){
        if(isSelected && canBeSelected){
            itemUI.ShowInfo();
        }else{
            if(itemUI!=null){
                itemUI.HideInfo();
            }
        }

    }

    void OnMouseDown() {
        if(canBeSelected){
            isSelected=true;
            if(!itemUI.isQueen){
                MoveMenu menu=moveMenu.GetComponent<MoveMenu>();
                menu.SetSelectedAnt(this.gameObject);
            }
            foreach(SelectableItem item in selectableItems){
                if(item!=this){
                    item.isSelected=false;
                } 
        }
        }
    }


    public void SetUIManager(UIManager ui){
        itemUI=ui;
    }

    public UIManager GetUIManager(){
        return itemUI;
    }

    
    
        
    
}
