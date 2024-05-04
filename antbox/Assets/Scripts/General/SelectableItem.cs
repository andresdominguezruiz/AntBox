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
    [SerializeField]
    private bool isSelected = false;

    [SerializeField]
    private bool canBeSelected = true;

    [SerializeField]
    private Color32 originalColor;

    [SerializeField]
    private Color32 selectedColor;

    [SerializeField]
    private GameObject moveMenu;

    [SerializeField]
    private GameObject farmMenu;

    [SerializeField]
    private GameObject digMenu;

    [SerializeField]
    private GameObject attackMenu;

    private static List<SelectableItem> selectableItems = new List<SelectableItem>();
    private static HashSet<Vector3> availablePath = new HashSet<Vector3>();

    [SerializeField]
    private UIManager itemUI;

    [SerializeField]
    private UIFarmManager farmUI;

    [SerializeField]
    private UIEnemyManager enemyUI;

    [SerializeField]
    private ItemType type;

    public bool IsSelected { get => isSelected; set => isSelected = value; }
    public bool CanBeSelected { get => canBeSelected; set => canBeSelected = value; }
    public Color32 OriginalColor { get => originalColor; set => originalColor = value; }
    public Color32 SelectedColor { get => selectedColor; set => selectedColor = value; }
    public GameObject MoveMenu { get => moveMenu; set => moveMenu = value; }
    public GameObject FarmMenu { get => farmMenu; set => farmMenu = value; }
    public GameObject DigMenu { get => digMenu; set => digMenu = value; }
    public GameObject AttackMenu { get => attackMenu; set => attackMenu = value; }
    public static List<SelectableItem> SelectableItems { get => selectableItems; set => selectableItems = value; }
    public static HashSet<Vector3> AvailablePath { get => availablePath; set => availablePath = value; }
    public UIManager ItemUI { get => itemUI; set => itemUI = value; }
    public UIFarmManager FarmUI { get => farmUI; set => farmUI = value; }
    public UIEnemyManager EnemyUI { get => enemyUI; set => enemyUI = value; }
    public ItemType Type { get => type; set => type = value; }

    public List<FarmStats> GetAllFarms(){
        List<FarmStats> list=new List<FarmStats>();
        foreach(SelectableItem item in SelectableItems){
            if(item.Type.Equals(ItemType.FARM)){
                list.Add(item.gameObject.GetComponent<FarmStats>());
            }
        }
        return list;
    }

    //solo dejar esos tipos porque este método lo usan los enemigos, no los aliados
    public List<Transform> GetItemsByTarget(TargetType target){
        List<Transform> items=new List<Transform>();
        foreach(SelectableItem item in SelectableItems){
            if((target.Equals(TargetType.ANT) && item.Type.Equals(ItemType.ANT)) ||
             (target.Equals(TargetType.FARM) && item.Type.Equals(ItemType.FARM)) ||
             (target.Equals(TargetType.QUEEN) && item.Type.Equals(ItemType.QUEEN)) ||
             (target.Equals(TargetType.ANTHILL) &&
              (item.Type.Equals(ItemType.ANT) || item.Type.Equals(ItemType.QUEEN)))){
                items.Add(item.transform);
              }
        }
        return items;
    }

    public void RemoveSelectableItem(){
        SelectableItems.Remove(this);
    }

    public void RecoverAnthillAction(bool isFeeding){
        ContainerData container=FindObjectOfType<ContainerData>(false);
        foreach(SelectableItem item in SelectableItems){
            CharacterStats character=item.GetComponent<CharacterStats>();
            bool condition=(item.Type.Equals(ItemType.ANT) || item.Type.Equals(ItemType.QUEEN))
            && character!=null && container!=null;
            if( condition && isFeeding){
                character.Eat(container);
            }else if(condition){
                character.Drink(container);
            }
        }
    }

    public void AntFromAnthillAction(bool toSleep){
        foreach(SelectableItem item in SelectableItems){
            AntStats character=item.GetComponent<AntStats>();
            if(character!=null && toSleep){
                character.CancelAntAction();
                character.GoToSleep();
            }else if(item.Type.Equals(ItemType.ANT) && character!=null){
                character.CancelAntAction();
                character.StartAttackingWithoutTarget();
            }
        }
    }
    public void AddPath(List<Vector3Int> path,Tilemap destructablePath){
        foreach(Vector3Int localPosition in path){
            Vector3 worldPosition=destructablePath.CellToWorld(localPosition);
            AvailablePath.Add(worldPosition);
        }
    }

    public void InitSelectableItemOfEnemy(UIEnemyManager uIEnemy,GameObject attackMenu){
        SetUIEnemyManager(uIEnemy);
        EnemyUI.HideInfo();
        Type=ItemType.ENEMY;
        this.AttackMenu=attackMenu;
    }

    public void InitAntItem(){
        SetUIManager(this.gameObject.GetComponentInChildren<UIManager>(true));
        ItemUI.HideInfo();
        Type=ItemType.ANT;
    }



    
    //USAR ESTE METODO CUANDO VAYAS A AÑADIR UN NUEVO ELEMENTO SELECCIONABLE
    public void InitSelectableItem(List<Vector3Int> path,Tilemap destructableMap,ItemType itemType){
        AddPath(path,destructableMap);
        if(itemType.Equals(ItemType.FARM)){
            SetUIFarmManager(this.gameObject.GetComponentInChildren<UIFarmManager>(true));
            FarmUI.HideInfo();
            Type=ItemType.FARM;


        }
        else if(itemType.Equals(ItemType.ANT)){
            InitAntItem();
        }
        else{
            SetUIManager(this.gameObject.GetComponentInChildren<UIManager>(true));
            ItemUI.HideInfo();
            Type=ItemType.QUEEN;

        }

    }
    void Start(){
        SelectableItems.Add(this);
        OriginalColor=this.gameObject.GetComponentInChildren<SpriteRenderer>().color;
        SelectedColor=Color.green;
        QueenStats queenStats=this.GetComponent<QueenStats>();
        if(queenStats!=null){
            Type=ItemType.QUEEN;
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
        for(int i=0;i<SelectableItems.Count;i++){
            SelectableItems[i].CanBeSelected=false;
        }
    }
    public void MakeEveryoneUnselectableAndUnselected(){
        for(int i=0;i<SelectableItems.Count;i++){
            SelectableItems[i].CanBeSelected=false;
            if(SelectableItems[i].IsSelected){
                SelectableItems[i].IsSelected=false;
                SelectableItems[i].ChangeColor(OriginalColor);
            }
        }
    }

    public void MakeEveryonedUnselected(){
        for(int i=0;i<SelectableItems.Count;i++){
            if(SelectableItems[i].IsSelected){
                SelectableItems[i].IsSelected=false;
                SelectableItems[i].ChangeColor(OriginalColor);
            }
        }
    }

    public void HideAllInfo(){
        for(int i=0;i<SelectableItems.Count;i++){
            if(SelectableItems[i].gameObject.GetComponentInChildren<UIManager>(true)!=null){
                SelectableItems[i].gameObject.GetComponentInChildren<UIManager>(true).HideInfo();
            }else if(SelectableItems[i].gameObject.GetComponentInChildren<UIFarmManager>(true)!=null){
                SelectableItems[i].gameObject.GetComponentInChildren<UIFarmManager>(true).HideInfo();
            }
        }
    }

    public void MakeEveryoneSelectable(){
        for(int i=0;i<SelectableItems.Count;i++){
            SelectableItems[i].CanBeSelected=true;
        }
    }

    void Update(){
        if(IsSelected && CanBeSelected){
            if(ItemUI!=null){
                ItemUI.ShowInfo();
            }else if(FarmUI!=null){
                FarmUI.ShowInfo();
            }else if(EnemyUI!=null){
                EnemyUI.ShowInfo();
            }
        }else{
            if(ItemUI!=null){
                ItemUI.HideInfo();
            }else if(FarmUI!=null){
                FarmUI.HideInfo();
            }else if(EnemyUI!=null){
                EnemyUI.HideInfo();
            }
        }

    }
    public void ChangeToOriginal(){
        SpriteRenderer sprite=this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if(sprite!=null){
            sprite.material.color=OriginalColor;
        }
    }

    public void ChangeColorWithoutSelecting(){
        SpriteRenderer sprite=this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if(sprite!=null && sprite.material.color.Equals(OriginalColor)){
            sprite.material.color=SelectedColor;
        }
        else if(sprite!=null && sprite.material.color.Equals(SelectedColor)){
            sprite.material.color=OriginalColor;
        }
        else if(sprite!=null){
            sprite.material.color=OriginalColor;
        }
    }

    public void ChangeColorOfAllAnts(bool toOriginal=false){
        foreach(SelectableItem item in SelectableItems){
            if(item.Type.Equals(ItemType.ANT) && !toOriginal){
                item.ChangeColorWithoutSelecting();
            }else if(item.Type.Equals(ItemType.ANT) && toOriginal){
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
            if(CanBeSelected && !IsSelected){
            CardDisplay cardInHand=FindObjectOfType<CardDisplay>(false);
            if(cardInHand!=null){
                cardInHand.HideCardsInHand();
            }
            IsSelected=true;
            ChangeColor(this.SelectedColor);
            if(ItemUI!=null && !ItemUI.IsQueen){
                MoveMenu menu=MoveMenu.GetComponent<MoveMenu>();
                menu.SetSelectedAnt(this.gameObject);
                FarmingMenu otherMenu=FarmMenu.GetComponent<FarmingMenu>();
                otherMenu.SetSelectedAnt(this.gameObject);
                DigMenu dig=DigMenu.GetComponent<DigMenu>();
                dig.SetSelectedAnt(this.gameObject);
            }
            if((ItemUI!=null && !ItemUI.IsQueen) || (ItemUI==null && EnemyUI!=null)){
                AttackMenu attack=AttackMenu.GetComponent<AttackMenu>();
                attack.SetSelectedItem(this.gameObject,ItemUI==null);
            }
            foreach(SelectableItem item in SelectableItems){
                if(item!=this){
                    item.IsSelected=false;
                    item.ChangeColor(item.OriginalColor);
                } 
            }
        }else if(CanBeSelected && IsSelected){ //This allows to unselect items
            IsSelected=false;
            ChangeColor(OriginalColor);
        }
        }
    }


    public void SetUIManager(UIManager ui){
        ItemUI=ui;
    }

    public void SetUIFarmManager(UIFarmManager ui){
        FarmUI=ui;
    }

    public void SetUIEnemyManager(UIEnemyManager ui){
        EnemyUI=ui;
    }



    public UIManager GetUIManager(){
        return ItemUI;
    }

    
    
        
    
}
