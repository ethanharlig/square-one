using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static readonly int MenuSceneIndex = 2;

    void Start()
    {
        MusicController.Instance.PlayTitleMusic();
    }

    public void PlayGame()
    {
        Debug.LogFormat("If play button is not working, is the first level right after menu?");
        LevelTransitioner.NextLevel();
    }

    public void QuitGame()
    {
        Application.Quit();

        DeselectCurrentlySelectedGameObject();
    }

    public void DeselectCurrentlySelectedGameObject()
    {
        GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }
}
