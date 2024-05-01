using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public Enemy enemy;
    public int actualHP;
    public float timeLastFrame;
    private System.Random random = new System.Random();
    public GameObject dangerZone;
    public int kills=0;
    public BarManager healthBar;

    public void Heal(int extraHP){
        if(extraHP+actualHP>enemy.maxHP){
            actualHP=enemy.maxHP;
        }
        else{
            actualHP+=extraHP;
        }
        healthBar.SetBarValue(actualHP);
    }

    public bool IsDead(){
        return actualHP<=0;
    }

    void GiveResourcesAfterDead(){
        //TODO: efecto sonoro al matarlo
        ContainerData container=FindObjectOfType<ContainerData>();
        container.AddResourcesRandomly(enemy.resources,random.NextDouble());
    }

    public void Hurt(int damage){
        actualHP-=damage;
        if(IsDead()){
            SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
            item.isSelected=false;
            item.RemoveSelectableItem();
            GiveResourcesAfterDead();
            Destroy(this.gameObject);
        }else{
            healthBar.SetBarValue(actualHP);
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        timeLastFrame=0f;
        actualHP=enemy.maxHP;
        SpriteRenderer spriteRenderer=GetComponentInChildren<SpriteRenderer>(false);
        healthBar.SetMaxBarValue(enemy.maxHP);
        if(spriteRenderer!=null){
            spriteRenderer.sprite=enemy.enemySprite;
        }
        if(enemy.battleStats.CriticalEffects.Contains(CriticalEffects.AREA_ATTACK) && !enemy.enemyType.Equals(EnemyType.EARTHWORM)){
            Transform area=this.gameObject.transform.Find("AreaAttack");
            if(area!=null){
                area.gameObject.SetActive(true);
                dangerZone=area.gameObject;
            }
        }else if(enemy.battleStats.CriticalEffects.Contains(CriticalEffects.AREA_ATTACK) && enemy.enemyType.Equals(EnemyType.EARTHWORM)){
            dangerZone.SetActive(true);
        }
    }

   

}
