using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitioner : MonoBehaviour
{
    public void NextLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting level");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void ToMenu()
    {
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }
}
