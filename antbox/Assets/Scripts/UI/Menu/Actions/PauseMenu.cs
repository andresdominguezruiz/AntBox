using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject map;
    public static bool isPaused=false;

    private static float pastTime;

    public static float PastTime { get => pastTime; set => pastTime = value; }

    public static void SetPause(bool paused){
        isPaused=paused;
    }
    public void Pause(){
        PastTime=Time.timeScale;
        Time.timeScale=0f;
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
        SetPause(true);
    }

    public void DebuggingNextLevel(){
        Time.timeScale=1f;
        StatisticsOfGame.Instance.counterOfExams++;
        StatisticsOfGame.Instance.counterOfPassedExams++;
        SetPause(false);
        StatisticsOfGame.Instance.NextLevel();
    }

    public void Quit(){
        Time.timeScale=1f;
        LevelLoader.Instance.StartNewLevel(SceneManager.GetActiveScene().buildIndex-1);
        StatisticsOfGame.Instance.ResetData();
        SetPause(false);
    }

    public void Resume(){
        Time.timeScale=PastTime;
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
        map.SetActive(true);
        SetPause(false);
    }
}
