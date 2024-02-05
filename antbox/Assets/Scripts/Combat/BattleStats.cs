using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StartBattleType{
    WAITER,ONLY_SEARCH,SEARCH_AND_RESPOND
}

[System.Serializable]
public class BattleStats
{
    public bool isEnemy=false;
    public float attackSpeed;
    public int damage;
    public double criticalProbability;
    public double missProbability;
    public StartBattleType startBattleType=StartBattleType.WAITER;
    public List<CriticalEffects> criticalEffects=new List<CriticalEffects>();

    public BattleStats(bool isEnemy,int damage,int speed,int miss,int critical){
        this.isEnemy=isEnemy;
        this.damage=damage;
        attackSpeed=(float)(speed*1.0/100.0);
        criticalProbability=critical*1.0/100.0;
        missProbability=miss*1.0/100.0;
    }
    
}
