using System;
using System.Collections;
using Bunnogram;
using RTLTMPro;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameStateMono : MonoBehaviour
{

    private GameState _state;

    [SerializeField] private InGameDialog dialog;
    [SerializeField] private GameObject loseDialog;
    [SerializeField] private GameObject winDialog;
    private void Awake()
    {
        _state = GameState.Instance;
        DontDestroyOnLoad(this.gameObject);
        
        // Create observables 
        // TODO move global properties these to first scene
        var gameStatesOb = new ReactiveProperty<GameStates>(); // TODO replace with hardcoded value
        var healthOb = new ReactiveProperty<int>(3); // TODO replace with hardcoded value
        var hintsOb = new ReactiveProperty<int>(3); // TODO replace with hardcoded value
        var clickModeOb = new ReactiveProperty<ClickMode>(ClickMode.ForeGroundSelection); // TODO replace with hardcoded value
        var level = new ReactiveProperty<int>(1);
        var pixelatedImage = Resources.Load<PixelatedImage>($"Levels/Level_{level.Value:000}");

        var allSquares = pixelatedImage.bounds.x * pixelatedImage.bounds.y;
        var backgroundSquares = pixelatedImage.backgroundPixels.Count;
        var currentSquareCount = new ReactiveProperty<int>(allSquares - backgroundSquares); // TODO replace with hardcoded value
       
        // Store observables
        try
        {
            GameState.Instance.Store(gameStatesOb, Constants.GameStatesKey);
            GameState.Instance.Store(healthOb, Constants.HealthPointKey);
            GameState.Instance.Store(hintsOb, Constants.HintsCountKey);
            GameState.Instance.Store(clickModeOb, Constants.ClickModeKey);
            GameState.Instance.Store(level, Constants.LevelKey);
            GameState.Instance.Store(pixelatedImage, Constants.PixelatedImageKey);
            GameState.Instance.Store(currentSquareCount, Constants.CurrentSquareKey);
           // GameState.Instance.Store(new ReactiveProperty<bool>(false), Constants.RestartKey);
            
            // Win condition
            currentSquareCount.Where(x => x == 0).Subscribe(x =>
            {
                var go = dialog.SetGameObject(winDialog.GetComponent<CanvasGroup>());
                go.GetComponent<WinDialog>().SetLevelName(pixelatedImage.levelName);
                dialog.Show();
            });

            // Lose condition
            healthOb.Where(x => x == 0).Subscribe(x =>
            {
                dialog.SetGameObject(loseDialog.GetComponent<CanvasGroup>());
                dialog.Show();
            });
        }
        catch (Exception e)
        {
            GameState.Instance.Get<ReactiveProperty<int>>(Constants.HealthPointKey).Value = 3;
            GameState.Instance.Get<ReactiveProperty<int>>(Constants.HintsCountKey).Value = 3;
            GameState.Instance.Get<ReactiveProperty<ClickMode>>(Constants.ClickModeKey).Value = ClickMode.ForeGroundSelection;
            GameState.Instance.Get<ReactiveProperty<int>>(Constants.CurrentSquareKey).Value = allSquares - backgroundSquares;
            //GameState.Instance.Get<ReactiveProperty<bool>>(Constants.RestartKey).Value = false;
        }

    }


}