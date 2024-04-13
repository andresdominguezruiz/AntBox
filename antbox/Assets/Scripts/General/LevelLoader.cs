using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    public Animator animator;
    public float transitionTime=1f;

    private float actualVolume=0.5f;

    public float ActualVolume { get => actualVolume; set => actualVolume = value; }

    private static void SetInstance(LevelLoader level){
        LevelLoader.Instance=level;
    }

//Al pasarlo a una instancia, podemos utilizar esto para aplicar todas las transiciones
    private void Awake(){
        if(LevelLoader.Instance==null){
            SetInstance(this);
            DontDestroyOnLoad(this.gameObject);
        }else{
            Destroy(this.gameObject);
        }
    }

    public void StartNewLevel(int levelIndex){
        StartCoroutine(MakeTransition(levelIndex));
    }

    IEnumerator MakeTransition(int levelIndex){
        animator.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(transitionTime);
        SceneManager.LoadScene(levelIndex);
        animator.SetTrigger("End");
        
    }
}
