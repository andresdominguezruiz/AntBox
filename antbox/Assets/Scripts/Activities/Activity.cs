using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ComplexityType{
    VERY_EASY,EASY,MEDIUM,HARD,VERY_HARD
}

[CreateAssetMenu(fileName = "New Activity", menuName = "Activity")]
public class Activity : ScriptableObject
{
    [SerializeField]
    private new string name;

    [SerializeField]
    private Sprite optionalImage;

    [SerializeField]
    private string description;

    [SerializeField]
    private ComplexityType complexityType;
    
    [SerializeField]
    private Sprite[] options;

    [SerializeField]
    private int correctAnswer;

    public string Name { get => name; set => name = value; }
    public Sprite OptionalImage { get => optionalImage; set => optionalImage = value; }
    public string Description { get => description; set => description = value; }
    public ComplexityType ComplexityType { get => complexityType; set => complexityType = value; }
    public Sprite[] Options { get => options; set => options = value; }
    public int CorrectAnswer { get => correctAnswer; set => correctAnswer = value; }
}
