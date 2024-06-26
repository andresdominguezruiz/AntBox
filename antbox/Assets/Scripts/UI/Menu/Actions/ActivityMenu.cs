using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class ActivityMenu : MonoBehaviour
{
    public Activity[] activities;
    public bool[] answers;
    public bool isBoss=false;
    public bool applyDamageToEnemies=false;
    public Sprite correct;
    public Sprite empty;
    public Sprite incorrect;
    public int index;
    [SerializeField] private GameObject lightTemplate;
    [SerializeField] private GameObject containerForTemplates;

    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private Image optionalImage;
    [SerializeField] private Button[] options;
    [SerializeField] private GameObject[] selectedAnswers;
    private bool[] evaluation;


    public void ClickOnOption(Button selectedButton){
        if(options.Contains(selectedButton)){
            Image spriteAnswer=selectedAnswers[index].GetComponent<Image>();
            if(selectedButton.Equals(options[activities[index].CorrectAnswer])){
                spriteAnswer.sprite=correct;
                evaluation[index]=true;
                Player.Instance.knownActivities.Add(activities[index]);
            }else{
                spriteAnswer.sprite=incorrect;
                evaluation[index]=false;
            }
        }
        NextActivity();
    }

    void NextActivity(){
        index++;
        if(index>=activities.Length){
            FinishActivities();
        }
        else{
            ProcessActivity();
        }
    }

    public void SetActivitiesAndStartPlaying(Activity[] activitiesFromCard,bool isBoss,bool applyDamageToEnemies){
        activities=activitiesFromCard;
        index=0;
        Time.timeScale=0f;
        this.gameObject.SetActive(true);
        this.isBoss=isBoss;
        this.applyDamageToEnemies=applyDamageToEnemies;
        DestroyAllLights();
        ProcessActivity();
        if(activities!=null && activities.Length>0){
            selectedAnswers=new GameObject[activities.Length];
            evaluation=new bool[activities.Length];
            for(int i=0;i<activities.Length;i++){
                GameObject newLight=Instantiate(lightTemplate
                ,lightTemplate.transform.position,Quaternion.identity,containerForTemplates.transform);
                selectedAnswers[i]=newLight;
            }
        }
    }

    void ProcessActivity(){
        Activity nextActivity=activities[index];
        descriptionText.text=nextActivity.Description;
        difficultyText.text="Difficulty: "+nextActivity.ComplexityType;
        if(nextActivity.OptionalImage!=null){
            this.optionalImage.sprite=nextActivity.OptionalImage;
            this.optionalImage.gameObject.SetActive(true);
        }else{
            this.optionalImage.gameObject.SetActive(false);
        }
        int buttonIndex=0;
        foreach(Sprite option in nextActivity.Options){
            Image buttonImage=options[buttonIndex].gameObject.GetComponentsInChildren<Image>()[1];
            if(buttonImage!=null){
                buttonImage.sprite=option;
            }
            buttonIndex++;
        }
    }

    void DestroyAllLights(){
        List<GameObject> objectsToDestroy=new List<GameObject>(selectedAnswers);
        foreach(GameObject answer in objectsToDestroy){
            Destroy(answer);
        }
    }

    void FinishActivities(){
        index=0;
        Time.timeScale=1f;
        DestroyAllLights();
        this.gameObject.SetActive(false);
        ContainerData containerData=FindObjectOfType<ContainerData>();
        containerData.StartProcessEvaluation(evaluation,isBoss,applyDamageToEnemies);
        isBoss=false;
        applyDamageToEnemies=false;
    }
}
