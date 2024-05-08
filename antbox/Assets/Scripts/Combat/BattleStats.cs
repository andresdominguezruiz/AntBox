using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StartBattleType{
    WAITER,ONLY_SEARCH,SEARCH_AND_RESPOND
}

[System.Serializable]
public class BattleStats
{
    [SerializeField]
    private bool isEnemy = false;

    [SerializeField]
    private float attackSpeed;

    [SerializeField]
    private int damage;

    [SerializeField]
    private double criticalProbability;

    [SerializeField]
    private double missProbability;

    [SerializeField]
    private StartBattleType startBattleType = StartBattleType.WAITER;

    [SerializeField]
    private List<CriticalEffects> criticalEffects = new List<CriticalEffects>();

    public bool IsEnemy { get => isEnemy; set => isEnemy = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public int Damage { get => damage; set => damage = value; }
    public double CriticalProbability { get => criticalProbability; set => criticalProbability = value; }
    public double MissProbability { get => missProbability; set => missProbability = value; }
    public StartBattleType StartBattleType { get => startBattleType; set => startBattleType = value; }
    public List<CriticalEffects> CriticalEffects { get => criticalEffects; set => criticalEffects = value; }

    public BattleStats(bool isEnemy,int damage,int speed,int miss,int critical){
        this.IsEnemy=isEnemy;
        this.Damage=damage;
        AttackSpeed=(float)(speed*1.0/100.0);
        CriticalProbability=critical*1.0/100.0;
        MissProbability=miss*1.0/100.0;
    }
    
}
