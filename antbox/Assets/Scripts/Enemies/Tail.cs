using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tail : MonoBehaviour
{
    public int length;
    public LineRenderer lineRenderer;
    public Vector3[] segmentPoses;
    public Vector3[] segmentVelocity;
    public float smoothSpeed;
    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleDir;
    public float trailSpeed;
    private NavMeshAgent head;
    public GameObject[] bodyParts;
    public GameObject bodyPart;

    public Transform targetDir;
    public float targetDistance;
    private EnemyStats enemyStats;

    // Start is called before the first frame update
    void Start()
    {
        enemyStats=this.gameObject.GetComponentInParent<EnemyStats>();
        lineRenderer.positionCount=length;
        segmentPoses=new Vector3[length];
        segmentVelocity=new Vector3[length];
        head=this.gameObject.GetComponentInParent<NavMeshAgent>();
        bodyParts=new GameObject[length];
        bodyParts[0]=bodyPart;
        for(int i=0;i<length;i++){
            if(i!=0){
                GameObject newBodyPart=Instantiate(bodyPart,bodyPart.transform.position,Quaternion.identity,bodyPart.transform.parent);
                bodyParts[i]=newBodyPart;
            }
            bodyParts[i].GetComponent<SpriteRenderer>().sprite=enemyStats.enemy.enemyBodySprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        wiggleDir.localRotation=Quaternion.Euler(0,0,Mathf.Sin(Time.time*wiggleSpeed)*wiggleMagnitude);
        targetDir=head.transform;
        segmentPoses[0]=targetDir.position;
        for(int i=1;i< segmentPoses.Length;i++){
            Vector3 targetPos=segmentPoses[i-1]+(segmentPoses[i]-segmentPoses[i-1]).normalized*targetDistance;
            segmentPoses[i]= Vector3
            .SmoothDamp(segmentPoses[i],targetPos,ref segmentVelocity[i],smoothSpeed+i/trailSpeed);
            bodyParts[i-1].transform.position=segmentPoses[i];
        }
        lineRenderer.SetPositions(segmentPoses);
    }
}
