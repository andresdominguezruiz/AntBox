using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    private System.Random random = new System.Random();

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
