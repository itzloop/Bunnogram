using System.Collections;
using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinDialog : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro levelName;
    [SerializeField] private Image levelImage;

    public void SetLevelName(string levelName, Sprite levelSprite)
    {
        this.levelName.text = levelName;
        this.levelImage.sprite = levelSprite;
    }
}
