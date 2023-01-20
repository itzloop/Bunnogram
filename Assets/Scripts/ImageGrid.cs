using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ImageGrid : MonoBehaviour
{
    [SerializeField] private PixelatedImage pixelatedImage;
    [SerializeField] private Cell cell;
    
    private GridLayoutGroup _gridLayoutGroup;
    private Image _image;
    private List<Cell> _cells;
    
    // Main grid
    [SerializeField] private GameObject mainGrid;
    
    // Number indicators
    [SerializeField] private GameObject row;
    [SerializeField] private GameObject col;
    [SerializeField] private NumberIndicator numberIndicator;
    
    // Bottom panel's buttons
    [SerializeField] private Button x;
    [SerializeField] private Button o;
    [SerializeField] private Button hint;

    private void Awake()
    {
        // init variables
        _gridLayoutGroup = mainGrid.GetComponent<GridLayoutGroup>();
        _image = mainGrid.GetComponent<Image>();
        _cells = new List<Cell>();
        
        // set the image to panel
        _image.sprite = pixelatedImage.sprite;

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
        {
            for (var j = 0; j < pixelatedImage.bounds.y; j++)
            {
                var c = Instantiate(cell, mainGrid.transform);
                c.Init(new Vector2Int(j, i), pixelatedImage.backgroundPixels);
                _cells.Add(c);
            }
        }
        
        // handle row number indicators
        var rightPadding = 10;
        var topPadding = 10;
        
        row.GetComponent<GridLayoutGroup>().cellSize = new Vector2(wSquare, hSquare + 100);
        row.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
        row.GetComponent<GridLayoutGroup>().constraintCount = 1;
        row.GetComponent<RectTransform>().anchoredPosition= new Vector2(-rightPadding, (hImage + hSquare + 100) / 2 + topPadding);
        for (int i = 0; i < pixelatedImage.bounds.x; i++)
        {
            var indicator = Instantiate(numberIndicator, row.transform);
        }
        
        // handle col number indicators
        col.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(wImage + rightPadding), 0);
        col.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        col.GetComponent<GridLayoutGroup>().constraintCount = 1;
        col.GetComponent<GridLayoutGroup>().cellSize = new Vector2(wSquare + 100, hSquare);
        for (int i = 0; i < pixelatedImage.bounds.y; i++)
        {
            var indicator = Instantiate(numberIndicator, col.transform);
        }
    }

    private void Start()
    {
        
        // TODO move this some other place
        this.x.onClick.AddListener(() =>
        {
            GameStateHelper.GetClickMode().Value = ClickMode.BackgroundSelection;
        });
        this.o.onClick.AddListener(() =>
        {
            GameStateHelper.GetClickMode().Value = ClickMode.ForeGroundSelection;
        });
        this.hint.onClick.AddListener(() =>
        {
            GameStateHelper.GetClickMode().Value = ClickMode.HintSelection;
        });

        GameStateHelper.GetClickMode().AsObservable().Subscribe(mode =>
        {
            Debug.Log($"Click mode change to {mode}");
        });

    }
}