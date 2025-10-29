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
    }
}
