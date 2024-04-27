using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum ItemType{
    ANT,QUEEN,FARM,ENEMY
}

[System.Serializable]
public enum AnthillAction{
    FEED,HYDRATE,ATTACK,SLEEP
}
public class SelectableItem : MonoBehaviour
{
    public bool isSelected=false;
    public bool canBeSelected=true;
    private Color32 originalColor;
    private Color32 selectedColor;

    public GameObject moveMenu;
    public GameObject farmMenu;
    public GameObject digMenu;
    public GameObject attackMenu;

    public static List<SelectableItem> selectableItems=new List<SelectableItem>();
    public static HashSet<Vector3> availablePath=new HashSet<Vector3>();
    
    public UIManager itemUI;

    public UIFarmManager farmUI;

    public UIEnemyManager enemyUI;

    public ItemType type;

    public List<FarmStats> GetAllFarms(){
        List<FarmStats> list=new List<FarmStats>();
        foreach(SelectableItem item in selectableItems){
            if(item.type.Equals(ItemType.FARM)){
                list.Add(item.gameObject.GetComponent<FarmStats>());
            }
        }
        return list;
    }

    //solo dejar esos tipos porque este método lo usan los enemigos, no los aliados
    public List<Transform> GetItemsByTarget(TargetType target){
        List<Transform> items=new List<Transform>();
        foreach(SelectableItem item in selectableItems){
            if((target.Equals(TargetType.ANT) && item.type.Equals(ItemType.ANT)) ||
             (target.Equals(TargetType.FARM) && item.type.Equals(ItemType.FARM)) ||
             (target.Equals(TargetType.QUEEN) && item.type.Equals(ItemType.QUEEN)) ||
             (target.Equals(TargetType.ANTHILL) &&
              (item.type.Equals(ItemType.ANT) || item.type.Equals(ItemType.QUEEN)))){
                items.Add(item.transform);
              }
        }
        return items;
    }

    public void RemoveSelectableItem(){
        selectableItems.Remove(this);
    }

    public void RecoverAnthillAction(bool isFeeding){
        ContainerData container=FindObjectOfType<ContainerData>(false);
        foreach(SelectableItem item in selectableItems){
            CharacterStats character=item.GetComponent<CharacterStats>();
            bool condition=(item.type.Equals(ItemType.ANT) || item.type.Equals(ItemType.QUEEN))
            && character!=null && container!=null;
            if( condition && isFeeding){
                character.Eat(container);
            }else if(condition){
                character.Drink(container);
            }
        }
    }

    public void AntFromAnthillAction(bool toSleep){
        foreach(SelectableItem item in selectableItems){
            AntStats character=item.GetComponent<AntStats>();
            if(character!=null && toSleep){
                character.CancelAntAction();
                character.GoToSleep();
            }else if(item.type.Equals(ItemType.ANT) && character!=null){
                character.CancelAntAction();
                character.StartAttackingWithoutTarget();
            }
        }
    }
    public void AddPath(List<Vector3Int> path,Tilemap destructablePath){
        foreach(Vector3Int localPosition in path){
            Vector3 worldPosition=destructablePath.CellToWorld(localPosition);
            availablePath.Add(worldPosition);
        }
    }

    public void InitSelectableItemOfEnemy(UIEnemyManager uIEnemy,GameObject attackMenu){
        SetUIEnemyManager(uIEnemy);
        enemyUI.HideInfo();
        type=ItemType.ENEMY;
        this.attackMenu=attackMenu;
    }

    public void InitAntItem(){
        SetUIManager(this.gameObject.GetComponentInChildren<UIManager>(true));
        itemUI.HideInfo();
        type=ItemType.ANT;
    }



    
    //USAR ESTE METODO CUANDO VAYAS A AÑADIR UN NUEVO ELEMENTO SELECCIONABLE
    public void InitSelectableItem(List<Vector3Int> path,Tilemap destructableMap,ItemType itemType){
        AddPath(path,destructableMap);
        if(itemType.Equals(ItemType.FARM)){
            SetUIFarmManager(this.gameObject.GetComponentInChildren<UIFarmManager>(true));
            farmUI.HideInfo();
            type=ItemType.FARM;


        }
        else if(itemType.Equals(ItemType.ANT)){
            InitAntItem();
        }
        else{
            SetUIManager(this.gameObject.GetComponentInChildren<UIManager>(true));
            itemUI.HideInfo();
            type=ItemType.QUEEN;

        }

    }
    void Start(){
        selectableItems.Add(this);
        originalColor=this.gameObject.GetComponentInChildren<SpriteRenderer>().color;
        selectedColor=Color.green;
        QueenStats queenStats=this.GetComponent<QueenStats>();
        if(queenStats!=null){
            type=ItemType.QUEEN;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Ant") || collision.gameObject.CompareTag("Queen"))
        {
            // Ignora la colisión
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
    }

    public void MakeEveryoneUnselectable(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=false;
        }
    }
    public void MakeEveryoneUnselectableAndUnselected(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=false;
            if(selectableItems[i].isSelected){
                selectableItems[i].isSelected=false;
                selectableItems[i].ChangeColor(originalColor);
            }
        }
    }

