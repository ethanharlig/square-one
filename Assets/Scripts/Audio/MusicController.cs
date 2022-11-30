using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource _audioSource;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

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
