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
    public GameObject attackMenu;
    private System.Random random = new System.Random();

    public List<GameObject> enemiesInGame=new List<GameObject>();

    public string GetOptimalUrl(Nest nest){
        return GetOptimalUrlByType(nest.nestType);
    }

    private string GetOptimalUrlByType(NestType type){
        string url="Enemies";
        if(type.Equals(NestType.ANTS)){
            url+="/Ants";
        }
        else if(type.Equals(NestType.EARTHWORMS)){
            url+="/Earthworms";
        }
        else if(type.Equals(NestType.WORMS)){
            url+="/Worms";
        }
        return url;
    }
    public void SpawnEnemiesForHorde(){
        string url="Enemies";
        //2º Filtrar por nivel (de momento que el rango de enemigos siempre aumente)
        Enemy[] enemies=Resources.LoadAll<Enemy>(url);
        List<Enemy> availableEnemies=new List<Enemy>();
        foreach(Enemy enemy in enemies){
            if(enemy.EnemyLevel<=Player.Instance.complexityLevelOfGame+1){
                availableEnemies.Add(enemy);
            }
        }
        int r=random.Next(0,availableEnemies.Count-1);
        GenerationTilemap generationTilemap=FindObjectOfType<GenerationTilemap>();
        Enemy selectedEnemy=availableEnemies[r];
        int number=1;
        if(selectedEnemy.EnemyType.Equals(EnemyType.ANT)) {
            number=2;
        }
        else if(selectedEnemy.EnemyType.Equals(EnemyType.WORM)){
            number=3;
        }
        for(int i=0;i<number;i++){
            int v=random.Next(0,generationTilemap.path.Count);
            SpawnEnemy(selectedEnemy,generationTilemap.dirtMap,generationTilemap.path[v]);
        }

    }
    
    public void ReleaseEnemies(Nest nest,Tilemap dirtMap){
        //1º Seleccionar carpeta
        string url=GetOptimalUrl(nest);
        //2º Filtrar por nivel (de momento que el rango de enemigos siempre aumente)
        Enemy[] enemies=Resources.LoadAll<Enemy>(url);
        List<Enemy> availableEnemies=new List<Enemy>();
        foreach(Enemy enemy in enemies){
            if(enemy.EnemyLevel<=nest.maxLevel){
                availableEnemies.Add(enemy);
            }
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
        if(enemy.EnemyType.Equals(EnemyType.WORM)){
            template=wormTemplate;
        }
        else if(enemy.EnemyType.Equals(EnemyType.EARTHWORM)){
            template=earthwormTemplate;
        }
        GameObject newEnemy=Instantiate(template,map.GetCellCenterWorld(pos),Quaternion.identity,template.transform.parent);
        newEnemy.name=enemy.name;
        newEnemy.GetComponent<EnemyStats>().Enemy=enemy;
        newEnemy.AddComponent<SelectableItem>();
        newEnemy.GetComponent<SelectableItem>().InitSelectableItemOfEnemy(newEnemy.GetComponentInChildren<UIEnemyManager>(),attackMenu);
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
