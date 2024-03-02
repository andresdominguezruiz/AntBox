using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class NestManager : MonoBehaviour
{

    public GameObject antTemplate;
    public GameObject wormTemplate;
    public GameObject earthwormTemplate;
    private System.Random random = new System.Random();

    public List<GameObject> enemiesInGame=new List<GameObject>();

    private string GetOptimalUrl(Nest nest){
        string url="Enemies";
        if(nest.nestType.Equals(NestType.ANTS)) url+="/Ants";
        else if(nest.nestType.Equals(NestType.EARTHWORMS)) url+="/Earthworms";
        else if(nest.nestType.Equals(NestType.WORMS)) url+="/Worms";
        return url;
    }
    
    public void ReleaseEnemies(Nest nest,Tilemap dirtMap){
        //1º Seleccionar carpeta
        string url=GetOptimalUrl(nest);
        //2º Filtrar por nivel (de momento que el rango de enemigos siempre aumente)
        Enemy[] enemies=Resources.LoadAll<Enemy>(url);
        List<Enemy> availableEnemies=new List<Enemy>();
        foreach(Enemy enemy in enemies){
            if(enemy.enemyLevel<=nest.maxLevel) availableEnemies.Add(enemy);
        }
        if(availableEnemies.Count>0){
            int randomIndex=random.Next(0,availableEnemies.Count);
            Enemy selected=availableEnemies[randomIndex];
            List<Vector3Int> availablePositions=new List<Vector3Int>();
            availablePositions.AddRange(nest.nestPositions);
            while(nest.numberOfEnemies>0 && availablePositions.Count>0){
                randomIndex=random.Next(0,availablePositions.Count);
                Vector3Int pos=availablePositions[randomIndex];
                availablePositions.Remove(pos);
                SpawnEnemy(selected,dirtMap,pos);
                nest.numberOfEnemies--;
            }

        }
    }

    private void SpawnEnemy(Enemy enemy,Tilemap map,Vector3Int pos){
        GameObject template=antTemplate;
        if(enemy.enemyType.Equals(EnemyType.WORM)) template=wormTemplate;
        else if(enemy.enemyType.Equals(EnemyType.EARTHWORM)) template=earthwormTemplate;
        GameObject newEnemy=Instantiate(template,map.GetCellCenterWorld(pos),Quaternion.identity,template.transform.parent);
        newEnemy.name=enemy.name;
        newEnemy.AddComponent<SelectableItem>();
        newEnemy.GetComponent<SelectableItem>().InitSelectableItemOfEnemy(newEnemy.GetComponentInChildren<UIEnemyManager>());
        GenerationTilemap generation=GetComponent<GenerationTilemap>();
        generation.BakeMap();
        newEnemy.SetActive(true);
        enemiesInGame.Add(newEnemy);
    }


    public void ResetEnemies(){
        for (int i = 0; i < enemiesInGame.Count; i++)
        {
            Destroy(enemiesInGame[i]);
        }
        enemiesInGame.Clear();
    }
}
