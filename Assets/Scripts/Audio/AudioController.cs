using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource menuClickAudio;
    AudioSource loseAudio;
    AudioSource moveAudio;
    AudioSource winAudio;
    AudioSource waypointAudio;
    // SINGLETON
    public static float Volume = 1f;
    public static AudioController Instance { get; private set; }

#pragma warning disable IDE0051
    void Awake()
    {
        // If there is an instance and it's not me, delete myself.
        Debug.LogFormat("Spawning instance of {0}", GetType().ToString());

        if (Instance != null && Instance != this)
        {
            Debug.LogFormat("destroy {0}", GetType().ToString());
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
#pragma warning restore IDE0051
    void Start()
    {
        menuClickAudio = transform.GetChild(0).GetComponent<AudioSource>();
        winAudio = transform.GetChild(1).GetComponent<AudioSource>();
        loseAudio = transform.GetChild(2).GetComponent<AudioSource>();
        moveAudio = transform.GetChild(3).GetComponent<AudioSource>();
        waypointAudio = transform.GetChild(4).GetComponent<AudioSource>();
    }

    public void PlayMenuClick()
    {
        menuClickAudio.Play();
    }

    public void PlayMoveAudio()
    {
        moveAudio.Play();
    }

    public void PlayWinAudio()
    {
        winAudio.Play();
    }

    public void PlayLoseAudio()
    {
        loseAudio.Play();
    }

    public void PlayWaypointAudio()
    {
        waypointAudio.Play();
    }

    public void AdjustVolume(float val)
    {
        menuClickAudio.volume = GetVolume(val);
        winAudio.volume = GetVolume(val);
        loseAudio.volume = GetVolume(val);
        moveAudio.volume = GetVolume(val);
        waypointAudio.volume = GetVolume(val);

        Volume = GetVolume(val);
    }

    private float GetVolume(float val)
    {
        return val * GameManager.MainVolume * GameManager.DefaultVolume;
    }
}
