using System.Collections.Generic;
using UnityEngine;

public class Map
{
    Texture2D texture;
    List<List<int>> pointLists = new();

    public Map(Texture2D texture)
    {
        this.texture = texture;

        for (int i = 0; i < texture.height; i++)
        {
            pointLists.Add(new());
            for (int j = 0; j < texture.width; j++)
            {
                pointLists[^1].Add(0);
            }
        }
    }

    public void SetPixel((int x, int y) pos)
    {
        pointLists[pos.y][pos.x]++;
    }

    public void RemovePixels(List<(int, int)> pixels)
    {
        foreach ((int x, int y) in pixels)
        {
            pointLists[y][x]--;
            if (pointLists[y][x] == 0) texture.SetPixel(x, y, Color.clear);
        }
        texture.Apply();
    }
}

public class Line
{
    Map map;
    public List<(int, int)> pixels = new();

    public Line(Map map)
    {
        this.map = map;
    }

    public void SetPixel((int, int) pos)
    {
        pixels.Add(pos);
        map.SetPixel(pos);
    }

    public void Remove()
    {
        map.RemovePixels(pixels);
    }
}
