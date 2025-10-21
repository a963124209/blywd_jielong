using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMathLib
{
    public static Vector2 GetFootPoint(Vector2 ptTest, Vector2 p1, Vector2 p2)
    {
        if (p2.x == p1.x && p2.y == p1.y) {
            return p1;
        }
        float k = -((p1.x - ptTest.x) * (p2.x - p1.x) + (p1.y - ptTest.y) * (p2.y - p1.y)) / ((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y));
        float xx = k * (p2.x - p1.x) + p1.x;
        float yy = k * (p2.y - p1.y) + p1.y;
        return new Vector2(xx, yy);
    }

}
