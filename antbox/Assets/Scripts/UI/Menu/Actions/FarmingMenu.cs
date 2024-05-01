using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class FarmingMenu : MonoBehaviour
{
    [SerializeField] private GameObject farmMenu;

    [SerializeField] private Tilemap map;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private GameObject cancelActionButton;

    [SerializeField] private GameObject selectedAnt;

    private NavMeshAgent agent;

    public NavMeshAgent Agent { get => agent; set => agent = value; }

    // Start is called before the first frame update

    public void CancelFarming(){
        FarmStats[] allFarms=FindObjectsOfType<FarmStats>();
        foreach(FarmStats farm in allFarms){
            if(farm.antsOfFarm.Contains(selectedAnt)){
                farm.antsOfFarm.Remove(selectedAnt);
                farm.antsWorkingInFarm.Remove(selectedAnt);
                selectedAnt.GetComponent<AntStats>().DoNothing();
                selectedAnt.GetComponent<NavMeshAgent>().SetDestination(selectedAnt.transform.position);
                Debug.Log("He cancelado, ahora su estado es "+selectedAnt.GetComponent<AntStats>().GetAction());
                break;
            }
        }
    }


    
    public void StartFarmingMenu()
    {
        this.Agent=selectedAnt.GetComponent<NavMeshAgent>();
        CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>();
        if(anyCardDisplay!=null){
            anyCardDisplay.MakeEveryCardUnselectableAndUnselected();
        }
        selectedAnt.GetComponentInChildren<UIManager>(true).HideInfo();
        farmMenu.gameObject.SetActive(true);
        consoleText.text=selectedAnt.name+"-Select an available farm";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneUnselectable();
        ShowStateOfFarms();
    }

    void ShowStateOfFarms(){
        SelectableMaskManager[] masks=FindObjectsOfType<SelectableMaskManager>(true);
        foreach (SelectableMaskManager mask in masks)
        {
            mask.ShowRender();
        }  
    }
    void HideStateOfFarms(){
        SelectableMaskManager[] masks=FindObjectsOfType<SelectableMaskManager>(true);
        foreach (SelectableMaskManager mask in masks)
        {
            mask.HideRender();
        }  
    }

    void Update(){
        if(this.Agent==null || this.Agent.gameObject==null){
            FinishFarmingMenu();
        }
        if(!PauseMenu.isPaused){
            if(Input.GetMouseButtonDown(0)){
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
            if((mousePos.x>=MenuTool.MinX && mousePos.x<=MenuTool.MaxX) && (mousePos.y>=MenuTool.MinY && mousePos.y<=MenuTool.MaxY) && 
            (hit.collider!=null && hit.collider.CompareTag("Farm"))){
                FarmStats farm=hit.collider.gameObject.GetComponent<FarmStats>();
                if(farm.CanAntWorkInHere()){
                    Vector3Int selectedTile=map.WorldToCell(mousePos);
                    this.Agent.SetDestination(map.GetCellCenterWorld(selectedTile));
                    AntStats antStats=selectedAnt.GetComponent<AntStats>();
                    if(antStats!=null){
                        antStats.CancelAntAction();
                        antStats.StartFarming();
                    }
                    farm.AddAntToFarm(selectedAnt);
                    FinishFarmingMenu();
                }else{
                    consoleText.text=hit.collider.gameObject.name+" cannot be selected, choose another one";
                }
            }

        }
        }

    }


    public void SetSelectedAnt(GameObject ant){
        selectedAnt=ant;
    }

    // Update is called once per frame
    public void FinishFarmingMenu()
    {
        
        this.Agent=null;
        HideStateOfFarms();
        farmMenu.SetActive(false);
        if(selectedAnt!=null){
            selectedAnt.GetComponentInChildren<UIManager>(true).ShowInfo();
        }
        ContainerData.EnableGameAfterAction(consoleText);
    }
}
