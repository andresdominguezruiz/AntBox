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
    private GameObject ant;
    private Vector3Int selectedTile;
    private Vector3Int actualTile;
    public Vector3Int direction= new Vector3Int(0,0,0);
    private Vector3Int routeTile;
    [SerializeField]
    private bool isDigging=false;



    public float timeLastFrame;
    [SerializeField] private Tilemap destructableMap;
    readonly static List<ExcavationMovement> itemsWhoDig=new List<ExcavationMovement>();

    private ContainerData containerData;

    public bool IsDigging { get => isDigging; set => isDigging = value; }

    

    public void StopDigging(){
        direction=new Vector3Int(0,0,0);
        canDig=false;
        IsDigging=false;
        GenerationTilemap generation=FindObjectOfType<GenerationTilemap>();
        TileData tileData=generation.GetTileData(selectedTile);
        tileData.AntsDiggingSameTile.Remove(this.gameObject.GetComponent<AntStats>());
    }
    public bool CanDig(){
        GenerationTilemap generation=FindObjectOfType<GenerationTilemap>();
        TileData tileData=generation.GetTileData(selectedTile);
        return this.gameObject.GetComponent<AntStats>().GetActualEnergy()>=tileData.GetEnergyCostToDig();
    }

    public void InitComponent(Tilemap map){
        destructableMap=map;
    }

    public void Up(){
        destructableMap.SetTile(selectedTile,containerData.dirtTile);
        selectedTile=selectedTile-direction;
        direction=Vector3Int.up;
        selectedTile=selectedTile+direction;
 
        
    }
    public void Down(){
        destructableMap.SetTile(selectedTile,containerData.dirtTile);
        selectedTile=selectedTile-direction;
        direction=Vector3Int.down;
        selectedTile=selectedTile+direction;

    }
    public void Right(){
        destructableMap.SetTile(selectedTile,containerData.dirtTile);
        selectedTile=selectedTile-direction;
        direction=Vector3Int.right;
        selectedTile=selectedTile+direction;
        
    }
    public void Left(){
        destructableMap.SetTile(selectedTile,containerData.dirtTile);
        selectedTile=selectedTile-direction;
        direction=Vector3Int.left;
        selectedTile=selectedTile+direction;

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
        Debug.Log("initial dir:"+direction);
        canDig=true;
    }

    public void StartDiggingAndFirstDirection(){
        IsDigging=true;
        direction=selectedTile-routeTile;
        if(destructableMap.GetTile(selectedTile)==null){
            selectedTile+=direction;
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
        GenerationTilemap generation=FindObjectOfType<GenerationTilemap>();
        destructableMap=generation.dirtMap;
        NavMeshAgent agent=ant.GetComponent<NavMeshAgent>();
        for(int i=0;i<=7;i++){
            selectedTile+=direction;
            routeTile+=direction;
            Debug.Log(ant.GetComponentInChildren<UIManager>()==null);
            Debug.Log(ant.name+" esta buscando");
            if(generation.GetTileOfTilesData(selectedTile)!=null
             && generation.GetTileData(selectedTile)!=null
              && !generation.GetTileData(selectedTile).GetTileType().Equals(TileType.EMPTY)){
                break;
             }
        }
        if(generation.GetTileOfTilesData(selectedTile)==null
         || generation.GetTileData(selectedTile)==null 
         || generation.GetTileData(selectedTile).GetTileType().Equals(TileType.EMPTY)){
            this.gameObject.GetComponent<AntStats>().StopDigging();
        }else{
            if(!(selectedTile-routeTile).Equals(direction)){ 
                routeTile+=direction;
                
            }
            agent.SetDestination(generation.dirtMap.GetCellCenterWorld(routeTile));
        }
    }

    void Update()
    {
        if(canDig){
            actualTile=destructableMap.WorldToCell(ant.transform.position);
            
            if(actualTile.x==routeTile.x && actualTile.y==routeTile.y && !IsDigging){
                StartDiggingAndFirstDirection();
            }  
            if(IsDigging){

                if(destructableMap.GetTile(selectedTile)!=null){
                    DigTile();
                }
                if(destructableMap.GetTile(selectedTile)==null){
                    DigMenu menu=FindObjectOfType<DigMenu>(false);
                    GenerationTilemap generationTilemap=FindFirstObjectByType<GenerationTilemap>();
                    generationTilemap.AddWalkableTile(selectedTile);
                    generationTilemap.BakeMap();
                    if(menu!=null && menu.isSelectingDestructableTile){
                        menu.PreparingSelectableTiles();
                    }
                    IsDigging=false;
                    NextPositionsAfterDig();//TODO:Poner límite para evitar tardar mucho en elegir siguiente tile
                }
            }

        }
    }
    void DigTile(){
        GenerationTilemap generationTilemap=FindFirstObjectByType<GenerationTilemap>();
        TileData tileData=generationTilemap.GetTileData(selectedTile);
        DigMenu menu=FindObjectOfType<DigMenu>(false);
        if(tileData!=null){
            if(Time.time -timeLastFrame>=1.0f){
            tileData.DiggingTile(ant.GetComponent<AntStats>(),menu!=null);
            Debug.Log("RESISTANCE:"+tileData.ActualResistance);
            timeLastFrame=Time.time;
        }
        }

    }

    
}

