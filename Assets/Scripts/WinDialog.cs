using System.Collections;
using System.Collections.Generic;
using RTLTMPro;
using UnityEngine;

public class WinDialog : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro levelName;

    public void SetLevelName(string levelName)
    {
        this.levelName.text = levelName;
    }
}
