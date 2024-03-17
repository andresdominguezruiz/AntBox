using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class HelpButton : MonoBehaviour
{
    public float timeLastFrame;
    public TextMeshProUGUI buttonText;
    public Image buttonImage;
    public bool canHelp=true;
    void Start()
    {
        timeLastFrame=0;
        VerifyHelper();
    }
    void VerifyHelper(){
        int helpCounter=Player.Instance.helpCounter;
        string message;
        if(helpCounter>0){
            message="__"+helpCounter+"__";
            canHelp=false;
            buttonImage.color=Color.black;
        }else{
            canHelp=true;
            message="Help!!";
            buttonImage.color=Color.white;
        }
        buttonText.text=message;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time-timeLastFrame>=1f && !canHelp){
            Player.Instance.helpCounter--;
            VerifyHelper();
            timeLastFrame=Time.time;
        }
    }

    public void StartHelper(){
        if(canHelp){
            Player.Instance.helpCounter=15;
            VerifyHelper();
            ActivityMenu activityMenu=FindObjectOfType<ActivityMenu>(true);
            CardDisplay cardDisplay=FindObjectOfType<CardDisplay>(true);
            activityMenu.SetActivitiesAndStartPlaying(cardDisplay.GenerateActivitiesByComplexity(false),false,true);
        }
    }
}
