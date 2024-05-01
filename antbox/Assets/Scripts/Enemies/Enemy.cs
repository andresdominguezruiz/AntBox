using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType{
    ANT,WORM,EARTHWORM
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

    [SerializeField]
    private EnemyType enemyType = EnemyType.ANT;

    [SerializeField]
    private TargetType targetType = TargetType.ANT;

    [SerializeField]
    private Sprite enemySprite;

    [SerializeField]
    private Sprite enemyBodySprite;

    [SerializeField]
    private int enemyLevel = 1;

    [SerializeField]
    private int resources = 1;

    [SerializeField]
    private int maxHP;


    [SerializeField]
    private BattleStats battleStats;

    public EnemyType EnemyType { get => enemyType; set => enemyType = value; }
    public TargetType TargetType { get => targetType; set => targetType = value; }
    public Sprite EnemySprite { get => enemySprite; set => enemySprite = value; }
    public Sprite EnemyBodySprite { get => enemyBodySprite; set => enemyBodySprite = value; }
    public int EnemyLevel { get => enemyLevel; set => enemyLevel = value; }
    public int Resources { get => resources; set => resources = value; }
    public int MaxHP { get => maxHP; set => maxHP = value; }
    public BattleStats BattleStats { get => battleStats; set => battleStats = value; }
}
