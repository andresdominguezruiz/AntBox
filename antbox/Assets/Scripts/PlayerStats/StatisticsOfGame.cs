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

    private static void SetInstance(StatisticsOfGame game){
        StatisticsOfGame.Instance=game;
    }

    private void Awake(){
        if(StatisticsOfGame.Instance==null){
            SetInstance(this);
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
        LevelLoader.Instance.StartNewLevel(SceneManager.GetActiveScene().buildIndex+2);
        Player.Instance.helpCounter=0;
    }
    public void DestroyItems(){
        SelectableItem[] items=FindObjectsOfType<SelectableItem>(false);
        foreach(SelectableItem item in items){
            item.RemoveSelectableItem();
            Destroy(item.gameObject);
        }
    }
    public OptimalNest GetOptimalNestByLevelAndRandomValue(double randomValue){
        NestType optimalType=NestType.RANDOM;
        int number=1;
        int level=1;
        //Pick max enemy level
        if(actualLevel>=0 && actualLevel<4){
            level=2;
        }
        else if(actualLevel>=4 && actualLevel<8){
            level=4;
        }
        else if(actualLevel>=8){
            level=10;
        }

        //Pick type and number
        if((actualLevel>=0 && actualLevel<2 && randomValue<0.5) ||
         (actualLevel>=2 && actualLevel<6 && randomValue<0.33)){
            number=3;
            optimalType=NestType.WORMS;
         }
        else if((actualLevel>=0 && actualLevel<2 && randomValue>=0.5) ||
         (actualLevel>=2 && actualLevel<6 && randomValue>=0.33 && randomValue<0.66)){
            number=2;
            optimalType=NestType.ANTS;
         }
        else if(
         actualLevel>=2 && actualLevel<6 && randomValue>=0.66 && randomValue<0.9){
            number=1;
            optimalType=NestType.EARTHWORMS;
         }
        else if((actualLevel>=2 && actualLevel<6 && randomValue>=0.0) || actualLevel>=6){
            number=3;
            optimalType=NestType.RANDOM;
         }

        OptimalNest optimal=new OptimalNest(number,optimalType,level);
        return optimal;
    }
    public void ResetData(){
        NestManager nestManager=FindObjectOfType<NestManager>();
        if(nestManager!=null){
            nestManager.ResetEnemies();
        }
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
