using System;
using System.Collections;
using System.Collections.Generic;
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

    public GameObject queen;

    private List<Vector3Int> path=new List<Vector3Int>();
    private HashSet<Vector3Int> excavablePath=new HashSet<Vector3Int>();

    public float originX=-5.5f;
    public int width=21;
    public int height=13;
    public float originY=-3.5f;

    public int leftCounter=7;
    public int rightCounter=7;

    public int depthCounter=8;

    public int upCounter=0;

    public int pathSize=50;

    private System.Random random = new System.Random();


    void Start()
    {
        if(path.Count==0){
            FillTilemap();
            CreateRandomPath();
            PlaceFarms();
            ObtainExcavableTiles();
            PlaceQueenAndAnts();
            //TODO: Revisar cantidad de tiles == null del path
            Debug.Log(excavablePath.Count);
        }
        BakeMap();

        
    }

    public void BakeMap(){
        NavMeshSurface n=navMesh.GetComponent<NavMeshSurface>();
        Debug.Log(n!=null);
        if(n!=null){n.BuildNavMesh();}
    }
    void ObtainExcavableTiles(){
        foreach(Vector3Int tile in path){
            List<Vector3Int> coverage=NextToDirtPositions(tile,dirtMap);
            foreach(Vector3Int position in coverage){
                excavablePath.Add(position);
            }
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

    void PlaceFarms(){
        FarmGenerator generator=farms.GetComponent<FarmGenerator>();
        generator.InitializeGeneratorAndPlaceFarms(path);
    }

    void FillTilemap(){
        dirtMap.gameObject.transform.position=new Vector3(originX,originY,0);
        //CUANDO AUMENTE EL TAMAÑO DEL MAPA, MODIFICAR LÍMITES
        for(int i=0;i<=width;i++){
            for(int j=0;j<=height;j++){
                dirtMap.SetTile(new Vector3Int(i,j,0),dirt);
                TileBase tile=dirtMap.GetTile(new Vector3Int(i,j,0));
            }
        }
    }

    void PlaceQueenAndAnts(){
        int v=random.Next(0,path.Count-1);
        queen.transform.position=new Vector3(dirtMap.CellToWorld(path[v]).x,dirtMap.CellToWorld(path[v]).y,0f);
        queen.AddComponent<QueenStats>();
        path.Remove(path[v]);
        AntGenerator antGenerator=queen.GetComponent<AntGenerator>();
        antGenerator.placeAntsIn(path,dirtMap);

    }

    public void AddWalkableTile(Vector3Int position){
        path.Add(position);
        excavablePath.Remove(position);
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
