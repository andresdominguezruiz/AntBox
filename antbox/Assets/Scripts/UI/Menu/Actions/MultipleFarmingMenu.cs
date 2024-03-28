using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MultipleFarmingMenu : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private TextMeshProUGUI capacityText;
    private FarmStats selectedFarm;

    private List<AntStats> antsToPlay=new List<AntStats>();

    void UpdateAntsToPlay(AntStats ant){
        SelectableItem item=ant.gameObject.GetComponent<SelectableItem>();
        if(item!=null) item.ChangeColorWithoutSelecting();
        if(antsToPlay.Contains(ant)){
            antsToPlay.Remove(ant);
        }
        else if(!antsToPlay.Contains(ant)
         && (selectedFarm.actualCapacity+antsToPlay.Count)<selectedFarm.GetMaxCapacity()){
            antsToPlay.Add(ant);
        }
        capacityText.text="Capacity:"
        +(selectedFarm.actualCapacity+antsToPlay.Count)+" of "+selectedFarm.GetMaxCapacity();
        consoleText.text="You can select "
        +(selectedFarm.GetMaxCapacity()-selectedFarm.actualCapacity+antsToPlay.Count)
        +" ants";
    }

    void InitMultipleFarmingMenu(FarmStats farm){
        selectedFarm=farm;
        this.gameObject.SetActive(true);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
