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
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSource = GameState.Instance.Get<AudioSource>(Constants.BgMusicKey);
        musicToggle.isOn = GameState.Instance.Get<ReactiveProperty<bool>>(Constants.BgMusicOnKey).Value;
        soundsToggle.isOn = GameState.Instance.Get<ReactiveProperty<bool>>(Constants.SoundFXOnKey).Value;
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
        // return changes
        musicToggle.isOn = !audioSource.mute;
        // hide modal
        settingsPanel.SetActive(false);
    }
}
