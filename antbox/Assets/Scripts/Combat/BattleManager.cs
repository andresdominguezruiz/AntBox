using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    public float timeLastFrame;
    public bool inBattle=false;
    readonly System.Random random = new System.Random();
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
        if(isEnemy && inBattle && (Time.time-timeLastFrame)>=enemyStats.enemy.battleStats.AttackSpeed){
            Transform target=this.GetComponent<BattleMovement>()
            .ActualTarget;
            if(target!=null && target.gameObject!=null){
                AttackPlayerItemByEnemy(target);
            }
            timeLastFrame=Time.time;
        }else if(!isEnemy && inBattle && (Time.time-timeLastFrame)>=antStats.battleStats.AttackSpeed){
            Transform target=this.GetComponent<BattleMovement>()
            .ActualTarget;
            if(target!=null && target.gameObject!=null){
                AttackPlayerItemByAlly(target);
            }
            timeLastFrame=Time.time;
        }
    }
    void AttackPlayerItemByAlly(Transform target){
        EnemyStats enemyStats=target.gameObject.GetComponent<EnemyStats>();
        if(enemyStats!=null){
            double randomValue=random.NextDouble();
            if(randomValue>=antStats.battleStats.MissProbability){
                bool isCritic=false;
                if(randomValue>=antStats.battleStats.CriticalProbability){
                    isCritic=true;
                    ApplyCriticalEffectsByAlly(enemyStats);
                }
                if(enemyStats.gameObject!=null
                && !enemyStats.IsDead()){
                    ApplyDamageToEnemy(enemyStats,antStats.battleStats.Damage,isCritic,true);
                }
            }
        }
    }

    //NO UTILIZAR IsDestroyed(), al ensamblar deja de existir, utiliza gameObject!=null

    void ApplyDamageToEnemy(EnemyStats enemy, int damage,bool isCritic,bool initAnimation){
        enemy.Hurt(damage);
        if(enemy.gameObject!=null && (enemy.enemy.battleStats.StartBattleType.Equals(StartBattleType.WAITER) ||
        enemy.enemy.battleStats.StartBattleType.Equals(StartBattleType.SEARCH_AND_RESPOND))){
            BattleMovement battleMovement=enemy.gameObject.GetComponent<BattleMovement>();
            if(battleMovement!=null){
                 battleMovement.ActualTarget=this.gameObject.transform;
            }
        }
    }
    void ApplyCriticalEffectsByAlly(EnemyStats enemy){
        foreach(CriticalEffects effect in antStats.battleStats.CriticalEffects){
            if(effect.Equals(CriticalEffects.DRAIN_HP)){
                antStats.Heal(antStats.battleStats.Damage/5);
            }
            else if(effect.Equals(CriticalEffects.DOUBLE_DAMAGE)){
                enemy.Hurt(antStats.battleStats.Damage);
            }
        }
    }

    void AttackPlayerItemByEnemy(Transform target){
        CharacterStats characterStats=target.gameObject.GetComponent<CharacterStats>();
        if(characterStats!=null){
            double randomValue=random.NextDouble();
            if(randomValue>=enemyStats.enemy.battleStats.MissProbability){
                bool isCritic=false;
                if(randomValue>=enemyStats.enemy.battleStats.CriticalProbability){
                    isCritic=true;
                    ApplyCriticalEffectsByEnemy(characterStats);
                }
                if(!characterStats.IsDead()){
                    ApplyDamageToCharacter(characterStats,enemyStats.enemy.battleStats.Damage,isCritic,true);
                    if(target!=null){
                        AntStats ant=target.gameObject.GetComponent<AntStats>();
                        if(!ant.GetAction().Equals(ActualAction.ATTACKING)){
                        ant.CancelAntAction();
                        ant.StartAttacking(this.gameObject.transform);
                    }
                    }
                }
            }else{
                AntAnimatorManager antAnimator=characterStats.gameObject.GetComponent<AntAnimatorManager>();
                if(antAnimator!=null){
                    antAnimator.DodgeAttack();
                }
            }
        }
        else{
            FarmStats farmStats=target.gameObject.GetComponent<FarmStats>();
            ApplyDamageToFarm(farmStats,enemyStats.enemy.battleStats.Damage*20/100);
        }
    }
    void ApplyDamageToFarm(FarmStats farmStats,int damage){
            farmStats.timePerCycleConsumed-=damage;
            farmStats.CheckTimePerCycle();
            if(farmStats.broken && enemyStats!=null){
                enemyStats.kills++;
            }
    }

    void StartCounterAttack(GameObject enemyOrAlly){
        BattleManager targetBattleManager=enemyOrAlly.GetComponent<BattleManager>();
        if(targetBattleManager!=null &&!targetBattleManager.inBattle){
            targetBattleManager.inBattle=true;
            BattleMovement targetMovement=enemyOrAlly.GetComponent<BattleMovement>();
            if(targetMovement!=null){
                targetMovement.KillingMode=true;
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
        if(characterStats.IsDead()){
            enemyStats.kills++;
        }
        else{
            AntStats ant=characterStats.gameObject.GetComponent<AntStats>();
            if(ant!=null && (ant.battleStats.StartBattleType.Equals(StartBattleType.WAITER)
             || ant.battleStats.StartBattleType.Equals(StartBattleType.SEARCH_AND_RESPOND))){
                ant.CancelAntAction();
                StartCounterAttack(characterStats.gameObject);
                
            }
        }
    }

    void ApplyCriticalEffectsByEnemy(CharacterStats targetStats){
        foreach(CriticalEffects critical in enemyStats.enemy.battleStats.CriticalEffects){
            if(critical.Equals(CriticalEffects.DRAIN_HP)){
                enemyStats.Heal(enemyStats.enemy.battleStats.Damage/5);
            }
            else if(critical.Equals(CriticalEffects.DOUBLE_DAMAGE)){
                ApplyDamageToCharacter(targetStats,enemyStats.enemy.battleStats.Damage,true,false);
            }
            else if(critical.Equals(CriticalEffects.EAT_RESOURCES)){
                targetStats.EatWithoutCost(-enemyStats.enemy.battleStats.Damage);
                targetStats.DrinkWithoutCost(-enemyStats.enemy.battleStats.Damage);
            }
            else if(critical.Equals(CriticalEffects.POISONOUS)){
                targetStats.PoisonSecons+=10;
            }
            else if(critical.Equals(CriticalEffects.AREA_ATTACK)){
                BattleMovement enemyMovement=GetComponent<BattleMovement>();
                if(enemyMovement!=null){
                    foreach(Transform other in enemyMovement.OtherAvailableTargets){
                        if(other.gameObject!=null){
                            CharacterStats character=other.gameObject.GetComponent<CharacterStats>();
                            FarmStats farm=other.gameObject.GetComponent<FarmStats>();
                            if(character!=null){
                                ApplyDamageToCharacter(character,enemyStats.enemy.battleStats.Damage/2,false,true);
                            }
                            else if(farm!=null){
                                ApplyDamageToFarm(farm,enemyStats.enemy.battleStats.Damage*10/100);
                            }
                        }
                    }
                }
            }
        }
    }
}
