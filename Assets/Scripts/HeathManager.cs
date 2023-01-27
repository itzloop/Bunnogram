using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HeathManager : MonoBehaviour
{
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private List<Image> _images;

    private IDisposable _disposable;
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

        var maxHp = hp.Value;
        _disposable = hp.AsObservable().Subscribe(x =>
        {
            if (x == maxHp)
            {
                foreach (var image in _images)
                {
                    image.color = Color.white;
                    image.sprite = fullHeart;
                }
                return;
            }
            
            _images[x].DOFade(.1f, .3f)
                .OnComplete(() => _images[x].DOFade(1, .1f)
                    .OnStart(() => _images[x].sprite = emptyHeart));
        });
    }

    private void OnDestroy()
    {
        if (_disposable == null) return;
        
        _disposable.Dispose();
    }
}