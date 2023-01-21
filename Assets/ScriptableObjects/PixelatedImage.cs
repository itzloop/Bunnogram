using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "PixelatedImage", fileName = "PixelatedImage")]
    public class PixelatedImage : ScriptableObject
    {
        public Sprite sprite;
        public Vector2Int bounds;
        public List<Vector2Int> backgroundPixels;
    }
}
