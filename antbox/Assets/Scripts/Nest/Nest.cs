using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum NestType{
    RANDOM,EARTHWORMS,WORMS,ANTS
}


public class Nest
{
    public HashSet<Vector3Int> triggerPositions=new HashSet<Vector3Int>();
    public HashSet<Vector3Int> nestPositions=new HashSet<Vector3Int>();
    public int numberOfEnemies=1;
    public NestType nestType=NestType.RANDOM;

    public int maxLevel=1;

    public bool sleeping=true;


    public Nest(int numberOfEnemies,NestType type,int max){
        this.numberOfEnemies=numberOfEnemies;
        nestType=type;
        maxLevel=max;
    }

    public void AddNewNestPosition(Vector3Int position,Tilemap dirtMap){
        if(dirtMap!=null && dirtMap.GetTile(position)!=null){
            nestPositions.Add(position);
            List<Vector3Int> adjacencies=new List<Vector3Int>(){
            new Vector3Int(position.x-1,position.y,position.z),
            new Vector3Int(position.x+1,position.y,position.z),
            new Vector3Int(position.x,position.y+1,position.z),
            new Vector3Int(position.x,position.y-1,position.z)
        };
        //RemoveAll funciona a base de predicados
        adjacencies.RemoveAll((Vector3Int pos)=>nestPositions.Contains(pos));
        triggerPositions.UnionWith(adjacencies);
    }

    }



//IMPORTANTE, ten en cuenta de que se podría dar el caso de que compartan triggerPositions
    public HashSet<Vector3Int> GetCoverOfNest(){
        HashSet<Vector3Int> positions=new HashSet<Vector3Int>();
        //AddRange es el AddAll de c#, PERO UTILIZA UnionWith que sino al ensamblar no lo detecta
        positions.UnionWith(nestPositions);
        positions.UnionWith(triggerPositions);
        return positions;
    }
    
}
