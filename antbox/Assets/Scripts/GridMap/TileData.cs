using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private float actualResistance;
    private float maxResistance=7f;

    private static GenerationTilemap generationTilemap;
    private GiftType gift = GiftType.NOTHING;
    private int energyCostToDig=2;
    private static ContainerData containerData;
    private bool discovered = false;


    private List<AntStats> antsDiggingSameTile = new List<AntStats>();

    public TileType TileType { get => tileType; set => tileType = value; }
    public Vector3Int TilePosition { get => tilePosition; set => tilePosition = value; }
    public float ActualResistance { get => actualResistance; set => actualResistance = value; }
    public float MaxResistance { get => maxResistance; set => maxResistance = value; }
    public static GenerationTilemap GenerationTilemap { get => generationTilemap; set => generationTilemap = value; }
    public GiftType Gift { get => gift; set => gift = value; }
    public int EnergyCostToDig { get => energyCostToDig; set => energyCostToDig = value; }
    public static ContainerData ContainerData { get => containerData; set => containerData = value; }
    public bool Discovered { get => discovered; set => discovered = value; }
    public List<AntStats> AntsDiggingSameTile { get => antsDiggingSameTile; set => antsDiggingSameTile = value; }

    public TileType GetTileType(){
        return TileType;
    }

    public int GetEnergyCostToDig(){
        return EnergyCostToDig;
    }

//LOS CONSTRUCTORES NO SIRVEN CON MOHOBEHAVIOR, TIENES QUE HACER ADDCOMPONENT
    public TileData(Vector3Int position,TileType type,System.Random randomMap,GenerationTilemap generator,ContainerData container){
        if(random==null){
            random=randomMap; //Al ser static, esto debe afectar a todos los TileData
        }
        if(GenerationTilemap==null){
            GenerationTilemap=generator;
        }
        if(ContainerData==null){
            ContainerData=container;
        }
        TilePosition=position;
        TileType=type;
        if(TileType.Equals(TileType.EMPTY)){
            ActualResistance=0;
            
        }else{
            ActualResistance=MaxResistance;
            UpdateGift(random.NextDouble());
        }

    }

    public TileData(bool discovered,Vector3Int position,TileType type,System.Random randomMap,GenerationTilemap generator,ContainerData container)
    :this(position,type,randomMap,generator,container){

        this.Discovered=discovered;

    }

    void UpdateGift(double randomValue){
        if(randomValue<=0.2){
            Gift=GiftType.NOTHING;
        }else if(randomValue>0.4 && randomValue<=0.6 && GenerationTilemap.GetFarmGenerator().CanBePlaceFarmInPosition(TilePosition)
         && !TileType.Equals(TileType.STONE)){
            double v= random.NextDouble();
            Gift=v<=0.5?GiftType.WATER_FARM:GiftType.FOOD_FARM;
        }else{
            Gift=GiftType.CARD;
        }
    }

    public void DiggingTile(AntStats antStats,bool isMenuDigInUse){
        ActualResistance-=antStats.GetDiggingSpeed();
        antStats.ApplyEnergyCost(EnergyCostToDig);
        if(!AntsDiggingSameTile.Contains(antStats)){
            AntsDiggingSameTile.Add(antStats);
        }
        ProcessStateOfTile(isMenuDigInUse);
    }

    

    void ProcessGift(){
        if(this.Gift.Equals(GiftType.CARD) && ContainerData.CanAddNewCard()){
            ContainerData.AddNewCard();
        }else if(this.Gift.Equals(GiftType.FOOD_FARM)){
            ContainerData.AddNewFarm(Type.FOOD,TilePosition);
        }else if(this.Gift.Equals(GiftType.WATER_FARM)){
            ContainerData.AddNewFarm(Type.WATER,TilePosition);
        }


    }
    void ProcessStateOfTile(bool isMenuDigInUse){
        if(!isMenuDigInUse){
            GenerationTilemap.dirtMap.SetTile(TilePosition,GetTileByStateAndType());
        }
        if(TileType.Equals(TileType.DIRT) && ActualResistance<=0f){
            TileType=TileType.EMPTY;
            GenerationTilemap.dirtMap.SetTile(TilePosition,null);
            ProcessGift();
            if(!this.Gift.Equals(GiftType.WATER_FARM) || !this.Gift.Equals(GiftType.FOOD_FARM)){
                GenerationTilemap.CanWakeUpNest(TilePosition);
            }
            
        }else if(TileType.Equals(TileType.STONE) && ActualResistance<=0f){
            ActualResistance=MaxResistance;
            ProcessGift();
            UpdateGift(random.NextDouble());
        }
        List<AntStats> antsWithoutEnoughEnergy=new List<AntStats>();
        foreach(AntStats ant in AntsDiggingSameTile){
            if(ant.GetActualEnergy()<EnergyCostToDig){
                antsWithoutEnoughEnergy.Add(ant);
            }
        }
        foreach(AntStats ant in antsWithoutEnoughEnergy){
            ant.CancelAntAction();
        }
    }

    public Tile GetTileByStateAndType(){
        Tile tile=null;
        if(TileType.Equals(TileType.DIRT)){
            if(ActualResistance<MaxResistance && ActualResistance>= MaxResistance*2/4f){
                tile=ContainerData.diggingDirtTile1;
            }else if(ActualResistance< MaxResistance*2/4f && ActualResistance>= MaxResistance*1/4f){
                tile=ContainerData.diggingDirtTile2;
            }else if(ActualResistance<MaxResistance*1/4f && ActualResistance>=0f){
                tile=ContainerData.diggingDirtTile3;
            }else if(ActualResistance>=MaxResistance){
                tile=ContainerData.dirtTile;
            }
        }else if(TileType.Equals(TileType.STONE)){
            //TODO:Sprites al minar
            tile=ContainerData.stoneTile;

        }
        return tile;
    }
}
