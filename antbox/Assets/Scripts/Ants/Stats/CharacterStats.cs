using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public enum AgeType{
    YOUNG,ADULT,ELDER
}

public class CharacterStats : MonoBehaviour
{
    private System.Random random = new System.Random();

    public System.Random GetRandom(){
        return random;
    }
    public float timeLastFrame;


    [SerializeField] private static int growingTime=24; //Cada t tiempo real, se considera un día

    private int counterOfSecons=0;

    public AllBarsManager allBarsManager;

    //LIMITS FOR VARIABLES----------------------
    [SerializeField] protected int MIN_HP=40;
    [SerializeField] protected int MAX_HP=60;
    [SerializeField] protected int HP_PER_AGE=5;

    [SerializeField] protected int MIN_HUNGER=50;
    [SerializeField] protected int MAX_HUNGER=100;
    [SerializeField] protected int HUNGER_PER_AGE=5;

    [SerializeField] protected int MIN_THIRST=50;

    [SerializeField] protected int MAX_THIRST=100;
    [SerializeField] protected int THIRST_PER_AGE=5;


    //----------------------------
    [SerializeField] private int maxHP;
    [SerializeField] private int actualHP;

    [SerializeField] private int maxHunger;
    [SerializeField] private int actualHunger;

    [SerializeField] private int maxThirst;
    [SerializeField] private int actualThirst;

    [SerializeField] private int age;

    [SerializeField] private bool isDead=false;
    [SerializeField] private int adultAge=4;
    [SerializeField] private int elderAge=8;
    public int poisonSecons=0;
    public bool unpoisonable=false;
    private Clock clockOfGame;

    public Clock GetClockOfGame(){
        return clockOfGame;
    }

    public void SetClockOfGame(Clock clock){
        clockOfGame=clock;
    }

    public bool IsDead(){
        return isDead;
    }

    
    void Update(){
        if(Time.time -timeLastFrame>=(1.0f+Player.Instance.GetTimeValue()-StatisticsOfGame.Instance.timeSpeed*0.1f)){
            counterOfSecons++;
            if(counterOfSecons==growingTime){
                age++;
                UpdateStatsPerAge();
                counterOfSecons=0;
            }
            UpdateStats();
            timeLastFrame=Time.time;
        }
    }

    void UpdateStatsPerAge(){
        if(age<=adultAge){
            maxHP+=HP_PER_AGE;
            maxHunger+=HUNGER_PER_AGE;
            maxThirst+=THIRST_PER_AGE;
        }else if(age>=elderAge && Player.Instance.CanAnthillDieByOldAge()){
            maxHP-=HP_PER_AGE;
            maxHunger-=HUNGER_PER_AGE/2;
            maxThirst-=THIRST_PER_AGE/2;
        }
    }

