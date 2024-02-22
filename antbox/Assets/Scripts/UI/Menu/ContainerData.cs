using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ContainerData : MonoBehaviour
{
    public int FOOD_CONTAINER=20;
    public int WATER_CONTAINER=20;
    [SerializeField] private int minNutritionalValue=10;
    [SerializeField] private int maxNutritionalValue=60;
    public int maxCards=10;
    private System.Random random = new System.Random();
    public List<CardDisplay> cardsInHand=new List<CardDisplay>();
    private FarmGenerator farmGenerator;
    public GameObject cardPlatform;
    public GameObject activityMenu;
    public List<Action> executableActions;
    public int foodValue=24;
    public int waterValue=24;
    //PUEDES AÑADIR EL GAMEOBJECT QUE CONTIENE EL COMPONENTE TAMBIEN
    public CardDisplay cardTemplate;

    public TextMeshProUGUI foodText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI foodValueText;
    public TextMeshProUGUI waterValueText;

    public TextMeshProUGUI foodLimitText;
    public TextMeshProUGUI waterLimitText;
    public TextMeshProUGUI cardText;
    public Tile dirtTile;
    public Tile stoneTile;
    public Tile diggingDirtTile1;
    public Tile diggingDirtTile2;
    public Tile diggingDirtTile3;

    public TextMeshProUGUI counterOfExamsText;
    public TextMeshProUGUI counterOfPassedExamsText;
    public TextMeshProUGUI counterOfFailedExamsText;

    // Start is called before the first frame update

    void UpdateCountersOfExams(){
        counterOfExamsText.text="TOTAL EXAMS: "+StatisticsOfGame.Instance.counterOfExams;
        counterOfPassedExamsText.text="PASSED EXAMS: "+StatisticsOfGame.Instance.counterOfPassedExams;
        counterOfFailedExamsText.text="FAILED EXAMS: "+StatisticsOfGame.Instance.counterOfFailedExams;

    }
    void Start()
    {
        farmGenerator=FindObjectOfType<FarmGenerator>();
        UpdateTextOfContainer();
        
    }
    public bool CanAddNewCard(){
        return cardsInHand.Count<maxCards;
    }
    public void AddNewFarm(Type farmType,Vector3Int tilePosition){
        FarmGenerator generator=FindObjectOfType<FarmGenerator>();
        if(generator!=null){
            generator.AddNewFarmInValidPosition(farmType,tilePosition);
        }
    }

    void UpdateTextOfContainer(){
        foodText.text="F:"+FOOD_CONTAINER;
        waterText.text="W:"+WATER_CONTAINER;
        foodValueText.text="VALUE:"+foodValue;
        waterValueText.text="VALUE:"+waterValue;
        cardText.text="Cards:"+cardsInHand.Count+"/10";
        foodLimitText.text=farmGenerator.foodFarms.Count+"/"+farmGenerator.GetMaxNumberOfFarms();
        waterLimitText.text=farmGenerator.waterFarms.Count+"/"+farmGenerator.GetMaxNumberOfFarms();
        UpdateCountersOfExams();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTextOfContainer();

    }

    public void ApplyEffect(ContainerEffect containerEffect){
        if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.FOOD)){
            FOOD_CONTAINER=containerEffect.multiplicatorValue*(FOOD_CONTAINER+(int)containerEffect.sumValue);
            if(FOOD_CONTAINER<0) FOOD_CONTAINER=0;
        }
        else if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.WATER)){
            WATER_CONTAINER=containerEffect.multiplicatorValue*(WATER_CONTAINER+(int)containerEffect.sumValue);
            if(WATER_CONTAINER<0) WATER_CONTAINER=0;
        }
        else if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.WATER_VALUE)){
            waterValue=containerEffect.multiplicatorValue*(waterValue+(int)containerEffect.sumValue);
            if(waterValue>maxNutritionalValue) waterValue=maxNutritionalValue;
            else if(waterValue<minNutritionalValue) waterValue=minNutritionalValue;
        }else if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.FOOD_VALUE)){
            foodValue=containerEffect.multiplicatorValue*(foodValue+(int)containerEffect.sumValue);
            if(foodValue>maxNutritionalValue) foodValue=maxNutritionalValue;
            else if(foodValue<minNutritionalValue) foodValue=minNutritionalValue;
        }
        else if(containerEffect.containerEffect.Equals(UpdateEffectOnContainer.MIRROR)){
            //Cambia valores nutritivos de la partida.
            int originalValue=foodValue;
            int container=FOOD_CONTAINER;
            foodValue=waterValue;
            FOOD_CONTAINER=WATER_CONTAINER;
            waterValue=originalValue;
            WATER_CONTAINER=container;
        }
    }
    public void ProcessUpdateEffectOfAction(List<ContainerEffect> containerEffects){
        foreach(ContainerEffect effect in containerEffects){
            ApplyEffect(effect);
        }
    }

    public void AddResources(int value,Type type){
        if(type.Equals(Type.FOOD)) FOOD_CONTAINER+=value;
        else WATER_CONTAINER+=value;
    }
    public void RelocateCardsInHand(){
        GameObject cardDataTemplate=cardTemplate.transform.Find("Data").gameObject;
        foreach(CardDisplay card in cardsInHand){
            GameObject cardData=card.transform.Find("Data").gameObject;
            cardData.transform.position=new Vector3(cardDataTemplate.transform.position.x+35f*cardsInHand.IndexOf(card),cardDataTemplate.transform.position.y,0);
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
            if(result>=0.5) StatisticsOfGame.Instance.counterOfCorrectCards++;
            else StatisticsOfGame.Instance.counterOfFailedCards++;
            StatisticsOfGame.Instance.counterOfUsedCards++;
        }

    }

    public void ProcessEvaluation(bool[] evaluation,bool isBoss){
        double result=0.0;
        foreach(bool point in evaluation){
            if(point) result+=1/(evaluation.Length*1.0);
        }
        AnalyseResultToUpdateCounters(result,isBoss);
        if(isBoss){
            Clock clock=FindObjectOfType<Clock>();
            if(clock!=null){
                clock.GoBackToNothingEvent();
                clock.UpdateMessageOfConsoleByEvent();
                clock.eventTypeText.text=clock.eventType.ToString();
            }
        } 
        if(result>=0.5 && !isBoss){
            ActionMenu actionMenu=FindObjectOfType<ActionMenu>(true);
            actionMenu.InitActions(executableActions);
        }
        else if(result<0.5 && isBoss){
            //TODO:Si evaluación es negativa y es jefe, pillar carta negativa aleatoria y ejecutarla
            Card[] allCards=Resources.LoadAll<Card>("Cards/PowerDowns");
            int v=random.Next(0,allCards.Length);
            Card badCard=allCards[v];
            executableActions=badCard.actions;
            ActionMenu actionMenu=FindObjectOfType<ActionMenu>(true);
            actionMenu.InitActions(executableActions);
        }
        else{ //Si el resultado a sido negativo, vuelve a la normalidad
            GoBackToGameAfterActivity();
        }
    }
    public void GoBackToGameAfterActivity(){
        SelectableItem anyItem=FindObjectOfType<SelectableItem>();
            if(anyItem!=null){
                anyItem.MakeEveryoneSelectable();
            }
            cardTemplate.MakeEveryCardSelectable();
    }
    public void RemoveCardFromHand(CardDisplay cardDisplay){
        if(cardsInHand.Contains(cardDisplay)){
            cardsInHand.Remove(cardDisplay);
            Destroy(cardDisplay.gameObject);
            RelocateCardsInHand();

        }
    }

    public void AddCard(Card card){
        CardDisplay newCard=Instantiate<CardDisplay>(cardTemplate,cardTemplate.transform.position,Quaternion.identity,cardPlatform.transform);
        newCard.card=card;
        newCard.activityMenu=activityMenu;
        GameObject newCardData=newCard.transform.Find("Data").gameObject;
        GameObject cardDataTemplate=cardTemplate.transform.Find("Data").gameObject;
        GameObject newCardHUD=newCard.transform.Find("HUD").gameObject;
        GameObject cardHUDTemplate=cardTemplate.transform.Find("HUD").gameObject;
        newCardHUD.transform.localScale=cardHUDTemplate.transform.localScale+new Vector3(0.65f,0.65f,0.7f);
        newCardHUD.transform.position=cardHUDTemplate.transform.position;
        newCardData.transform.position=new Vector3(cardDataTemplate.transform.position.x+35f*cardsInHand.Count,cardDataTemplate.transform.position.y,0);
        newCardData.transform.localScale=cardDataTemplate.transform.localScale+new Vector3(0.5f,0.5f,0.5f);
        newCard.gameObject.SetActive(true);
        cardsInHand.Add(newCard);
    }

    public void AddNewCard(){
        Card[] allCards=Resources.LoadAll<Card>("Cards/PowerUps");
        int v=random.Next(0,allCards.Length);
        //OJO,para buscar datos con Resources, debe existir la carpeta Resources
        //Esto puede servir para hacer test, tenlo en cuenta
        AddCard(allCards[v]);
        
    }
}
