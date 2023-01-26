using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuGameControl : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button exitButton;
    [SerializeField] Button aboutButton;
    [SerializeField] Button settingsButton;
    
    [SerializeField] GameObject exitPanel;


    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GameState.Instance.Get<AudioSource>(Constants.BgMusicKey);
    }
    
    public void OnPlayButtonClick()
    {
        // redirect to levels scene
        SceneManager.LoadScene("LevelsScene");
    }

    public void OnExitButtonClick()
    {
        // show modal
        exitPanel.SetActive(true);
    }

    public void onExitPanelYesButtonClick()
    {
        // exit game
        Application.Quit();
    }

    public void onExitPanelNoButtonClick()
    {
        // hide modal
        exitPanel.SetActive(false);
    }

    public void OnAboutButtonClick()
    {
        // redirect to about us scene
    }
    
}