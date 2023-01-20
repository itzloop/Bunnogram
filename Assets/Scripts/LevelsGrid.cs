using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsGrid : LayoutGroup
{
    public int rows;
    private int columns = 2;
    public Vector2 cellSize;
    public float spacing;


    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        rows = Mathf.CeilToInt(transform.childCount / columns);
        float parentWidth = rectTransform.rect.width;

        float cellWidth = (parentWidth - spacing - m_Padding.left - m_Padding.right) / (columns);
        cellSize.x = cellWidth;
        cellSize.y = cellWidth;

        int colCount = 0;
        int rowCount = 0;
        bool isFirstChildOfRow;
        for (int i = 0; i < rectChildren.Count; i++)
        {
            isFirstChildOfRow = i % columns == 0;
            rowCount = i / columns;
            colCount = i % columns;
            var item = rectChildren[i];
            var xPos = colCount * cellSize.x + m_Padding.left + (!isFirstChildOfRow ? spacing * (colCount) : 0 );
            var yPos = rowCount * cellSize.y + (spacing * rowCount);
            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
        
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows * (cellSize.y + spacing) + 500);

    }

    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
