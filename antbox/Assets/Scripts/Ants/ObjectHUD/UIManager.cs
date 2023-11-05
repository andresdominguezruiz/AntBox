using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject infoCanvas; // Referencia al objeto Canvas de tu UI.
    public TextMeshProUGUI hpText; // Referencia al objeto Text para mostrar el nombre.
    public TextMeshProUGUI hungerText; // Referencia al objeto Text para mostrar la descripción.
    public TextMeshProUGUI thirstText; // Referencia al objeto Text para mostrar las estadísticas.
    public bool isQueen=false;

    public TextMeshProUGUI energyText;

    public TextMeshProUGUI ageText;

    public TextMeshProUGUI nameText;

    void Update(){
        if(!isQueen){
            AntStats antStats=this.gameObject.GetComponentInParent<AntStats>();
            UpdateCanvasWithAntStats(antStats,this.transform.parent.name);
        }else{
            QueenStats queenStats=this.gameObject.GetComponentInParent<QueenStats>();
            UpdateCanvasWithQueenStats(queenStats,this.transform.parent.name);
        }
    }



    public void ShowInfo(){
        infoCanvas.gameObject.SetActive(true);
    }
    public void UpdateAndShowInfo(string hp, string hunger, string thirst,string age,string name,string energy)
    {
        UpdateInfo(hp,hunger,thirst,age,name,energy);
        ShowInfo();
    }

    public void UpdateInfo(string hp, string hunger, string thirst,string age,string name,string energy){
        hpText.text = "HP:" + hp;
        hungerText.text = "Hunger:" + hunger;
        thirstText.text = "Thirst:" + thirst;
        ageText.text="Age:"+age;
        nameText.text="Name:"+name;
        if(!isQueen) energyText.text="Energy:"+energy;
    }

    public void UpdateCanvasWithAntStats(AntStats antStats,string name){
        hpText.text="HP:"+antStats.GetTextHP();
        hungerText.text = "Hunger:"+antStats.GetTextHunger();
        thirstText.text ="Thirst:" +antStats.GetTextThirst();
        ageText.text="Age:"+antStats.GetTextAge();
        energyText.text="Energy:"+antStats.GetEnergyText();
        nameText.text="Name:"+name;
    }
    public void UpdateCanvasWithQueenStats(QueenStats queenStats,string name){
        hpText.text="HP:"+queenStats.GetTextHP();
        hungerText.text = "Hunger:"+queenStats.GetTextHunger();
        thirstText.text ="Thirst:" +queenStats.GetTextThirst();
        ageText.text="Age:"+queenStats.GetTextAge();
        nameText.text="Name:"+name;
        isQueen=true;
    }

    public void HideInfo()
    {
        infoCanvas.gameObject.SetActive(false); // Oculta el Canvas.
    }
}
