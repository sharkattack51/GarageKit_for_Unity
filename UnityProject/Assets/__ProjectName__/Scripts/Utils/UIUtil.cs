using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GarageKit
{
    public class UIUtil
    {
        // for left top align only
        public static Vector2 CalcGridLayoutColumnsAndRowsCount(GridLayoutGroup grid, int cells)
        {
            int x = 0;
            int y = 0;
            int x_max = 0;

            if(cells <= 1)
            {
                x_max = cells;
                y = cells;
            }
            else
            {
                x = 0;
                y = 1;
                RectTransform rectTrns = grid.GetComponent<RectTransform>();

                for(int i = 1; i <= cells; i++)
                {
                    if(((i * grid.cellSize.x) + ((i - 1) * grid.spacing.x)) - ((y - 1) * rectTrns.rect.width) <= rectTrns.rect.width)
                    {
                        x++;
                        if(x_max < x)
                            x_max = x;
                    }
                    else
                    {
                        x = 0;
                        y++;
                    }
                }
            }

            return new Vector2(x_max, y);
        }

        public static float ScrollToPosition(ScrollRect scrlRect, RectTransform toTarget, float padding = 0.0f, bool asVertical = true)
        {
            if(scrlRect == null || toTarget == null)
                return 0.0f;

            if(asVertical)
            {
                // ScrollRectのスクロール可能範囲は content.height - viewport.height 
                float scrollableHeight = scrlRect.content.rect.height - scrlRect.viewport.rect.height;
                if(scrollableHeight > 0.0f)
                    return Mathf.Clamp01(1.0f - ((-toTarget.anchoredPosition.y - padding) / scrollableHeight));
                else
                    return 1.0f;
            }
            else
            {
                float scrollableWidth = scrlRect.content.rect.width - scrlRect.viewport.rect.width;
                if(scrollableWidth > 0.0f)
                    return Mathf.Clamp01(1.0f - ((-toTarget.anchoredPosition.x - padding) / scrollableWidth));
                else
                    return 1.0f;
            }
        }
    }
}
