using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum EventType{
    NOTHING,WINTER,SUMMER,EXAM,HORDE
}
public class Clock : MonoBehaviour
{
    public float timeLastFrame;
    private NestManager nestManager;
    public EventType eventType=EventType.NOTHING;
    private System.Random random = new System.Random();

    public int day=0;
    public int daysForNextEvent=1;
    public List<EventType> processedEvents=new List<EventType>();

    [SerializeField] private Image clock;
    [SerializeField] private Sprite[] states=new Sprite[24];


    readonly static int growingTime=24; //Cada t tiempo real, se considera un día

    private int counterOfSecons=0;

    public TextMeshProUGUI dayCounterText;
    public TextMeshProUGUI eventTypeText;
    public TextMeshProUGUI consoleText;
    public string messageOfEvent="Waiting...";

    public int GetCounterOfSecons(){
        return counterOfSecons;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        eventTypeText.text=eventType.ToString();
        UpdateTimer();
        nestManager=FindObjectOfType<NestManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time -timeLastFrame>=(1.0f+Player.Instance.GetTimeValue() - StatisticsOfGame.Instance.timeSpeed*0.1f)){
            counterOfSecons++;
            if(counterOfSecons==growingTime){
                day++;
                StatisticsOfGame.Instance.counterOfDays++;
                if(day%daysForNextEvent==0){
                    FinishPastEventAndIniciateNewEvent();
                    eventTypeText.text=eventType.ToString();
                }
                UpdateTimer();
                counterOfSecons=0;
            }
            clock.sprite=states[counterOfSecons];
            timeLastFrame=Time.time;
        }
    }
    void FinishPastEventAndIniciateNewEvent(){
        if(eventType.Equals(EventType.NOTHING)){
            double v=random.NextDouble();
            if((v<=0.33 && !processedEvents.Contains(EventType.WINTER))
             || (processedEvents.Contains(EventType.SUMMER) 
             && !processedEvents.Contains(EventType.WINTER) && processedEvents.Contains(EventType.HORDE))){
                ProcessWinter();
            }else if((v>0.33 && v<=0.66 && !processedEvents.Contains(EventType.SUMMER)) 
            || (processedEvents.Contains(EventType.WINTER)
             && !processedEvents.Contains(EventType.SUMMER) && processedEvents.Contains(EventType.HORDE))){
                ProcessSummer();
            }else if((v>0.66 && !processedEvents.Contains(EventType.HORDE)) ||
            (processedEvents.Contains(EventType.SUMMER) 
             && processedEvents.Contains(EventType.WINTER) && !processedEvents.Contains(EventType.HORDE))){
                ProcessHorde();
            }
            else{
                processedEvents=new List<EventType>();
                ProcessExam();
            }
        }else{
            GoBackToNothingEvent();
        }
        UpdateMessageOfConsoleByEvent();
    }

    public void ProcessHorde(){
        eventType=EventType.HORDE;
        processedEvents.Add(eventType);
        dayCounterText.color=Color.magenta;
        eventTypeText.color=Color.magenta;
        consoleText.color=Color.magenta;
        nestManager.SpawnEnemiesForHorde();
    }

    public void UpdateMessageOfConsoleByEvent(){
        if(eventType.Equals(EventType.NOTHING)){
            messageOfEvent="Waiting...";
        }else if(eventType.Equals(EventType.SUMMER)){
            messageOfEvent="Energy is consumed faster";
        }else if(eventType.Equals(EventType.WINTER)){
            messageOfEvent="Thirst and hunger are consumed faster";
        }
        else if(eventType.Equals(EventType.EXAM)){
            messageOfEvent="Prepare for the exam";
        }else if(eventType.Equals(EventType.HORDE)){
            messageOfEvent="A horde of enemies is coming!!";
        }
        DigMenu digMenu=FindObjectOfType<DigMenu>(false);
        FarmingMenu farmingMenu=FindObjectOfType<FarmingMenu>(false);
        MoveMenu moveMenu=FindObjectOfType<MoveMenu>(false);
        if(digMenu==null && farmingMenu==null && moveMenu==null){
            consoleText.text=messageOfEvent;
        }
    }

    public void GoBackToNothingEvent()
    {
        eventType=EventType.NOTHING;
        dayCounterText.color=new Color(0,255,0);
        eventTypeText.color=new Color(0,255,0);
        consoleText.color=new Color(0,255,0);

        
    }

    private void ProcessExam()
    {
        eventType=EventType.EXAM;
        dayCounterText.color=Color.red;
        eventTypeText.color=Color.red;
        consoleText.color=Color.red;
        ActivityMenu activityMenu=FindObjectOfType<ActivityMenu>(true);
        CardDisplay cardDisplay=FindObjectOfType<CardDisplay>(true);
        cardDisplay.MakeEveryCardUnselectableAndUnselected();
        SelectableItem item=FindObjectOfType<SelectableItem>(false);
        item.MakeEveryoneUnselectableAndUnselected();
        activityMenu.SetActivitiesAndStartPlaying(cardDisplay.GenerateActivitiesByComplexity(true),true,false);
    }

    private void ProcessSummer()
    {
        eventType=EventType.SUMMER;
        processedEvents.Add(eventType);
        dayCounterText.color=Color.yellow;
        eventTypeText.color=Color.yellow;
        consoleText.color=Color.yellow;
        
    }

    private void ProcessWinter()
    {
        eventType=EventType.WINTER;
        processedEvents.Add(eventType);
        dayCounterText.color=new Color(0,255,255);
        eventTypeText.color=new Color(0,255,255);
        consoleText.color=new Color(0,255,255);
        
    }

    void UpdateTimer(){
        dayCounterText.text="Day "+day;
    }
}
