using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EatButton : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isQueen=false;
    private Button eatButton;
    private AntStats antStats=null;
    private QueenStats queenStats=null;
    public GameObject container;
    void Start()
    {
        eatButton=this.GetComponent<Button>();
        if(!isQueen){
            antStats=this.gameObject.GetComponentInParent<AntStats>();
        }else{
            queenStats=this.gameObject.GetComponentInParent<QueenStats>();
        }
        
    }
    public void Feeding(){
        if(antStats!=null){
            Debug.Log(antStats.gameObject.name);
            ContainerData cont=container.GetComponent<ContainerData>();
            antStats.Eat(cont);
        }else if(queenStats!=null){
            ContainerData cont=container.GetComponent<ContainerData>();
            queenStats.Eat(cont);
        }else if(queenStats ==null){
            queenStats=this.gameObject.GetComponentInParent<QueenStats>();
            ContainerData cont=container.GetComponent<ContainerData>();
            queenStats.Eat(cont);
        }
    }
}
