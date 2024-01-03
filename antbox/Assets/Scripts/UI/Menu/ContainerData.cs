using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ContainerData : MonoBehaviour
{
    public int FOOD_CONTAINER=20;
    public int WATER_CONTAINER=20;
    public int maxCards=10;
    private System.Random random = new System.Random();
    public List<CardDisplay> cardsInHand=new List<CardDisplay>();
    public GameObject cardPlatform;
    public GameObject activityMenu;
    public List<Action> executableActions;
    public int foodValue=24;
    public int waterValue=24;
    //PUEDES AÑADIR EL GAMEOBJECT QUE CONTIENE EL COMPONENTE TAMBIEN
    public CardDisplay cardTemplate;

    public TextMeshProUGUI foodText;
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI cardText;
    public Tile dirtTile;
    public Tile stoneTile;
    public Tile diggingDirtTile1;
    public Tile diggingDirtTile2;
    public Tile diggingDirtTile3;
    // Start is called before the first frame update
    void Start()
    {
        foodText.text="F:"+FOOD_CONTAINER;
        waterText.text="W:"+WATER_CONTAINER;
        cardText.text="Cards:"+cardsInHand.Count+"/10";
        
    }

    // Update is called once per frame
    void Update()
    {
        foodText.text="F:"+FOOD_CONTAINER;
        waterText.text="W:"+WATER_CONTAINER;
        cardText.text="Cards:"+cardsInHand.Count+"/10";

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

    public void ProcessEvaluation(bool[] evaluation,bool isBoss){
        double result=0.0;
        foreach(bool point in evaluation){
            if(point) result+=evaluation.Length/10.0;
        }
        if(result>=0.5){
            
        }
        else if(result<0.5 && isBoss){
            //TODO:Si evaluación es negativa y es jefe, pillar carta negativa aleatoria y ejecutarla
        }
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

    public void AddNewCard(){
        Card[] allCards=Resources.LoadAll<Card>("Cards");
        int v=random.Next(0,allCards.Length);
        //OJO,para buscar datos con Resources, debe existir la carpeta Resources
        //Esto puede servir para hacer test, tenlo en cuenta
        CardDisplay newCard=Instantiate<CardDisplay>(cardTemplate,cardTemplate.transform.position,Quaternion.identity,cardPlatform.transform);
        newCard.card=allCards[v];
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
        Debug.Log(cardsInHand.Count);
        
    }
}
