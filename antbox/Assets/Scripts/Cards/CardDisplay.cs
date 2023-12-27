using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    // Start is called before the first frame update
    public Card card;
    public Image image;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    void Start()
    {
        cardName.text=card.name;
        cardDescription.text=card.description;
        image.sprite=card.artWorks;
        
    }

    public void UseCard(){
        Debug.Log("Aplicado");
    }

}
