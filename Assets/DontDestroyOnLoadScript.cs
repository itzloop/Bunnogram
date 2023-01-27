using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadScript : MonoBehaviour
{
    private void Awake()
    {
        var objs = FindObjectsOfType<DontDestroyOnLoadScript>();
        if (objs.Length > 1)
        {
            for (var i = 1; i < objs.Length; i++)
            {
                Destroy(objs[i].gameObject);
            }
        }
        DontDestroyOnLoad(gameObject);
    }
}
