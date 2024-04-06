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
    public float minX=-5.5f;
    public float maxX=7f;
    public float minY=-3.5f;
    public float maxY=3f;

    public float speed=0.5f;

    private NavMeshAgent agent;

    // Start is called before the first frame update

    void Start()
    {
        
    }
    public void StartMoveMenu()
    {
        this.agent=selectedAnt.GetComponent<NavMeshAgent>();
        CardDisplay anyCardDisplay=FindObjectOfType<CardDisplay>();
        if(anyCardDisplay!=null) anyCardDisplay.MakeEveryCardUnselectableAndUnselected();
        selectedAnt.GetComponentInChildren<UIManager>(true).HideInfo();
        moveMenu.gameObject.SetActive(true);
        consoleText.text=selectedAnt.name+"-Select an accessible area";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneUnselectable();
    }

    void Update(){
        if(this.agent==null || this.agent.gameObject==null){
            FinishMoveMenu();
        }
        if(Input.GetMouseButtonDown(0) && !PauseMenu.isPaused){
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
            if((mousePos.x>=minX && mousePos.x<=maxX) && (mousePos.y>=minY && mousePos.y<=maxY) && 
            (hit.collider==null || !hit.collider.CompareTag("Dirt"))){
                Vector3Int selectedTile=map.WorldToCell(mousePos);
                this.agent.SetDestination(map.GetCellCenterWorld(selectedTile));
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
        this.agent=null;
        moveMenu.SetActive(false);
        if(selectedAnt!=null){
            selectedAnt.GetComponentInChildren<UIManager>(true).ShowInfo();
        }
        ContainerData.EnableGameAfterAction(consoleText);
    }
    
}
