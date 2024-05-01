using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public HashSet<UpdateEffectOnPlayer> playerPassives=new HashSet<UpdateEffectOnPlayer>();
    public int helpCounter=0;
    public double complexityLevelOfGame=1.0;
    public List<Activity> knownActivities=new List<Activity>();
    public List<Card> cardsInHand=new List<Card>();
    [SerializeField] private float minimunComplexity=0.5f;

    private static void SetInstance(Player player){
        Player.Instance=player;
    }


    private void Awake(){
        if(Player.Instance==null){
            SetInstance(this);
            DontDestroyOnLoad(this.gameObject);
        }else{
            Destroy(this.gameObject);
        }
    }

    public void SaveCards(List<CardDisplay> containerCards){
        foreach(CardDisplay cardDisplay in containerCards){
            cardsInHand.Add(cardDisplay.card);
        }
    }

    public void GiveCardsToContainer(){
        ContainerData containerData=FindObjectOfType<ContainerData>();
        if(containerData!=null){
            foreach(Card card in cardsInHand){
                containerData.AddCard(card);
            }
        }
        cardsInHand=new List<Card>();
    }
    public void ResetPlayerData(){
        playerPassives=new HashSet<UpdateEffectOnPlayer>();
        complexityLevelOfGame=1.0;
        helpCounter=0;
        ForgetActivities();
        cardsInHand=new List<Card>();
    }

    public void ForgetActivities(){
        knownActivities=new List<Activity>();
    }
    public bool CanAnthillDieByOldAge(){
        return playerPassives.Contains(UpdateEffectOnPlayer.DISABLE_DEAD_BY_AGE);
    }

    public bool AllowNegativeAge(){
        return playerPassives.Contains(UpdateEffectOnPlayer.ALLOW_NEGATIVE_AGE);
    }
    public float GetTimeValue(){
        return playerPassives.Contains(UpdateEffectOnPlayer.MAKE_TIME_SLOWER)?2f:0.0f;
    }

    public void AddComplexity(float value){
        if(complexityLevelOfGame+value>=minimunComplexity){
            complexityLevelOfGame+=value;
        }
    }

    public void ProcessUpdateEffectOfAction(Action actualAction){
        playerPassives.Add(actualAction.PlayerEffect);
    }
}
