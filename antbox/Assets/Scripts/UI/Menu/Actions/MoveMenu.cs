using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class MoveMenu : MonoBehaviour
{
    [SerializeField] private GameObject moveMenu;

    [SerializeField] private Tilemap map;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private GameObject cancelActionButton;

    [SerializeField] private GameObject selectedAnt;



    private NavMeshAgent agent;

    public NavMeshAgent Agent { get => agent; set => agent = value; }

    public void StartMoveMenu()
    {
        Time.timeScale=0f;
        this.Agent=selectedAnt.GetComponent<NavMeshAgent>();
        CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>();
        if(anyCardDisplay!=null){
            anyCardDisplay.MakeEveryCardUnselectableAndUnselected();
        }
        selectedAnt.GetComponentInChildren<UIManager>(true).HideInfo();
        moveMenu.gameObject.SetActive(true);
        consoleText.text=selectedAnt.name+"-Select an accessible area";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneUnselectable();
    }

    void Update(){
        if(this.Agent==null || this.Agent.gameObject==null){
            FinishMoveMenu();
        }
        if(Input.GetMouseButtonDown(0) && !PauseMenu.isPaused){
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
            if((mousePos.x>=MenuTool.MinX && mousePos.x<=MenuTool.MaxX) && (mousePos.y>=MenuTool.MinY && mousePos.y<=MenuTool.MaxY) && 
            (hit.collider==null || !hit.collider.CompareTag("Dirt"))){
                Vector3Int selectedTile=map.WorldToCell(mousePos);
                this.Agent.SetDestination(map.GetCellCenterWorld(selectedTile));
                FinishMoveMenu();
            }
        }

    }


    public void SetSelectedAnt(GameObject ant){
        selectedAnt=ant;
    }

    // Update is called once per frame
    public void FinishMoveMenu()
    {
        Time.timeScale=1f;
        this.Agent=null;
        moveMenu.SetActive(false);
        if(selectedAnt!=null){
            selectedAnt.GetComponentInChildren<UIManager>(true).ShowInfo();
        }
        ContainerData.EnableGameAfterAction(consoleText);
    }
    
}
