using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public float timeLastFrame;

    public int day=0;


    public static int growingTime=24; //Cada t tiempo real, se considera un día

    private int counterOfSecons=0;

    public TextMeshProUGUI text;

    public int GetCounterOfSecons(){
        return counterOfSecons;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time -timeLastFrame>=1.0f){
            counterOfSecons++;
            if(counterOfSecons==growingTime){
                day++;
                UpdateTimer();
                counterOfSecons=0;
            }
            timeLastFrame=Time.time;
        }
    }

    void UpdateTimer(){
        text.text="Day "+day;
    }
}
