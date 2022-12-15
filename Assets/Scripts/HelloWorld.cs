using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{

    private Tweener _tweener;
    // Start is called before the first frame update
    void Start()
    {
        _tweener = this.gameObject.transform.DOShakeScale( 1f, Vector3.one * 0.5f)
            .SetLoops(-1)
            .OnComplete(() =>
            {
                Debug.Log("done on punch scale");
            });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
