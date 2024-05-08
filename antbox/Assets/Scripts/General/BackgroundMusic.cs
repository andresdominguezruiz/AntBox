using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] music;
    readonly System.Random random = new System.Random();

    public AudioSource AudioSource { get => audioSource; set => audioSource = value; }
    public AudioClip[] Music { get => music; set => music = value; }

    void Start()
    {
        AudioSource=this.GetComponent<AudioSource>();
        ChangeMusic();
    }
    public void ChangeMusic(){
        int v = random.Next(0,Music.Length-1);
        AudioSource.clip=Music[v];
        AudioSource.loop=true;
        AudioSource.Play();
    }
}