    void UpdateStats(){
        AntStats antStats=this.gameObject.GetComponent<AntStats>();
        if(isDead){
            SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
            item.isSelected=false;
            FarmStats[] farms=FindObjectsOfType<FarmStats>();
            foreach(FarmStats farm in farms){
                if(farm.antsOfFarm.Contains(this.gameObject)){
                    farm.antsOfFarm.Remove(this.gameObject);
                    farm.antsWorkingInFarm.Remove(this.gameObject);
                }
            }
            item.RemoveSelectableItem();
            IsEndOfGame();
            Destroy(this.gameObject);
        }else{
            bool needToCheckHP=false;
            if(actualHunger>0 && actualThirst>0){
                if(clockOfGame!=null && !clockOfGame.eventType.Equals(EventType.WINTER)){
                    actualHunger--;
                    actualThirst--;
                }else{
                    actualHunger-=3;
                    actualThirst-=3;
                }
                Heal(1);
            }else if(actualHunger>0){
                if(clockOfGame!=null && !clockOfGame.eventType.Equals(EventType.WINTER)) actualHunger--;
                else actualHunger-=3;
                actualHP--;
                needToCheckHP=true;
            }else if(actualThirst>0){
                if(clockOfGame!=null && !clockOfGame.eventType.Equals(EventType.WINTER)) actualThirst--;
                else actualThirst-=3;
                actualHP--;
                needToCheckHP=true;
            }else{
                actualHP-=2;
                needToCheckHP=true;
            }
            if(poisonSecons>0){
                poisonSecons--;
                if(!unpoisonable){
                    Heal(-actualHP*5/100); //EL VENENO LE QUITARÁ 5% DE SU VIDA
                    needToCheckHP=true;
                }
                else{
                    Heal(actualHP*5/100);
                }
            }

            if(antStats!=null){
                if(antStats.GetAction().Equals(ActualAction.SLEEPING)){
                    antStats.ApplyEnergyCost(-1*antStats.GetRecoverSpeed());
                    if(antStats.IsFullOfEnergy()) antStats.CancelAntAction();
                }
            }
            if(actualThirst<0) actualThirst=0;
            if(actualHunger<0) actualHunger=0;

            if(needToCheckHP) CheckHP();
        }
    }
    public void EatWithoutCost(int foodValue){
        if(maxHunger<actualHunger+foodValue) SetActualHunger(maxHunger);
        else SetActualHunger(actualHunger+foodValue);
    }
    public void DrinkWithoutCost(int waterValue){
        if(maxThirst<actualThirst+waterValue) SetActualThirst(maxThirst);
        else SetActualThirst(actualThirst+waterValue);
    }

    public void Eat(ContainerData container){
        if(container.FOOD_CONTAINER>0){
            container.FOOD_CONTAINER--;
            if(maxHunger<actualHunger+container.foodValue) SetActualHunger(maxHunger);
            else SetActualHunger(actualHunger+container.foodValue);
        }else{
        }
    }
    public void Drink(ContainerData container){
        if(container.WATER_CONTAINER>0){
            container.WATER_CONTAINER--;
            if(maxThirst<actualThirst+container.waterValue) SetActualThirst(maxThirst);
            else SetActualThirst(actualThirst+container.waterValue);
        }
    }

