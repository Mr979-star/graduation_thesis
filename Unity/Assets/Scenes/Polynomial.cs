using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fraction
{
    int sgn;
    public int Numerator { get; private set; }
    public int Denominator { get; private set; }

    public Fraction(int numerator, int denominator = 1)
    {
        Numerator = numerator;
        Denominator = denominator;
        Update();
    }

    public void Add(Fraction add)
    {
        Numerator = sgn * Numerator * add.Denominator + add.sgn * add.Numerator * Denominator;
        Denominator *= add.Denominator;
        Update();
    }

    public void Divide(int div)
    {
        Numerator = sgn * Numerator;
        Denominator *= div;
        Update();
    }

    void Update()
    {
        sgn = (int)Mathf.Sign(Numerator * Denominator);
        Numerator = Mathf.Abs(Numerator);
        Denominator = Mathf.Abs(Denominator);

        int n = 2;
        while (n <= Mathf.Min(Numerator, Denominator))
        {
            while (Numerator % n == 0 && Denominator % n == 0)
            {
                Numerator /= n;
                Denominator /= n;
            }
            n++;
        }
    }

    public float Float() => (float)sgn * Numerator / Denominator;
    public bool Equal(Fraction fraction) => sgn == fraction.sgn && Numerator == fraction.Numerator && Denominator == fraction.Denominator;
    public string String()
    {
        if (sgn == 1 && Numerator == 1 && Denominator == 1) return "";
        else if (Denominator == 1) return (sgn * Numerator).ToString();
        else return $"{sgn * Numerator}/{Denominator}";
    }

    public Fraction Copy() => new(sgn * Numerator, Denominator);
}

public class Term
{
    public int coefficient;
    public Fraction exponent;

    public Term(int coefficient, Fraction exponent)
    {
        this.coefficient = coefficient;
        this.exponent = exponent;
    }

    public void Times(Term term)
    {
        coefficient *= term.coefficient;
        exponent.Add(term.exponent);
    }

    public string String(string var)
    {
        string cof = coefficient.ToString("+#;-#;");
        string exp;

        if (exponent.Numerator == 0)
        {
            return cof;
        }
        else
        {
            exp = $"<sup>{exponent.String()}</sup>";

            if (coefficient == 1) cof = "+";
            if (coefficient == -1) cof = "-";

            return cof + var + exp;
        }
    }

    public Term Copy() => new(coefficient, exponent.Copy());
}

public class Polynomial
{
    public List<Term> terms;

    public Polynomial(List<Term> terms) { this.terms = terms; }
    public Polynomial(params Term[] terms) { this.terms = terms.ToList(); }

    public void Add(Polynomial add)
    {
        int i = 0;
        while (i < add.terms.Count)
        {
            int j = 0;
            while (j < terms.Count)
            {
                if (terms[j].exponent.Equal(add.terms[i].exponent))
                {
                    terms[j].coefficient += add.terms[i].coefficient;

                    add.terms.RemoveAt(i);
                    i--;

                    if (terms[j].coefficient == 0) terms.RemoveAt(j);

                    break;
                }
                j++;
            }
            i++;
        }
        terms.AddRange(add.terms);
    }

    void Times(Term term)
    {
        foreach (Term ownTerm in terms) ownTerm.Times(term);
    }

    public void Times(Polynomial times)
    {
        Polynomial answer = new();
        foreach (Term term in times.terms)
        {
            Polynomial polynomial = Copy();
            polynomial.Times(term);
            answer.Add(polynomial);
        }
        terms = answer.terms;
    }

    public string String(string var)
    {
        terms = terms.OrderBy(term => term.exponent.Float()).ToList();

        string text = string.Join("", terms.Select(term => term.String(var)));

        if (text[0] == '+') text = text[1..];

        return text;
    }

    public Polynomial Copy() => new(terms.Select(term => term.Copy()).ToList());
}
