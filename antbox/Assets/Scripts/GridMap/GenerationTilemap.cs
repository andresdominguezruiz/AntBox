using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerationTilemap : MonoBehaviour
{
    // Start is called before the first frame update

    public Tilemap dirtMap;
    public GameObject navMesh;

    public GameObject farms;
    public Tile dirt;
    public Tile stone;

    public GameObject queen;

    public Dictionary<Vector3Int,TileData> allTilesOfMap=new Dictionary<Vector3Int, TileData>();

    private List<Vector3Int> path=new List<Vector3Int>();
    private HashSet<Vector3Int> excavablePath=new HashSet<Vector3Int>();

    

    public float originX=-6.0f;
    public int width=23;
    public int height=15;
    public float originY=-4f;

    public int leftCounter=7;
    public int rightCounter=7;

    public int depthCounter=8;

    public int upCounter=0;

    public int pathSize=50;

    private System.Random random = new System.Random();

    public System.Random GetRandom(){
        return random;
    }

    public Tile GetTileOfTilesData(Vector3Int position){
        Tile tile=null;
        bool result=allTilesOfMap.TryGetValue(position,out TileData data);
        if(result==true && data.GetTileType().Equals(TileType.DIRT)) tile=data.GetTileByStateAndType();
        else if(result==true && data.GetTileType().Equals(TileType.STONE)) tile=stone;
        return tile;
    }
    public TileData GetTileData(Vector3Int position){
        TileData tileData=null;
        bool result=allTilesOfMap.TryGetValue(position,out TileData data);
        if(result==true) tileData=data;
        return tileData;
    }

    void Start()
    {
        if(path.Count==0){
            FillTilemap();
            CreateRandomPath();
            PlaceFarms();
            ObtainExcavableTiles();
            PlaceQueenAndAnts();
            CreateAllTilesData();
            //TODO: Revisar cantidad de tiles == null del path
        }
        BakeMap();

        
    }
    void CreateAllTilesData(){
        ContainerData containerData=FindObjectOfType<ContainerData>();
        for(int i=0;i<=width;i++){
            for(int j=0;j<=height;j++){
                Vector3Int pos=new Vector3Int(i,j,0);
                TileBase tile=dirtMap.GetTile(pos);
                if(tile==null) allTilesOfMap.Add(pos,new TileData(pos,TileType.EMPTY,random,this,containerData));
                else if(tile.Equals(dirt)) allTilesOfMap.Add(pos,new TileData(pos,TileType.DIRT,random,this,containerData));
                else if(tile.Equals(stone)) allTilesOfMap.Add(pos,new TileData(pos,TileType.STONE,random,this,containerData));
            }
        }
    }

    public void BakeMap(){
        NavMeshSurface n=navMesh.GetComponent<NavMeshSurface>();
        Debug.Log(n!=null);
        if(n!=null){n.BuildNavMesh();}
    }
    void ObtainExcavableTiles(){
        foreach(Vector3Int tile in path){
            AddNextToDirtPositionsOfPosition(tile,dirtMap);
        }
    }
    public void AddNextToDirtPositionsOfPosition(Vector3Int pos,Tilemap map){
        List<Vector3Int> coverage=NextToDirtPositions(pos,map);
        foreach(Vector3Int position in coverage){
                excavablePath.Add(position);
        }
    }
    public HashSet<Vector3Int> GetExcavableTiles(){
        return excavablePath;
    }
    public List<Vector3Int> NextToDirtPositions(Vector3Int tile,Tilemap dirtMap){
        List<Vector3Int> list=new List<Vector3Int>();
        if(dirtMap.GetTile(tile)==null){
            Vector3Int left=new Vector3Int(tile.x-1,tile.y,tile.z);
            Vector3Int right=new Vector3Int(tile.x+1,tile.y,tile.z);
            Vector3Int up=new Vector3Int(tile.x,tile.y+1,tile.z);
            Vector3Int down=new Vector3Int(tile.x,tile.y-1,tile.z);
            List<Vector3Int> options=new List<Vector3Int>{left,right,up,down};
            foreach(Vector3Int option in options){
                if(dirtMap.GetTile(option)!=null) list.Add(option);
            }
        }
        return list;

    }
    public void AddNewAnt(){
        AntGenerator antGenerator=FindObjectOfType<AntGenerator>(false);
        antGenerator.PlaceOneAnt(dirtMap);
    }

    void PlaceFarms(){
        FarmGenerator generator=farms.GetComponent<FarmGenerator>();
        generator.InitializeGeneratorAndPlaceFarms(path,random);
    }

    public FarmGenerator GetFarmGenerator(){
        return FindFirstObjectByType<FarmGenerator>();
    }

    void FillTilemap(){
        dirtMap.gameObject.transform.position=new Vector3(originX,originY,0);
        //CUANDO AUMENTE EL TAMAÑO DEL MAPA, MODIFICAR LÍMITES
        for(int i=0;i<=width;i++){
            for(int j=0;j<=height;j++){
                if(isALimit(i,j)){
                    dirtMap.SetTile(new Vector3Int(i,j,0),stone);
                }else{
                    dirtMap.SetTile(new Vector3Int(i,j,0),dirt);
                }
            }
        }
    }
    private bool isALimit(int i,int j){
        return i==0 || j==0 || i==width || j==height;
    }

    void PlaceQueenAndAnts(){
        int v=random.Next(0,path.Count-1);
        int cont=0;
        while(!CanPlaceQueenInTilePosition(path[v]) && cont<5){
            v=random.Next(0,path.Count-1);
            cont++;
        }
        queen.transform.position=new Vector3(dirtMap.CellToWorld(path[v]).x,dirtMap.CellToWorld(path[v]).y,0f);
        queen.AddComponent<QueenStats>();
        queen.GetComponent<QueenStats>().InitQueenStats(random);
        path.Remove(path[v]);
        AntGenerator antGenerator=queen.GetComponent<AntGenerator>();
        antGenerator.placeAntsIn(path,dirtMap,random);

    }

    public bool CanPlaceQueenInTilePosition(Vector3Int position){
        FarmStats[] allFarms=FindObjectsOfType<FarmStats>(false);
        List<Vector3Int> farmPositions=new List<Vector3Int>();
        foreach(FarmStats farm in allFarms){
            farmPositions.Add(dirtMap.WorldToCell(farm.gameObject.transform.position));
        }
        List<Vector3Int> coverageOfQueen=GetCoverageOfItem(position);
        bool res=true;
        foreach(Vector3Int pos in coverageOfQueen){
            if(farmPositions.Contains(pos)){
                res=false;
                break;
            }
        }
        return res;
    }

    private List<Vector3Int> GetCoverageOfItem(Vector3Int position){
        List<Vector3Int> list=new List<Vector3Int>();
        for(int i=-2;i<=2;i++){
            for(int j=-2;j<=2;j++){
                list.Add(new Vector3Int(position.x+i,position.y+j,position.z));
            }
        }
        return list;
    }

    public void AddWalkableTile(Vector3Int position){
        path.Add(position);
        List<Vector3Int> coverage=NextToDirtPositions(position,dirtMap);
        excavablePath.Remove(position);
        foreach(Vector3Int pos in coverage){
            excavablePath.Add(pos);
        }
    }

    void CreateRandomPath()
    {
        int exit=width/2;
        Vector3Int actualTile=new Vector3Int(exit, height, 0);
        dirtMap.SetTile(actualTile,null);
        path.Add(actualTile);
        for(int i=0;i<3;i++){
            actualTile.y--;
            dirtMap.SetTile(actualTile,null);
            path.Add(actualTile);
        }
        while(pathSize>0){
            bool nextTileSelected=false;
            while(!nextTileSelected){
                double v = random.NextDouble();
                if (v<0.25 && depthCounter>0){//IF CAN GO DOWN
                    actualTile=goDown(actualTile);
                    nextTileSelected=true;
                }else if(v>=0.25 && v<0.5 && rightCounter>0){//IF CAN GO RIGHT
                    actualTile=goRight(actualTile);
                    nextTileSelected=true;
                }
                else if( v>=0.5 && v<0.75 && leftCounter>0){//IF CAN GO LEFT
                    actualTile=goLeft(actualTile);
                    nextTileSelected=true;
                }else if(v>=0.75 && v<=1.0 && upCounter>0){//IF CAN GO UP
                    actualTile=goUp(actualTile);
                    nextTileSelected=true;
                }

                if(nextTileSelected && dirtMap.GetTile(actualTile)!=null){
                    path.Add(actualTile);
                    dirtMap.SetTile(actualTile,null);
                    pathSize--;
                }
            }
            

        }

    }

    Vector3Int goDown(Vector3Int actualTile)
    {
        actualTile.y--;
        depthCounter--;
        upCounter++;
        return actualTile;
    }

    Vector3Int goRight(Vector3Int actualTile){
        actualTile.x++;
        rightCounter--;
        leftCounter++;
        return actualTile;
    }

    Vector3Int goLeft(Vector3Int actualTile){
        actualTile.x--;
        leftCounter--;
        rightCounter++;
        return actualTile;
    }

    Vector3Int goUp(Vector3Int actualTile){
        actualTile.y++;
        depthCounter++;
        upCounter--;
        return actualTile;
    }


    
}
