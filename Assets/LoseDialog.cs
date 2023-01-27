using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoseDialog : MonoBehaviour
{

    [SerializeField] private Button restart;

    public Action restartAction;
    private void Awake()
    {
        restart.onClick.AddListener(Restart);
    }

    private void Restart()
    {
        restartAction();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        restart.onClick.RemoveAllListeners();
    }
}