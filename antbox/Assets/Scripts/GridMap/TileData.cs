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
public class TileData : MonoBehaviour
{
    private static System.Random random=null;
    private TileType tileType;
    private Vector3Int tilePosition;
    private int actualResistance;
    private int maxResistance=8;
    private GiftType gift=GiftType.NOTHING;
    private List<GameObject> antsDiggingTile=new List<GameObject>();

    public TileData(Vector3Int position,TileType type){
        if(random==null){
            random=FindObjectOfType<GenerationTilemap>().GetRandom(); //Al ser static, esto debe afectar a todos los TileData
        }
        tilePosition=position;
        tileType=type;
        if(tileType.Equals(TileType.EMPTY)){
            actualResistance=0;
            
        }else{
            actualResistance=maxResistance;
        }

    }

    void UpdateGift(double randomValue){
        FarmGenerator generator=FindObjectOfType<FarmGenerator>();
        if(randomValue<=0.7){
            gift=GiftType.NOTHING;
        }else if(randomValue>0.7 && randomValue<=0.8 && generator.CanBePlaceFarmInPosition(tilePosition)){
            double v= random.NextDouble();
            gift=v<=0.5?GiftType.WATER_FARM:GiftType.FOOD_FARM;
        }else{
            gift=GiftType.CARD;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
