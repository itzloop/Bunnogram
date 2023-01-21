using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Button))]
public class Cell : MonoBehaviour
{
    private Button _button;
    private Image _image;
    private bool _tint;
    private GameState _state;

    public Vector2Int CellCord { get; private set; }
    public bool IsBackground { get; private set; }

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
            MarkCell();
            return;
        }
        
        MarkWrongCell();
    }

    private void HandleForeGroundSelectionMode()
    {
        if (!IsBackground)
        {
            MarkCell();
            return;
        }
        
        MarkWrongCell();
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
        MarkCell();
    }

    private void MarkWrongCell()
    {
        var hp = GameStateHelper.GetHealthPoints();
        hp.Value--;
        Debug.Log("Wrong Cell");
        MarkCell();
    }

    private void MarkCell()
    {
        // TODO find a better way for this
        _image.color = IsBackground ? Color.blue : Color.red;
        // TODO update left square counts
    }
}