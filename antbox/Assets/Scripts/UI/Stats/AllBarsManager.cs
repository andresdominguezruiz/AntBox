using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllBarsManager : MonoBehaviour
{
    [SerializeField]
    private BarManager healthBar;

    [SerializeField]
    private BarManager thirstBar;

    [SerializeField]
    private BarManager hungerBar;
    
    [SerializeField]
    private BarManager energyBar;

    public BarManager HealthBar { get => healthBar; set => healthBar = value; }
    public BarManager ThirstBar { get => thirstBar; set => thirstBar = value; }
    public BarManager HungerBar { get => hungerBar; set => hungerBar = value; }
    public BarManager EnergyBar { get => energyBar; set => energyBar = value; }
}
