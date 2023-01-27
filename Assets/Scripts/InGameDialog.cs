using System;
using System.Collections;
using System.Collections.Generic;
using Bunnogram;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InGameDialog : MonoBehaviour
{
    private Image _image;
    private CanvasGroup _gameObject;

    private bool _showed;

    private float _duration = .4f;
    private float _alpha = .4f;
    
    private void Start()
    {
        _image = gameObject.GetComponent<Image>();
        _showed = false;
        
        // GameState.Instance.Get<ReactiveProperty<bool>>(Constants.RestartKey)
        //     .Where(x => x).Subscribe(x =>
        //     {
        //         _showed = false;
        //     });
    }

    public GameObject SetGameObject(CanvasGroup go)
    {
        if (_showed)
            throw new Exception("can't change _gameObject when showing the dialog.");
                
        // Destroy old game object
        if (_gameObject != null)
        {
            Destroy(_gameObject);
        }

        _gameObject = Instantiate(go, gameObject.transform);
        return _gameObject.gameObject;
    }
    
    
    public GameObject SetGameObject(CanvasGroup go, Action<GameObject> callback)
    {
        if (_showed)
            throw new Exception("can't change _gameObject when showing the dialog.");
                
        // Destroy old game object
        if (_gameObject != null)
        {
            Destroy(_gameObject);
        }

        _gameObject = Instantiate(go, gameObject.transform);
        var o = _gameObject.gameObject;
        callback(o);
        return o;
    }

    public void Show()
    {
        if (_showed) return;
        
        _showed = true;
        DOTween.Sequence()
            .Join(_image.DOFade(_alpha, _duration))
            .Join(_gameObject.DOFade(1, _duration));
    }

    public void Hide()
    {
        if (!_showed) return;
            
        _showed = false;
        DOTween.Sequence()
            .Join(_image.DOFade(0, _alpha))
            .Join(_gameObject.DOFade(0, _alpha))
            .OnComplete(() =>
            {
                Destroy(_gameObject.gameObject);
                _gameObject = null;
            });
    }
}