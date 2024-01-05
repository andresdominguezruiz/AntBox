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

}

