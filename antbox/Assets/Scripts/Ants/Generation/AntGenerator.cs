using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class AntGenerator : MonoBehaviour
{
    public GameObject ant;

    public GameObject moveMenu;
    public GameObject farmingMenu;
    public GameObject digMenu;


    public int initialSize=5;



    private System.Random random = new System.Random();


    public void placeAntsIn(List<Vector3Int> path,Tilemap map,System.Random random){
        GameObject queen=this.gameObject;
        queen.GetComponentInChildren<UIManager>().UpdateCanvasWithQueenStats(queen.GetComponent<QueenStats>(),queen.name);
        queen.AddComponent<SelectableItem>();
        queen.GetComponent<SelectableItem>().SetUIManager(queen.GetComponentInChildren<UIManager>());
        queen.GetComponentInChildren<UIManager>().HideInfo();
        for(int i=0;i<initialSize;i++){
            int v=random.Next(0,path.Count-1);
            GameObject newAnt=Instantiate(ant,map.CellToWorld(path[v]),Quaternion.identity,ant.transform.parent);
            //el último parámetro de Instantiate sirve para donde colocar el objeto según la jerarquía.
            NavMeshAgent agent=newAnt.GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            newAnt.name="Ant-"+i;
            newAnt.transform.position=new Vector3(newAnt.transform.position.x,newAnt.transform.position.y,0f);
            newAnt.transform.localPosition=new Vector3(newAnt.transform.localPosition.x,newAnt.transform.localPosition.y,0f);
            newAnt.AddComponent<AntStats>();
            newAnt.GetComponent<AntStats>().InitAntStats(random);
            newAnt.GetComponentInChildren<UIManager>().UpdateCanvasWithAntStats(newAnt.GetComponent<AntStats>(),newAnt.name);
            newAnt.AddComponent<SelectableItem>();
            newAnt.GetComponent<SelectableItem>().InitSelectableItem(path,map,moveMenu,farmingMenu,digMenu,ItemType.ANT);
            newAnt.AddComponent<ExcavationMovement>();
            newAnt.GetComponent<ExcavationMovement>().InitComponent(map);
            Debug.Log(newAnt.transform.position.z==0);
            //newAnt.AddComponent<AntMovement>();
            //newAnt.GetComponent<AntMovement>().AddPath(path,map);
            //AÑADIR ESTE COMPONENTE CUANDO TODAS LAS ACCIONES ESTÉN ACABADAS
            path.Remove(path[v]);
        }
        Destroy(ant);
    }

}
