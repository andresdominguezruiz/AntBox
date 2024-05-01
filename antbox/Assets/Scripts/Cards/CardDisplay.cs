using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    readonly System.Random random = new System.Random();

    [SerializeField]
    private Card card;

    [SerializeField]
    private Image image;

    [SerializeField]
    private TextMeshProUGUI cardName;

    [SerializeField]
    private GameObject infoCanvas;

    [SerializeField]
    private GameObject activityMenu;

    [SerializeField]
    private RawImage littleTemplate;

    [SerializeField]
    private RawImage bigTemplate;

    [SerializeField]
    private bool canBeSelected = true;

    public Card Card { get => card; set => card = value; }
    public Image Image { get => image; set => image = value; }
    public TextMeshProUGUI CardName { get => cardName; set => cardName = value; }
    public GameObject InfoCanvas { get => infoCanvas; set => infoCanvas = value; }
    public GameObject ActivityMenu { get => activityMenu; set => activityMenu = value; }
    public RawImage LittleTemplate { get => littleTemplate; set => littleTemplate = value; }
    public RawImage BigTemplate { get => bigTemplate; set => bigTemplate = value; }
    public bool CanBeSelected { get => canBeSelected; set => canBeSelected = value; }

    void Start()
    {
        CardName.text=Card.Name;
        Image.sprite=Card.ArtWorks;
        if(Card.HasPassive()){
            LittleTemplate.color=new Color32(255,0,229,255);
            BigTemplate.color=new Color32(255,0,229,255);
            CardName.color=new Color32(0,255,0,255);
        }
        HideInfo();
        
    }
    public Activity[] GenerateActivitiesByComplexity(bool isBoss){
        //TODO:Añadir complejidad dependiendo de los efectos de la carta
        List<Activity> notKnownActivities=GetNotKnownActivitiesByComplexity(isBoss);
        int n=1;
        if(isBoss){
            n=10;
        }
        else{
            for(int i=(int)Player.Instance.complexityLevelOfGame;i>0 && n<=3;i--){
                if(i%3==0){
                    n++;
                }
            }
        }
        Activity[] activities=new Activity[n];
        for(int i=0;i<n;i++){
            if(notKnownActivities.Count<1){
                Player.Instance.ForgetActivities();
                notKnownActivities=GetNotKnownActivitiesByComplexity(isBoss);
            }
            int v=random.Next(0,notKnownActivities.Count);
            activities[i]=notKnownActivities[v];
            notKnownActivities.Remove(notKnownActivities[v]);
        }
        return activities;

    }
    public List<Activity> GetNotKnownActivitiesByComplexity(bool isBoss){
        HashSet<ComplexityType> complexityTypes=PickAreaOfComplexity(isBoss);
        Activity[] allActivities=Resources.LoadAll<Activity>("Activities");
        List<Activity> activitiesInComplexityRange=new List<Activity>();
        List<Activity> notKnownActivities=new List<Activity>();
        foreach(Activity activity in allActivities){
            if(complexityTypes.Contains(activity.ComplexityType)){
                activitiesInComplexityRange.Add(activity);
            }
        }
        foreach(Activity act in activitiesInComplexityRange){
            if(!Player.Instance.knownActivities.Contains(act)){
                notKnownActivities.Add(act);
            }
        }
        return notKnownActivities;
    }
    public HashSet<ComplexityType> PickAreaOfComplexity(bool isBoss){
        HashSet<ComplexityType> complexitiesToSearch=new HashSet<ComplexityType>();
        double complexity=0.0;
        int multiplicator=2;
        if(isBoss){
            int result=StatisticsOfGame.Instance.counterOfCorrectCards-StatisticsOfGame.Instance.counterOfFailedCards;
            if(result<0){
                result=0;
            }
            complexity+=Player.Instance.complexityLevelOfGame+0.25*result;
            multiplicator=3;
        }else{
            complexity+=Card.GetComplexityOfCard(Player.Instance.complexityLevelOfGame);
        }
        //Para los areas de complejidad he aprovechado los indices del enumerado, ya que estos estaban ordenados
        //, y crear otras variables para los límites me parece redundante
        if(complexity>=(int)ComplexityType.VERY_EASY*multiplicator && complexity<(int)ComplexityType.EASY*multiplicator){
            complexitiesToSearch.Add(ComplexityType.VERY_EASY);
            complexitiesToSearch.Add(ComplexityType.EASY);
        }else if(complexity>=(int)ComplexityType.EASY*multiplicator && complexity<(int)ComplexityType.MEDIUM*multiplicator){
            complexitiesToSearch.Add(ComplexityType.EASY);
            complexitiesToSearch.Add(ComplexityType.MEDIUM);
        }else if(complexity>=(int)ComplexityType.MEDIUM*multiplicator && complexity<(int)ComplexityType.HARD*multiplicator){
            complexitiesToSearch.Add(ComplexityType.MEDIUM);
            complexitiesToSearch.Add(ComplexityType.HARD);
        }else if(complexity>=(int)ComplexityType.HARD*multiplicator && complexity<(int)ComplexityType.VERY_HARD*multiplicator){
            complexitiesToSearch.Add(ComplexityType.HARD);
            complexitiesToSearch.Add(ComplexityType.VERY_HARD);
        }else if(complexity>=(int)ComplexityType.VERY_HARD*multiplicator){
            complexitiesToSearch.Add(ComplexityType.VERY_HARD);
        }

        return complexitiesToSearch;
    }

    public void DiscardCard(){
        ContainerData containerData=FindObjectOfType<ContainerData>();
        containerData.RemoveCardFromHand(this);
        containerData.GoBackToGameAfterActivity();
    }

    public void UseCard(){
        SelectableItem anyItem=FindObjectOfType<SelectableItem>();
                if(anyItem!=null){
                    anyItem.MakeEveryoneUnselectableAndUnselected();
                }
        ActivityMenu activityMenu=FindObjectOfType<ActivityMenu>(true);
        activityMenu.SetActivitiesAndStartPlaying(GenerateActivitiesByComplexity(false),false,false);
        ContainerData containerData=FindObjectOfType<ContainerData>();
        containerData.executableActions=Card.Actions;
        containerData.RemoveCardFromHand(this);
    }
    public void ShowCardData(){
        if(CanBeSelected){
                UIManager[] uIManagers=FindObjectsOfType<UIManager>(true);
                foreach(UIManager ui in uIManagers){
                    ui.HideInfo();
                }
                UIFarmManager[] uIFarmManagers=FindObjectsOfType<UIFarmManager>(true);
                foreach(UIFarmManager ui in uIFarmManagers){
                    ui.HideInfo();
                }
                SelectableItem anyItem=FindObjectOfType<SelectableItem>();
                if(anyItem!=null){
                    anyItem.MakeEveryonedUnselected();
                }

        ContainerData containerData=FindObjectOfType<ContainerData>();
        foreach(CardDisplay card in containerData.cardsInHand){
            card.InfoCanvas.gameObject.SetActive(false);
        }
        InfoCanvas.gameObject.SetActive(true);
        }
    }

    public void CancelCard(){
        SelectableItem anyItem=FindObjectOfType<SelectableItem>();
        if(anyItem!=null){
            anyItem.MakeEveryoneSelectable();
        }
        InfoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
    public void HideInfo()
    {
        InfoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
    public void MakeEveryCardUnselectableAndUnselected(){
        foreach(CardDisplay cardDisplay in FindObjectsOfType<CardDisplay>()){
            cardDisplay.CanBeSelected=false;
            cardDisplay.HideInfo();
        }
    }

    public void HideCardsInHand(){
        foreach(CardDisplay cardDisplay in FindObjectsOfType<CardDisplay>()){
            cardDisplay.HideInfo();
        }
    }
    public void MakeEveryCardSelectable(){
        foreach(CardDisplay cardDisplay in FindObjectsOfType<CardDisplay>()){
            cardDisplay.CanBeSelected=true;
        }
    }

}
