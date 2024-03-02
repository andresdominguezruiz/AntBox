using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class ActivityMenu : MonoBehaviour
{
    public Activity[] activities;
    public bool[] answers;
    public bool isBoss=false;
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
            if(selectedButton.Equals(options[activities[index].correctAnswer])){
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
        if(index>=activities.Length) FinishActivities();
        else ProcessActivity();
    }

    public void SetActivitiesAndStartPlaying(Activity[] activitiesFromCard,bool isBoss){
        activities=activitiesFromCard;
        index=0;
        Time.timeScale=0f;
        this.gameObject.SetActive(true);
        this.isBoss=isBoss;
        ProcessActivity();
        if(activities!=null && activities.Length>0){
            selectedAnswers=new GameObject[activities.Length];
            evaluation=new bool[activities.Length];
            for(int i=0;i<activities.Length;i++){
                GameObject newLight=Instantiate<GameObject>(lightTemplate
                ,lightTemplate.transform.position,Quaternion.identity,containerForTemplates.transform);
                selectedAnswers[i]=newLight;
            }
        }
    }

    void ProcessActivity(){
        Activity nextActivity=activities[index];
        descriptionText.text=nextActivity.description;
        difficultyText.text="Difficulty: "+nextActivity.complexityType;
        if(nextActivity.optionalImage!=null){
            this.optionalImage.sprite=nextActivity.optionalImage;
            this.optionalImage.gameObject.SetActive(true);
        }else{
            this.optionalImage.gameObject.SetActive(false);
        }
        int buttonIndex=0;
        foreach(Sprite option in nextActivity.options){
            Image buttonImage=options[buttonIndex].gameObject.GetComponentsInChildren<Image>()[1];
            if(buttonImage!=null) buttonImage.sprite=option;
            buttonIndex++;
        }
    }

    void FinishActivities(){
        index=0;
        Time.timeScale=1f;
        List<GameObject> objectsToDestroy=new List<GameObject>(selectedAnswers);
        foreach(GameObject answer in objectsToDestroy) Destroy(answer);
        this.gameObject.SetActive(false);
        ContainerData containerData=FindObjectOfType<ContainerData>();
        containerData.ProcessEvaluation(evaluation,isBoss);
        isBoss=false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
