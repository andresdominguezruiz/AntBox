using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FarmGenerator : MonoBehaviour
{
    private System.Random random = new System.Random();

    public GameObject dirtMap;


    public GameObject waterFarmBase;

    public GameObject foodFarmBase;

    public List<GameObject> waterFarms;
    public List<GameObject> foodFarms;
    private List<Vector3Int> coveredPositions=new List<Vector3Int>();

    private List<Vector3Int> availablePath;
    



    public void InitializeGeneratorAndPlaceFarms(List<Vector3Int> path,System.Random random){
        this.random=random;
        availablePath=path;
        FarmPlacerInGrid(true);
        FarmPlacerInGrid(false);
        Destroy(waterFarmBase);
        Destroy(foodFarmBase);

    }

    public void FarmPlacerInGrid(bool placeWaterFarm){
        int v=random.Next(0,availablePath.Count-1);
        Tilemap map=dirtMap.GetComponent<Tilemap>();
        Vector3Int position=availablePath[v];
        int contBreaker=0;
        while(!CanBePlaceFarmInPosition(position) && contBreaker<10){
            v=random.Next(0,availablePath.Count-1);
            position=availablePath[v];
            contBreaker++;
        }
        if(placeWaterFarm) PlaceWaterFarm(map,position);
        else PlaceFoodFarm(map,position);
        availablePath.Remove(availablePath[v]);
        DestroyAroundFarmAndAddCoverage(position,map);
    }

    public bool CanBePlaceFarmInPosition(Vector3Int position){
        List<Vector3Int> myCoverage= GetCoverageOfFarm(position);
        //ESTA HECHO DE TAL FORMA QUE HAYA DOS CELDAS DE DIFERENCIA CON CADA GRANJA
        //, DE ESTA FORMA CADA COBERTURA DE CADA GRANJA SE DISTINGUE EN LA INTERFAZ
        bool res=true;
        Tilemap map=dirtMap.GetComponent<Tilemap>();
        TileBase stone=FindObjectOfType<ContainerData>().stoneTile;
        foreach(Vector3Int pos in myCoverage){
            if(coveredPositions.Contains(pos) || stone.Equals(map.GetTile(pos))){
                res=false;
                break;
            }
        }
        return res;
    }

    private void PlaceWaterFarm(Tilemap map,Vector3Int position){
        GameObject newWaterFarm=Instantiate(waterFarmBase,map.CellToWorld(position),Quaternion.identity,waterFarmBase.transform.parent);
        newWaterFarm.name="WaterFarm-"+waterFarms.Count;
        newWaterFarm.AddComponent<FarmStats>();
        newWaterFarm.GetComponent<FarmStats>().InitWaterFarm(false,random);
        newWaterFarm.GetComponentInChildren<UIFarmManager>().UpdateCanvasWithFarmStats(newWaterFarm.GetComponent<FarmStats>());
        newWaterFarm.AddComponent<SelectableItem>();
        newWaterFarm.GetComponent<SelectableItem>().InitSelectableItem(availablePath,map,null,null,null,ItemType.FARM);
        waterFarms.Add(newWaterFarm);
    }

    private void PlaceFoodFarm(Tilemap map,Vector3Int position){
        GameObject newFoodFarm=Instantiate(foodFarmBase,map.CellToWorld(position),Quaternion.identity,foodFarmBase.transform.parent);
        newFoodFarm.name="FoodFarm-"+foodFarms.Count;
        newFoodFarm.AddComponent<FarmStats>();
        newFoodFarm.GetComponent<FarmStats>().InitFoodFarm(false,random);
        newFoodFarm.GetComponentInChildren<UIFarmManager>().UpdateCanvasWithFarmStats(newFoodFarm.GetComponent<FarmStats>());
        newFoodFarm.AddComponent<SelectableItem>();
        newFoodFarm.GetComponent<SelectableItem>().InitSelectableItem(availablePath,map,null,null,null,ItemType.FARM);
        foodFarms.Add(newFoodFarm);
    }

    private List<Vector3Int> GetAdjacentPositionsFromPositionOfFarm(Vector3Int position){
        List<Vector3Int> list=new List<Vector3Int>();
        for(int i=-1;i<=1;i++){
            for(int j=-1;j<=1;j++){
                list.Add(new Vector3Int(position.x+i,position.y+j,position.z));
            }
        }
        return list;
    }
    private List<Vector3Int> GetCoverageOfFarm(Vector3Int position){
        List<Vector3Int> list=new List<Vector3Int>();
        for(int i=-2;i<=2;i++){
            for(int j=-2;j<=2;j++){
                list.Add(new Vector3Int(position.x+i,position.y+j,position.z));
            }
        }
        return list;
    }

    public void DestroyAroundFarmAndAddCoverage(Vector3Int position,Tilemap map){
        List<Vector3Int> list=GetAdjacentPositionsFromPositionOfFarm(position);
        foreach(Vector3Int pos in list){
            if(map.GetTile(pos)!=null){
                map.SetTile(pos,null);
                availablePath.Add(pos);
            }
        }
        List<Vector3Int> myCoverage=GetCoverageOfFarm(position);
        foreach(Vector3Int pos in myCoverage){
            if(map.GetTile(pos)!=null){
                coveredPositions.Add(pos);
            }
        }
    }
}
