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
    [SerializeField] private AudioSource bgMusic;

    private void Awake()
    {

        var stateMonos = FindObjectsOfType<GameStateMono>();
        if (stateMonos.Length > 1)
        {
            for (var i = 1; i < stateMonos.Length; i++)
            {
                Destroy(stateMonos[i].gameObject);
            }
        }
        
        _state = GameState.Instance;
        DontDestroyOnLoad(this.gameObject);
        
        // Create observables 
        // TODO move global properties these to first scene
        var gameStatesOb = new ReactiveProperty<GameStates>(); // TODO replace with hardcoded value
        var healthOb = new ReactiveProperty<int>(3); // TODO replace with hardcoded value
        var hintsOb = new ReactiveProperty<int>(3); // TODO replace with hardcoded value
        var clickModeOb = new ReactiveProperty<ClickMode>(ClickMode.ForeGroundSelection); // TODO replace with hardcoded value
        var level = new ReactiveProperty<int>(5);
        var pixelatedImage = Resources.Load<PixelatedImage>($"Levels/Level_{level.Value:000}");

        var allSquares = pixelatedImage.bounds.x * pixelatedImage.bounds.y;
        var backgroundSquares = pixelatedImage.backgroundPixels.Count;
        var currentSquareCount = new ReactiveProperty<int>(allSquares - backgroundSquares); // TODO replace with hardcoded value
        
        var bgMusic = this.bgMusic;

        var levelPage = 1;
       
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
            
            GameState.Instance.Store(levelPage, Constants.LevelPageKey);

            GameState.Instance.Store(bgMusic, Constants.BgMusicKey);

            
        }
        catch (Exception e)
        {
            GameState.Instance.Get<ReactiveProperty<int>>(Constants.HealthPointKey).Value = 3;
            GameState.Instance.Get<ReactiveProperty<int>>(Constants.HintsCountKey).Value = 3;
            GameState.Instance.Get<ReactiveProperty<ClickMode>>(Constants.ClickModeKey).Value = ClickMode.ForeGroundSelection;
            GameState.Instance.Get<ReactiveProperty<int>>(Constants.CurrentSquareKey).Value = allSquares - backgroundSquares;
        }

    }


}