using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public Enemy enemy;
    public int actualHP;
    public float timeLastFrame;
    public bool inBattle=false;
    private System.Random random = new System.Random();
    private GameObject dangerZone;
    public int kills=0;

    public void Heal(int extraHP){
        if(extraHP+actualHP>enemy.maxHP) actualHP=enemy.maxHP;
        else actualHP+=extraHP;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        timeLastFrame=0f;
        actualHP=enemy.maxHP;
        SpriteRenderer spriteRenderer=GetComponentInChildren<SpriteRenderer>(false);
        if(spriteRenderer!=null){
            spriteRenderer.sprite=enemy.enemySprite;
        }
        if(enemy.criticalEffects.Contains(CriticalEffects.AREA_ATTACK)){
            Transform area=this.gameObject.transform.Find("AreaAttack");
            if(area!=null){
                area.gameObject.SetActive(true);
                dangerZone=area.gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(inBattle && Time.time-timeLastFrame>=enemy.attackSpeed){
            Transform target=this.GetComponent<EnemyMovement>()
            .actualTarget;
            if(target!=null && !target.IsDestroyed()){
                AttackTarget(target);
            }
            timeLastFrame=Time.time;
        }
    }
    void AttackTarget(Transform target){
        CharacterStats characterStats=target.gameObject.GetComponent<CharacterStats>();
        if(characterStats!=null){
            double randomValue=random.NextDouble();
            if(randomValue>enemy.missProbability){
                if(randomValue>=enemy.criticalProbability){
                    Debug.Log("CRITIC");
                    ApplyCriticalEffectsByEnemy(characterStats);
                }
                if(!characterStats.IsDead())ApplyDamage(characterStats,enemy.damage);
                Debug.Log("PUM");
            }else{
                Debug.Log("MISS");
            }
        }
        else{
            FarmStats farmStats=target.gameObject.GetComponent<FarmStats>();
            ApplyDamageToFarm(farmStats,enemy.damage*20/100);
        }
    }
    void ApplyDamageToFarm(FarmStats farmStats,int damage){
            farmStats.timePerCycleConsumed-=damage;
            farmStats.CheckTimePerCycle();
            if(farmStats.broken) kills++;
    }
    void ApplyDamage(CharacterStats characterStats,int damage){
        characterStats.Heal(-damage);
            characterStats.CheckHP();
            if(characterStats.IsDead()) kills++;
    }

    void ApplyCriticalEffectsByEnemy(CharacterStats targetStats){
        foreach(CriticalEffects critical in enemy.criticalEffects){
            if(critical.Equals(CriticalEffects.DRAIN_HP)){
                Heal(enemy.damage/5);
            }
            else if(critical.Equals(CriticalEffects.DOUBLE_DAMAGE)){
                ApplyDamage(targetStats,enemy.damage);
            }
            else if(critical.Equals(CriticalEffects.EAT_RESOURCES)){
                targetStats.EatWithoutCost(-enemy.damage);
                targetStats.DrinkWithoutCost(-enemy.damage);
            }
            else if(critical.Equals(CriticalEffects.POISONOUS)){
                targetStats.poisonSecons+=10;
            }
            else if(critical.Equals(CriticalEffects.AREA_ATTACK)){
                EnemyMovement enemyMovement=GetComponent<EnemyMovement>();
                if(enemyMovement!=null){
                    foreach(Transform other in enemyMovement.otherAvailableTargets){
                        CharacterStats character=other.gameObject.GetComponent<CharacterStats>();
                        FarmStats farm=other.gameObject.GetComponent<FarmStats>();
                        if(character!=null) ApplyDamage(character,enemy.damage/2);
                        else if(farm!=null) ApplyDamageToFarm(farm,enemy.damage*10/100);
                    }
                }
            }
        }
    }

}
