using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitioner : MonoBehaviour
{
    public static void NextLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    public static void RestartLevel()
    {
        Debug.Log("Restarting level");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public static void ToMenu()
    {
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }

    private static int GetNextPossibleSceneBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex + 1;
    }

    public void ToNextTitleScene()
    {
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
