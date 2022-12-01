using UnityEngine;

public class MusicController : MonoBehaviour
{
    AudioSource titleMusic;
    AudioSource gameMusic;

    AudioSource _currentSource;

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

        titleMusic = transform.GetChild(0).GetComponent<AudioSource>();
        gameMusic = transform.GetChild(1).GetComponent<AudioSource>();
    }

    public void PlayTitleMusic()
    {
        Debug.Log("Playing title music!");
        if (titleMusic.isPlaying)
        {
            return;
        }
        if (gameMusic.isPlaying)
        {
            gameMusic.Stop();
        }

        titleMusic.Play();
        _currentSource = titleMusic;
    }

    public void PlayGameMusic()
    {
        Debug.Log("Playing game music!");
        if (gameMusic.isPlaying)
        {
            return;
        }
        if (titleMusic.isPlaying)
        {
            titleMusic.Stop();
        }

        gameMusic.Play();
        _currentSource = gameMusic;
    }

    public void StopMusic()
    {
        _currentSource.Stop();
    }
}
