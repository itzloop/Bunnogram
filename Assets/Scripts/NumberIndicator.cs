using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RTLTMPro;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class NumberIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject indicatorText;
    public void SetTransform(float offset, bool row)
    {
        if (row)
        {
            rectTransform.offsetMax = new Vector2(-offset, rectTransform.offsetMax.y);
            rectTransform.offsetMin = new Vector2(offset, rectTransform.offsetMin.x);
            var verticalLayoutGroup = rectTransform.gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
            verticalLayoutGroup.childForceExpandHeight = false;
            return;
        }
        
        
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -offset);
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, offset);
        var horizontalLayoutGroup = rectTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
        horizontalLayoutGroup.childAlignment = TextAnchor.MiddleRight;
        horizontalLayoutGroup.padding.right = 20;
        horizontalLayoutGroup.childForceExpandWidth = false;
    }

    public void AddIndicators(List<int> counts)
    {
        for (var i = 0; i < counts.Count; i++)
        {
            var textComponent = Instantiate(indicatorText, rectTransform.gameObject.transform).GetComponent<RTLTextMeshPro>();
            textComponent.text = $"{counts[i]}";
            textComponent.fontSize = 48;
        }
    }

    public void SetRowIndicatorNumbers(PixelatedImage pixelatedImage, int j)
    {
        var diffs = new List<int>();
        var list = pixelatedImage.backgroundPixels.Where(pixel => pixel.y == j).ToList();
        list.Sort((a, b) => a.x.CompareTo(b.x));
        if (list.Count == 0)
        {
            AddIndicators(new List<int>{pixelatedImage.bounds.x});
            return;
        }

        var p1 = list[0];
        // begin bounds check
        var firstDiff = p1.x - 0 ;
        if (firstDiff != 0)
            diffs.Add(firstDiff);
        
        for (int i = 1; i < list.Count; i++)
        {
            var p2 = list[i];
            var diff = p2.x - p1.x - 1;
            if (diff != 0)
                diffs.Add(diff);

            p1 = p2;
        }
        
        // end bounds check
        var lastDiff = pixelatedImage.bounds.x - p1.x - 1;
        if (lastDiff != 0)
            diffs.Add(lastDiff);

        AddIndicators(diffs);
    }
    
    
    public void SetColIndicatorNumbers(PixelatedImage pixelatedImage, int i)
    {
        var diffs = new List<int>();
        var list = pixelatedImage.backgroundPixels.Where(pixel => pixel.x == i).ToList();
        list.Sort((a, b) => a.y.CompareTo(b.y));
        if (list.Count == 0)
        {
            AddIndicators(new List<int>(){pixelatedImage.bounds.y});
            return;
        }

        var p1 = list[0];
        // begin bounds check
        var firstDiff = p1.y - 0 ;
        if (firstDiff != 0)
            diffs.Add(firstDiff);
        
        for (int j = 1; j < list.Count; j++)
        {
            var p2 = list[j];
            var diff = p2.y - p1.y - 1;
            if (diff != 0)
                diffs.Add(diff);

            p1 = p2;
        }
        
        // end bounds check
        var lastDiff = pixelatedImage.bounds.y - p1.y - 1;
        if (lastDiff != 0)
            diffs.Add(lastDiff);

        AddIndicators(diffs);
    }
}