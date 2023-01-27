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
    private PlayerDataControl playerData;
    void Start()
    {
        playerData = GameObject.Find("PlayerDataControl").GetComponent<PlayerDataControl>();
        audioSource = GameState.Instance.Get<AudioSource>(Constants.BgMusicKey);
        musicToggle.isOn = playerData.PlayerData.bgMusicOn;
        soundsToggle.isOn = playerData.PlayerData.soundFXOn;

        if (!playerData.PlayerData.bgMusicOn)
        {
            audioSource.mute = true;
        }
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
            playerData.SaveMusicOnState(true);
        }
        else if (!musicToggle.isOn && !audioSource.mute)
        {
            audioSource.mute = true;
            playerData.SaveMusicOnState(false);
        }
        
        if (soundsToggle.isOn)
        {
            playerData.SaveSoundFXOnState(true);
        }
        else if (!soundsToggle.isOn)
        {
            playerData.SaveSoundFXOnState(false);
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
