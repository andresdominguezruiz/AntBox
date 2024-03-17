using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UIEnemyManager : MonoBehaviour
{
    public GameObject infoCanvas;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI attackSpeedText;
    public TextMeshProUGUI criticsText;
    public Image icon;
    public EnemyStats enemyStats;

    void Start(){
        enemyStats=this.gameObject.GetComponentInParent<EnemyStats>();
        if(enemyStats!=null) StartCanvasWithEnemyStats(enemyStats);
    }



    void Update(){
        if(enemyStats!=null) UpdateCanvasWithEnemyStats(enemyStats);
    }
    public void ShowInfo(){
        infoCanvas.gameObject.SetActive(true);
    }
    public void UpdateAndShowInfo()
    {
        UpdateCanvasWithEnemyStats(enemyStats);
        ShowInfo();
    }

    public void UpdateCanvasWithEnemyStats(EnemyStats stats){
        hpText.text="HP:"+stats.actualHP+"/"+stats.enemy.maxHP;
    }

    public void StartCanvasWithEnemyStats(EnemyStats stats){
        icon.sprite=stats.enemy.enemySprite;
        hpText.text="HP:"+stats.actualHP+"/"+stats.enemy.maxHP;
        nameText.text="Name:"+stats.enemy.name;
        damageText.text="Damage:"+stats.enemy.battleStats.damage;
        attackSpeedText.text="Attack speed:"+stats.enemy.battleStats.attackSpeed;
        criticsText.text="Critical Effects:";
        foreach(CriticalEffects critical in stats.enemy.battleStats.criticalEffects)
            criticsText.text+="-"+critical;
    }


    public void HideInfo()
    {
        infoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
}
