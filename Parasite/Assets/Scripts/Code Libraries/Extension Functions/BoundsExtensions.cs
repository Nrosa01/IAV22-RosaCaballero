using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class BoundsExtensions
{
    public static bool ContainsAt(this BoundsInt bounds, Vector3Int position) => position.x < bounds.max.x &&
                                                                                 position.x > bounds.min.x &&
                                                                                 position.y < bounds.max.y &&
                                                                                 position.y > bounds.min.y;

    public static bool IntersectsWith(this BoundsInt boxA, BoundsInt boxB) => boxA.xMax >= boxB.xMin &&
                                                                              boxA.xMin <= boxB.xMax &&
                                                                              boxA.yMax >= boxB.yMin &&
                                                                              boxA.yMin <= boxB.yMax;

    public static void IteratePositions(this BoundsInt bounds, Action<Vector2Int> function)
    {
        for (int col = 0; col < bounds.size.x; col++)
        {
            for (int row = 0; row < bounds.size.y; row++)
            {
                Vector2Int position = (Vector2Int)bounds.min + new Vector2Int(col, row);
                function(position);
            }
        }
    }
}