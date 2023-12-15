using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public enum Direction{
    UP,DOWN,LEFT,RIGHT,NONE
}

public class ExcavationMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private bool canDig=false;
    //Utilizar para seguir excavando
    private Direction actualDirection=Direction.NONE;
    private GameObject ant;
    private Vector3Int selectedTile;
    private Vector3Int actualTile;
    public Vector3 direction= new Vector3(0,0,0);
    private Vector3Int routeTile;
    public bool isDigging=false;
    [SerializeField] private static int digTime=10; //Cada t tiempo real, se considera un día

    private int counterOfSecons=0;
    public float timeLastFrame;
    [SerializeField] private Tilemap destructableMap;

    public bool IsDigging(){
        return isDigging;
    }

    public void StopDigging(){
        direction=new Vector3(0,0,0);
        canDig=false;
        isDigging=false;
    }

    public void InitComponent(Tilemap map){
        destructableMap=map;
    }

    public void Up(){
        direction=Vector3.up;
    }
    public void Down(){
        direction=Vector3.down;
    }
    public void Right(){
        direction=Vector3.right;
    }
    public void Left(){
        direction=Vector3.left;
    }
    public bool CanGoLeft(){
        return destructableMap.GetTile(routeTile+Vector3Int.left)!=null ;
    }
    public bool CanGoRight(){
        return destructableMap.GetTile(routeTile+Vector3Int.right)!=null;
    }
    public bool CanGoUp(){
        return destructableMap.GetTile(routeTile+Vector3Int.up)!=null;
    }
    public bool CanGoDown(){
        return destructableMap.GetTile(routeTile+Vector3Int.down)!=null;
    }

    public void InitExcavation(Vector3Int selectedDestructableTile,Vector3Int selectedRoute){
        actualTile=destructableMap.WorldToCell(ant.transform.position);
        routeTile=selectedRoute;
        //destination guarda el punto medio entre el centro del tile objetivo y el tile de la ruta (asi no falla)
        //el navmeshagent
        ant.gameObject.GetComponent<NavMeshAgent>().SetDestination(destructableMap.GetCellCenterWorld(routeTile));
        selectedTile=selectedDestructableTile;
        direction=selectedDestructableTile-selectedRoute;
        actualDirection=ParseVectorToDirection(direction);
        canDig=true;
    }

    public void StartDiggingAndFirstDirection(){
        isDigging=true;
        actualDirection=ParseVectorToDirection(direction);
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Dirt") && isDigging)
        {
            //poniendo al final false haces lo contrario, obligas a que no ignore las colisiones
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.collider, false);
        }
    }

    public Direction ParseVectorToDirection(Vector3 vector){
        Direction res=Direction.NONE;
        if(vector.x>0 && vector.y==0){
            res=Direction.RIGHT;
        }else if(vector.x<0 && vector.y==0){
            res=Direction.LEFT;
        }else if(vector.y>0 && vector.x==0){
            res=Direction.UP;
        }else if(vector.y<0 && vector.x==0){
            res=Direction.DOWN;
        }
        return res;
    }
    void Start()
    {
        ant=this.gameObject;
        
    }


    // Update is called once per frame
    void Update()
    {
        if(canDig){
            actualTile=destructableMap.WorldToCell(ant.transform.position);
            NavMeshAgent agent=ant.GetComponent<NavMeshAgent>();
            if(actualTile.x==routeTile.x && actualTile.y==routeTile.y && !isDigging){
                StartDiggingAndFirstDirection();
            }
            if(isDigging){
                DigTile();
                if(destructableMap.GetTile(selectedTile)==null){

                }
            }

        }
    }
    void DigTile(){
        if(Time.time -timeLastFrame>=1.0f){
            counterOfSecons++;
            if(counterOfSecons==digTime/3){
                
            }else if(counterOfSecons==digTime*2/3){

            }else if(counterOfSecons==3*digTime/4){

            }
            if(counterOfSecons==digTime){
                counterOfSecons=0;
                destructableMap.SetTile(selectedTile,null);

                
            }
            timeLastFrame=Time.time;
        }

    }
}
