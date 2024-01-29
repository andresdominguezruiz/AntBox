using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform actualTarget;
    public HashSet<Transform> otherAvailableTargets=new HashSet<Transform>();
    NavMeshAgent enemyAgent;
    EnemyStats stats;
    void Start()
    {
        enemyAgent=this.GetComponent<NavMeshAgent>();
        enemyAgent.updateRotation = false;
        enemyAgent.updateUpAxis = false;
        stats=this.GetComponent<EnemyStats>();

        
    }

    void UpdateTarget(){
        EnemyStats enemyStats=this.GetComponent<EnemyStats>();
        SelectableItem selectableItem=FindObjectOfType<SelectableItem>(false);
        List<SelectableItem> allItems=selectableItem.GetItemsByTarget(enemyStats.enemy.targetType);
        actualTarget=ChooseTarget(allItems);
    }

    public Transform ChooseTarget(List<SelectableItem> items){
        float closestTargetDistance=float.MaxValue;
        Transform newTarget=null;
        NavMeshPath path=new NavMeshPath();
        foreach(SelectableItem item in items){
            if(NavMesh.CalculatePath(this.transform.position,item.transform.position,enemyAgent.areaMask,path)){
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
    void OnTriggerEnter2D(Collider2D collider){
        if(collider.gameObject.transform.Equals(actualTarget)){
            stats.inBattle=true;
        }
    }
    void OnTriggerStay2D(Collider2D collider){
        if(collider.gameObject.transform.Equals(actualTarget)){
            stats.inBattle=true;
        }
        else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null) otherAvailableTargets.Add(collider.transform);
        }
    }
    void OnTriggerExit2D(Collider2D collider) {
        if(collider.gameObject.transform.Equals(actualTarget)){
            stats.inBattle=false;
        }else{
            SelectableItem item=collider.gameObject.GetComponent<SelectableItem>();
            if(item!=null) otherAvailableTargets.Remove(collider.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Enemy enemy=GetComponent<EnemyStats>().enemy;
        if(actualTarget!=null && !enemy.targetType.Equals(TargetType.NONE)){
            enemyAgent.SetDestination(actualTarget.position);
        }else if(actualTarget==null && !enemy.targetType.Equals(TargetType.NONE)){
            UpdateTarget();
        }
        
    }
}
