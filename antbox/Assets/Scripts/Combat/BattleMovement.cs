using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BattleMovement : MonoBehaviour
{
    public Transform actualTarget;
    public HashSet<Transform> otherAvailableTargets=new HashSet<Transform>();
    NavMeshAgent agent;
    public BattleManager battleManager;
    public bool killingMode=false;
    void Start()
    {
        agent=this.GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        battleManager=this.GetComponent<BattleManager>();
        if(battleManager.isEnemy){
            killingMode=true;
        }

        
    }

    public void UpdateTarget(){
        List<Transform> allItems=new List<Transform>();
        if(battleManager.isEnemy){
            EnemyStats enemyStats=this.GetComponent<EnemyStats>();
            SelectableItem selectableItem=FindObjectOfType<SelectableItem>(false);
            allItems=selectableItem.GetItemsByTarget(enemyStats.enemy.targetType);
        }else if(!battleManager.isEnemy){
            EnemyStats[] enemies=FindObjectsOfType<EnemyStats>(false);
            foreach(EnemyStats enemyStats in enemies) allItems.Add(enemyStats.transform);
            Debug.Log(allItems.Count);
        }

        if(allItems.Count>0){
            actualTarget=ChooseTarget(allItems);
        }
        else{
            actualTarget=null;
        }
    }


    public Transform ChooseTarget(List<Transform> items){
        float closestTargetDistance=float.MaxValue;
        Transform newTarget=null;
        NavMeshPath path=new NavMeshPath();
        foreach(Transform item in items){
            if(NavMesh.CalculatePath(this.transform.position,item.position,agent.areaMask,path)){
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
        if(collider.gameObject.transform.Equals(actualTarget)){
            battleManager.inBattle=true;
        }
    }
    void OnTriggerStay2D(Collider2D collider){
        if(collider.gameObject.transform.Equals(actualTarget)){
            battleManager.inBattle=true;
        }
        else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null) otherAvailableTargets.Add(collider.transform);
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if(collider.gameObject.transform.Equals(actualTarget)){
            battleManager.inBattle=false;
        }else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null) otherAvailableTargets.Remove(collider.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(battleManager.isEnemy){
            Enemy enemy=GetComponent<EnemyStats>().enemy;
            if(actualTarget!=null && !enemy.targetType.Equals(TargetType.NONE)){
                agent.SetDestination(actualTarget.position);
            }else if(actualTarget==null && !enemy.targetType.Equals(TargetType.NONE)){
                UpdateTarget();
            }
        }
        else{
            if(actualTarget!=null && killingMode){
                agent.SetDestination(actualTarget.position);
            }else if(actualTarget==null && killingMode){
                UpdateTarget();
                if(actualTarget==null){
                    killingMode=false;
                    battleManager.inBattle=false;
                    AntStats ant=this.gameObject.GetComponent<AntStats>();
                    if(ant!=null) ant.StopAttacking();
                }
            }
        }
        
    }
}
