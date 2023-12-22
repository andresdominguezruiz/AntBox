using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class DigMenu : MonoBehaviour
{
    [SerializeField] private GameObject digMenu;

    [SerializeField] private Tilemap destructableMap;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private GameObject cancelActionButton;

    [SerializeField] private GameObject selectedAnt;

    [SerializeField] private Tile selectable;
    [SerializeField] private Tile dirt;
    [SerializeField] private Tile route;

    private HashSet<Vector3Int> excavablePath;
    private Vector3Int selectedDestructableTile;
    private List<Vector3Int> routes=new List<Vector3Int>();
    public float minX=-5.5f;
    public float maxX=7f;
    public float minY=-3.5f;
    public float maxY=3f;

    public float speed=0.5f;

    public bool areMoreRutes=false;
    public bool isSelectingDestructableTile=false;

    private NavMeshAgent agent;

    public void StartDigMenu()
    {
        GenerationTilemap generationTilemap=FindFirstObjectByType<GenerationTilemap>();
        generationTilemap.BakeMap();
        this.agent=selectedAnt.GetComponent<NavMeshAgent>();
        selectedAnt.GetComponentInChildren<UIManager>(true).HideInfo();
        digMenu.gameObject.SetActive(true);
        consoleText.text=selectedAnt.name+"-Select the start of your excavation";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneUnselectable();
        excavablePath=FindObjectOfType<GenerationTilemap>().GetExcavableTiles();
        PreparingSelectableTiles();
        isSelectingDestructableTile=true;
        Debug.Log(excavablePath.Count);
    }
    //ESTE MÉTODO DEVUELVE LAS POSICIONES EXCAVABLES SELECCIONABLES
    public void PreparingSelectableTiles(){
        foreach(Vector3Int pos in excavablePath){
            destructableMap.SetTile(pos,selectable);
        }
    }
    void RollBackDirtTiles(){
        foreach(Vector3Int pos in excavablePath){
            destructableMap.SetTile(pos,dirt);
        }
    }
    //ESTE MÉTODO DEVUELVE LAS POSIBLES RUTAS DE UNA POSICION EXCAVABLE
    public List<Vector3Int> GetAdjacentTileOfDiggableTile(Tilemap map,Vector3Int selectedAndDiggableTile){
        Vector3Int left=new Vector3Int(selectedAndDiggableTile.x-1,selectedAndDiggableTile.y,selectedAndDiggableTile.z);
        Vector3Int right=new Vector3Int(selectedAndDiggableTile.x+1,selectedAndDiggableTile.y,selectedAndDiggableTile.z);
        Vector3Int up=new Vector3Int(selectedAndDiggableTile.x,selectedAndDiggableTile.y+1,selectedAndDiggableTile.z);
        Vector3Int down=new Vector3Int(selectedAndDiggableTile.x,selectedAndDiggableTile.y-1,selectedAndDiggableTile.z);
        List<Vector3Int> list=new List<Vector3Int>{left,right,up,down};
        List<Vector3Int> options=new List<Vector3Int>(); //List<<{} hace listas INMUTABLES
        foreach(Vector3Int pos in list){
            if(map.GetTile(pos)==null){
                options.Add(pos);
            }
        }
        return options;
    }

    void Update(){
        if(Input.GetMouseButtonDown(0) && !areMoreRutes){
            SelectStart();
        }else if(Input.GetMouseButtonDown(0) && areMoreRutes){
            SelectRoute();
        }

    }
    void SelectRoute(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
        if((mousePos.x>=minX && mousePos.x<=maxX) && (mousePos.y>=minY && mousePos.y<=maxY) && 
            (hit.collider!=null && hit.collider.CompareTag("Dirt"))){
                Vector3Int selectedTile=destructableMap.WorldToCell(mousePos);
                if(routes.Contains(selectedTile)){
                    this.agent.SetDestination(destructableMap.GetCellCenterWorld(selectedTile));
                    selectedAnt.GetComponent<AntStats>().StartDigging();
                    selectedAnt.GetComponent<ExcavationMovement>().InitExcavation(selectedDestructableTile,selectedTile);
                    FinishDigMenu();
                }
        }else{
            consoleText.text=selectedAnt.name+"-Select a real route,please";
            }
    }

    void SelectStart(){
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
            if((mousePos.x>=minX && mousePos.x<=maxX) && (mousePos.y>=minY && mousePos.y<=maxY) && 
            (hit.collider!=null && hit.collider.CompareTag("Dirt"))){
                Vector3Int selectedTile=destructableMap.WorldToCell(mousePos);
                Debug.Log(selectedTile);
                selectedDestructableTile=selectedTile;
                if(excavablePath.Contains(selectedTile)){
                    isSelectingDestructableTile=false;
                    List<Vector3Int> availableRutes=GetAdjacentTileOfDiggableTile(destructableMap,selectedTile);
                    if(availableRutes.Count!=1){
                        StartSelectRouteMenu(availableRutes,selectedTile);
                    }else{
                        //GetCellCenterWorld te devuelve el PUNTO EXACTO DEL CENTRO DE UNA CELDA
                        this.agent.SetDestination(destructableMap.GetCellCenterWorld(availableRutes[0]));
                        selectedAnt.GetComponent<AntStats>().StartDigging();
                        selectedAnt.GetComponent<ExcavationMovement>().InitExcavation(selectedDestructableTile,availableRutes[0]);
                        FinishDigMenu();
                    }
                }else{
                    consoleText.text=selectedAnt.name+"-Select a real start,please";
                }
            }
    }

    void StartSelectRouteMenu(List<Vector3Int> availableRoutes,Vector3Int selectedTile){
        areMoreRutes=true;
        routes=availableRoutes;
        selectedDestructableTile=selectedTile;
        consoleText.text=selectedAnt.name+"-Select one of the available routes";
        RollBackDirtTiles();
        destructableMap.SetTile(selectedTile,selectable);
        ColorAvailableRoutes(availableRoutes);
    }

    void ColorAvailableRoutes(List<Vector3Int> availableRoutes){
        foreach(Vector3Int pos in availableRoutes){
            destructableMap.SetTile(pos,route);
        }
    }
    void RollBackRouteTiles(){
        foreach(Vector3Int pos in routes){
            destructableMap.SetTile(pos,null);
        }
    }



    public void SetSelectedAnt(GameObject ant){
        selectedAnt=ant;
    }

    // Update is called once per frame
    public void FinishDigMenu()
    {
        
        this.agent=null;
        RollBackDirtTiles();
        areMoreRutes=false;
        RollBackRouteTiles();
        routes=new List<Vector3Int>();
        digMenu.SetActive(false);
        selectedAnt.GetComponentInChildren<UIManager>(true).ShowInfo();
        consoleText.text="Waiting . . .";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneSelectable();
    }
}
