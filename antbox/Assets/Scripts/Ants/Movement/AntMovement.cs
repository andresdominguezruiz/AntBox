using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class AntMovement : MonoBehaviour
{
    public bool canMoveAuto=true;

    public HashSet<Vector3> availablePath=new HashSet<Vector3>();

    private System.Random random = new System.Random();

    public void AddPath(List<Vector3Int> path,Tilemap destructablePath){
        foreach(Vector3Int localPosition in path){
            Vector3 worldPosition=destructablePath.CellToWorld(localPosition);
            availablePath.Add(worldPosition);
        }
    }

    void Update(){
        SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
        if(canMoveAuto || (!item.isSelected && item.canBeSelected)){
            StartCoroutine(this.AutomaticMovement());
        }
    }

    void OnMouseDown(){
        //si lo he seleccionado y se puede seleccionar==> parar corutina
        //si lo he seleccionado pero no se puede seleccionar==> nada
        SelectableItem item=this.gameObject.GetComponent<SelectableItem>();
        if(item.isSelected && item.canBeSelected){
            //StopAllCoroutines para todas las corutinas DEL SCRIPT, no de todos
            StopAllCoroutines();
            NavMeshAgent agent=this.gameObject.GetComponent<NavMeshAgent>();
            agent.SetDestination(this.transform.position);
            canMoveAuto=false;
        }
    }

    
    IEnumerator AutomaticMovement(){
        NavMeshAgent agent=this.gameObject.GetComponent<NavMeshAgent>();
        while(canMoveAuto){
            int v=random.Next(0,availablePath.Count-1);
            Vector3 randomPosition=availablePath.ToList<Vector3>()[v];
            agent.SetDestination(randomPosition);
            yield return null;
        }
    }
}
