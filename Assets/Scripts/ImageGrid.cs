using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bunnogram;
using DG.Tweening;
using RTLTMPro;
using ScriptableObjects;
using UniRx;
using UniRx.Triggers;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ImageGrid : MonoBehaviour
{
    [SerializeField] private Cell cell;

    private GridLayoutGroup _gridLayoutGroup;

    // private PixelatedImage _pixelatedImage;
    private List<Cell> _cells;
    private List<NumberIndicator> _indicators;
    private Image _image;
    private PixelatedImage _currentPixelatedImage;
    private AudioSource _audioSource;
    // Main grid
    [SerializeField] private GameObject mainGrid;

    // Number indicators
    [SerializeField] private GameObject row;
    [SerializeField] private GameObject col;
    [SerializeField] private NumberIndicator numberIndicator;

    private void InitVariables()
    {
        if (_gridLayoutGroup == null)
            _gridLayoutGroup = mainGrid.GetComponent<GridLayoutGroup>();

        if (_image == null)
            _image = mainGrid.GetComponent<Image>();

        _cells ??= new List<Cell>();

        _indicators ??= new List<NumberIndicator>();
    }

    private void SetCurrentSquareCount(PixelatedImage pixelatedImage)
    {
        var allSquares = pixelatedImage.bounds.x * pixelatedImage.bounds.y;
        var backgroundSquares = pixelatedImage.backgroundPixels.Count;
        var currentSquareCount = allSquares - backgroundSquares;

        GameState.Instance.Get<ReactiveProperty<int>>(Constants.CurrentSquareKey).Value = currentSquareCount;
    }

    public void SetupGrid(PixelatedImage pixelatedImage, AudioSource source)
    {
        DestroyPrevGrid(pixelatedImage);

        InitVariables();
        SetCurrentSquareCount(pixelatedImage);

        // set current pixelated image
        _currentPixelatedImage = pixelatedImage;
        
        // set audio source
        _audioSource = source;

        // _image.sprite = pixelatedImage.sprite;
        // get the width and height of the image
        var wImage = pixelatedImage.sprite.rect.width;
        var hImage = pixelatedImage.sprite.rect.height;

        // calculate the width and height of each square in pixels
        var wSquare = wImage / pixelatedImage.bounds.x;
        var hSquare = hImage / pixelatedImage.bounds.y;

        // set the cell size
        _gridLayoutGroup.cellSize = new Vector2(wSquare, hSquare);
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        _gridLayoutGroup.constraintCount = pixelatedImage.bounds.y;

        // generate cells
        for (var i = 0; i < pixelatedImage.bounds.x; i++)
        for (var j = 0; j < pixelatedImage.bounds.y; j++)
        {
            var c = Instantiate(cell, mainGrid.transform);
            c.Init(new Vector2Int(i, j), pixelatedImage.backgroundPixels, CheckForOCompletion);
            _cells.Add(c);
        }

        // handle row number indicators
        var rightPadding = 10;
        var topPadding = 10;

        row.GetComponent<GridLayoutGroup>().cellSize = new Vector2(wSquare, hSquare + 100);
        row.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
        row.GetComponent<GridLayoutGroup>().constraintCount = 1;
        row.GetComponent<RectTransform>().anchoredPosition =
            new Vector2(-rightPadding, (hImage + hSquare + 100) / 2 + topPadding);
        for (var j = 0; j < pixelatedImage.bounds.x; j++)
        {
            var indicator = Instantiate(numberIndicator, row.transform);
            indicator.SetTransform(15, true);
            indicator.SetRowIndicatorNumbers(pixelatedImage, j);
            _indicators.Add(indicator);
        }

        // handle col number indicators
        col.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(wImage + rightPadding), 0);
        col.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        col.GetComponent<GridLayoutGroup>().constraintCount = 1;
        col.GetComponent<GridLayoutGroup>().cellSize = new Vector2(wSquare + 100, hSquare);
        for (var i = 0; i < pixelatedImage.bounds.y; i++)
        {
            var indicator = Instantiate(numberIndicator, col.transform);
            indicator.SetTransform(15, false);
            indicator.SetColIndicatorNumbers(pixelatedImage, i);
            _indicators.Add(indicator);
        }
    }

    private void CheckForOCompletion(ClickMode mode, Cell markedCell)
    {
        // only check when an O has been marked
        if (mode == ClickMode.BackgroundSelection) return;

        if (_currentPixelatedImage == null) return;

        // cell cords
        var markedRow = markedCell.CellCord.x;
        var markedCol = markedCell.CellCord.y;

        // max cords
        var maxRow = _currentPixelatedImage.bounds.x;
        var maxCol = _currentPixelatedImage.bounds.y;

        // flags
        var allOClicked = true;

        // iterate over the entire row
        for (var j = 0; j < maxRow; j++)
        {
            // check if all O cells have been marked
            var idx = markedRow * maxRow + j;
            var c = _cells[idx];

            // skip X cells
            if (c.IsBackground) continue;

            // we don't care about marked cells
            if (c.Marked) continue;

            // if O cell is not marked skip the row
            allOClicked = false;
            break;
        }

        // if yes, then mark entire X cells of that row
        if (allOClicked) MarkAllRowXCells(markedRow, maxRow);

        allOClicked = true;
        // iterate over the entire col
        for (var i = 0; i < maxCol; i++)
        {
            // check if all O cells have been marked
            var idx = i * maxCol + markedCol;
            var c = _cells[idx];

            // skip X cells
            if (c.IsBackground) continue;

            // we don't care about marked cells
            if (c.Marked) continue;

            // if O cell is not marked skip the col
            allOClicked = false;
            break;
        }

        // if yes, then mark entire X cells of that col
        if (allOClicked) MarkAllColXCells(markedCol, maxCol);
    }

    private void MarkAllColXCells(int markedCol, int maxCol)
    {
        for (var i = 0; i < maxCol; i++)
        {
            var idx = i * maxCol + markedCol;
            var c = _cells[idx];

            // we don't care about marked cells
            if (c.Marked) continue;

            // we are only marking X, so skip Os
            if (!c.IsBackground) continue;

            c.MarkCell(Cell.CellMarkMode.CorrectBackground);
        }

        // play sound
        if (!IsPlaying())
            _audioSource.Play();
        
        // animate row finished
        for (var i = 0; i < maxCol; i++)
        {
            var idx = i * maxCol + markedCol;
            var c = _cells[idx];
            c.AnimateFinished(i * .05f);
        }
    }

    private bool IsPlaying()
    {
        return _audioSource.isPlaying && (_audioSource.time / _audioSource.clip.length > .7f);
    }

    private void MarkAllRowXCells(int markedRow, int maxRow)
    {
        for (var j = 0; j < maxRow; j++)
        {
            var idx = markedRow * maxRow + j;
            var c = _cells[idx];

            // we don't care about marked cells
            if (c.Marked) continue;

            // we are only marking X, so skip Os
            if (!c.IsBackground) continue;

            c.MarkCell(Cell.CellMarkMode.CorrectBackground);
        }

        // play sound
        if (!IsPlaying())
            _audioSource.Play();
        
        // animate row finished
        for (var j = 0; j < maxRow; j++)
        {
            var idx = markedRow * maxRow + j;
            var c = _cells[idx];
            c.AnimateFinished(j * .05f);
        }
    }


    public void DestroyPrevGrid(PixelatedImage pixelatedImage)
    {
        if (_cells == null) return;

        if (_indicators == null) return;

        foreach (var indicator in _indicators) Destroy(indicator.gameObject);

        foreach (var c in _cells) Destroy(c.gameObject);

        _indicators.Clear();
        _cells.Clear();
    }
}