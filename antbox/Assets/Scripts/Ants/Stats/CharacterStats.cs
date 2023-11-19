using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class CharacterStats : MonoBehaviour
{
    private System.Random random = new System.Random();
    public float timeLastFrame;


    [SerializeField] private static int growingTime=20; //Cada t tiempo real, se considera una semana

    //LIMITS FOR VARIABLES----------------------
    [SerializeField] protected int MIN_HP=40;
    [SerializeField] protected int MAX_HP=60;

    [SerializeField] protected int MIN_HUNGER=50;
    [SerializeField] protected int MAX_HUNGER=100;

    [SerializeField] protected int MIN_THIRST=50;

    [SerializeField] protected int MAX_THIRST=100;


    //----------------------------
    [SerializeField] private int maxHP;
    [SerializeField] private int actualHP;

    [SerializeField] private int maxHunger;
    [SerializeField] private int actualHunger;

    [SerializeField] private int maxThirst;
    [SerializeField] private int actualThirst;

    [SerializeField] private int age;

    [SerializeField] private bool isDead=false;
    
    void Update(){
        if(Time.time -timeLastFrame>=1.0f){
            UpdateStats();
            timeLastFrame=Time.time;
        }
    }


    void UpdateStats(){
        if(isDead){
            SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
            item.isSelected=false;
            Destroy(this.gameObject);
        }else{
            bool needToCheckHP=false;
            if(Time.deltaTime%growingTime==0) age++;
            if(actualHunger>0 && actualThirst>0){
                actualHunger--;
                actualThirst--;
            }else if(actualHunger>0){
                actualHunger--;
                actualHP--;
                needToCheckHP=true;
            }else if(actualThirst>0){
                actualThirst--;
                actualHP--;
                needToCheckHP=true;
            }else{
                actualHP-=2;
                needToCheckHP=true;
            }

            if(needToCheckHP) CheckHP();
        }
    }

    public void Eat(ContainerData container){
        if(container.FOOD_CONTAINER>0){
            container.FOOD_CONTAINER--;
            Debug.Log("ha comido");
            if(maxHunger<actualHunger+container.foodValue) SetActualHunger(maxHunger);
            else SetActualHunger(actualHunger+container.foodValue);
        }else{
            Debug.Log("no ha comido");
        }
    }
    public void Drink(ContainerData container){
        if(container.WATER_CONTAINER>0){
            container.WATER_CONTAINER--;
            if(maxThirst<actualThirst+container.waterValue) SetActualThirst(maxThirst);
            else SetActualThirst(actualThirst+container.waterValue);
        }
    }




    public void CheckHP(){
        if(actualHP<=0){
            actualHP=0;
            Die();
        }else if(actualHP>=maxHP){
            actualHP=maxHP;
        }
    }

    public void Die(){
        isDead=true;
    }

    public void TakeDamage(int damage){
        SetActualHP(actualHP-damage);
    }

    public void Heal(int extraHp){
        SetActualHP(actualHP+extraHp);
    }

    public void SetActualHP(int hp){
        actualHP=hp;
    }

    public void SetActualHunger(int hunger){
        actualHunger=hunger;
    }

    public void SetActualThirst(int thirst){
        actualThirst=thirst;
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

    public void InitVariables(){
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