    public void IsEndOfGame(){
        QueenStats queenStats=this.gameObject.GetComponent<QueenStats>();
        AntStats[] allAnts=FindObjectsOfType<AntStats>(false);
        if((queenStats!=null && queenStats.isDead==true) || (allAnts.Length==1 && allAnts[0].isDead==true)){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }




    public void CheckHP(){
        if(actualHP<=0 || maxHP==0){
            actualHP=0;
            Die();
        }else if(actualHP>=maxHP){
            actualHP=maxHP;
            allBarsManager.healthBar.SetMaxBarValue(maxHP);
        }
        allBarsManager.healthBar.SetBarValue(actualHP);
    }

    public void Die(){
        isDead=true;
    }

    public void TakeDamage(int damage){
        SetActualHP(actualHP-damage);
        allBarsManager.healthBar.SetBarValue(actualHP);
    }
    public void IncrementHP(int extraHP){
        maxHP+=extraHP;
        Heal(maxHP);
    }

    public void Heal(int extraHp){
        if(extraHp+actualHP>maxHP) SetActualHP(maxHP);
        else SetActualHP(extraHp+actualHP);
        allBarsManager.healthBar.SetBarValue(actualHP);
    }

    public int GetMaxHP(){
        return maxHP;
    }

    public void SetActualHP(int hp){
        actualHP=hp;
    }

    public void SetActualHunger(int hunger){
        if(hunger<0){
            actualHunger=0;
        }else{
            actualHunger=hunger;
        }
    }

    public void SetActualThirst(int thirst){
        if(thirst<0){
            actualThirst=0;
        }else{
            actualThirst=thirst;
        }
    }

    public String GetTextHP(){
        String text=maxHP.ToString()+'/'+actualHP.ToString();
        return text;
    }
    public string GetTextHunger(){
        String text=maxHunger.ToString()+'/'+actualHunger.ToString();
        return text;
    }
    public string GetTextThirst(){
        String text=maxThirst.ToString()+'/'+actualThirst.ToString();
        return text;
    }

    public string GetTextAge(){
        return age.ToString();
    }

    public void ProcessUpdateEffectOfAction(List<CharacterEffect> effects){
        foreach(CharacterEffect effect in effects){
            ApplyEffect(effect);
        }
    }

    public void ApplyEffect(CharacterEffect effect){
        AntStats stats=this.gameObject.GetComponent<AntStats>();
        if(!effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.NONE)){
            if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.HP_LIMIT)) {
                maxHP=effect.multiplicatorValue*(maxHP+(int)effect.sumValue);
                if(maxHP<MIN_HP/2) maxHP=MIN_HP/2;
                else if(maxHP>MAX_HP*2) maxHP=MAX_HP*2;
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.AGE)){
                age=effect.multiplicatorValue*(age+(int)effect.sumValue);
                if(age<0 && !Player.Instance.AllowNegativeAge()) age=0;
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.FEED)){
                this.EatWithoutCost(effect.multiplicatorValue*actualHunger+(int)effect.sumValue);
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.HYDRATE)){
                this.DrinkWithoutCost(effect.multiplicatorValue*actualThirst+(int)effect.sumValue);
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_ENERGY) && stats!=null){
                stats.SetEnergy(stats.GetMaxEnergy());
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_HP))this.SetActualHP(maxHP);
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_HUNGER))this.SetActualHunger(maxHunger);
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RESTORE_THIRST)) this.SetActualThirst(maxThirst);
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.HUNGER_LIMIT)){
                maxHunger=effect.multiplicatorValue*(maxHunger+(int)effect.sumValue);
                if(maxHunger<MIN_HUNGER/2) maxHunger=MIN_HUNGER/2;
                else if(maxHunger>MAX_HUNGER*2) maxHunger=MAX_HUNGER*2;
            }else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.THIRST_LIMIT)){
                maxThirst=effect.multiplicatorValue*(maxThirst+(int)effect.sumValue);
                if(maxThirst<MIN_THIRST/2) maxThirst=MIN_THIRST/2;
                else if(maxThirst>MAX_THIRST*2) maxThirst=MAX_THIRST*2;
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.ENERGY_LIMIT) && stats!=null){
                stats.SetMaxEnergy(effect.multiplicatorValue*(stats.GetMaxEnergy()+(int)effect.sumValue));
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.DIGGING_SPEED) && stats!=null){
                stats.SetDiggingSpeed((float)effect.multiplicatorValue*(stats.GetDiggingSpeed()+effect.sumValue));
            }else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.FARMING_SPEED) && stats!=null){
                stats.SetFarmingSpeed((float)effect.multiplicatorValue*(stats.GetFarmingSpeed()+effect.sumValue));
            }
            else if(effect.characterEffect.Equals(UpdateEffectOnAntOrQueen.RECOVER_SPEED) && stats!=null){
                stats.SetRecoverSpeed(effect.multiplicatorValue*(stats.GetRecoverSpeed()+(int)effect.sumValue));
            }

        }
    }



    public void InitVariables(System.Random random){
        Clock clock=FindObjectOfType<Clock>();
        SetClockOfGame(clock);
        this.random=random;
        int randomHP=random.Next(MIN_HP,MAX_HP);
        int randomHunger=random.Next(MIN_HUNGER,MAX_HUNGER);
        int randomThirst=random.Next(MIN_THIRST,MAX_THIRST);

        maxHP=randomHP;
        actualHP=randomHP;
        maxHunger=randomHunger;
        actualHunger=randomHunger;
        maxThirst=randomThirst;
        actualThirst=randomThirst;

        isDead=false;
        age=0;
    }
}
