using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private bool toggledToMenu = false;

    public static GameManager Instance { get; private set; }
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

    void Start()
    {
        PlayMusic();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    [RuntimeInitializeOnLoadMethod]
    public static void OnGameLoad()
    {
        // put any singletons we want in every scene here
        GameObject audio = (GameObject)Resources.Load("Prefabs/Sounds");
        GameObject music = (GameObject)Resources.Load("Prefabs/Music");
        GameObject gm = (GameObject)Resources.Load("Prefabs/GameManager");

        Instantiate(audio);
        Instantiate(music);
        Instantiate(gm);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusic();
    }

    private void PlayMusic()
    {
        if (SceneManager.GetActiveScene().buildIndex < MainMenuController.MenuSceneIndex)
        {
            MusicController.Instance.PlayTitleMusic();
        }
        else
        {
            MusicController.Instance.PlayGameMusic();

        }
    }
}
