using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContainerEffect
{
    public UpdateEffectOnContainer containerEffect=UpdateEffectOnContainer.NONE;

    [SerializeField]
    private int multiplicatorValue = 1;

    [SerializeField]
    private float sumValue = 0f;

    public int MultiplicatorValue { get => multiplicatorValue; set => multiplicatorValue = value; }
    public float SumValue { get => sumValue; set => sumValue = value; }
}
