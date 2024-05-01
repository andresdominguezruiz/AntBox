using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;

    [SerializeField]
    private int actualHP;

    [SerializeField]
    private float timeLastFrame;

    [SerializeField]
    readonly System.Random random = new System.Random();

    [SerializeField]
    private GameObject dangerZone;

    [SerializeField]
    private int kills = 0;

    [SerializeField]
    private BarManager healthBar;

    public GameObject DangerZone { get => dangerZone; set => dangerZone = value; }
    public int Kills { get => kills; set => kills = value; }
    public BarManager HealthBar { get => healthBar; set => healthBar = value; }
    public Enemy Enemy { get => enemy; set => enemy = value; }
    public int ActualHP { get => actualHP; set => actualHP = value; }
    public float TimeLastFrame { get => timeLastFrame; set => timeLastFrame = value; }

    public void Heal(int extraHP){
        if(extraHP+ActualHP>Enemy.MaxHP){
            ActualHP=Enemy.MaxHP;
        }
        else{
            ActualHP+=extraHP;
        }
        HealthBar.SetBarValue(ActualHP);
    }

    public bool IsDead(){
        return ActualHP<=0;
    }

    void GiveResourcesAfterDead(){
        //TODO: efecto sonoro al matarlo
        ContainerData container=FindObjectOfType<ContainerData>();
        container.AddResourcesRandomly(Enemy.Resources,random.NextDouble());
    }

    public void Hurt(int damage){
        ActualHP-=damage;
        if(IsDead()){
            SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
            item.isSelected=false;
            item.RemoveSelectableItem();
            GiveResourcesAfterDead();
            Destroy(this.gameObject);
        }else{
            HealthBar.SetBarValue(ActualHP);
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        TimeLastFrame=0f;
        ActualHP=Enemy.MaxHP;
        SpriteRenderer spriteRenderer=GetComponentInChildren<SpriteRenderer>(false);
        HealthBar.SetMaxBarValue(Enemy.MaxHP);
        if(spriteRenderer!=null){
            spriteRenderer.sprite=Enemy.EnemySprite;
        }
        if(Enemy.BattleStats.CriticalEffects.Contains(CriticalEffects.AREA_ATTACK) && !Enemy.EnemyType.Equals(EnemyType.EARTHWORM)){
            Transform area=this.gameObject.transform.Find("AreaAttack");
            if(area!=null){
                area.gameObject.SetActive(true);
                DangerZone=area.gameObject;
            }
        }else if(Enemy.BattleStats.CriticalEffects.Contains(CriticalEffects.AREA_ATTACK) && Enemy.EnemyType.Equals(EnemyType.EARTHWORM)){
            DangerZone.SetActive(true);
        }
    }

   

}
