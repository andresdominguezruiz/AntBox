using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private Slider volume;


    [SerializeField]
    private BackgroundVolume backgroundVolume;

    public Slider Volume { get => volume; set => volume = value; }
    public BackgroundVolume BackgroundVolume { get => backgroundVolume; set => backgroundVolume = value; }

    public void GoBackToPause(){
        this.gameObject.SetActive(false);
    }

    public void OpenSettings(){
        this.gameObject.SetActive(true);
        Volume.value =LevelLoader.Instance.ActualVolume;
    }

    public void UpdateVolume(){
        float value=volume.value;
        BackgroundVolume.SetNewVolume(value);
        LevelLoader.Instance.ActualVolume=value;
    }
}
