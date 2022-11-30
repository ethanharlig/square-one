using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource _audioSource;

    public static MusicController Instance { get; private set; }

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

        _audioSource = GetComponent<AudioSource>();
    }

    public void SetMusic(AudioClip audioClip, bool loop)
    {
        _audioSource.clip = audioClip;
        _audioSource.loop = loop;
    }

    public void PlayMusic()
    {
        if (_audioSource.isPlaying)
        {
            return;
        }
        _audioSource.Play();
    }

    public void StopMusic()
    {
        _audioSource.Stop();
    }
}
