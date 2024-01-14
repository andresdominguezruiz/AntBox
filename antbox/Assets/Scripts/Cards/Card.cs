using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType{
    POWER_UP,POWER_DOWN
}
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {
    public new string name;
    public string description;
    public Sprite artWorks;
    public CardType type;

    public List<Action> actions=new List<Action>();

    public double GetComplexityOfCard(double complexityLevelOfGame){
        double result=complexityLevelOfGame;
        if(type.Equals(CardType.POWER_UP)){
            result+=StatisticsOfGame.Instance.counterOfPassedExams;
            foreach(Action action in actions){
                result+=action.GetComplexityOfAction();
            }
        }
        return result;
    }

}

