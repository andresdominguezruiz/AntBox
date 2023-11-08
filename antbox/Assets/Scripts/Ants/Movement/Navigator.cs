using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Navigator : MonoBehaviour
{
    // Start is called before the first frame update
    private int[,] distanceMap;
    public Tilemap map;
    private List<Vector3> path;
    void Start()
    {
        this.path=new List<Vector3>();
        this.distanceMap=new int[this.map.size.x,this.map.size.y];

        var min=this.map.localBounds.min;
        for(int i=0;i<this.map.size.x;i++){
            for(int j=0;j<this.map.size.y;j++){
                int mapX=(int)(i+min.x);
                int mapY=(int)(j+min.y);

                var tile=this.map.GetTile(new Vector3Int(mapX,mapY,1));

                if(tile!=null){
                    this.distanceMap[i,j]=-1;
                }else{
                    this.distanceMap[i,j]=0;
                }
            }
        }
        
    }

    Vector3Int ToLocal(Vector3 world){
        var local=(world - this.map.transform.position)-this.map.localBounds.min;
        return new Vector3Int((int)local.x,(int)local.y,1);
    }

    Vector3 ToGlobal(Vector3Int local){
        var f_local=new Vector3(local.x,local.y,1);
        return f_local+this.transform.position+this.map.localBounds.min;
    }

    Vector3 ToGlobal(Vector3 local){
        return local+this.transform.position+this.map.localBounds.min;
    }

    // Update is called once per frame
    public List<Vector3> GetPath(Vector3 start,Vector3 end){
        return new List<Vector3>();
    }
}
