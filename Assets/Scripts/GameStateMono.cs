using System;
using System.Collections;
using Bunnogram;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameStateMono : MonoBehaviour
{

    private GameState _state;
    private void Awake()
    {
        _state = GameState.Instance;
        _state.Store(1234, "player_score");
        DontDestroyOnLoad(this.gameObject);
        
        // Create observables 
        var gameStatesOb = new ReactiveProperty<GameStates>(); // TODO replace with hardcoded value
        var healthOb = new ReactiveProperty<int>(3); // TODO replace with hardcoded value
        var hintsOb = new ReactiveProperty<int>(3); // TODO replace with hardcoded value
        var clickModeOb = new ReactiveProperty<ClickMode>(ClickMode.ForeGroundSelection); // TODO replace with hardcoded value
       
        // Store observables
        GameState.Instance.Store(gameStatesOb, Constants.GameStatesKey);
        GameState.Instance.Store(healthOb, Constants.HealthPointKey);
        GameState.Instance.Store(hintsOb, Constants.HintsCountKey);
        GameState.Instance.Store(clickModeOb, Constants.ClickModeKey);
    }


}