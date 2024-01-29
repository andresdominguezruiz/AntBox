using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType{
    ANT,WORM,EARTHWORM,SPIDER
}
public enum CriticalEffects{
    NONE,DRAIN_HP,POISONOUS,EAT_RESOURCES,DOUBLE_DAMAGE,AREA_ATTACK
}

public enum TargetType{
    ANT,ANTHILL,QUEEN,FARM,NONE
}

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public EnemyType enemyType=EnemyType.ANT;
    public TargetType targetType=TargetType.ANT;
    public Sprite enemySprite;
    public int enemyLevel=1;
    public int resources=1;
    public int damage;
    public int maxHP;
    public float attackSpeed;
    public double criticalProbability;
    public double missProbability;
    public List<CriticalEffects> criticalEffects=new List<CriticalEffects>();

    
}
