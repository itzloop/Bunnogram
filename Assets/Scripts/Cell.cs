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
    
    public enum CellMarkMode {
        CorrectBackground, CorrectForeground, WrongForeground, WrongBackground
    }
    private Button _button;
    private Image _image;
    private bool _tint;
    private GameState _state;

    [SerializeField] private Image cross;
    [SerializeField] private Image background;
    [SerializeField] private Image square;

    [SerializeField] private float scaleUpDuration = .15f;
    [SerializeField] private float scaleDownDuration = .15f;
    [SerializeField] private Vector2 scaleUpVector = Vector2.one * 1.2f;
    
    public Vector2Int CellCord { get; private set; }
    public bool IsBackground { get; private set; }

    public bool Marked { get; private set; }

    private Action<ClickMode, Cell> _callback;

    public void Init(Vector2Int cellCord, List<Vector2Int> backgroundPixelCords, Action<ClickMode, Cell> callback)
    {
        CellCord = cellCord;
        IsBackground = false;
        _callback = callback; 
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

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OnClick()
    {
        if(Marked) return;
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

        _callback(cm, this);
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

    public void MarkCell(CellMarkMode mode)
    {
        if(Marked) return;
        Marked = true;

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

    public void AnimateFinished(float startDelay)
    {
        
        var scaleUp = transform
            .DOScale(new Vector3(scaleUpVector.x, scaleUpVector.y, 1), scaleUpDuration)
            .SetEase(Ease.InCubic);
        
        var  scaleDown = transform
            .DOScale(new Vector3(1f,1f,0), scaleDownDuration)
            .SetEase(Ease.OutCubic);

        DOTween.Sequence()
            .Prepend(scaleUp)
            .Append(scaleDown)
            .SetDelay(startDelay);
    }
}