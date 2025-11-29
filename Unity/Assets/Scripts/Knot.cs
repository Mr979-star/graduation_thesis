using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Knot
{
    public List<Cross> crosses = new();
    public List<List<(int, int)>> edgeTracks;

    public Knot(List<Cross> crosses)
    {
        this.crosses = crosses;
    }

    public Knot Copy() => new(new List<Cross>(crosses));

    public (int, int) ConnectedEdge(int crossIndex, int edgeIndex) => crosses[crossIndex].connectedEdges[edgeIndex];

    public void SetOrientation()
    {
        edgeTracks = new();

        if (crosses.Count == 0) return;

        (int, int) startEdge = (0, 0);
        (int, int) currentEdge = (0, 0);
        edgeTracks.Add(new());

        while (true)
        {
            (int, int) nextEdge = ConnectedEdge(currentEdge.Item1, currentEdge.Item2);

            edgeTracks[^1].Add(currentEdge);
            edgeTracks[^1].Add(nextEdge);

            if (edgeTracks.Sum(track => track.Count) == 4 * crosses.Count) break;            

            currentEdge.Item1 = nextEdge.Item1;
            currentEdge.Item2 = (nextEdge.Item2 + 2) % 4;

            if (currentEdge == startEdge)
            {
                edgeTracks.Add(new());

                for (int i = 0; i < crosses.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (edgeTracks.All(track => !track.Contains((i, j))))
                        {
                            startEdge = (i, j);
                            currentEdge = (i, j);
                        }
                    }
                }
            }
        }
    }

    public void ChangeOrientation((int, int) edge)
    {
        foreach (List<(int, int)> track in edgeTracks)
        {
            if (track.Contains(edge))
            {
                track.Reverse(); break;
            }
        }
    }

    int Writhe()
    {
        for (int i = 0; i < edgeTracks.Count; i++)
        {
            for (int j = 0; j < edgeTracks[i].Count; j += 2)
            {
                crosses[edgeTracks[i][j].Item1].SetTrack(edgeTracks[i][j].Item2);
            }
        }

        return crosses.Sum(cross => cross.Sign());
    }

    void ConnectEdges((int, int) edge1, (int, int) edge2)
    {
        crosses[edge1.Item1].connectedEdges[edge1.Item2] = edge2;
        crosses[edge2.Item1].connectedEdges[edge2.Item2] = edge1;
    }

    int A_Separate(Knot knot)
    {
        int crossIndex = knot.crosses.Count - 1;
        int trivialCount = 0;
        (int, int) connect0 = knot.ConnectedEdge(crossIndex, 0);
        (int, int) connect1 = knot.ConnectedEdge(crossIndex, 1);
        (int, int) connect2 = knot.ConnectedEdge(crossIndex, 2);
        (int, int) connect3 = knot.ConnectedEdge(crossIndex, 3);

        if (connect0 == (crossIndex, 1) && connect2 == (crossIndex, 3))
        {
            trivialCount = 1;
        }
        else if (connect0 == (crossIndex, 3) && connect1 == (crossIndex, 2))
        {
            trivialCount = 2;
        }
        else if (connect0 == (crossIndex, 1))
        {
            knot.ConnectEdges(connect2, connect3);
        }
        else if (connect2 == (crossIndex, 3))
        {
            knot.ConnectEdges(connect0, connect1);
        }
        else if (connect0 == (crossIndex, 3))
        {
            trivialCount = 1;
            knot.ConnectEdges(connect1, connect2);
        }
        else if (connect1 == (crossIndex, 2))
        {
            trivialCount = 1;
            knot.ConnectEdges(connect0, connect3);
        }
        else
        {
            knot.ConnectEdges(connect0, connect3);
            knot.ConnectEdges(connect1, connect2);
        }
        knot.crosses.RemoveAt(crossIndex);

        return trivialCount;
    }

    int B_Separate(Knot knot)
    {
        int crossIndex = knot.crosses.Count - 1;
        int trivialCount = 0;
        (int, int) connect0 = knot.ConnectedEdge(crossIndex, 0);
        (int, int) connect1 = knot.ConnectedEdge(crossIndex, 1);
        (int, int) connect2 = knot.ConnectedEdge(crossIndex, 2);
        (int, int) connect3 = knot.ConnectedEdge(crossIndex, 3);

        if (connect0 == (crossIndex, 1) && connect2 == (crossIndex, 3))
        {
            trivialCount = 2;
        }
        else if (connect0 == (crossIndex, 3) && connect1 == (crossIndex, 2))
        {
            trivialCount = 1;
        }
        else if (connect0 == (crossIndex, 1))
        {
            trivialCount = 1;
            knot.ConnectEdges(connect2, connect3);
        }
        else if (connect2 == (crossIndex, 3))
        {
            trivialCount = 1;
            knot.ConnectEdges(connect0, connect1);
        }
        else if (connect0 == (crossIndex, 3))
        {            
            knot.ConnectEdges(connect1, connect2);
        }
        else if (connect1 == (crossIndex, 2))
        {
            knot.ConnectEdges(connect0, connect3);
        }
        else
        {
            knot.ConnectEdges(connect0, connect1);
            knot.ConnectEdges(connect2, connect3);
        }
        knot.crosses.RemoveAt(crossIndex);

        return trivialCount;
    }

    Polynomial SeparatedPolynomial(string separates)
    {
        Knot knot = Copy();
        Polynomial polynomial = new(new Term(1, new Fraction(0)));
        Polynomial a = new(new Term(1, new Fraction(1)));
        Polynomial b = new(new Term(1, new Fraction(-1)));
        Polynomial trivial = new(new Term(-1, new Fraction(-2)), new Term(-1, new Fraction(2)));
        int trivialCount = 0;

        foreach (char separate in separates)
        {
            if (separate == 'a')
            {
                polynomial.Times(a);
                trivialCount += A_Separate(knot);
            }
            else if (separate == 'b')
            {
                polynomial.Times(b);
                trivialCount += B_Separate(knot);
            }
        }

        for (int i = 0; i < trivialCount - 1; i++) polynomial.Times(trivial);

        return polynomial;
    }

    Polynomial Bracket_Polynomial()
    {
        Polynomial polynomial = new();

        void Func(string separates)
        {
            if (separates.Length == crosses.Count)
            {
                Polynomial add = SeparatedPolynomial(separates);
                //Debug.Log(separates + ": " + add.String("A"));
                polynomial.Add(add);
            }
            else
            {
                Func(separates + "a");
                Func(separates + "b");
            }
        }

        Func("");
        Debug.Log("Bracket: " + polynomial.String("A"));

        return polynomial;
    }

    Polynomial X_Polynomial()
    {
        int writhe = Writhe();
        Term term = new(1 - 2 * (Mathf.Abs(writhe) % 2), new Fraction(3 * -writhe));
        Polynomial polynomial = Bracket_Polynomial();

        polynomial.Times(new Polynomial(term));

        Debug.Log("X: " + polynomial.String("A"));

        return polynomial;
    }

    public Polynomial Jones_Polynomial()
    {
        Polynomial polynomial = X_Polynomial();

        foreach (Term term in polynomial.terms)
        {
            term.exponent.Divide(-4);
        }

        Debug.Log("Jones: " + polynomial.String("t"));

        return polynomial;
    }
}