using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveMenu : MonoBehaviour
{
    [SerializeField] private GameObject moveMenu;

    [SerializeField] private GameObject map;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private GameObject cancelActionButton;

    [SerializeField] private GameObject selectedAnt;
    // Start is called before the first frame update
    public void StartMoveMenu()
    {
        selectedAnt.GetComponentInChildren<UIManager>(true).HideInfo();
        moveMenu.gameObject.SetActive(true);
        consoleText.text=selectedAnt.name+"-Select an accessible area";
        selectedAnt.GetComponent<SelectableItem>().MakeEveryoneUnselectable();
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
