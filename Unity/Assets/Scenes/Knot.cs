using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Knot
{
    public List<Cross> crosses = new();

    public List<List<Edge>> edgeTracks;
    public int Writhe { get; set; }

    public Knot(List<Cross> crosses)
    {
        this.crosses = crosses; SetOrientation();
    }

    public Knot Copy() => new Knot(new List<Cross>(crosses));

    public Edge ConnectedEdge(int crossIndex, int edgeIndex) => crosses[crossIndex].ConnectedEdge(edgeIndex);

    public void Print()
    {
        foreach (List<Edge> track in edgeTracks)
        {
            string text = "";
            foreach (Edge edge in track) text += edge.String();

            Debug.Log(text);
        }
    }

    void SetOrientation()
    {
        edgeTracks = new();

        if (crosses.Count() == 0) return;

        Edge startEdge = new(0, 0);
        Edge currentEdge = new(0, 0);
        edgeTracks.Add(new());

        while (true)
        {
            Edge connected = crosses[currentEdge.crossIndex].ConnectedEdge(currentEdge.edgeIndex);

            edgeTracks[^1].Add(currentEdge.Copy());
            edgeTracks[^1].Add(connected.Copy());

            if (edgeTracks.Sum(track => track.Count) == 4 * crosses.Count) break;            

            currentEdge.crossIndex = connected.crossIndex;
            currentEdge.edgeIndex = (connected.edgeIndex + 2) % 4;

            if (currentEdge.crossIndex == startEdge.crossIndex && currentEdge.edgeIndex == startEdge.edgeIndex)
            {
                edgeTracks.Add(new());

                for (int i = 0; i < crosses.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (edgeTracks.Sum(track => track.Count(edge => edge.crossIndex == i && edge.edgeIndex == j)) == 0)
                        {
                            startEdge = new(i, j);
                            currentEdge = new(i, j);
                        }
                    }
                }
            }
        }        
    }

    public void ChangeOrientation(int trackIndex)
    {
        List<Edge> track = edgeTracks[trackIndex];
        track.Reverse();
    }

    (Knot, int) A_Separate(Knot target)
    {
        Knot knot = target.Copy();
        int crossIndex = knot.crosses.Count() - 1;

        Edge with0 = knot.ConnectedEdge(crossIndex, 0);
        Edge with1 = knot.ConnectedEdge(crossIndex, 1);
        Edge with2 = knot.ConnectedEdge(crossIndex, 2);
        Edge with3 = knot.ConnectedEdge(crossIndex, 3);

        int trivialCount = 0;
        if (with0.crossIndex == crossIndex && with0.edgeIndex == 3) trivialCount++;
        if (with1.crossIndex == crossIndex && with1.edgeIndex == 2) trivialCount++;
        if (with0.crossIndex == crossIndex && with0.edgeIndex == 1 && with2.crossIndex == crossIndex && with2.edgeIndex == 3) trivialCount++;

        if (with0.crossIndex == crossIndex && with0.edgeIndex == 1)
        {
            knot.crosses[with2.crossIndex].connectedEdges[with2.edgeIndex] = with3;
            knot.crosses[with3.crossIndex].connectedEdges[with3.edgeIndex] = with2;
        }
        else if (with2.crossIndex == crossIndex && with2.edgeIndex == 3)
        {
            knot.crosses[with0.crossIndex].connectedEdges[with0.edgeIndex] = with1;
            knot.crosses[with1.crossIndex].connectedEdges[with1.edgeIndex] = with0;
        }
        else
        {
            knot.crosses[with0.crossIndex].connectedEdges[with0.edgeIndex] = with3;
            knot.crosses[with3.crossIndex].connectedEdges[with3.edgeIndex] = with0;
            knot.crosses[with1.crossIndex].connectedEdges[with1.edgeIndex] = with2;
            knot.crosses[with2.crossIndex].connectedEdges[with2.edgeIndex] = with1;
        }

        knot.crosses.RemoveAt(crossIndex);

        return (knot, trivialCount);
    }

    (Knot, int) B_Separate(Knot target)
    {
        Knot knot = target.Copy();
        int crossIndex = knot.crosses.Count() - 1;

        Edge with0 = knot.ConnectedEdge(crossIndex, 0);
        Edge with1 = knot.ConnectedEdge(crossIndex, 1);
        Edge with2 = knot.ConnectedEdge(crossIndex, 2);
        Edge with3 = knot.ConnectedEdge(crossIndex, 3);

        int trivialCount = 0;
        if (with0.crossIndex == crossIndex && with0.edgeIndex == 1) trivialCount++;
        if (with2.crossIndex == crossIndex && with2.edgeIndex == 3) trivialCount++;
        if (with1.crossIndex == crossIndex && with1.edgeIndex == 2 && with0.crossIndex == crossIndex && with0.edgeIndex == 3) trivialCount++;

        if (with0.crossIndex == crossIndex && with0.edgeIndex == 3)
        {
            knot.crosses[with1.crossIndex].connectedEdges[with1.edgeIndex] = with2;
            knot.crosses[with2.crossIndex].connectedEdges[with2.edgeIndex] = with1;
        }
        else if (with1.crossIndex == crossIndex && with1.edgeIndex == 2)
        {
            knot.crosses[with0.crossIndex].connectedEdges[with0.edgeIndex] = with3;
            knot.crosses[with3.crossIndex].connectedEdges[with3.edgeIndex] = with0;
        }
        else
        {
            knot.crosses[with0.crossIndex].connectedEdges[with0.edgeIndex] = with1;
            knot.crosses[with1.crossIndex].connectedEdges[with1.edgeIndex] = with0;
            knot.crosses[with2.crossIndex].connectedEdges[with2.edgeIndex] = with3;
            knot.crosses[with3.crossIndex].connectedEdges[with3.edgeIndex] = with2;
        }

        knot.crosses.RemoveAt(crossIndex);

        return (knot, trivialCount);
    }

    Polynomial SeparatedPolynomial(Knot target, string separates)
    {
        Polynomial polynomial = new(new Term(1, 0));
        Polynomial A = new(new Term(1, 1));
        Polynomial B = new(new Term(1, -1));
        Polynomial trivial = new(new Term(-1, -2), new Term(-1, 2));

        int trivialCount = 0;

        foreach (char separate in separates)
        {
            if (separate == 'A')
            {
                polynomial.Times(A);
                (Knot, int) separateResult = A_Separate(target);

                target = separateResult.Item1;
                trivialCount += separateResult.Item2;
            }

            if (separate == 'B')
            {
                polynomial.Times(B);
                (Knot, int) separateResult = B_Separate(target);

                target = separateResult.Item1;
                trivialCount += separateResult.Item2;
            }
        }

        for (int i = 0; i < trivialCount - 1; i++) polynomial.Times(trivial);

        return polynomial;
    }

    Polynomial Bracket_Polynomial(Knot target)
    {
        Polynomial polynomial = new();

        void Func(string separates, int n)
        {
            if (separates.Length == n)
            {
                Polynomial add = SeparatedPolynomial(target, separates);
                Debug.Log(separates + ": " + add.String("A"));
                polynomial.Add(add);
            }
            else
            {
                Func(separates + "A", n);
                Func(separates + "B", n);
            }
        }

        Func("", target.crosses.Count());
        Debug.Log("Bracket: " + polynomial.String("A"));

        return polynomial;
    }

    Polynomial X_Polynomial(Knot target)
    {
        Polynomial polynomial = Bracket_Polynomial(target);
        Polynomial factor = new(new Term(-1, 3));
        int writhe = -target.Writhe;

        if (writhe < 0)
        {
            writhe *= -1;
            factor = new(new Term(-1, -3));
        }

        for (int i = 0; i < writhe; i++)
        {
            polynomial.Times(factor);
        }

        Debug.Log("X: " + polynomial.String("A"));

        return polynomial;
    }

    public Polynomial Jones_Polynomial()
    {
        Polynomial polynomial = X_Polynomial(this);

        foreach (Term term in polynomial.terms)
        {
            term.exponent *= -1;
            term.denominator = 4;
        }

        Debug.Log("Jones: " + polynomial.String("t"));

        return polynomial;
    }
}