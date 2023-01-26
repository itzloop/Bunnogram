using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] GameObject settingsPanel;

    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle soundsToggle;

    private AudioSource audioSource;
    
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

    public void onSettingsButtonClick()
    {
        // open settings modal
        settingsPanel.SetActive(true);
    }

    public void onSettingsPanelSaveButtonClick()
    {
        // save change

        if (musicToggle.isOn && audioSource.mute)
        {
            audioSource.mute = false;
        }
        else if (!musicToggle.isOn && !audioSource.mute)
        {
            audioSource.mute = true;
        }
        // hide modal
        settingsPanel.SetActive(false);
    }

    public void onSettingsPanelCancelButtonClick()
    {
        // TODO return changes
        
        // hide modal
        settingsPanel.SetActive(false);
    }

    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
        DontDestroyOnLoad(audioSource);
    }

    // Update is called once per frame
    void Update()
    {
    }
}