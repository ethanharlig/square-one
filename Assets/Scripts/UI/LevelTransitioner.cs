using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitioner : MonoBehaviour
{

    public static void NextLevel()
    {
        AudioController.Instance.PlayMenuClick();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    public static void RestartLevel()
    {
        Debug.Log("Restarting level");
        AudioController.Instance.PlayMenuClick();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public static void ToMenu()
    {
        AudioController.Instance.PlayMenuClick();
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    private static int GetNextPossibleSceneBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    public void ToNextTitleScene()
    {
        AudioController.Instance.PlayMenuClick();
        Debug.LogFormat("Something going wrong with title screen changing? Does this main menu scene index look right: {0}", MainMenuController.MenuSceneIndex);
        if (GetNextPossibleSceneBuildIndex() >= MainMenuController.MenuSceneIndex)
        {
            Debug.LogFormat("Would be going to next index of {0} when menu is at index {1}. Going back to first scene.", GetNextPossibleSceneBuildIndex(), MainMenuController.MenuSceneIndex);
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
            return;
        }
        NextLevel();
    }
}
