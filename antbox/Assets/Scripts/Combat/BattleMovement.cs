using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BattleMovement : MonoBehaviour
{
    [SerializeField]
    private Transform actualTarget;

    [SerializeField]
    private HashSet<Transform> otherAvailableTargets=new HashSet<Transform>();

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private BattleManager battleManager;
    
    [SerializeField]
    private bool killingMode=false;

    public Transform ActualTarget { get => actualTarget; set => actualTarget = value; }
    public HashSet<Transform> OtherAvailableTargets { get => otherAvailableTargets; set => otherAvailableTargets = value; }
    public NavMeshAgent Agent { get => agent; set => agent = value; }
    public BattleManager BattleManager { get => battleManager; set => battleManager = value; }
    public bool KillingMode { get => killingMode; set => killingMode = value; }

    void Start()
    {
        Agent=this.GetComponent<NavMeshAgent>();
        Agent.updateUpAxis = false;
        Agent.updateRotation = false;
        BattleManager=this.GetComponent<BattleManager>();
        if(BattleManager.isEnemy){
            KillingMode=true;
        }

        
    }

    public void UpdateTarget(){
        List<Transform> allItems=new List<Transform>();
        if(BattleManager.isEnemy){
            EnemyStats enemyStats=this.GetComponent<EnemyStats>();
            SelectableItem selectableItem=FindObjectOfType<SelectableItem>(false);
            allItems=selectableItem.GetItemsByTarget(enemyStats.Enemy.TargetType);
        }else if(!BattleManager.isEnemy){
            EnemyStats[] enemies=FindObjectsOfType<EnemyStats>(false);
            foreach(EnemyStats enemyStats in enemies){
                allItems.Add(enemyStats.transform);
            }
        }

        if(allItems.Count>0){
            ActualTarget=ChooseTarget(allItems);
        }
        else{
            ActualTarget=null;
        }
    }


    public Transform ChooseTarget(List<Transform> items){
        float closestTargetDistance=float.MaxValue;
        Transform newTarget=null;
        NavMeshPath path=new NavMeshPath();
        foreach(Transform item in items){
            if(NavMesh.CalculatePath(this.transform.position,item.position,Agent.areaMask,path)){
                float distance=Vector3.Distance(this.transform.position,path.corners[0]);
                for(int i=1;i<path.corners.Length;i++){
                    distance+=Vector3.Distance(path.corners[i-1],path.corners[i]);
                }
                if(distance<closestTargetDistance){
                    closestTargetDistance=distance;
                    newTarget=item.transform;
                }
            }
        }
        return newTarget;
    }

    //COLISIONES SON PARA HORMIGAS
    
    
    //TRIGGERS SON PARA ENEMIGOS
    void OnTriggerEnter2D(Collider2D collider){
        BattleTool.OnTriggerUpdater(collider,this,TriggerType.ENTER);
    }
    void OnTriggerStay2D(Collider2D collider){
        BattleTool.OnTriggerUpdater(collider,this,TriggerType.STAY);
    }
    void OnTriggerExit2D(Collider2D collider) {
        BattleTool.OnTriggerUpdater(collider,this,TriggerType.EXIT);
    }

    // Update is called once per frame
    void Update()
    {
        if(BattleManager.isEnemy){
            Enemy enemy=GetComponent<EnemyStats>().Enemy;
            if(ActualTarget!=null && !enemy.TargetType.Equals(TargetType.NONE)){
                Agent.SetDestination(ActualTarget.position);
            }else if(ActualTarget==null && !enemy.TargetType.Equals(TargetType.NONE)){
                UpdateTarget();
            }
        }
        else{
            if(ActualTarget!=null && KillingMode){
                Agent.SetDestination(ActualTarget.position);
            }else if(ActualTarget==null && KillingMode){
                UpdateTarget();
                if(ActualTarget==null){
                    KillingMode=false;
                    BattleManager.inBattle=false;
                    AntStats ant=this.gameObject.GetComponent<AntStats>();
                    if(ant!=null){
                        ant.StopAttacking();
                    }
                }
            }
        }
        
    }
}
