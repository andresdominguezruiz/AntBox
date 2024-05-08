using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarManager : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Gradient gradient;
    
    [SerializeField]
    private Image fill;

    public Slider Slider { get => slider; set => slider = value; }
    public Gradient Gradient { get => gradient; set => gradient = value; }
    public Image Fill { get => fill; set => fill = value; }

    public void SetMaxBarValue(int value){
        Slider.maxValue=value;
        Slider.value=value;
        Fill.color=Gradient.Evaluate(1f);
    }

    public void SetBarValue(int value){
        Slider.value=value;
        Fill.color=Gradient.Evaluate(Slider.normalizedValue);
    }
}
