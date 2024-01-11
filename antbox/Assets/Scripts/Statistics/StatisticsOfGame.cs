using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsOfGame : MonoBehaviour
{
    public static StatisticsOfGame Instance;
    public int counterOfExams=0;
    public int counterOfPassedExams=0;
    public int counterOfFailedExams=0;
    public int counterOfUsedCards=0;
    public int counterOfCorrectCards=0;
    public int counterOfFailedCards=0;
    public int counterOfDays=0;

    private void Awake(){
        if(StatisticsOfGame.Instance==null){
            StatisticsOfGame.Instance=this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(gameObject);
        }

    }
    public void ResetData(){
        counterOfExams=0;
        counterOfPassedExams=0;
        counterOfFailedExams=0;
        counterOfUsedCards=0;
        counterOfCorrectCards=0;
        counterOfFailedCards=0;
        counterOfDays=0;
    }
}
