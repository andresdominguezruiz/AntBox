using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItem : MonoBehaviour
{
    public bool isSelected=false;
    public bool canBeSelected=true;

    public GameObject moveMenu;

    public static List<SelectableItem> selectableItems=new List<SelectableItem>();
    public UIManager itemUI;
    void Start(){
        selectableItems.Add(this);
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

    
    
        
    
}
