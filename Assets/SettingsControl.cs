using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour
{
    [SerializeField] GameObject settingsPanel;

    private AudioSource audioSource;

    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle soundsToggle;
    private PlayerData playerData;
    void Start()
    {
        playerData = GameObject.Find("PlayerDataControl").GetComponent<PlayerDataControl>().PlayerData;
        audioSource = GameState.Instance.Get<AudioSource>(Constants.BgMusicKey);
        musicToggle.isOn = playerData.bgMusicOn;
        soundsToggle.isOn = playerData.soundFXOn;
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
            playerData.bgMusicOn = true;
        }
        else if (!musicToggle.isOn && !audioSource.mute)
        {
            audioSource.mute = true;
            playerData.soundFXOn = true;
        }
        // hide modal
        settingsPanel.SetActive(false);
    }

    public void onSettingsPanelCancelButtonClick()
    {
        // return changes
        musicToggle.isOn = !audioSource.mute;
        // hide modal
        settingsPanel.SetActive(false);
    }
}
