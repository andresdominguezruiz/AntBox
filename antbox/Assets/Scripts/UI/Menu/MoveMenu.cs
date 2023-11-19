using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        selectedAnt.GetComponentInChildren<UIManager>(true).HideInfo();
        moveMenu.gameObject.SetActive(true);
        consoleText.text=selectedAnt.name+"-Select an accessible area";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneUnselectable();
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);//hit== null cuando no choque con nada
            if((mousePos.x>=minX && mousePos.x<=maxX) && (mousePos.y>=minY && mousePos.y<=maxY) && 
            (hit.collider==null || !hit.collider.CompareTag("Dirt"))){
                this.agent.SetDestination(mousePos);
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
        selectedAnt.GetComponentInChildren<UIManager>(true).ShowInfo();
        consoleText.text="Waiting . . .";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneSelectable();
    }
    
}
