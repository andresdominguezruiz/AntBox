using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ContainerData : MonoBehaviour
{

    [SerializeField]
    private int fOOD_CONTAINER = 20;

    [SerializeField]
    private int wATER_CONTAINER = 20;
    [SerializeField] private int minNutritionalValue=10;
    [SerializeField] private int maxNutritionalValue=60;

    [SerializeField]
    private int maxCards = 10;
    readonly System.Random random = new System.Random();

    [SerializeField]
    private List<CardDisplay> cardsInHand = new List<CardDisplay>();

    [SerializeField]
    private FarmGenerator farmGenerator;

    [SerializeField]
    private GameObject cardPlatform;

    [SerializeField]
    private GameObject activityMenu;

    [SerializeField]
    private List<Action> executableActions;

    [SerializeField]
    private int foodValue = 24;

    [SerializeField]
    private int waterValue = 24;
    //PUEDES AÑADIR EL GAMEOBJECT QUE CONTIENE EL COMPONENTE TAMBIEN

    [SerializeField]
    private CardDisplay cardTemplate;

    [SerializeField]
    private TextMeshProUGUI foodText;

    [SerializeField]
    private TextMeshProUGUI waterText;

    [SerializeField]
    private TextMeshProUGUI foodValueText;

    [SerializeField]
    private TextMeshProUGUI waterValueText;

    [SerializeField]
    private TextMeshProUGUI foodLimitText;

    [SerializeField]
    private TextMeshProUGUI waterLimitText;

    [SerializeField]
    private TextMeshProUGUI cardText;

    [SerializeField]
    private Tile dirtTile;

    [SerializeField]
    private Tile stoneTile;

    [SerializeField]
    private Tile diggingDirtTile1;

    [SerializeField]
    private Tile diggingDirtTile2;

    [SerializeField]
    private Tile diggingDirtTile3;

    [SerializeField]
    private TextMeshProUGUI counterOfExamsText;

    [SerializeField]
    private TextMeshProUGUI counterOfPassedExamsText;

    [SerializeField]
    private TextMeshProUGUI counterOfFailedExamsText;

    public int FOOD_CONTAINER { get => fOOD_CONTAINER; set => fOOD_CONTAINER = value; }
    public int WATER_CONTAINER { get => wATER_CONTAINER; set => wATER_CONTAINER = value; }
    public int MinNutritionalValue { get => minNutritionalValue; set => minNutritionalValue = value; }
    public int MaxNutritionalValue { get => maxNutritionalValue; set => maxNutritionalValue = value; }
    public int MaxCards { get => maxCards; set => maxCards = value; }

    public System.Random Random => random;

    public List<CardDisplay> CardsInHand { get => cardsInHand; set => cardsInHand = value; }
    public FarmGenerator FarmGenerator { get => farmGenerator; set => farmGenerator = value; }
    public GameObject CardPlatform { get => cardPlatform; set => cardPlatform = value; }
    public GameObject ActivityMenu { get => activityMenu; set => activityMenu = value; }
    public List<Action> ExecutableActions { get => executableActions; set => executableActions = value; }
    public int FoodValue { get => foodValue; set => foodValue = value; }
    public int WaterValue { get => waterValue; set => waterValue = value; }
    public CardDisplay CardTemplate { get => cardTemplate; set => cardTemplate = value; }
    public TextMeshProUGUI FoodText { get => foodText; set => foodText = value; }
    public TextMeshProUGUI WaterText { get => waterText; set => waterText = value; }
    public TextMeshProUGUI FoodValueText { get => foodValueText; set => foodValueText = value; }
    public TextMeshProUGUI WaterValueText { get => waterValueText; set => waterValueText = value; }
    public TextMeshProUGUI FoodLimitText { get => foodLimitText; set => foodLimitText = value; }
    public TextMeshProUGUI WaterLimitText { get => waterLimitText; set => waterLimitText = value; }
    public TextMeshProUGUI CardText { get => cardText; set => cardText = value; }
    public Tile DirtTile { get => dirtTile; set => dirtTile = value; }
    public Tile StoneTile { get => stoneTile; set => stoneTile = value; }
    public Tile DiggingDirtTile1 { get => diggingDirtTile1; set => diggingDirtTile1 = value; }
    public Tile DiggingDirtTile2 { get => diggingDirtTile2; set => diggingDirtTile2 = value; }
    public Tile DiggingDirtTile3 { get => diggingDirtTile3; set => diggingDirtTile3 = value; }
    public TextMeshProUGUI CounterOfExamsText { get => counterOfExamsText; set => counterOfExamsText = value; }
    public TextMeshProUGUI CounterOfPassedExamsText { get => counterOfPassedExamsText; set => counterOfPassedExamsText = value; }
    public TextMeshProUGUI CounterOfFailedExamsText { get => counterOfFailedExamsText; set => counterOfFailedExamsText = value; }

    // Start is called before the first frame update

    void UpdateCountersOfExams(){
        CounterOfExamsText.text="TOTAL EXAMS: "+StatisticsOfGame.Instance.counterOfExams;
        CounterOfPassedExamsText.text="PASSED EXAMS: "+StatisticsOfGame.Instance.counterOfPassedExams;
        CounterOfFailedExamsText.text="FAILED EXAMS: "+StatisticsOfGame.Instance.counterOfFailedExams;

    }

    public static void EnableGameAfterAction(TextMeshProUGUI consoleText){
        SelectableItem item=FindObjectOfType<SelectableItem>(false);
        if(item!=null){
            item.MakeEveryoneSelectable();
        }
        Clock clock=FindObjectOfType<Clock>();
        if(clock!=null){
            clock.UpdateMessageOfConsoleByEvent();
            consoleText.text=clock.messageOfEvent;
        }
        CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>();
        if(anyCardDisplay!=null){
            anyCardDisplay.MakeEveryCardSelectable();
        }
    }
    void Start()
    {
        FarmGenerator=FindObjectOfType<FarmGenerator>();
        UpdateTextOfContainer();
        
    }
    public bool CanAddNewCard(){
        return CardsInHand.Count<MaxCards;
    }
    public void AddNewFarm(Type farmType,Vector3Int tilePosition){
        FarmGenerator generator=FindObjectOfType<FarmGenerator>();
        if(generator!=null){
            generator.AddNewFarmInValidPosition(farmType,tilePosition);
        }
    }

    void UpdateTextOfContainer(){
        FoodText.text=":"+FOOD_CONTAINER;
        WaterText.text=":"+WATER_CONTAINER;
        FoodValueText.text="VALUE:"+FoodValue;
        WaterValueText.text="VALUE:"+WaterValue;
        CardText.text="Cards:"+CardsInHand.Count+"/10";
        FoodLimitText.text=FarmGenerator.foodFarms.Count+"/"+FarmGenerator.GetMaxNumberOfFarms();
        WaterLimitText.text=FarmGenerator.waterFarms.Count+"/"+FarmGenerator.GetMaxNumberOfFarms();
        UpdateCountersOfExams();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTextOfContainer();

    }

    public void ApplyEffect(ContainerEffect containerEffect){
        if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.FOOD)){
            FOOD_CONTAINER=containerEffect.MultiplicatorValue*(FOOD_CONTAINER+(int)containerEffect.SumValue);
            if(FOOD_CONTAINER<0){
                FOOD_CONTAINER=0;
            }
        }
        else if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.WATER)){
            WATER_CONTAINER=containerEffect.MultiplicatorValue*(WATER_CONTAINER+(int)containerEffect.SumValue);
            if(WATER_CONTAINER<0){
                WATER_CONTAINER=0;
            }
        }
        else if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.WATER_VALUE)){
            WaterValue=containerEffect.MultiplicatorValue*(WaterValue+(int)containerEffect.SumValue);
            if(WaterValue>MaxNutritionalValue){
                WaterValue=MaxNutritionalValue;
            }
            else if(WaterValue<MinNutritionalValue){
                WaterValue=MinNutritionalValue;
            }
        }else if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.FOOD_VALUE)){
            FoodValue=containerEffect.MultiplicatorValue*(FoodValue+(int)containerEffect.SumValue);
            if(FoodValue>MaxNutritionalValue){
                FoodValue=MaxNutritionalValue;
            }
            else if(FoodValue<MinNutritionalValue){
                FoodValue=MinNutritionalValue;
            }
        }
        else if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.MIRROR)){
            //Cambia valores nutritivos de la partida.
            int originalValue=FoodValue;
            int container=FOOD_CONTAINER;
            FoodValue=WaterValue;
            FOOD_CONTAINER=WATER_CONTAINER;
            WaterValue=originalValue;
            WATER_CONTAINER=container;
        }
    }
    public void ProcessUpdateEffectOfAction(List<ContainerEffect> containerEffects){
        foreach(ContainerEffect effect in containerEffects){
            ApplyEffect(effect);
        }
    }

    public void AddResourcesRandomly(int value,double randomValue){
        if(randomValue<=0.5){
            FOOD_CONTAINER+=value;
        }
        else{
            WATER_CONTAINER+=value;
        }
    }

    public void AddResources(int value,Type type){
        if(type.Equals(Type.FOOD)) FOOD_CONTAINER+=value;
        else WATER_CONTAINER+=value;
    }
    public void RelocateCardsInHand(){
        GameObject cardDataTemplate=CardTemplate.transform.Find("Data").gameObject;
        foreach(CardDisplay card in CardsInHand){
            GameObject cardData=card.transform.Find("Data").gameObject;
            cardData.transform.position=new Vector3(cardDataTemplate.transform.position.x+35f*CardsInHand.IndexOf(card),cardDataTemplate.transform.position.y,0);
        }
    }
    void AnalyseResultToUpdateCounters(double result,bool isExam)
    {
        if(isExam){
            if(result>=0.5){ 
                StatisticsOfGame.Instance.counterOfPassedExams++;
                Player.Instance.AddComplexity(1f);
                StatisticsOfGame.Instance.NextLevel();
            }
            else{
                StatisticsOfGame.Instance.counterOfFailedExams++;
                Player.Instance.AddComplexity(-0.5f);
            }
            StatisticsOfGame.Instance.counterOfExams++;
            UpdateCountersOfExams();
        }else{
            if(result>=0.5) {
                StatisticsOfGame.Instance.counterOfCorrectCards++;
            }
            else{
                StatisticsOfGame.Instance.counterOfFailedCards++;
            }
            StatisticsOfGame.Instance.counterOfUsedCards++;
        }

    }

    public void StartProcessEvaluation(bool[] evaluation, bool isBoss, bool applyDamageToEnemies)
    {
        StartCoroutine(ProcessEvaluation(evaluation, isBoss, applyDamageToEnemies));
    }

    IEnumerator ProcessEvaluation(bool[] evaluation, bool isBoss, bool applyDamageToEnemies)
    {
        double result = 0.0;
        foreach (bool point in evaluation)
        {
            if (point){
                result += 1 / (evaluation.Length * 1.0);
            }
        }

        AnalyseResultToUpdateCounters(result, isBoss);

        if (isBoss)
        {
            Clock clock = FindObjectOfType<Clock>();
            if (clock != null)
            {
                clock.GoBackToNothingEvent();
                clock.UpdateMessageOfConsoleByEvent();
                clock.eventTypeText.text = clock.eventType.ToString();
                yield return null; // Si necesitas una espera específica, reemplaza null por new WaitForSeconds(tiempoDeEspera)
            }
        }

        if (result >= 0.5 && !isBoss)
        {
            if (!applyDamageToEnemies)
            {
                ActionMenu actionMenu = FindObjectOfType<ActionMenu>(true);
                actionMenu.InitActions(ExecutableActions);
            }
            else
            {
                EnemyStats[] allEnemies = FindObjectsOfType<EnemyStats>(false);
                foreach (EnemyStats enemy in allEnemies)
                {
                    enemy.Hurt(enemy.Enemy.MaxHP / 5);
                }
            }
        }
        else if (result < 0.5 && isBoss)
        {
            Card[] allCards = Resources.LoadAll<Card>("Cards/PowerDowns");
            int v = Random.Next(0, allCards.Length);
            Card badCard = allCards[v];
            ExecutableActions = badCard.Actions;
            ActionMenu actionMenu = FindObjectOfType<ActionMenu>(true);
            actionMenu.InitActions(ExecutableActions);
            ExecutableActions = new List<Action>();
        }
        else
        {
            GoBackToGameAfterActivity();
        }

        yield return null;
    }

    public void AddNumberOfCards(int number){
        for(int i=0;i<number;i++){
            if(CanAddNewCard()){
                AddNewCard();
            }
            else{
                break;
            }
        }
    }
    public void GoBackToGameAfterActivity(){
        SelectableItem anyItem=FindObjectOfType<SelectableItem>();
            if(anyItem!=null){
                anyItem.MakeEveryoneSelectable();
            }
            CardTemplate.MakeEveryCardSelectable();
    }
    public void RemoveCardFromHand(CardDisplay cardDisplay){
        if(CardsInHand.Contains(cardDisplay)){
            CardsInHand.Remove(cardDisplay);
            Destroy(cardDisplay.gameObject);
            RelocateCardsInHand();

        }
    }

    public void AddCard(Card card){
        CardDisplay newCard=Instantiate<CardDisplay>(CardTemplate,CardTemplate.transform.position,Quaternion.identity,CardPlatform.transform);
        newCard.Card=card;
        newCard.ActivityMenu=ActivityMenu.GetComponent<ActivityMenu>();
        GameObject newCardData=newCard.transform.Find("Data").gameObject;
        GameObject cardDataTemplate=CardTemplate.transform.Find("Data").gameObject;
        GameObject newCardHUD=newCard.transform.Find("HUD").gameObject;
        GameObject cardHUDTemplate=CardTemplate.transform.Find("HUD").gameObject;
        newCardHUD.transform.localScale=cardHUDTemplate.transform.localScale+new Vector3(0.65f,0.65f,0.7f);
        newCardHUD.transform.position=cardHUDTemplate.transform.position;
        newCardData.transform.position=new Vector3(cardDataTemplate.transform.position.x+35f*CardsInHand.Count,cardDataTemplate.transform.position.y,0);
        newCardData.transform.localScale=cardDataTemplate.transform.localScale+new Vector3(0.5f,0.5f,0.5f);
        newCard.gameObject.SetActive(true);
        CardsInHand.Add(newCard);
    }

    public void AddNewCard(){
        Card[] allCards=Resources.LoadAll<Card>("Cards/PowerUps");
        int v=Random.Next(0,allCards.Length);
        //OJO,para buscar datos con Resources, debe existir la carpeta Resources
        //Esto puede servir para hacer test, tenlo en cuenta
        AddCard(allCards[v]);
        
    }
}
