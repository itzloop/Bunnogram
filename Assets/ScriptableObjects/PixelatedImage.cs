using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ScriptableObjects
{

    [CreateAssetMenu(menuName = "PixelatedImage", fileName = "PixelatedImage")]
    public class PixelatedImage : ScriptableObject
    {
        public Sprite sprite;
        public Vector2Int bounds;
        public List<Vector2Int> backgroundPixels;
        public string levelName;
    }
}