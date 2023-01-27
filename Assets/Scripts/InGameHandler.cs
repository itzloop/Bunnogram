using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Bunnogram;
using DG.Tweening;
using RTLTMPro;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameHandler : MonoBehaviour
{
    // Bottom panel's buttons
    [SerializeField] private Button x;
    [SerializeField] private Button o;
    [SerializeField] private Button hint;
    [SerializeField] private RTLTextMeshPro hintCount;
   
    // Menu
    [SerializeField] private Button menuButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnToLevelSelectionSceneButton;
    [SerializeField] private Button returnToGameButton;
    [SerializeField] private CanvasGroup menu;
   
    // Level title
    [SerializeField] private RTLTextMeshPro levelTitle;

    [SerializeField] private InGameDialog dialog;
    [SerializeField] private LoseDialog loseDialog;
    [SerializeField] private GameObject winDialog;
    [SerializeField] private ImageGrid imageGrid;
    
    // Audio source
    [SerializeField] private AudioSource source;

    private int _maxHP, _maxHints;
    private ReactiveProperty<int> _hp;
    private ReactiveProperty<int> _squareCount;
    private ReactiveProperty<int> _hints;
    private ReactiveProperty<int> _level;

    private List<IDisposable> _disposables;
    private void Awake()
    {
        _disposables = new List<IDisposable>();
        
        this.x.onClick.AddListener(() => SetBottomPanelButtonsColor(ClickMode.BackgroundSelection));
        this.o.onClick.AddListener(() => SetBottomPanelButtonsColor(ClickMode.ForeGroundSelection));
        this.hint.onClick.AddListener(() => SetBottomPanelButtonsColor(ClickMode.HintSelection));
        
        _disposables.Add(GameStateHelper.GetHints().AsObservable().Subscribe(n =>
        {
            hintCount.text = n.ToString();
        }));

        _level = GameState.Instance.Get<ReactiveProperty<int>>(Constants.LevelKey);
        _hints = GameState.Instance.Get<ReactiveProperty<int>>(Constants.HintsCountKey);
        _squareCount = GameState.Instance.Get<ReactiveProperty<int>>(Constants.CurrentSquareKey);
        var pixelatedImage = Resources.Load<PixelatedImage>($"Levels/Level_{_level.Value:000}");
        _hp = GameState.Instance.Get<ReactiveProperty<int>>(Constants.HealthPointKey);
        _maxHP = _hp.Value;
        _maxHints = _hints.Value;
        // Win condition
        _disposables.Add(_squareCount.Where(count => count == 0).Subscribe(_ =>
        {
            var pixelatedImage = Resources.Load<PixelatedImage>($"Levels/Level_{_level.Value:000}");
            
            // save
            GameObject.Find("PlayerDataControl").GetComponent<PlayerDataControl>().SavePlayedLevel(_level.Value - 1);
            // show dialog
            var go = dialog.SetGameObject(winDialog.GetComponent<CanvasGroup>());
            go.GetComponent<WinDialog>().SetLevelName(pixelatedImage.levelName, pixelatedImage.sprite);
            go.GetComponent<WinDialog>().onNextLevelAction += () =>
            {
                dialog.Hide();
            };
            dialog.Show();
        }));

        // Lose condition
        _disposables.Add(_hp.Where(hp => hp == 0).Subscribe(_ =>
        {
            var go = dialog.SetGameObject(loseDialog.GetComponent<CanvasGroup>());
            go.GetComponent<LoseDialog>().restartAction += () =>
            {
                dialog.Hide();
            };
            
            dialog.Show();
        }));

        _disposables.Add(_level.AsObservable().Subscribe(l =>
        {
            levelTitle.text = $"مرحله {l}";
            SetupLevel(l);
        }));
        
        // add listener to menu button
        menuButton.onClick.AddListener(ShowMenu);
        restartButton.onClick.AddListener(Restart);
        returnToGameButton.onClick.AddListener(HideMenu);
        returnToLevelSelectionSceneButton.onClick.AddListener(ReturnToLevelSelectionScene);
        
        // SetupLevel(_level.Value);
    }

    private void OnDestroy()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }

        // remove all listeners
        menuButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        returnToGameButton.onClick.RemoveAllListeners();
        returnToLevelSelectionSceneButton.onClick.RemoveAllListeners();
        x.onClick.RemoveAllListeners();
        o.onClick.RemoveAllListeners();
        hint.onClick.RemoveAllListeners();
    }

    private void SetupLevel(int level)
    {
        // set health
        _hp.Value = _maxHP;
        
        // set hints
        _hints.Value = _maxHints;
        
        var pixelatedImage = Resources.Load<PixelatedImage>($"Levels/Level_{level:000}");
        
        // setup the grid
        imageGrid.SetupGrid(pixelatedImage, source);
        
        // Set the default selected button
        SetBottomPanelButtonsColor(ClickMode.ForeGroundSelection);
    }

    private void ShowMenu()
    {
        menu.blocksRaycasts = true;
        menu.interactable = true;
        menu.DOFade(1, .3f);
    }

    private void HideMenu()
    {
        menu.DOFade(0, .3f);
        menu.blocksRaycasts = false;
        menu.interactable = false;
    }

    private void Restart()
    {
        SetupLevel(_level.Value);
        HideMenu();
    }

    private void ReturnToLevelSelectionScene()
    {
        // Debug.Log("ReturnToLevelSelectionScene");
        SceneManager.LoadScene("Scenes/LevelsScene");
    }

    private void SetBottomPanelButtonsColor(ClickMode mode)
    {
        const float duration = .3f;
        Color onColor = new Color(0.682353f, 0.7333333f, 0.6156863f);
        Color defColor = new Color(0.9686275f,0.9566621f,0.9347255f);
        switch (mode)
        {
            case ClickMode.BackgroundSelection:
                this.x.GetComponent<Image>().DOColor(onColor, duration);
                this.o.GetComponent<Image>().DOColor(defColor, duration);
                this.hint.GetComponent<Image>().DOColor(defColor, duration);
                GameStateHelper.GetClickMode().Value = ClickMode.BackgroundSelection;
                break;
            case ClickMode.ForeGroundSelection:
                this.x.GetComponent<Image>().DOColor(defColor, duration);
                this.o.GetComponent<Image>().DOColor(onColor, duration);
                this.hint.GetComponent<Image>().DOColor(defColor, duration);
                GameStateHelper.GetClickMode().Value = ClickMode.ForeGroundSelection;
                break;
            case ClickMode.HintSelection:
                this.x.GetComponent<Image>().DOColor(defColor, duration);
                this.o.GetComponent<Image>().DOColor(defColor, duration);
                this.hint.GetComponent<Image>().DOColor(onColor, duration);
                GameStateHelper.GetClickMode().Value = ClickMode.HintSelection;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }
}