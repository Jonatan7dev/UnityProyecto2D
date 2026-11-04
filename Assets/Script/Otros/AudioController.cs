using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource audio;
    public AudioClip clipDisparo;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        
    }

    public void AudioDisparo()
    {
        audio.clip = clipDisparo;
        audio.Play();
    }
}
