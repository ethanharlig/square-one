using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelUIElements : Singleton<LevelUIElements>
{
    public delegate void TogglePauseAction(bool isPaused);
    public static event TogglePauseAction OnTogglePause;
    bool _isPaused = false;

    [SerializeField] private TextMeshProUGUI MoveCountText;
    [SerializeField] private GameObject SuccessElements;
    [SerializeField] private GameObject FailedElements;
    [SerializeField] private GameObject PauseMenuElements;
    [SerializeField] private GameObject PauseButton;
    [SerializeField] private GameObject Compass;

#pragma warning disable IDE0051
    void Update()
    {
#pragma warning restore
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }


    public void SetMoveCountText(string text)
    {
        MoveCountText.text = text;
    }

    public void EnableMoveCountText()
    {
        if (!MoveCountText.gameObject.activeSelf)
            MoveCountText.gameObject.SetActive(true);
    }

    public GameObject GetSuccessElements()
    {
        return SuccessElements;
    }

    public GameObject GetFailedElements()
    {
        return FailedElements;
    }

    public void PauseLevel()
    {
        _isPaused = true;
        DoPause();
        MusicController.Instance.StopMusic();
        MusicController.Instance.PlayTitleMusic();
    }

    public void ResumeLevel()
    {
        _isPaused = false;
        DoPause();
        MusicController.Instance.StopMusic();
        MusicController.Instance.PlayGameMusic();
    }

    private void DoPause()
    {
        PauseButton.SetActive(!_isPaused);
        PauseMenuElements.SetActive(_isPaused);

        OnTogglePause?.Invoke(_isPaused);
    }

    private void TogglePause()
    {
        if (_isPaused)
        {
            ResumeLevel();
        }
        else
        {
            PauseLevel();
        }
    }

    public void ToggleCompass()
    {
        Compass.SetActive(!Compass.activeSelf);
    }

    public void DeselectClickedButton()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}