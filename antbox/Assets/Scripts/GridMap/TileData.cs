using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType{
    DIRT,STONE,MAGMA,EMPTY
}

public enum GiftType{
    NOTHING,CARD,WATER_FARM,FOOD_FARM
}
public class TileData
{
    private static System.Random random=null;

    private TileType tileType;
    private Vector3Int tilePosition;
    public float actualResistance;
    private float maxResistance=7f;

    private static GenerationTilemap generationTilemap;
    private GiftType gift=GiftType.NOTHING;
    private int energyCostToDig=2;
    private static ContainerData containerData;

    public List<AntStats> antsDiggingSameTile=new List<AntStats>();

    public TileType GetTileType(){
        return tileType;
    }

    public int GetEnergyCostToDig(){
        return energyCostToDig;
    }

//LOS CONSTRUCTORES NO SIRVEN CON MOHOBEHAVIOR, TIENES QUE HACER ADDCOMPONENT
    public TileData(Vector3Int position,TileType type,System.Random randomMap,GenerationTilemap generator,ContainerData container){
        if(random==null){
            random=randomMap; //Al ser static, esto debe afectar a todos los TileData
        }
        if(generationTilemap==null){
            generationTilemap=generator;
        }
        if(containerData==null){
            containerData=container;
        }
        tilePosition=position;
        tileType=type;
        if(tileType.Equals(TileType.EMPTY)){
            actualResistance=0;
            
        }else{
            actualResistance=maxResistance;
            UpdateGift(random.NextDouble());
        }

    }

    void UpdateGift(double randomValue){
        if(randomValue<=0.7){
            gift=GiftType.NOTHING;
        }else if(randomValue>0.7 && randomValue<=0.8 && generationTilemap.GetFarmGenerator().CanBePlaceFarmInPosition(tilePosition)){
            double v= random.NextDouble();
            gift=v<=0.5?GiftType.WATER_FARM:GiftType.FOOD_FARM;
        }else{
            gift=GiftType.CARD;
        }
    }

    public void DiggingTile(AntStats antStats,bool isMenuDigInUse){
        actualResistance-=antStats.GetDiggingSpeed();
        antStats.ApplyEnergyCost(energyCostToDig);
        if(!antsDiggingSameTile.Contains(antStats)) antsDiggingSameTile.Add(antStats);
        ProcessStateOfTile(isMenuDigInUse);
    }

    
    void UpdateDirectionsAndTilesData(){

    }

    void ProcessGift(){

    }
    void ProcessStateOfTile(bool isMenuDigInUse){
        if(!isMenuDigInUse){
            generationTilemap.dirtMap.SetTile(tilePosition,GetTileByStateAndType());
        }
        if(tileType.Equals(TileType.DIRT) && actualResistance<=0f){
            generationTilemap.dirtMap.SetTile(tilePosition,null);
            
        }else if(tileType.Equals(TileType.STONE) && actualResistance<=0f){
            actualResistance=maxResistance;
        }
        List<AntStats> antsWithoutEnoughEnergy=new List<AntStats>();
        foreach(AntStats ant in antsDiggingSameTile){
            if(ant.GetActualEnergy()<energyCostToDig) antsWithoutEnoughEnergy.Add(ant);
        }
        foreach(AntStats ant in antsWithoutEnoughEnergy){
            ant.CancelAntAction();
        }
    }

    public Tile GetTileByStateAndType(){
        Tile tile=null;
        if(tileType.Equals(TileType.DIRT)){
            if(actualResistance<maxResistance && actualResistance>= maxResistance*2/4f){
                tile=containerData.diggingDirtTile1;
            }else if(actualResistance< maxResistance*2/4f && actualResistance>= maxResistance*1/4f){
                tile=containerData.diggingDirtTile2;
            }else if(actualResistance<maxResistance*1/4f && actualResistance>=0f){
                tile=containerData.diggingDirtTile3;
            }else if(actualResistance>=maxResistance){
                tile=containerData.dirtTile;
            }
        }else if(tileType.Equals(TileType.STONE)){
            //TODO:Sprites al minar
            tile=containerData.stoneTile;

        }
        return tile;
    }
}
