using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tail : MonoBehaviour
{
    [SerializeField]
    private int length;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private Vector3[] segmentPoses;

    [SerializeField]
    private Vector3[] segmentVelocity;

    [SerializeField]
    private float smoothSpeed;

    [SerializeField]
    private float wiggleSpeed;

    [SerializeField]
    private float wiggleMagnitude;

    [SerializeField]
    private Transform wiggleDir;

    [SerializeField]
    private float trailSpeed;

    [SerializeField]
    private NavMeshAgent head;

    [SerializeField]
    private GameObject[] bodyParts;

    [SerializeField]
    private GameObject bodyPart;

    [SerializeField]
    private Transform targetDir;

    [SerializeField]
    private float targetDistance;

    [SerializeField]
    private EnemyStats enemyStats;

    public int Length { get => length; set => length = value; }
    public LineRenderer LineRenderer { get => lineRenderer; set => lineRenderer = value; }
    public Vector3[] SegmentPoses { get => segmentPoses; set => segmentPoses = value; }
    public Vector3[] SegmentVelocity { get => segmentVelocity; set => segmentVelocity = value; }
    public float SmoothSpeed { get => smoothSpeed; set => smoothSpeed = value; }
    public float WiggleSpeed { get => wiggleSpeed; set => wiggleSpeed = value; }
    public float WiggleMagnitude { get => wiggleMagnitude; set => wiggleMagnitude = value; }
    public Transform WiggleDir { get => wiggleDir; set => wiggleDir = value; }
    public float TrailSpeed { get => trailSpeed; set => trailSpeed = value; }
    public NavMeshAgent Head { get => head; set => head = value; }
    public GameObject[] BodyParts { get => bodyParts; set => bodyParts = value; }
    public GameObject BodyPart { get => bodyPart; set => bodyPart = value; }
    public Transform TargetDir { get => targetDir; set => targetDir = value; }
    public float TargetDistance { get => targetDistance; set => targetDistance = value; }
    public EnemyStats EnemyStats { get => enemyStats; set => enemyStats = value; }

    // Start is called before the first frame update
    void Start()
    {
        EnemyStats=this.gameObject.GetComponentInParent<EnemyStats>();
        LineRenderer.positionCount=Length;
        SegmentPoses=new Vector3[Length];
        SegmentVelocity=new Vector3[Length];
        Head=this.gameObject.GetComponentInParent<NavMeshAgent>();
        BodyParts=new GameObject[Length];
        BodyParts[0]=BodyPart;
        for(int i=0;i<Length;i++){
            if(i!=0){
                GameObject newBodyPart=Instantiate(BodyPart,BodyPart.transform.position,Quaternion.identity,BodyPart.transform.parent);
                BodyParts[i]=newBodyPart;
            }
            BodyParts[i].GetComponent<SpriteRenderer>().sprite=EnemyStats.Enemy.EnemyBodySprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        WiggleDir.localRotation=Quaternion.Euler(0,0,Mathf.Sin(Time.time*WiggleSpeed)*WiggleMagnitude);
        TargetDir=Head.transform;
        SegmentPoses[0]=TargetDir.position;
        for(int i=1;i< SegmentPoses.Length;i++){
            Vector3 targetPos=SegmentPoses[i-1]+(SegmentPoses[i]-SegmentPoses[i-1]).normalized*TargetDistance;
            SegmentPoses[i]= Vector3
            .SmoothDamp(SegmentPoses[i],targetPos,ref SegmentVelocity[i],SmoothSpeed+i/TrailSpeed);
            BodyParts[i-1].transform.position=SegmentPoses[i];
        }
        LineRenderer.SetPositions(SegmentPoses);
    }
}
