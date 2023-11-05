using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItem : MonoBehaviour
{
    public bool isSelected=false;

    public static List<SelectableItem> selectableItems=new List<SelectableItem>();
    public UIManager itemUI;
    void Start(){
        selectableItems.Add(this);
    }

    void Update(){
        if(isSelected){
            itemUI.ShowInfo();
        }else{
            if(itemUI!=null){
                itemUI.HideInfo();
            }
        }
    }

    void OnMouseDown() {
        isSelected=true;
        foreach(SelectableItem item in selectableItems){
            if(item!=this) item.isSelected=false;
        }
    }


    public void SetUIManager(UIManager ui){
        itemUI=ui;
    }

    
    
        
    
}
