using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using RTLTMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinDialog : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro levelName;
    [SerializeField] private Image levelImage;
    [SerializeField] private Button ListButton;
    [SerializeField] private Button nextLevelButton;

    public Action onNextLevelAction;
    public void SetLevelName(string levelName, Sprite levelSprite)
    {
        this.levelName.text = levelName;
        this.levelImage.sprite = levelSprite;
        this.ListButton.onClick.AddListener(ReturnToLevelSelectionScene);
        this.nextLevelButton.onClick.AddListener(NextLevel); 
    }

    private void OnDestroy()
    {
        ListButton.onClick.RemoveAllListeners();
        nextLevelButton.onClick.RemoveAllListeners();
    }


    private void ReturnToLevelSelectionScene()
    {
        // Debug.Log("ReturnToLevelSelectionScene");
        SceneManager.LoadScene("Scenes/LevelsScene");
    }

    private void NextLevel()
    {
        onNextLevelAction();
        GameState.Instance.Get<ReactiveProperty<int>>(Constants.LevelKey).Value++;
    }
}