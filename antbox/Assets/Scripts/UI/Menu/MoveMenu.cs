using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveMenu : MonoBehaviour
{
    [SerializeField] private GameObject moveMenu;

    [SerializeField] private Tilemap map;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private GameObject cancelActionButton;

    [SerializeField] private GameObject selectedAnt;

    public float speed=0.5f;

    private Navigator navigator;

    // Start is called before the first frame update

    void Start()
    {
        this.navigator=FindObjectOfType<Navigator>();
    }
    public void StartMoveMenu()
    {
        selectedAnt.GetComponentInChildren<UIManager>(true).HideInfo();
        moveMenu.gameObject.SetActive(true);
        consoleText.text=selectedAnt.name+"-Select an accessible area";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneUnselectable();
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            var screen=Input.mousePosition;
            var world=Camera.main.ScreenToWorldPoint(screen);
            world.z=1;
            Debug.Log("Mi hormiga z"+selectedAnt.transform.position.z);
            this.navigator.SetSelectedAnt(selectedAnt);
            var path=this.navigator.GetPath(selectedAnt.transform.position,world);
            path.Add(world);

            StopAllCoroutines();
            StartCoroutine(this.RunPath(path));
            //FinishMoveMenu();

        }

    }
    IEnumerator RunPath(List<Vector3> path){
        foreach(var target in path){
            var origin=selectedAnt.transform.position;
            Debug.Log("target:"+target);
            Debug.Log("local:"+origin);
            Debug.Log(path.Count);
            float percent=0;
            while(percent<1f){
                selectedAnt.transform.position=Vector3.Lerp(origin,target,percent);
                percent+=Time.deltaTime*this.speed;
                yield return null;
            }
        }
        this.navigator.SetSelectedAnt(null);
    }

    public void SetSelectedAnt(GameObject ant){
        selectedAnt=ant;
    }

    // Update is called once per frame
    public void FinishMoveMenu()
    {
        moveMenu.SetActive(false);
        selectedAnt.GetComponentInChildren<UIManager>(true).ShowInfo();
        consoleText.text="Waiting . . .";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneSelectable();
    }
    
}
