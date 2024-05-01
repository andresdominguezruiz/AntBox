using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class MultipleFarmingMenu : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private TextMeshProUGUI capacityText;
    private FarmStats selectedFarm;

    private List<AntStats> antsToPlay=new List<AntStats>();

    void UpdateAntsToPlay(AntStats ant){
        bool elseIfCondition=(selectedFarm.actualCapacity+antsToPlay.Count)<selectedFarm.GetMaxCapacity();
        MenuTool.UpdaterTool(ant,antsToPlay,elseIfCondition);
        UpdateText();
    }

    void ShowState(){
        SelectableMaskManager mask=selectedFarm.GetComponentInChildren<SelectableMaskManager>(true);
        if(mask!=null){
            mask.ShowRender();
        }
        foreach(GameObject ant in selectedFarm.antsOfFarm){
            if(ant!=null && ant.GetComponent<SelectableItem>()!=null){
                ant.GetComponent<SelectableItem>().ChangeColor(Color.red);
            }
        }
    }
    void HideState(){
        SelectableMaskManager mask=selectedFarm.GetComponentInChildren<SelectableMaskManager>(true);
        if(mask!=null){
            mask.HideRender();
        }
        SelectableItem item=selectedFarm.GetComponent<SelectableItem>();
        if(item!=null){
            item.ChangeColorOfAllAnts(true);
        }
    }

    void UpdateText(){
        capacityText.text="Capacity:"
            +(selectedFarm.actualCapacity+antsToPlay.Count)+" of "+selectedFarm.GetMaxCapacity();
            consoleText.text="You can select "
            +(selectedFarm.GetMaxCapacity()-(selectedFarm.actualCapacity+antsToPlay.Count))
            +" ants";
    }

    public void InitMultipleFarmingMenu(FarmStats farm){
        if(farm.CanAntWorkInHere()){
            selectedFarm=farm;
            this.gameObject.SetActive(true);
            ShowState();
            UpdateText();
            CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>();
            if(anyCardDisplay!=null){
                anyCardDisplay.MakeEveryCardUnselectableAndUnselected();
            }
            SelectableItem item=selectedFarm.GetComponent<SelectableItem>();
            if(item!=null){
                item.MakeEveryoneUnselectableAndUnselected();
            }
        }
        else{
            FinishMultipleFarmingMenu(false);
        }
    }

    void Update()
    {
        if(!PauseMenu.isPaused){
            if(Input.GetMouseButtonDown(0)){
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
                if(hit.collider!=null && hit.collider.CompareTag("Ant")){
                    AntStats ant=hit.collider.gameObject.GetComponent<AntStats>();
                    Debug.Log(!selectedFarm.antsOfFarm.Contains(ant.gameObject));
                    if( ant!=null &&!selectedFarm.antsOfFarm.Contains(ant.gameObject)){
                        UpdateAntsToPlay(ant);
                    }
                }
            }
        }
    }

    void SendAntsToFarm(){
        foreach(AntStats ant in antsToPlay){
            NavMeshAgent agent=ant.GetComponent<NavMeshAgent>();
            Vector3Int selectedTile=map.WorldToCell(selectedFarm.gameObject.transform.position);
            agent.SetDestination(map.GetCellCenterWorld(selectedTile));
            ant.CancelAntAction();
            ant.StartFarming();
            selectedFarm.AddAntToFarm(ant.gameObject);
        }
    }

    public void FinishMultipleFarmingMenu(bool confirmedAction){
        if(selectedFarm.CanAntWorkInHere()){
            if(confirmedAction){
                SendAntsToFarm();
            }
            antsToPlay=new List<AntStats>();
            HideState();
            ContainerData.EnableGameAfterAction(consoleText);
        }
        selectedFarm=null;
        this.gameObject.SetActive(false);
    }
}
