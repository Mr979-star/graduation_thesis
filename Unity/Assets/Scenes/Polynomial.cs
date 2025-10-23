using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Term
{
    public int coefficient;
    public int exponent;
    public int denominator = 1;

    public Term(int coefficient, int exponent) { this.coefficient = coefficient; this.exponent = exponent; }
    public Term Copy() => new(coefficient, exponent);

    public void Times(Term term)
    {
        coefficient *= term.coefficient;
        exponent += term.exponent;
    }

    public string String(string varName)
    {
        string cofText = coefficient < 0 ? coefficient.ToString() : "+" + coefficient.ToString();
        string expText;

        if (exponent == 0)
        {
            varName = ""; expText = "";
        }
        else if (exponent == denominator)
        {
            expText = "";
        }
        else if (exponent % denominator == 0)
        {
            expText = $"<sup>{exponent / denominator}</sup>";
        }
        else
        {
            expText = $"<sup>{exponent}/{denominator}</sup>";
        }

        if (varName != "")
        {
            if (coefficient == 1) cofText = "+";
            if (coefficient == -1) cofText = "-";
        }

        return cofText + varName + expText;
    }
}

public class Polynomial
{
    public List<Term> terms;

    public Polynomial(List<Term> terms) { this.terms = terms; }
    public Polynomial(params Term[] terms) { this.terms = terms.ToList(); }
    public Polynomial Copy() => new(terms.Select(x => x.Copy()).ToList());

    void RemoveZero()
    {
        terms.RemoveAll(x => x.coefficient == 0);
    }

    public void Add(Polynomial p)
    {
        Polynomial polynomial = p.Copy();

        foreach (Term ownTerm in terms)
        {
            foreach (Term addTerm in polynomial.terms)
            {
                if (ownTerm.exponent == addTerm.exponent)
                {
                    ownTerm.coefficient += addTerm.coefficient;
                    addTerm.coefficient = 0;
                }
            }
        }

        polynomial.RemoveZero();
        terms.AddRange(polynomial.terms);
        RemoveZero();
    }

    void SimpleTimes(Term term)
    {
        foreach (Term ownTerm in terms) ownTerm.Times(term);
    }

    public void Times(Polynomial polynomial)
    {
        Polynomial answer = new();
        Polynomial original = Copy();

        foreach (Term term in polynomial.terms)
        {
            Polynomial tmp = original.Copy();
            tmp.SimpleTimes(term);
            answer.Add(tmp);
        }

        terms = answer.terms;
    }

    public string String(string varName)
    {
        terms = terms.OrderBy(x => x.exponent).ToList();
        string text = "";

        foreach (Term term in terms) text += term.String(varName);

        if (text.StartsWith('+')) text = text[1..];

        return text;
    }
}
