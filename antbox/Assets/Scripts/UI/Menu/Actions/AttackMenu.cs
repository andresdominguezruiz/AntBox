using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using TMPro;
using Unity.VisualScripting;
public class AttackMenu : MonoBehaviour
{
    public bool selectingEnemy=false;

    [SerializeField] private GameObject attackMenu;

    private List<AntStats> antsToPlay=new List<AntStats>();

    [SerializeField] private Tilemap map;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private TextMeshProUGUI aditionalText;
    [SerializeField] private GameObject cancelActionButton;
    [SerializeField] private GameObject sendButton;

    [SerializeField] private GameObject battleStarter;
    public float minX=-5.5f;
    public float maxX=7f;
    public float minY=-3.5f;
    public float maxY=3f;

    public float speed=0.5f;

    private NavMeshAgent agent;

    public void SetSelectedItem(GameObject agent,bool isEnemy){
        battleStarter=agent;
        selectingEnemy=!isEnemy;
    }
    void UpdateText(){
        aditionalText.text="You will send "+antsToPlay.Count+" ants";
    }

    void UpdateAntsToPlay(AntStats ant){
        MenuTool.UpdaterTool(ant,antsToPlay);
        UpdateText();
    }

    public void StartAttackMenu(){
        Time.timeScale=0f; //ATACAR PARARA EL TIEMPO
        this.agent=battleStarter.GetComponent<NavMeshAgent>();
        CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>();
        if(anyCardDisplay!=null){
            anyCardDisplay.MakeEveryCardUnselectableAndUnselected();
        }
        string adition;
        if(selectingEnemy){
            battleStarter.GetComponentInChildren<UIManager>(true).HideInfo();
            adition=battleStarter.name+"-Select an available enemy to attack";
            this.aditionalText.gameObject.SetActive(false);
            sendButton.SetActive(false);
        }
        else{
            battleStarter.GetComponentInChildren<UIEnemyManager>(true).HideInfo();
            adition="-Select an available ant to attack to attack "+battleStarter.name;
            this.aditionalText.gameObject.SetActive(true);
            sendButton.SetActive(true);
        }
        attackMenu.gameObject.SetActive(true);
        consoleText.text=adition;
        battleStarter.GetComponent<SelectableItem>().MakeEveryoneUnselectableAndUnselected();
        BattleMovement battleMovement=battleStarter.GetComponent<BattleMovement>();
        battleMovement.UpdateTarget();
    }

    void Update(){
        if(this.agent==null || this.agent.gameObject.IsDestroyed()){
            FinishAttackMenu(false);
        }
        if(!PauseMenu.isPaused){
            if(Input.GetMouseButtonDown(0)){
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
            if((mousePos.x>=minX && mousePos.x<=maxX) && (mousePos.y>=minY && mousePos.y<=maxY) && 
            (hit.collider!=null && ((hit.collider.CompareTag("Enemy") && selectingEnemy)||(hit.collider.CompareTag("Ant") && !selectingEnemy)))){
                Transform selectedItem=hit.collider.transform;
                if(selectingEnemy && !selectedItem.gameObject.IsDestroyed()) {
                        AntStats ant=battleStarter.GetComponent<AntStats>();
                        if(ant!=null){
                            ant.StartAttacking(selectedItem);
                            FinishAttackMenu(true);
                        }
                }else{
                    AntStats ant=selectedItem.gameObject.GetComponent<AntStats>();
                    if(ant!=null){
                        UpdateAntsToPlay(ant);
                        UpdateText();
                    }
                }
            }

            }
        }
    }


    public void FinishAttackMenu(bool confirmedAction){
        Time.timeScale=1f;
        this.agent=null;
        attackMenu.SetActive(false);
        if(battleStarter!=null){
            if(selectingEnemy){
                battleStarter.GetComponentInChildren<UIManager>(true).ShowInfo();
            }
            else{
                battleStarter.GetComponentInChildren<UIEnemyManager>(true).ShowInfo();
                if(confirmedAction){
                    foreach(AntStats ant in antsToPlay){
                        ant.CancelAntAction();
                        ant.StartAttacking(battleStarter.gameObject.transform);
                    }
                }
                antsToPlay=new List<AntStats>();
            }
        }
        ContainerData.EnableGameAfterAction(consoleText);
    }
}
