using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    public float timeLastFrame;
    public bool inBattle=false;
    private System.Random random = new System.Random();
    private EnemyStats enemyStats;
    private AntStats antStats;
    public bool isEnemy;
    void Start()
    {
        timeLastFrame=0f;
        enemyStats=this.gameObject.GetComponent<EnemyStats>();
        antStats=this.gameObject.GetComponent<AntStats>();
        if(enemyStats!=null){
            isEnemy=true;
        }else if(antStats!=null){
            isEnemy=false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnemy && inBattle && (Time.time-timeLastFrame)>=enemyStats.enemy.battleStats.attackSpeed){
            Transform target=this.GetComponent<BattleMovement>()
            .actualTarget;
            if(target!=null && !target.IsDestroyed()){
                AttackPlayerItemByEnemy(target);
            }
            timeLastFrame=Time.time;
        }else if(!isEnemy && inBattle && (Time.time-timeLastFrame)>=antStats.battleStats.attackSpeed){
            Transform target=this.GetComponent<BattleMovement>()
            .actualTarget;
            if(target!=null && !target.IsDestroyed()){
                AttackPlayerItemByAlly(target);
            }
            timeLastFrame=Time.time;
        }
    }
    void AttackPlayerItemByAlly(Transform target){
        EnemyStats enemyStats=target.gameObject.GetComponent<EnemyStats>();
        if(enemyStats!=null){
            double randomValue=random.NextDouble();
            if(randomValue>=antStats.battleStats.missProbability){
                bool isCritic=false;
                if(randomValue>=antStats.battleStats.criticalProbability){
                    isCritic=true;
                    ApplyCriticalEffectsByAlly(enemyStats);
                }
                if(!enemyStats.gameObject.IsDestroyed() 
                && !enemyStats.IsDead())ApplyDamageToEnemy(enemyStats,antStats.battleStats.damage,isCritic,true);
            }
        }
    }

    void ApplyDamageToEnemy(EnemyStats enemy, int damage,bool isCritic,bool initAnimation){
        enemy.Hurt(damage);
        if(!enemy.gameObject.IsDestroyed() && (enemy.enemy.battleStats.startBattleType.Equals(StartBattleType.WAITER) ||
        enemy.enemy.battleStats.startBattleType.Equals(StartBattleType.SEARCH_AND_RESPOND))){
            BattleMovement battleMovement=enemy.gameObject.GetComponent<BattleMovement>();
            if(battleMovement!=null) battleMovement.actualTarget=this.gameObject.transform;
        }
    }
    void ApplyCriticalEffectsByAlly(EnemyStats enemy){
        foreach(CriticalEffects effect in antStats.battleStats.criticalEffects){
            if(effect.Equals(CriticalEffects.DRAIN_HP)){
                antStats.Heal(antStats.battleStats.damage/5);
            }
            else if(effect.Equals(CriticalEffects.DOUBLE_DAMAGE)){
                enemy.Hurt(antStats.battleStats.damage);
            }
        }
    }

    void AttackPlayerItemByEnemy(Transform target){
        CharacterStats characterStats=target.gameObject.GetComponent<CharacterStats>();
        if(characterStats!=null){
            double randomValue=random.NextDouble();
            if(randomValue>=enemyStats.enemy.battleStats.missProbability){
                bool isCritic=false;
                if(randomValue>=enemyStats.enemy.battleStats.criticalProbability){
                    isCritic=true;
                    ApplyCriticalEffectsByEnemy(characterStats);
                }
                if(!characterStats.IsDead())ApplyDamageToCharacter(characterStats,enemyStats.enemy.battleStats.damage,isCritic,true);
            }else{
                AntAnimatorManager antAnimator=characterStats.gameObject.GetComponent<AntAnimatorManager>();
                if(antAnimator!=null) antAnimator.DodgeAttack();
            }
        }
        else{
            FarmStats farmStats=target.gameObject.GetComponent<FarmStats>();
            ApplyDamageToFarm(farmStats,enemyStats.enemy.battleStats.damage*20/100);
        }
    }
    void ApplyDamageToFarm(FarmStats farmStats,int damage){
            farmStats.timePerCycleConsumed-=damage;
            farmStats.CheckTimePerCycle();
            if(farmStats.broken && enemyStats!=null) enemyStats.kills++;
    }

    void StartCounterAttack(GameObject enemyOrAlly){
        BattleManager targetBattleManager=enemyOrAlly.GetComponent<BattleManager>();
        if(targetBattleManager!=null &&!targetBattleManager.inBattle){
            targetBattleManager.inBattle=true;
            BattleMovement targetMovement=enemyOrAlly.GetComponent<BattleMovement>();
            if(targetMovement!=null){
                targetMovement.killingMode=true;
                targetMovement.UpdateTarget();
        }
            }
    }
    void ApplyDamageToCharacter(CharacterStats characterStats,int damage,bool isCritic,bool playAnimation){
        characterStats.Heal(-damage);
        AntAnimatorManager antAnimator=characterStats.gameObject.GetComponent<AntAnimatorManager>();
        if(playAnimation){
            if(antAnimator!=null && isCritic){
                antAnimator.SufferCritical();
            }else if(antAnimator!=null){
                antAnimator.BeingHurt();
            }
        }
        characterStats.CheckHP();
        if(characterStats.IsDead()) enemyStats.kills++;
        else{
            AntStats ant=characterStats.gameObject.GetComponent<AntStats>();
            if(ant!=null && (ant.battleStats.startBattleType.Equals(StartBattleType.WAITER)
             || ant.battleStats.startBattleType.Equals(StartBattleType.SEARCH_AND_RESPOND))){
                StartCounterAttack(characterStats.gameObject);
            }
        }
    }

    void ApplyCriticalEffectsByEnemy(CharacterStats targetStats){
        foreach(CriticalEffects critical in enemyStats.enemy.battleStats.criticalEffects){
            if(critical.Equals(CriticalEffects.DRAIN_HP)){
                enemyStats.Heal(enemyStats.enemy.battleStats.damage/5);
            }
            else if(critical.Equals(CriticalEffects.DOUBLE_DAMAGE)){
                ApplyDamageToCharacter(targetStats,enemyStats.enemy.battleStats.damage,true,false);
            }
            else if(critical.Equals(CriticalEffects.EAT_RESOURCES)){
                targetStats.EatWithoutCost(-enemyStats.enemy.battleStats.damage);
                targetStats.DrinkWithoutCost(-enemyStats.enemy.battleStats.damage);
            }
            else if(critical.Equals(CriticalEffects.POISONOUS)){
                targetStats.poisonSecons+=10;
            }
            else if(critical.Equals(CriticalEffects.AREA_ATTACK)){
                BattleMovement enemyMovement=GetComponent<BattleMovement>();
                if(enemyMovement!=null){
                    foreach(Transform other in enemyMovement.otherAvailableTargets){
                        if(!other.gameObject.IsDestroyed()){
                            CharacterStats character=other.gameObject.GetComponent<CharacterStats>();
                            FarmStats farm=other.gameObject.GetComponent<FarmStats>();
                            if(character!=null) ApplyDamageToCharacter(character,enemyStats.enemy.battleStats.damage/2,false,true);
                            else if(farm!=null) ApplyDamageToFarm(farm,enemyStats.enemy.battleStats.damage*10/100);
                        }
                    }
                }
            }
        }
    }
}