    public void MakeEveryonedUnselected(){
        for(int i=0;i<selectableItems.Count;i++){
            if(selectableItems[i].isSelected){
                selectableItems[i].isSelected=false;
                selectableItems[i].ChangeColor(originalColor);
            }
        }
    }

    public void HideAllInfo(){
        for(int i=0;i<selectableItems.Count;i++){
            if(selectableItems[i].gameObject.GetComponentInChildren<UIManager>(true)!=null){
                selectableItems[i].gameObject.GetComponentInChildren<UIManager>(true).HideInfo();
            }else if(selectableItems[i].gameObject.GetComponentInChildren<UIFarmManager>(true)!=null){
                selectableItems[i].gameObject.GetComponentInChildren<UIFarmManager>(true).HideInfo();
            }
        }
    }

    public void MakeEveryoneSelectable(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=true;
        }
    }

    void Update(){
        if(isSelected && canBeSelected){
            if(itemUI!=null){
                itemUI.ShowInfo();
            }else if(farmUI!=null){
                farmUI.ShowInfo();
            }else if(enemyUI!=null){
                enemyUI.ShowInfo();
            }
        }else{
            if(itemUI!=null){
                itemUI.HideInfo();
            }else if(farmUI!=null){
                farmUI.HideInfo();
            }else if(enemyUI!=null){
                enemyUI.HideInfo();
            }
        }

    }
    public void ChangeToOriginal(){
        SpriteRenderer sprite=this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if(sprite!=null){
            sprite.material.color=originalColor;
        }
    }

    public void ChangeColorWithoutSelecting(){
        SpriteRenderer sprite=this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if(sprite!=null && sprite.material.color.Equals(originalColor)){
            sprite.material.color=selectedColor;
        }
        else if(sprite!=null && sprite.material.color.Equals(selectedColor)){
            sprite.material.color=originalColor;
        }
        else if(sprite!=null){
            sprite.material.color=originalColor;
        }
    }

    public void ChangeColorOfAllAnts(bool toOriginal=false){
        foreach(SelectableItem item in selectableItems){
            if(item.type.Equals(ItemType.ANT) && !toOriginal){
                item.ChangeColorWithoutSelecting();
            }else if(item.type.Equals(ItemType.ANT) && toOriginal){
                item.ChangeToOriginal();
            }
        }
    }

    public void ChangeColor(Color32 newColor){
        SpriteRenderer sprite=this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if(sprite!=null){
            sprite.material.color=newColor;
        }
    }

    void OnMouseDown() {
        if(!PauseMenu.isPaused){
            if(canBeSelected && !isSelected){
            CardDisplay cardInHand=FindObjectOfType<CardDisplay>(false);
            if(cardInHand!=null){
                cardInHand.HideCardsInHand();
            }
            isSelected=true;
            ChangeColor(this.selectedColor);
            if(itemUI!=null && !itemUI.isQueen){
                MoveMenu menu=moveMenu.GetComponent<MoveMenu>();
                menu.SetSelectedAnt(this.gameObject);
                FarmingMenu otherMenu=farmMenu.GetComponent<FarmingMenu>();
                otherMenu.SetSelectedAnt(this.gameObject);
                DigMenu dig=digMenu.GetComponent<DigMenu>();
                dig.SetSelectedAnt(this.gameObject);
            }
            if((itemUI!=null && !itemUI.isQueen) || (itemUI==null && enemyUI!=null)){
                AttackMenu attack=attackMenu.GetComponent<AttackMenu>();
                attack.SetSelectedItem(this.gameObject,itemUI==null);
            }
            foreach(SelectableItem item in selectableItems){
                if(item!=this){
                    item.isSelected=false;
                    item.ChangeColor(item.originalColor);
                } 
            }
        }else if(canBeSelected && isSelected){ //This allows to unselect items
            isSelected=false;
            ChangeColor(originalColor);
        }
        }
    }


    public void SetUIManager(UIManager ui){
        itemUI=ui;
    }

    public void SetUIFarmManager(UIFarmManager ui){
        farmUI=ui;
    }

    public void SetUIEnemyManager(UIEnemyManager ui){
        enemyUI=ui;
    }



    public UIManager GetUIManager(){
        return itemUI;
    }

    
    
        
    
}
