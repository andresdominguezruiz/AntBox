using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMenu : MonoBehaviour
{
    public void Play(){
        LevelLoader.Instance.StartNewLevel(SceneManager.GetActiveScene().buildIndex+1);
        Time.timeScale=1f;
        
    }

    public void Exit(){
        Debug.Log("Salir");
        Application.Quit();
    }
}
