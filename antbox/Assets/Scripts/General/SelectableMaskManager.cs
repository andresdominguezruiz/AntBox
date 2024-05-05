using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum State{
    AVAILABLE,NOT_AVAILABLE,WITHOUT_SHOWING
}
public class SelectableMaskManager : MonoBehaviour
{
    private SpriteRenderer spriteRender;

    private FarmStats farmStats;
    public State actualState=State.WITHOUT_SHOWING;

    void Init(){
        spriteRender=this.gameObject.GetComponent<SpriteRenderer>();
        farmStats=this.gameObject.GetComponentInParent<FarmStats>(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(!actualState.Equals(State.WITHOUT_SHOWING)){
            UpdateState();
            UpdateSprite();
        }
    }

    public void HideRender(){
        actualState=State.WITHOUT_SHOWING;
        this.gameObject.SetActive(false);
    }

    public void ShowRender(){
        this.gameObject.SetActive(true);
        Init();
        UpdateState();
        UpdateSprite();

    }

    void UpdateState(){
        if(farmStats!=null){
            if(farmStats.CanAntWorkInHere()){
                actualState=State.AVAILABLE;
            }
            else{
                actualState=State.NOT_AVAILABLE;
            }
        }
    }

    void UpdateSprite(){
        if(actualState.Equals(State.AVAILABLE)){
            spriteRender.color=Color.green;
        }
        else{
            spriteRender.color=Color.red;
        }
    }
}
