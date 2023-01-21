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
    private PixelatedImage _pixelatedImage;
    private List<Cell> _cells;
    private Image _image;
    
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
    [SerializeField] private RTLTextMeshPro hintCount;

    private void Start()
    {
        // init variables
        _gridLayoutGroup = mainGrid.GetComponent<GridLayoutGroup>();
        _image = mainGrid.GetComponent<Image>();
        _cells = new List<Cell>();
         
        // set the image to panel
        _pixelatedImage = GameState.Instance.Get<PixelatedImage>(Constants.PixelatedImageKey);
        // _image.sprite = _pixelatedImage.sprite;
 
        // get the width and height of the image
        var wImage = _pixelatedImage.sprite.rect.width;
        var hImage = _pixelatedImage.sprite.rect.height;
 
        // calculate the width and height of each square in pixels
        var wSquare = wImage / _pixelatedImage.bounds.x;
        var hSquare = hImage / _pixelatedImage.bounds.y;
         
        // set the cell size
        _gridLayoutGroup.cellSize = new Vector2(wSquare, hSquare);
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        _gridLayoutGroup.constraintCount = _pixelatedImage.bounds.y;
         
        // generate cells
        for (var i = 0; i < _pixelatedImage.bounds.x; i++)
        {
            for (var j = 0; j < _pixelatedImage.bounds.y; j++)
            {
                var c = Instantiate(cell, mainGrid.transform);
                c.Init(new Vector2Int(i ,j), _pixelatedImage.backgroundPixels);
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
        for (int j = 0; j < _pixelatedImage.bounds.x; j++)
        {
            var indicator = Instantiate(numberIndicator, row.transform);
            indicator.SetTransform(15,true);
            indicator.SetRowIndicatorNumbers(_pixelatedImage, j);
        }
         
        // handle col number indicators
        col.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(wImage + rightPadding), 0);
        col.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        col.GetComponent<GridLayoutGroup>().constraintCount = 1;
        col.GetComponent<GridLayoutGroup>().cellSize = new Vector2(wSquare + 100, hSquare);
        for (int i = 0; i < _pixelatedImage.bounds.y; i++)
        {
            var indicator = Instantiate(numberIndicator, col.transform);
            indicator.SetTransform(15,false);
            indicator.SetColIndicatorNumbers(_pixelatedImage, i);
        }


        var duration = .3f;
        // TODO move this some other place
        this.x.onClick.AddListener(() =>
        {
            this.x.GetComponent<Image>().DOColor(Color.cyan, duration);
            this.o.GetComponent<Image>().DOColor(Color.white, duration);
            this.hint.GetComponent<Image>().DOColor(Color.white, duration);
            GameStateHelper.GetClickMode().Value = ClickMode.BackgroundSelection;
        });
        this.o.onClick.AddListener(() =>
        {
            this.x.GetComponent<Image>().DOColor(Color.white, duration);
            this.o.GetComponent<Image>().DOColor(Color.cyan, duration);
            this.hint.GetComponent<Image>().DOColor(Color.white, duration);
            GameStateHelper.GetClickMode().Value = ClickMode.ForeGroundSelection;
        });
        this.hint.onClick.AddListener(() =>
        {
            this.x.GetComponent<Image>().DOColor(Color.white, duration);
            this.o.GetComponent<Image>().DOColor(Color.white, duration);
            this.hint.GetComponent<Image>().DOColor(Color.cyan, duration);
            GameStateHelper.GetClickMode().Value = ClickMode.HintSelection;
        });

        GameStateHelper.GetHints().AsObservable().Subscribe(n =>
        {
            hintCount.text = n.ToString();
        });
    }
}