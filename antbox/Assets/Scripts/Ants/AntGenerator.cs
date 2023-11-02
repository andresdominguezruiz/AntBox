using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AntGenerator : MonoBehaviour
{
    public GameObject ant;

    public int initialSize=5;


    private System.Random random = new System.Random();


    public void placeAntsIn(List<Vector3Int> path,Tilemap map){
        for(int i=0;i<initialSize;i++){
            int v=random.Next(0,path.Count-1);
            GameObject newAnt=Instantiate(ant,map.CellToWorld(path[v]),Quaternion.identity);
            newAnt.AddComponent<AntStats>();
            path.Remove(path[v]);
        }
    }
}
