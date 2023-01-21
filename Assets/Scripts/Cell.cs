using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using DG.Tweening;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class Cell : MonoBehaviour
{
    
    private enum CellMarkMode {
        CorrectBackground, CorrectForeground, WrongForeground, WrongBackground
    }
    private Button _button;
    private Image _image;
    private bool _tint;
    private GameState _state;

    [SerializeField] private Image cross;
    [SerializeField] private Image background;
    [SerializeField] private Image square;

    public Vector2Int CellCord { get; private set; }
    public bool IsBackground { get; private set; }

    private bool _marked = false;

    public void Init(Vector2Int cellCord, List<Vector2Int> backgroundPixelCords)
    {
        CellCord = cellCord;
        IsBackground = false;
        
        // Find if this cell is a background cell or not
        foreach (var pixelCord in backgroundPixelCords)
        {
            if (pixelCord.x == cellCord.x && pixelCord.y == cellCord.y)
            {
                IsBackground = true;
                return;
            }
        }
    }

    private void Awake()
    {
        // init variables
        _button = gameObject.GetComponent<Button>();
        _image = gameObject.GetComponent<Image>();
        _state = GameState.Instance;
        _tint = true;
        
        // add on click listener
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if(_marked) return;
        var cm = GameStateHelper.GetClickMode().Value;
        switch (cm)
        {
            case ClickMode.BackgroundSelection:
                HandleBackgroundSelectionMode();
                break;
            case ClickMode.ForeGroundSelection:
                HandleForeGroundSelectionMode();
                break;
            case ClickMode.HintSelection:
                HandleHintMode();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        } 
    }

    private void HandleBackgroundSelectionMode()
    {
        if (IsBackground)
        {
            MarkCell(CellMarkMode.CorrectBackground);
            return;
        }
        
        MarkWrongCell(IsBackground ? CellMarkMode.WrongBackground : CellMarkMode.WrongForeground);
    }

    private void HandleForeGroundSelectionMode()
    {
        if (!IsBackground)
        {
            MarkCell(CellMarkMode.CorrectForeground);
            return;
        }
        
        MarkWrongCell(IsBackground ? CellMarkMode.WrongBackground : CellMarkMode.WrongForeground);
    }

    private void HandleHintMode()
    {
        var hints = GameStateHelper.GetHints();
        if (hints.Value - 1 < 0)
        {
            // TODO handle no hint
            return;
        }

        hints.Value--;
        MarkCell(IsBackground ? CellMarkMode.CorrectBackground : CellMarkMode.CorrectForeground);
    }

    private void MarkWrongCell(CellMarkMode mode)
    {
        var hp = GameStateHelper.GetHealthPoints();
        hp.Value--;
        MarkCell(mode);
    }

    private void MarkCell(CellMarkMode mode)
    {
        if(_marked) return;
        _marked = true;

        if (!IsBackground)
        {
            _state.Get<ReactiveProperty<int>>(Constants.CurrentSquareKey).Value--;
        }
        const float duration = .3f;
        switch (mode)
        {
            case CellMarkMode.CorrectBackground:
                cross.DOFade(1, duration);
                break;
            case CellMarkMode.CorrectForeground:
                square.DOFade(1, duration);
                break;
            case CellMarkMode.WrongForeground:
                background.DOColor(Color.red, duration).OnComplete(() =>
                    DOTween.Sequence().Join(background.DOColor(Color.white, duration))
                        .Join(square.DOFade(1, duration)));
                break;
            case CellMarkMode.WrongBackground:
                background.DOColor(Color.red, duration).OnComplete(() =>
                    DOTween.Sequence().Join(background.DOColor(Color.white, duration))
                        .Join(cross.DOFade(1, duration)));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }
}