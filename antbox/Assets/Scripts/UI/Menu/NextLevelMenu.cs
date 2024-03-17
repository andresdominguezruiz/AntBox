using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelMenu : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI cards;
    public float timeLastFrame;
    private int counterOfSecons=0;
    public int seconsToIniciateNextLevel=2;
    void Start()
    {
        title.text="Level "+StatisticsOfGame.Instance.actualLevel;
        cards.text="X "+Player.Instance.cardsInHand.Count;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time -timeLastFrame>=1f){
            counterOfSecons++;
            if(counterOfSecons==seconsToIniciateNextLevel){
                counterOfSecons=0;
                IniciateNextLevel();
            }
            timeLastFrame=Time.time;
        }
        
    }
    void IniciateNextLevel(){
        LevelLoader.Instance.StartNewLevel(SceneManager.GetActiveScene().buildIndex-2);
    }
}
