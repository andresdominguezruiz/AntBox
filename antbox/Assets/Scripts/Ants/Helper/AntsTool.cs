using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public static class AntsTool
{

    public static void IsEndOfGame(){
        List<SelectableItem> anthill=SelectableItem.GetItemsByTargetType(TargetType.ANTHILL);
        List<SelectableItem> queens=SelectableItem.GetItemsByTargetType(TargetType.QUEEN);
        if(anthill.Count<=1 || queens.Count<1){
            StatisticsOfGame.Instance.DestroyItemsAndEnemies();
            LevelLoader.Instance.StartNewLevel(SceneManager.GetActiveScene().buildIndex+1);
        }
    }

    public static void GenerateAnt(GameObject newAnt,Tilemap map,System.Random random,int number,List<Vector3Int> path){
        NavMeshAgent agent=newAnt.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        newAnt.name="Ant-"+number;
        newAnt.transform.position=new Vector3(newAnt.transform.position.x,newAnt.transform.position.y,0f);
        newAnt.transform.localPosition=new Vector3(newAnt.transform.localPosition.x,newAnt.transform.localPosition.y,0f);
        newAnt.GetComponent<AntStats>().InitAntStats(random);
        UIManager uIManager=newAnt.GetComponentInChildren<UIManager>(true);
        if(uIManager!=null){
            uIManager.UpdateCanvasWithAntStats(newAnt.GetComponent<AntStats>(),newAnt.name);
        }
        newAnt.GetComponent<SelectableItem>().InitSelectableItem(path,map,ItemType.ANT);
        newAnt.GetComponent<ExcavationMovement>().InitComponent(map);
        newAnt.SetActive(true);
    }

}
