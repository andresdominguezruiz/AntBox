using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public int actualLevel=0;
    public int colorIndex=0;
    public int timeSpeed=0;

    private void Awake(){
        if(StatisticsOfGame.Instance==null){
            StatisticsOfGame.Instance=this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }

    public void NextLevel(){
        actualLevel++;
        colorIndex++;
        GenerationTilemap generationTilemap=FindObjectOfType<GenerationTilemap>();
        if(generationTilemap!=null && colorIndex>=generationTilemap.colorsForDirtMap.Count){
            colorIndex=0;
            if(timeSpeed<5){
                timeSpeed++;
            }
        }
        ContainerData containerData=FindObjectOfType<ContainerData>();
        if(containerData!=null){
            Player.Instance.SaveCards(containerData.cardsInHand);
        }
        DestroyItems();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+2);
    }
    public void DestroyItems(){
        SelectableItem[] items=FindObjectsOfType<SelectableItem>(false);
        foreach(SelectableItem item in items){
            item.RemoveSelectableItem();
            Destroy(item.gameObject);
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
        actualLevel=0;
        colorIndex=0;
        timeSpeed=0;
        DestroyItems();
        Player.Instance.ResetPlayerData();
    }
}
