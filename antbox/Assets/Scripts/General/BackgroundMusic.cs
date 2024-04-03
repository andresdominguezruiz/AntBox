using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] music;
    private System.Random random = new System.Random();

    void Start()
    {
        audioSource=this.GetComponent<AudioSource>();
        ChangeMusic();
    }
    public void ChangeMusic(){
        int v = random.Next(0,music.Length-1);
        audioSource.clip=music[v];
        audioSource.loop=true;
        audioSource.Play();
    }
}
