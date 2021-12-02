using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ARDrawHelper
{
    /// <summary>
    /// Get the list of drawn sorted dot points in tuple.
    /// </summary>
    /// <returns></returns>
    public static List<(int, int)> GetSortedDotPoints(Dictionary<GameObject, int> dotsGameObjDict)
    {
        var dotPoints = GetConnectedDotsInDict(dotsGameObjDict);
        dotPoints.Sort();
        return dotPoints;
    }
    private static List<(int, int)> GetConnectedDotsInDict(Dictionary<GameObject, int> dotsGameObjDict)
    {
        List<(int, int)> dotPoints = new List<(int, int)>();
        foreach (var item in dotsGameObjDict)
        {
            if (item.Value >= 2)
            {
                dotPoints.Add(DeserializeDotObj(item.Key));
            }
        }
        return dotPoints;
    }
    public static (int, int) DeserializeDotObj(GameObject dotObject)
    {
        var res = dotObject.name.Split('_');
        return (int.Parse(res[1]), int.Parse(res[2]));
    }
    /// <summary>
    /// check if it is rectangle and determines maxLength of rect
    /// </summary>
    /// <param name="minVector"></param>
    /// <param name="maxVector"></param>
    /// <returns></returns>
    public static List<(int, int)> CheckRectAndGetValue(List<(int, int)> connectedDots, int[,] edgeLists, int mul)
    {
        Dictionary<(int, int), List<(int, int)>> rectangles = new Dictionary<(int, int), List<(int, int)>>();
        int maxDots = connectedDots.Count;
        for (int i = 0; i < maxDots; i++)
        {
            for (int j = i+1; j < maxDots; j++)
            {
                for (int k = j + 1; k < maxDots; k++)
                {
                    for (int l = k + 1; l < maxDots; l++)
                    {
                        var newList = new List<(int, int)>();
                        newList.Add(connectedDots[i]);
                        newList.Add(connectedDots[j]);
                        newList.Add(connectedDots[k]);
                        newList.Add(connectedDots[l]);
                        newList.Sort();
                        Debug.Log("checking points");
                        if (IsRectangle(newList, edgeLists, mul))
                        {
                            newList.ForEach(p => Debug.Log(i + ": rect formed with dot: " + p));
                            (var maxWidth, var maxLength) = GetLenAndWidthPoints(newList);
                            rectangles.Add((maxWidth, maxLength), newList);
                        }
                    }
                }
            }
        }
        if (rectangles.Count == 0)
        {
            Debug.Log("no rectangle found");
            return null;
        }
        else
        {
            (var maxWidth, var maxLength) = (0, 0);
            foreach(var key in rectangles.Keys)
            {
                (maxWidth, maxLength) = (key.Item1 + key.Item2 > maxWidth + maxLength)
                    ? (key.Item1, key.Item2) : (maxWidth, maxLength);
            }
            return rectangles[(maxWidth, maxLength)];
        }
    }
    /// <summary>
    /// Get length and width of the rect drawn.
    /// </summary>
    /// <param name="dotPoints"></param>
    /// <returns></returns>
    public static (int, int) GetLenAndWidthPoints(List<(int, int)> dotPoints)
    {
        var width = dotPoints[2].Item1 - dotPoints[1].Item1;
        var length = dotPoints[1].Item2 - dotPoints[0].Item2;

        return (width, length);
    }
    // check if sorted points form a rectangle
    private static bool IsRectangle(List<(int, int)> points, int[,] edgeLists, int mul = 3)
    {
        // x0 == x1 && x2 == x3
        // y0 == y2 && y1 == y3
        if (!(points[0].Item1 == points[1].Item1 && points[2].Item1 == points[3].Item1
            && points[0].Item2 == points[2].Item2 && points[1].Item2 == points[3].Item2))
            return false;

        // check if y is connected (0 == 1 && 2 == 3)
        if (!CheckRectYValue(points[0], points[1], edgeLists, mul) || !CheckRectYValue(points[2], points[3], edgeLists, mul))
            return false;

        // check if x is connected all the way (0 == 2 && 1 == 3)
        if (!CheckRectXValue(points[0], points[2], edgeLists, mul) || !CheckRectXValue(points[1], points[3], edgeLists, mul))
            return false;

        return true;
    }

    private static bool CheckRectYValue((int, int) firstPoint, (int, int) secondPoint, int[,] edgeLists, int mul)
    {
        int x = firstPoint.Item1 * mul;
        int oldPoint = x + firstPoint.Item2;
        for (int y = firstPoint.Item2 + 1; y <= secondPoint.Item2; y++)
        {
            int newPoint = x + y;
            if (edgeLists[oldPoint, newPoint] != 1)
            {
                Debug.Log($"no Y edge formed for point new: {newPoint} and old: {oldPoint}");
                return false;
            }
            oldPoint = x + y;
        }
        return true;
    }

    private static bool CheckRectXValue((int, int) firstPoint, (int, int) secondPoint, int[,] edgeLists, int mul)
    {
        int y = firstPoint.Item2;
        int oldPoint = firstPoint.Item1 * mul + y;
        for (int x = firstPoint.Item1 + 1; x <= secondPoint.Item1; x++)
        {
            int newPoint = x * mul + y;
            if (edgeLists[oldPoint, newPoint] != 1)
            {
                Debug.Log($"no X edge formed for point new: {newPoint} and old: {oldPoint}");
                return false;
            }
            oldPoint = x * mul + y;
        }
        return true;
    }

    private static bool IsRectangleByPoints(int x1, int y1, int x2, int y2,
                                            int x3, int y3, int x4, int y4)
    {
        double cx, cy;
        double dd1, dd2, dd3, dd4;

        cx = (x1 + x2 + x3 + x4) / 4;
        cy = (y1 + y2 + y3 + y4) / 4;

        dd1 = Math.Pow(cx - x1, 2) + Math.Pow(cy - y1, 2);
        dd2 = Math.Pow(cx - x2, 2) + Math.Pow(cy - y2, 2);
        dd3 = Math.Pow(cx - x3, 2) + Math.Pow(cy - y3, 2);
        dd4 = Math.Pow(cx - x4, 2) + Math.Pow(cy - y4, 2);
        return dd1 == dd2 && dd1 == dd3 && dd1 == dd4;
    }

    private static bool IsDoubleTap()
    {
        bool result = false;
        float MaxTimeWait = 1;
        float VariancePosition = 1;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLenght = Input.GetTouch(0).deltaPosition.magnitude;

            if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLenght < VariancePosition)
                result = true;
        }
        return result;
    }

    public static void SetLineSettings(LineRenderer currentLineRenderer, float lineWidth)
    {
        currentLineRenderer.startWidth = lineWidth;
        currentLineRenderer.endWidth = lineWidth;
        currentLineRenderer.numCornerVertices = 8;
        currentLineRenderer.numCapVertices = 8;
        currentLineRenderer.startColor = Color.white;
        currentLineRenderer.endColor = Color.white;
    }

    public static void Print2DArray<T>(T[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            string s = i + ": ";
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                s += matrix[i, j] + " ,";
            }
            Debug.Log($"{s}\n");
        }
    }

    // figure out the mid point of a ramp
    public static Vector3 GetInitialRampPos(List<(int, int)> rectFromDots, Dictionary<GameObject, int> dotsGameObjDict)
    {
        var initializePos = Vector3.zero;
        foreach (var dot in rectFromDots)
        {
            string gameName = $"dot_{dot.Item1}_{dot.Item2}";
            foreach (var item in dotsGameObjDict)
            {
                if (item.Key.name.Equals(gameName))
                {
                    Debug.Log("add object: " + item.Key.name);
                    initializePos += item.Key.transform.position;
                }
            }
        }
        return initializePos;
    }
}
