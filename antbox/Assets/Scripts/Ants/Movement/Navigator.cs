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

    public bool mapWasUpdated=false;
    void Start()
    {
        UpdateDistanceMap();
        
    }
    void Update(){
        if(mapWasUpdated){
            UpdateDistanceMap();
            mapWasUpdated=false;
        }
    }

    private void UpdateDistanceMap(){
        this.path=new List<Vector3>();
        this.distanceMap=new int[this.map.size.x,this.map.size.y];
        var min=this.map.localBounds.min;
        for(int i=0;i<this.map.size.x;i++){
            for(int j=0;j<this.map.size.y;j++){
                int mapX=(int)(i+min.x);
                int mapY=(int)(j+min.y);

                var tile=this.map.GetTile(new Vector3Int(mapX,mapY,0));

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
        return new Vector3Int((int)local.x,(int)local.y,0);
    }

    Vector3 ToGlobal(Vector3Int local){
        var f_local=new Vector3(local.x,local.y,1);
        return f_local+this.transform.position+this.map.localBounds.min;
    }

    Vector3 ToGlobal(Vector3 local){
        return local+this.transform.position+this.map.localBounds.min;
    }

    private void GetPathUsingLocalCoordinates(Vector3Int localStart,Vector3Int localTarget){
        //Clear map
        for(int i=0;i<this.map.size.x;i++){
            for(int j=0;j<this.map.size.y;j++){
                if(this.distanceMap[i,j]!=-1) this.distanceMap[i,j]=0;
            }
        }
        this.path.Clear();
        if((this.distanceMap[localStart.x,localStart.y]!=0 || this.distanceMap[localTarget.x,localTarget.y]!=0)){
            return;
        }
        int distance=1;

            Queue<Vector3Int> visitedCells=new Queue<Vector3Int>();
            this.distanceMap[localTarget.x,localTarget.y]=distance;
            visitedCells.Enqueue(localTarget);
            Debug.Log(visitedCells.Count);
            while(visitedCells.Count!=0 && visitedCells.Count<1000){
                distance+=1;
                var cell=visitedCells.Dequeue();
                this.Visit(visitedCells,distance,cell.x,cell.y+1); //UP
                this.Visit(visitedCells,distance,cell.x,cell.y-1); //DOWN
                this.Visit(visitedCells,distance,cell.x+1,cell.y); //RIGHT
                this.Visit(visitedCells,distance,cell.x-1,cell.y); //LEFT
            }
            this.ComputePath(localStart);
    }
    private void ComputePath(Vector3Int localStart){
        Vector3Int currentLocalTitle=localStart;
        bool isWorking=true;
        while(isWorking){
            path.Add(this.ToGlobal(currentLocalTitle));
            var (x,y)= ((int)currentLocalTitle.x,(int)currentLocalTitle.y);
            var d=this.distanceMap[x,y];
            isWorking=false;
            if(this.distanceMap[x+1,y]<d && this.distanceMap[x+1,y]!=-1){
                currentLocalTitle.Set(x+1,y,0);
                isWorking=true;
                continue;
            }
            if(this.distanceMap[x-1,y]<d && this.distanceMap[x-1,y]!=-1){
                currentLocalTitle.Set(x-1,y,0);
                isWorking=true;
                continue;
            }
            if(this.distanceMap[x,y+1]<d && this.distanceMap[x,y+1]!=-1){
                currentLocalTitle.Set(x,y+1,0);
                isWorking=true;
                continue;
            }
            if(this.distanceMap[x,y-1]<d && this.distanceMap[x,y-1]!=-1){
                currentLocalTitle.Set(x,y-1,0);
                isWorking=true;
                continue;
            }
            
            
        }
    }

    private void Visit(Queue<Vector3Int> visitedCells,int distance,int x,int y){
        if(x<0 || x>=this.map.size.x) return;
        if(y<0 || y>=this.map.size.y) return;
        if(this.distanceMap[x,y]==0){
            this.distanceMap[x,y]=distance;
            visitedCells.Enqueue(new Vector3Int(x,y,0));
        }
    }

    // Update is called once per frame
    public List<Vector3> GetPath(Vector3 start,Vector3 end){
        var lStart=this.ToLocal(start);
        Debug.Log("m:"+lStart);
        var lEnd=this.ToLocal(end);
        Debug.Log("m:"+lEnd);
        this.GetPathUsingLocalCoordinates(lStart,lEnd);
        Debug.Log(this.path.Count);
        for(int i=0;i<this.path.Count;i++){
            var loc=this.path[i]+Vector3.one*0.5f;
            loc.z=1;
            this.path[i]=loc;
        }
        return this.path;
    }
}
