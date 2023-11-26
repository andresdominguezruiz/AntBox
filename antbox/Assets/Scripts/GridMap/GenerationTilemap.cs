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
            PlaceQueenAndAnts();
            PlaceFarms();
        }
        NavMeshSurface n=navMesh.GetComponent<NavMeshSurface>();
        Debug.Log(n!=null);
        if(n!=null){n.BuildNavMesh();}

        
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
        queen.transform.position=dirtMap.CellToWorld(path[v]);
        queen.AddComponent<QueenStats>();
        path.Remove(path[v]);
        AntGenerator antGenerator=queen.transform.GetComponent<AntGenerator>();
        antGenerator.placeAntsIn(path,dirtMap);

    }

    void CreateRandomPath()
    {
        int exit=width/2;
        Vector3Int actualTile=new Vector3Int(exit, height, 0);
        dirtMap.SetTile(actualTile,null);
        for(int i=0;i<3;i++){
            actualTile.y--;
            dirtMap.SetTile(actualTile,null);
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
