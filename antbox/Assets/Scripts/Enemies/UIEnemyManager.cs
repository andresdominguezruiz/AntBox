using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UIEnemyManager : MonoBehaviour
{

    [SerializeField]
    private GameObject infoCanvas;

    [SerializeField]
    private TextMeshProUGUI hpText;

    [SerializeField]
    private TextMeshProUGUI nameText;

    [SerializeField]
    private TextMeshProUGUI damageText;

    [SerializeField]
    private TextMeshProUGUI attackSpeedText;

    [SerializeField]
    private TextMeshProUGUI criticsText;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private EnemyStats enemyStats;

    public GameObject InfoCanvas { get => infoCanvas; set => infoCanvas = value; }
    public TextMeshProUGUI HpText { get => hpText; set => hpText = value; }
    public TextMeshProUGUI NameText { get => nameText; set => nameText = value; }
    public TextMeshProUGUI DamageText { get => damageText; set => damageText = value; }
    public TextMeshProUGUI AttackSpeedText { get => attackSpeedText; set => attackSpeedText = value; }
    public TextMeshProUGUI CriticsText { get => criticsText; set => criticsText = value; }
    public Image Icon { get => icon; set => icon = value; }
    public EnemyStats EnemyStats { get => enemyStats; set => enemyStats = value; }

    void Start(){
        EnemyStats=this.gameObject.GetComponentInParent<EnemyStats>();
        if(EnemyStats!=null){
            StartCanvasWithEnemyStats(EnemyStats);
        }
    }



    void Update(){
        if(EnemyStats!=null){
            UpdateCanvasWithEnemyStats(EnemyStats);
        }
    }
    public void ShowInfo(){
        InfoCanvas.gameObject.SetActive(true);
    }
    public void UpdateAndShowInfo()
    {
        UpdateCanvasWithEnemyStats(EnemyStats);
        ShowInfo();
    }

    public void UpdateCanvasWithEnemyStats(EnemyStats stats){
        HpText.text="HP:"+stats.ActualHP+"/"+stats.Enemy.MaxHP;
    }

    public void StartCanvasWithEnemyStats(EnemyStats stats){
        Icon.sprite=stats.Enemy.EnemySprite;
        HpText.text="HP:"+stats.ActualHP+"/"+stats.Enemy.MaxHP;
        NameText.text="Name:"+stats.Enemy.name;
        DamageText.text="Damage:"+stats.Enemy.BattleStats.Damage;
        AttackSpeedText.text="Attack speed:"+stats.Enemy.BattleStats.AttackSpeed;
        CriticsText.text="Critical Effects:";
        foreach(CriticalEffects critical in stats.Enemy.BattleStats.CriticalEffects){
            CriticsText.text+="-"+critical;
        }
    }


    public void HideInfo()
    {
        InfoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
}
