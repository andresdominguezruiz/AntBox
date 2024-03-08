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

    [SerializeField] private Tilemap map;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private GameObject cancelActionButton;

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

    public void StartAttackMenu(){
        Time.timeScale=0f; //ATACAR PARARA EL TIEMPO
        this.agent=battleStarter.GetComponent<NavMeshAgent>();
        CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>();
        if(anyCardDisplay!=null) anyCardDisplay.MakeEveryCardUnselectableAndUnselected();
        string aditionalText="";
        if(selectingEnemy){
            battleStarter.GetComponentInChildren<UIManager>(true).HideInfo();
            aditionalText=battleStarter.name+"-Select an available enemy to attack";
        }
        else{
            battleStarter.GetComponentInChildren<UIEnemyManager>(true).HideInfo();
            aditionalText="-Select an available ant to attack to attack "+battleStarter.name;
        }
        attackMenu.gameObject.SetActive(true);
        consoleText.text=aditionalText;
        battleStarter.GetComponent<SelectableItem>().MakeEveryoneUnselectable();
        BattleMovement battleMovement=battleStarter.GetComponent<BattleMovement>();
        battleMovement.UpdateTarget();
    }

    void Update(){
        if(this.agent==null || this.agent.gameObject.IsDestroyed())FinishAttackMenu();
        if(!PauseMenu.isPaused){
            if(Input.GetMouseButtonDown(0)){
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
            if((mousePos.x>=minX && mousePos.x<=maxX) && (mousePos.y>=minY && mousePos.y<=maxY) && 
            (hit.collider!=null && ((hit.collider.CompareTag("Enemy") && selectingEnemy)||(hit.collider.CompareTag("Ant") && !selectingEnemy)))){
                BattleMovement battleMovement=battleStarter.GetComponent<BattleMovement>();
                Transform selectedItem=hit.collider.transform;
                if(selectingEnemy && !selectedItem.gameObject.IsDestroyed()) {
                        AntStats ant=battleStarter.GetComponent<AntStats>();
                        if(ant!=null) ant.StartAttacking(selectedItem);
                }else{
                    AntStats ant=selectedItem.gameObject.GetComponent<AntStats>();
                    if(ant!=null) ant.StartAttacking(battleStarter.transform);
                }
                FinishAttackMenu();
            }

        }
        }
    }


    public void FinishAttackMenu(){
        Time.timeScale=1f;
        this.agent=null;
        attackMenu.SetActive(false);
        if(battleStarter!=null){
            if(selectingEnemy) battleStarter.GetComponentInChildren<UIManager>(true).ShowInfo();
            else battleStarter.GetComponentInChildren<UIEnemyManager>(true).ShowInfo();
            battleStarter.GetComponent<SelectableItem>().MakeEveryoneSelectable();
        }else{
            SelectableItem item=FindObjectOfType<SelectableItem>(false);
            item.MakeEveryoneSelectable();
        }
        Clock clock=FindObjectOfType<Clock>();
        if(clock!=null){
            clock.UpdateMessageOfConsoleByEvent();
            consoleText.text=clock.messageOfEvent;
        }
        CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>();
        if(anyCardDisplay!=null) anyCardDisplay.MakeEveryCardSelectable();
        selectingEnemy=false;
    }
}
