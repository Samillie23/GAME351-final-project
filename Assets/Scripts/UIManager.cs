using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public GameObject pauseUI;
    public GameObject menuUI;
    public GameObject creditUI;

    public void OnRestartPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnGameResumePress()
    {
        pauseUI.SetActive(false);
    }
    public void OnExitGamePress()
    {
        Application.Quit();
    }
    public void OnPauseGamePress()
    {
        pauseUI.SetActive(true);
    }
    public void OnStart()
    {
        menuUI.SetActive(false);
    }
    public void OnCredits()
    {
        creditUI.SetActive(true);
    }
    public void OffCredits()
    {
        creditUI.SetActive(false);
    }

}
