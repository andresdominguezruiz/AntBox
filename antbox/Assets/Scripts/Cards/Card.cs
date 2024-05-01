using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType{
    POWER_UP,POWER_DOWN
}
[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject {

    [SerializeField]
    private new string name;

    [SerializeField]
    private string description;

    [SerializeField]
    private Sprite artWorks;

    [SerializeField]
    private CardType type;


    [SerializeField]
    private List<Action> actions = new List<Action>();

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public Sprite ArtWorks { get => artWorks; set => artWorks = value; }
    public CardType Type { get => type; set => type = value; }
    public List<Action> Actions { get => actions; set => actions = value; }

    public double GetComplexityOfCard(double complexityLevelOfGame){
        double result=complexityLevelOfGame;
        if(Type.Equals(CardType.POWER_UP)){
            result+=StatisticsOfGame.Instance.counterOfPassedExams;
            foreach(Action action in Actions){
                result+=action.GetComplexityOfAction();
            }
        }
        return result;
    }

    public bool HasPassive(){
        bool res=false;
        foreach(Action action in Actions){
            if(action.Destination.Equals(Destination.PLAYER)){
                res=true;
                break;
            }
        }
        return res;
    }

}

