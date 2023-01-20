using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GridLayoutGroup)), RequireComponent(typeof(Image))]
public class ImageGrid : MonoBehaviour
{
    [SerializeField] private PixelatedImage pixelatedImage;
    [SerializeField] private Cell cell;
    
    private GridLayoutGroup _gridLayoutGroup;
    private Image _image;
    private List<Cell> _cells;

    [SerializeField] private Button x;
    [SerializeField] private Button o;
    [SerializeField] private Button hint;
     
    private void Awake()
    {
        // init variables
        _gridLayoutGroup = gameObject.GetComponent<GridLayoutGroup>();
        _image = gameObject.GetComponent<Image>();
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
                var c = Instantiate(cell, gameObject.transform);
                c.Init(new Vector2Int(j, i), pixelatedImage.backgroundPixels);
                _cells.Add(c);
            }
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