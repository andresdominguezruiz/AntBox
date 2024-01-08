using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ComplexityType{
    EASY,MEDIUM,HARD
}

[CreateAssetMenu(fileName = "New Activity", menuName = "Activity")]
public class Activity : ScriptableObject
{
    public new string name;
    public Sprite optionalImage;
    public string description;
    public ComplexityType complexityType;
    public string[] options;
    public int correctAnswer;
}