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

    private System.Random random = new System.Random();
    
    public UIManager itemUI;

    public UIFarmManager farmUI;

    public UIEnemyManager enemyUI;

    public ItemType type;

    public List<FarmStats> GetAllFarms(){
        List<FarmStats> list=new List<FarmStats>();
        foreach(SelectableItem item in selectableItems){
            if(item.type.Equals(ItemType.FARM)) list.Add(item.gameObject.GetComponent<FarmStats>());
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

    public void FeedAnthill(){
        ContainerData container=FindObjectOfType<ContainerData>(false);
        foreach(SelectableItem item in selectableItems){
            if( item.type.Equals(ItemType.ANT) || item.type.Equals(ItemType.QUEEN)){
                CharacterStats character=item.GetComponent<CharacterStats>();
                if(character!=null && container!=null) character.Eat(container);
            }
        }
    }
    public void HydrateAnthill(){
        ContainerData container=FindObjectOfType<ContainerData>(false);
        foreach(SelectableItem item in selectableItems){
            if( item.type.Equals(ItemType.ANT) || item.type.Equals(ItemType.QUEEN)){
                CharacterStats character=item.GetComponent<CharacterStats>();
                if(character!=null && container!=null) character.Drink(container);
            }
        }
    }

    public void SleepAnthill(){
        foreach(SelectableItem item in selectableItems){
            if(item.type.Equals(ItemType.ANT)){
                AntStats character=item.GetComponent<AntStats>();
                if(character!=null){
                    character.CancelAntAction();
                    character.GoToSleep();
                }
            }
        }
    }
    public void AttackAnthill(){
        foreach(SelectableItem item in selectableItems){
            if(item.type.Equals(ItemType.ANT)){
                AntStats character=item.GetComponent<AntStats>();
                if(character!=null){
                    character.CancelAntAction();
                    character.StartAttackingWithoutTarget();
                }
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


    
    //USAR ESTE METODO CUANDO VAYAS A AÑADIR UN NUEVO ELEMENTO SELECCIONABLE
    public void InitSelectableItem(List<Vector3Int> path,Tilemap destructableMap
    ,GameObject moveMenu,GameObject farmMenu,GameObject digMenu,ItemType itemType,GameObject attackMenu){
        AddPath(path,destructableMap);
        if(itemType.Equals(ItemType.FARM)){
            SetUIFarmManager(this.gameObject.GetComponentInChildren<UIFarmManager>(true));
            farmUI.HideInfo();
            type=ItemType.FARM;


        }
        else if(itemType.Equals(ItemType.ANT)){
            SetUIManager(this.gameObject.GetComponentInChildren<UIManager>(true));
            itemUI.HideInfo();
            this.moveMenu=moveMenu;
            this.farmMenu=farmMenu;
            this.digMenu=digMenu;
            this.attackMenu=attackMenu;
            type=ItemType.ANT;
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
        if(queenStats!=null) type=ItemType.QUEEN;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Ant") || collision.gameObject.CompareTag("Queen"))
        {
            // Ignora la colisión
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
        }
    }
    public void MakeEveryoneUnselectableButPrepareFarms(){
        for(int i=0;i<selectableItems.Count;i++){
            selectableItems[i].canBeSelected=false;
            if(selectableItems[i].gameObject.GetComponent<FarmStats>()!=null){
                FarmStats stats=selectableItems[i].gameObject.GetComponent<FarmStats>();

            }
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

    public void ChangeColorOfAllAnts(){
        foreach(SelectableItem item in selectableItems){
            if(item.type.Equals(ItemType.ANT)){
                item.ChangeColorWithoutSelecting();
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
            if(cardInHand!=null) cardInHand.HideCardsInHand();
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
