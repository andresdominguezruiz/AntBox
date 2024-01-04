using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;


public enum ItemType{
    ANT,QUEEN,FARM
}
public class SelectableItem : MonoBehaviour
{
    public bool isSelected=false;
    public bool canBeSelected=true;

    public GameObject moveMenu;
    public GameObject farmMenu;
    public GameObject digMenu;

    public static List<SelectableItem> selectableItems=new List<SelectableItem>();
    public static HashSet<Vector3> availablePath=new HashSet<Vector3>();

    private System.Random random = new System.Random();
    
    public UIManager itemUI;

    public UIFarmManager farmUI;

    private ItemType type;

    public List<FarmStats> GetAllFarms(){
        List<FarmStats> list=new List<FarmStats>();
        foreach(SelectableItem item in selectableItems){
            if(item.type.Equals(ItemType.FARM)) list.Add(item.gameObject.GetComponent<FarmStats>());
        }
        return list;
    }

    

    public void AddPath(List<Vector3Int> path,Tilemap destructablePath){
        foreach(Vector3Int localPosition in path){
            Vector3 worldPosition=destructablePath.CellToWorld(localPosition);
            availablePath.Add(worldPosition);
        }
    }
    //USAR ESTE METODO CUANDO VAYAS A AÑADIR UN NUEVO ELEMENTO SELECCIONABLE
    public void InitSelectableItem(List<Vector3Int> path,Tilemap destructableMap
    ,GameObject moveMenu,GameObject farmMenu,GameObject digMenu,ItemType itemType){
        AddPath(path,destructableMap);
        if(itemType.Equals(ItemType.FARM)){
            SetUIFarmManager(this.gameObject.GetComponentInChildren<UIFarmManager>(true));
            farmUI.HideInfo();
            type=ItemType.FARM;


        }
        else if(itemType.Equals(ItemType.ANT)){
            SetUIManager(this.gameObject.GetComponentInChildren<UIManager>(true));
            itemUI.HideInfo();
            this.moveMenu=moveMenu;
            this.farmMenu=farmMenu;
            this.digMenu=digMenu;
            type=ItemType.ANT;
        }
        else{
            SetUIManager(this.gameObject.GetComponentInChildren<UIManager>(true));
            itemUI.HideInfo();
            type=ItemType.QUEEN;

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
    public void MakeEveryoneUnselectableButPrepareFarms(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=false;
            if(selectableItems[i].gameObject.GetComponent<FarmStats>()!=null){
                FarmStats stats=selectableItems[i].gameObject.GetComponent<FarmStats>();

            }
        }
    }

    public void MakeEveryoneUnselectable(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=false;
        }
    }
    public void MakeEveryoneUnselectableAndUnselected(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=false;
            if(selectableItems[i].isSelected)selectableItems[i].isSelected=false;
        }
    }

    public void HideAllInfo(){
        for(int i=0;i<selectableItems.Count;i++){
            if(selectableItems[i].gameObject.GetComponentInChildren<UIManager>()!=null){
                selectableItems[i].gameObject.GetComponentInChildren<UIManager>().HideInfo();
            }else if(selectableItems[i].gameObject.GetComponentInChildren<UIFarmManager>()!=null){
                selectableItems[i].gameObject.GetComponentInChildren<UIFarmManager>().HideInfo();
            }
        }
    }

    public void MakeEveryoneSelectable(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=true;
        }
    }

    void Update(){
        if(isSelected && canBeSelected){
            if(itemUI!=null){
                itemUI.ShowInfo();
            }else if(farmUI!=null){
                farmUI.ShowInfo();
            }
        }else{
            if(itemUI!=null){
                itemUI.HideInfo();
            }else if(farmUI!=null){
                farmUI.HideInfo();
            }
        }

    }

    void OnMouseDown() {
        if(canBeSelected){
            isSelected=true;
            if(itemUI!=null && !itemUI.isQueen){
                MoveMenu menu=moveMenu.GetComponent<MoveMenu>();
                menu.SetSelectedAnt(this.gameObject);
                FarmingMenu otherMenu=farmMenu.GetComponent<FarmingMenu>();
                otherMenu.SetSelectedAnt(this.gameObject);
                DigMenu dig=digMenu.GetComponent<DigMenu>();
                dig.SetSelectedAnt(this.gameObject);
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

    public void SetUIFarmManager(UIFarmManager ui){
        farmUI=ui;
    }



    public UIManager GetUIManager(){
        return itemUI;
    }

    
    
        
    
}
