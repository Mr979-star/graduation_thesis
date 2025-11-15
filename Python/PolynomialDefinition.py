from fractions import Fraction

class Term:
    coefficient = 1
    exponent = Fraction()
    
    def __init__(self, coefficient, exponent):
        self.coefficient = coefficient
        self.exponent = exponent
        
    def Times(self, term):
        self.coefficient *= term.coefficient
        self.exponent += term.exponent
        
    def ToString(self, var):
        cof = str(self.coefficient)
        if self.coefficient > 0:
            cof = "+" + cof
        exp = ""
        
        if self.exponent == 0:
            var = ""
        else:
            if self.exponent != 1:
                exp = "(" + str(self.exponent) + ")"        
            if self.coefficient == 1:
                cof = "+"
            if self.coefficient == -1:
                cof = "-"
        
        return cof + var + exp
    
    def Copy(self):
        return Term(self.coefficient, self.exponent)

class Polynomial:
    terms = []
    
    def __init__(self, *terms):
        self.terms = list(terms)
    
    def Copy(self):
        polynomial = Polynomial()
        polynomial.terms = [term.Copy() for term in self.terms]
        return polynomial
    
    def Update(self):
        self.terms = [term for term in self.terms if term.coefficient != 0]
        self.terms.sort(key=lambda x: -x.exponent)
    
    def Add(self, polynomial):
        p = polynomial.Copy()
        for selfTerm in self.terms:
            for term in p.terms:
                if selfTerm.exponent == term.exponent:
                    selfTerm.coefficient += term.coefficient
                    term.coefficient = 0
                    
        p.Update()
        self.terms += p.terms
        self.Update()
        
    def TimesOnlyTerm(self, term):
        for t in self.terms:
            t.Times(term)
            
    def Times(self, polynomial):
        answer = Polynomial()  
        
        for term in polynomial.terms:
            p = self.Copy()
            p.TimesOnlyTerm(term)
            
            answer.Add(p)
            
        self.terms = answer.terms
        
    def ToString(self, var):
        text = "".join(term.ToString(var) for term in self.terms)
        if text[0] == "+":
            text = text[1:]
            
        return text