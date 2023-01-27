using System.Collections;
using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinDialog : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro levelName;
    [SerializeField] private Image levelImage;
    [SerializeField] private Button ListButton;
    public void SetLevelName(string levelName, Sprite levelSprite)
    {
        this.levelName.text = levelName;
        this.levelImage.sprite = levelSprite;
        this.ListButton.onClick.AddListener(ReturnToLevelSelectionScene);
    }
    
    private void ReturnToLevelSelectionScene()
    {
        // Debug.Log("ReturnToLevelSelectionScene");
        SceneManager.LoadScene("Scenes/LevelsScene");
    }
}
