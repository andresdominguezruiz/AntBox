using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameMenu : MonoBehaviour
{
    [SerializeField] private GameObject playAgainButton;
    [SerializeField] private GameObject goToInitialMenuButton;
    [SerializeField] private TextMeshProUGUI days;
    [SerializeField] private TextMeshProUGUI cards;
    [SerializeField] private TextMeshProUGUI correctCards;
    [SerializeField] private TextMeshProUGUI failedCards;
    [SerializeField] private TextMeshProUGUI exams;
    [SerializeField] private TextMeshProUGUI passedExams;
    [SerializeField] private TextMeshProUGUI failedExams;


    public void NewGame(){
        LevelLoader.Instance.StartNewLevel(SceneManager.GetActiveScene().buildIndex-1);
        PauseMenu.isPaused=false;
        if(StatisticsOfGame.Instance!=null){
            Debug.Log("hoaaaaaa");
            StatisticsOfGame.Instance.ResetData();   
        }
    }

    public void GoToInitialMenu(){
        if(StatisticsOfGame.Instance!=null){
            StatisticsOfGame.Instance.ResetData();
        }
        LevelLoader.Instance.StartNewLevel(SceneManager.GetActiveScene().buildIndex-2);
    }

    void Start(){
        if(StatisticsOfGame.Instance!=null){
            days.text="You survive "+StatisticsOfGame.Instance.counterOfDays+" days, well done";
            cards.text="Total used cards: "+StatisticsOfGame.Instance.counterOfUsedCards;
            correctCards.text="Correct cards: "+StatisticsOfGame.Instance.counterOfCorrectCards;
            failedCards.text="Failed cards: "+StatisticsOfGame.Instance.counterOfFailedCards;
            exams.text="Total exams made: "+StatisticsOfGame.Instance.counterOfExams;
            passedExams.text="Passed exams: "+StatisticsOfGame.Instance.counterOfPassedExams;
            failedExams.text="Failed exams: "+StatisticsOfGame.Instance.counterOfFailedExams;
        }

    }

}
