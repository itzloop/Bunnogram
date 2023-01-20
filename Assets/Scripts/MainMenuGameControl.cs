using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuGameControl : MonoBehaviour
{
    public Button playButton;

    public Button exitButton;

    public Button aboutButton;

    public Button settingsButton;

    public GameObject exitPanel;
    public GameObject settingsPanel;
    public void OnPlayButtonClick()
    {
        // redirect to levels scene
    }

    public void OnExitButtonClick()
    {
        // show modal
        exitPanel.SetActive(true);
    }

    public void onExitPanelYesButtonClick()
    {
        // exit game
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
        // hide modal
        settingsPanel.SetActive(false);
    }

    public void onSettingsPanelCancelButtonClick()
    {
        // hide modal
        settingsPanel.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
