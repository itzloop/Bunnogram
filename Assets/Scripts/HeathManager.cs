using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HeathManager : MonoBehaviour
{
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private List<Image> _images;

    private void Awake()
    {
        // clear child
    }

    void Start()
    {
        _images = new List<Image>();
        var hp = GameStateHelper.GetHealthPoints();

        for (int i = 0; i < hp.Value; i++)
        {
            var im  = new GameObject($"Heart{i}").AddComponent<Image>();
            im.sprite = fullHeart;
            im.rectTransform.sizeDelta = new Vector2(150, 150);
            _images.Add(Instantiate(im, gameObject.transform));
        } 
        
        hp.AsObservable().Subscribe(x =>
        {
            if (x == 3) return;
            _images[x].sprite = emptyHeart;
        });
        
    }

}