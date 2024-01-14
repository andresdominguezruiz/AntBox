using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public HashSet<UpdateEffectOnPlayer> playerPassives=new HashSet<UpdateEffectOnPlayer>();
    public double complexityLevelOfGame=1.0;
    public List<Activity> knownActivities=new List<Activity>();


    private void Awake(){
        if(Player.Instance==null){
            Player.Instance=this;
            DontDestroyOnLoad(this.gameObject);
        }else{
            Destroy(this.gameObject);
        }
    }
    public void ResetPlayerData(){
        playerPassives=new HashSet<UpdateEffectOnPlayer>();
        complexityLevelOfGame=1.0;
        ForgetActivities();
    }
    public void ForgetActivities(){
        knownActivities=new List<Activity>();
    }

    public bool AllowNegativeAge(){
        return playerPassives.Contains(UpdateEffectOnPlayer.ALLOW_NEGATIVE_AGE);
    }
    public float GetTimeValue(){
        return playerPassives.Contains(UpdateEffectOnPlayer.MAKE_TIME_SLOWER)?2f:0.0f;
    }

    public void ProcessUpdateEffectOfAction(Action actualAction){
        playerPassives.Add(actualAction.playerEffect);
    }
}
