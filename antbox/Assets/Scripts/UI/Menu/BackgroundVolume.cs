using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundVolume : MonoBehaviour
{
    [SerializeField]
    private static float volume=0.5f;


    [SerializeField]
    private AudioSource audioSource;

    public static float Volume { get => volume; set => volume = value; }
    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }

    public void Start(){
        SetNewVolume(volume);
    }

    public void SetNewVolume(float volume){
        Volume = volume;
        AudioSource.volume=Volume;
    }
}
