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
        if(StatisticsOfGame.Instance!=null){
            StatisticsOfGame.Instance.ResetData();   
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }

    public void GoToInitialMenu(){
        if(StatisticsOfGame.Instance!=null){
            StatisticsOfGame.Instance.ResetData();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-2);
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
