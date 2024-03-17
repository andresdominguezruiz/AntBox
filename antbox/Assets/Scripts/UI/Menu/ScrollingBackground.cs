using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class ScrollingBackground : MonoBehaviour
{
    public float speed=0.1f;
    private Image background;
    public bool inSettings=false;

    // Update is called once per frame
    void Start(){
        background=this.GetComponent<Image>();
    }
    void Update()
    {
        if(!inSettings){
            background.material.mainTextureOffset+=new Vector2(speed*Time.deltaTime,0);
        }
        else{
            background.material.mainTextureOffset-=new Vector2(speed*Time.deltaTime,0);
        }
    }

    public void UpdateState(){
        inSettings=!inSettings;
    }
}
