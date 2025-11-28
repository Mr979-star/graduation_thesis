using System.Collections.Generic;
using UnityEngine;

public class Line
{
    Texture2D texture;
    List<(int, int)> pixels = new();

    public Line(Texture2D texture)
    {
        this.texture = texture;
    }

    public void SetPixel((int, int) pos)
    {
        pixels.Add(pos);
    }

    public void Remove()
    {
        foreach ((int, int) pos in pixels) texture.SetPixel(pos.Item1, pos.Item2, Color.clear);
    }
}
