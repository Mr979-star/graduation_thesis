using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Line
{
    List<(int, int)> pixels = new();

    public void SetPixel((int, int) pos)
    {
        pixels.Add(pos);
    }

    public void Draw(Texture2D texture)
    {
        foreach ((int, int) pos in pixels) texture.SetPixel(pos.Item1, pos.Item2, Color.black);
    }

    EdgeBehaviour edge1;
    EdgeBehaviour edge2;

    public EdgeBehaviour Edge1 => edge1;
    public EdgeBehaviour Edge2 => edge2;

    public void Set(EdgeBehaviour edge1, EdgeBehaviour edge2)
    {
        this.edge1 = edge1; this.edge2 = edge2;
    }

    public bool ConnectWith(EdgeBehaviour edge) => edge1 == edge || edge2 == edge;
    public bool ConnectWith(CrossBehaviour cross) => edge1.transform.parent == cross.transform || edge2.transform.parent == cross.transform;

    public void ChangeEdge(EdgeBehaviour from, EdgeBehaviour to)
    {
        if (from == edge1) edge1 = to;
        else if (from == edge2) edge2 = to;
    }
}
