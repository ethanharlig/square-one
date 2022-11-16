using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // ensure this is properly set otherwise the play button won't work!
    public int gameLevelBuildIndex;

    public void PlayGame()
    {
        Debug.LogFormat("If play button is not working, is this the correct game level build index: {0}!", gameLevelBuildIndex);
        SceneManager.LoadScene(gameLevelBuildIndex);
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
