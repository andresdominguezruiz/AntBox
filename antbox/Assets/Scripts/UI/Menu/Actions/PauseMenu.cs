using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private GameObject map;
    public static bool isPaused=false;
    public void Pause(){
        Time.timeScale=0f;
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
        isPaused=true;
    }

    public void Quit(){
        StatisticsOfGame.Instance.ResetData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
        isPaused=false;
    }

    public void Resume(){
        Time.timeScale=1f;
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
        map.SetActive(true);
        isPaused=false;
    }
}
