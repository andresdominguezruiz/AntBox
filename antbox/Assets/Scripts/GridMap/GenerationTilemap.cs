using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using NavMeshPlus.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class GenerationTilemap : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private int cardsPerNewLevel=3;

    public Tilemap dirtMap;
    public Tilemap walkableMap;
    public GameObject navMesh;

    public GameObject farms;
    public Tile dirt;
    public Tile stone;

    public GameObject queen;

    public Dictionary<Vector3Int,TileData> allTilesOfMap=new Dictionary<Vector3Int, TileData>();

    public List<Vector3Int> path=new List<Vector3Int>();
    public List<Nest> nestsOfLevel=new List<Nest>();
    public List<Nest> awakeNests=new List<Nest>();
    private HashSet<Vector3Int> excavablePath=new HashSet<Vector3Int>();
    public List<Color32> colorsForDirtMap=new List<Color32>{
        new Color32(255,255,255,255),
        new Color32(149,255,126,255),
        new Color32(0,255,255,255),
        new Color32(0,196,255,255),
        new Color32(175,170,255,255)
    };
    public List<Color32> colorsForWalkableMap=new List<Color32>{
        new Color32(248,255,65,110),
        new Color32(219,65,255,110),
        new Color32(66,175,255,110),
        new Color32(66,255,163,110),
        new Color32(255,88,65,110)
    };

    

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

    public void WakeUpRandomNest(){
        IEnumerable<Nest> availableNests= nestsOfLevel.Except(awakeNests);
        List<Nest> list=availableNests.ToList();
        int r=random.Next(0,list.Count-1);
        Nest nest=list[r];
        WakeUpNest(nest);
        

    }

    public void WakeUpNest(Nest nest){
        nest.sleeping=false;
        awakeNests.Add(nest);
        foreach(Vector3Int pos in nest.nestPositions){
            dirtMap.SetTile(pos,null);
            BakeMap();
        }
        //es necesario hacer 2 foreach porque debo añadir las tiles excavables despues
        foreach(Vector3Int pos in nest.nestPositions){
            AddNextToDirtPositionsOfPosition(pos,dirtMap);
        }

        NestManager manager=GetComponent<NestManager>();
        BakeMap();
        if(manager!=null){
            manager.ReleaseEnemies(nest,dirtMap);
        }
    }

    public void CanWakeUpNest(Vector3Int diggedTile){
        foreach(Nest nest in nestsOfLevel){
            if(nest.triggerPositions.Contains(diggedTile) && nest.sleeping){
                WakeUpNest(nest);
            }
        }
    }

    public Tile GetTileOfTilesData(Vector3Int position){
        Tile tile=null;
        bool result=allTilesOfMap.TryGetValue(position,out TileData data);
        if(result==true && data.GetTileType().Equals(TileType.DIRT)){
            tile=data.GetTileByStateAndType();
        }
        else if(result==true && data.GetTileType().Equals(TileType.STONE)){
            tile=stone;
        }
        return tile;
    }
    public TileData GetTileData(Vector3Int position){
        TileData tileData=null;
        bool result=allTilesOfMap.TryGetValue(position,out TileData data);
        if(result==true){
            tileData=data;
        }
        return tileData;
    }

    void AddNewCardsToPlayer(){
        ContainerData containerData=FindObjectOfType<ContainerData>();
        if(containerData!=null){
            containerData.AddNumberOfCards(cardsPerNewLevel);
        }
    }

    void Start()
    {
        dirtMap.color=colorsForDirtMap[StatisticsOfGame.Instance.colorIndex];
        walkableMap.color=colorsForWalkableMap[StatisticsOfGame.Instance.colorIndex];
        Player.Instance.GiveCardsToContainer();
        AddNewCardsToPlayer();
        if(path.Count==0){
            FillTilemap();
            CreateRandomPath();
            PlaceFarms();
            ObtainExcavableTiles();
            PlaceQueenAndAnts();
            CreateAllTilesData();
            int numberOfNests=8;
            if((StatisticsOfGame.Instance.actualLevel+4)<8) numberOfNests=StatisticsOfGame.Instance.actualLevel+4;
            CreateNests(numberOfNests);
            awakeNests=new List<Nest>();

        }
        BakeMap();

        
    }

    void CreateNests(int numberOfNest){
        List<Vector3Int> nestPositions=GetAvailableNestPositions();
        while(numberOfNest>0 && nestPositions.Count>0){
            CreateOneNest(nestPositions);
            numberOfNest--;
        }
    }

    void CreateOneNest(List<Vector3Int> availablePos){
        //1ºcoger posicion aleatoria e inicializar contador
        //2º añadir posicion al nido y buscar otra direccion que este permitida
        //3º al finalizar contador o direcciones=> añadir a posiciones no disponibles la covertura
        Debug.Log("BJCDDBCDCBJDC:"+availablePos.Count);
        int numberOfPositions=availablePos.Count>8?random.Next(4,8):random.Next(1,availablePos.Count);
        int randomIndex=random.Next(0,availablePos.Count);
        OptimalNest optimal=StatisticsOfGame.Instance.GetOptimalNestByLevelAndRandomValue(random.NextDouble());
        Nest nest=new Nest(optimal.enemyNumber,optimal.nestType,optimal.maxEnemyLevel);
        Vector3Int start=availablePos[randomIndex];
        nest.AddNewNestPosition(start,dirtMap);
        numberOfPositions--;
        while(numberOfPositions>0){
            List<Vector3Int> options=OnlyNextToDirtPositions(start,dirtMap);
            options.RemoveAll((Vector3Int pos)=> !availablePos.Contains(pos)
             && nest.nestPositions.Contains(pos)); //coger posiciones no repetidas y válidas
             if(options.Count==0){
                break;
             }
             else{
                randomIndex=random.Next(0,options.Count);
                start=options[randomIndex];
                nest.AddNewNestPosition(start,dirtMap);
                numberOfPositions--;
             }
        }
        nestsOfLevel.Add(nest);
        availablePos.RemoveAll((Vector3Int pos)=> nest.GetCoverOfNest().Contains(pos));
    }

    public List<Vector3Int> GetAvailableNestPositions(){
        List<Vector3Int> availablePositions=new List<Vector3Int>();
        foreach(KeyValuePair<Vector3Int,TileData> pair in allTilesOfMap){
            if(pair.Value.GetTileType().Equals(TileType.DIRT) 
            && CanStartNest(pair.Key)){
               availablePositions.Add(pair.Key);

            }
        }
        return availablePositions;
    }

    public bool CanStartNest(Vector3Int position){
        bool res=true;
        for(int i=-2;i<=2;i++){
            for(int j=-2;j<=2;j++){
                Vector3Int pos=new Vector3Int(position.x+i,position.y+j,position.z);
                res=dirtMap.GetTile(pos)!=null && dirtMap.GetTile(pos).Equals(dirt);
                if(!res){
                    break;
                }
            }
            if(!res){
                break;
            }
        }
        return res;
    }

    void CreateAllTilesData(){
        ContainerData containerData=FindObjectOfType<ContainerData>();
        for(int i=0;i<=width;i++){
            for(int j=0;j<=height;j++){
                Vector3Int pos=new Vector3Int(i,j,0);
                TileBase tile=dirtMap.GetTile(pos);
                TileType tileType=TileType.EMPTY;
                if(tile!=null && tile.Equals(dirt)){
                    tileType=TileType.DIRT;
                }
                else if(tile!=null && tile.Equals(stone)){
                    tileType=TileType.STONE;
                }
                bool discovered=tile==null;
                allTilesOfMap.Add(pos,new TileData(discovered,pos,tileType,random,this,containerData));
            }
        }
    }

    public void BakeMap(){
        NavMeshSurface n=navMesh.GetComponent<NavMeshSurface>();
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

    public List<Vector3Int> OnlyNextToDirtPositions(Vector3Int tile,Tilemap dirtMap){
        List<Vector3Int> list=new List<Vector3Int>();
        if(dirtMap.GetTile(tile)!=null){
            Vector3Int left=new Vector3Int(tile.x-1,tile.y,tile.z);
            Vector3Int right=new Vector3Int(tile.x+1,tile.y,tile.z);
            Vector3Int up=new Vector3Int(tile.x,tile.y+1,tile.z);
            Vector3Int down=new Vector3Int(tile.x,tile.y-1,tile.z);
            List<Vector3Int> options=new List<Vector3Int>{left,right,up,down};
            foreach(Vector3Int option in options){
                if(dirtMap.GetTile(option)!=null && dirtMap.GetTile(option).Equals(dirt)){
                    list.Add(option);
                }
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

    void CreateLittleRandomPathForEnemies(Vector3Int actualTile,int pathSize
    ,int rightCounter,int leftCounter,int upCounter,int depthCounter)
    {

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
                    actualTile=goDownManually(actualTile,depthCounter,upCounter);
                    nextTileSelected=true;
                }else if(v>=0.25 && v<0.5 && rightCounter>0){//IF CAN GO RIGHT
                    actualTile=goRightManually(actualTile,rightCounter,leftCounter);
                    nextTileSelected=true;
                }
                else if( v>=0.5 && v<0.75 && leftCounter>0){//IF CAN GO LEFT
                    actualTile=goLeftManually(actualTile,rightCounter,leftCounter);
                    nextTileSelected=true;
                }else if(v>=0.75 && v<=1.0 && upCounter>0){//IF CAN GO UP
                    actualTile=goUpManually(actualTile,upCounter,depthCounter);
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

    //----------------------------------------------------------

    Vector3Int goDownManually(Vector3Int actualTile,int depthCounter,int upCounter)
    {
        actualTile.y--;
        depthCounter--;
        upCounter++;
        return actualTile;
    }

    Vector3Int goRightManually(Vector3Int actualTile,int rightCounter,int leftCounter){
        actualTile.x++;
        rightCounter--;
        leftCounter++;
        return actualTile;
    }

    Vector3Int goLeftManually(Vector3Int actualTile,int rightCounter, int leftCounter){
        actualTile.x--;
        leftCounter--;
        rightCounter++;
        return actualTile;
    }

    Vector3Int goUpManually(Vector3Int actualTile,int upCounter,int depthCounter){
        actualTile.y++;
        depthCounter++;
        upCounter--;
        return actualTile;
    }


    
}
