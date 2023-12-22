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
    public Vector3Int direction= new Vector3Int(0,0,0);
    private Vector3Int routeTile;
    public bool isDigging=false;
    [SerializeField] private static int digTime=5; //Cada t tiempo real, se considera un día


    private float counterOfSecons=0;
    public float timeLastFrame;
    [SerializeField] private Tilemap destructableMap;
     public static List<ExcavationMovement> itemsWhoDig=new List<ExcavationMovement>();

    private ContainerData containerData;

    public bool IsDigging(){
        return isDigging;
    }

    public void StopDigging(){
        direction=new Vector3Int(0,0,0);
        canDig=false;
        isDigging=false;
    }

    public void InitComponent(Tilemap map){
        destructableMap=map;
    }

    public void Up(){
        destructableMap.SetTile(selectedTile,containerData.dirtTile);
        selectedTile=selectedTile-direction;
        direction=Vector3Int.up;
        selectedTile=selectedTile+direction;
        counterOfSecons=0;
        
    }
    public void Down(){
        destructableMap.SetTile(selectedTile,containerData.dirtTile);
        selectedTile=selectedTile-direction;
        direction=Vector3Int.down;
        selectedTile=selectedTile+direction;
        counterOfSecons=0;
    }
    public void Right(){
        destructableMap.SetTile(selectedTile,containerData.dirtTile);
        selectedTile=selectedTile-direction;
        direction=Vector3Int.right;
        selectedTile=selectedTile+direction;
        counterOfSecons=0;
    }
    public void Left(){
        destructableMap.SetTile(selectedTile,containerData.dirtTile);
        selectedTile=selectedTile-direction;
        direction=Vector3Int.left;
        selectedTile=selectedTile+direction;
        counterOfSecons=0;
    }
    public bool CanGoLeft(){
        return destructableMap.GetTile(routeTile+Vector3Int.left)!=null;
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
        Debug.Log(direction);
        canDig=true;
    }

    public void StartDiggingAndFirstDirection(){
        isDigging=true;
        direction=selectedTile-routeTile;
        if(destructableMap.GetTile(selectedTile)==null) selectedTile+=direction;
        
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Dirt") && isDigging)
        {
            //poniendo al final false haces lo contrario, obligas a que no ignore las colisiones
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.collider, false);
        }
    }
    void Start()
    {
        ant=this.gameObject;
        containerData=FindFirstObjectByType<ContainerData>();
        itemsWhoDig.Add(this);
        
    }


    // Update is called once per frame

    void NextPositionsAfterDig(){
        selectedTile=selectedTile+direction;
        routeTile=routeTile+direction;
        while(destructableMap.GetTile(selectedTile)==null){
            selectedTile+=direction;
            routeTile+=direction;
        }
    }

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
                    isDigging=false;
                    NextPositionsAfterDig();//TODO:Poner límite para evitar tardar mucho en elegir siguiente tile
                    agent.SetDestination(destructableMap.GetCellCenterWorld(routeTile));
                }
            }

        }
    }
    void DigTile(){
        if(Time.time -timeLastFrame>=1.0f){
            //Estas 3 líneas permiten la excavación múltiple
            List<ExcavationMovement> itemsWhoDigSameTile=itemsWhoDig.FindAll(item=> item.selectedTile.Equals(this.selectedTile) && item.isDigging);
            foreach(ExcavationMovement item in itemsWhoDigSameTile){
                counterOfSecons+=item.ant.GetComponent<AntStats>().GetDiggingSpeed();
            }
            DigMenu menu=FindObjectOfType<DigMenu>(false);
            if(menu==null){//Cuando el dig menu no activo, cambiar tiles
                if(counterOfSecons<=digTime/4){
                    destructableMap.SetTile(selectedTile,containerData.diggingDirtTile1);
                }else if(counterOfSecons>digTime/4 && counterOfSecons<=digTime*2/4){
                    destructableMap.SetTile(selectedTile,containerData.diggingDirtTile2);
                }else if(counterOfSecons>digTime*2/4 && counterOfSecons<=digTime*3/4){
                    destructableMap.SetTile(selectedTile,containerData.diggingDirtTile3);
            }
            }
            if(counterOfSecons>=digTime){
                counterOfSecons=0;
                destructableMap.SetTile(selectedTile,null);
                GenerationTilemap generationTilemap=FindFirstObjectByType<GenerationTilemap>();
                generationTilemap.AddWalkableTile(selectedTile);
                generationTilemap.BakeMap();
                if(menu!=null && menu.isSelectingDestructableTile){
                    menu.PreparingSelectableTiles();
            }

                
            }
            timeLastFrame=Time.time;
        }

    }

    
}
