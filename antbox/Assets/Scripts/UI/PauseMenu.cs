using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenu;
    public void Pause(){
        Time.timeScale=0f;
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Quit(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }

    public void Resume(){
        Time.timeScale=1f;
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
    }
}
