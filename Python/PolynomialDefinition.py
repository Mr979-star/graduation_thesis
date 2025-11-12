class Term:
    coefficient = 1
    exponent = 0
    
    def __init__(self, coefficient, exponent):
        self.coefficient = coefficient
        self.exponent = exponent
        
    def Times(self, term):
        self.coefficient *= term.coefficient
        self.exponent += term.exponent
        
    def ToString(self, var, denom):
        cof = str(self.coefficient)
        if self.coefficient > 0:
            cof = "+" + cof
        exp = ""
        
        if self.exponent == 0:
            var = ""
        else:
            n = 2
            num = abs(self.exponent)
            while n <= min(num, denom):
                while num % n == 0 and denom % n == 0:                    
                    num = int(num / n)
                    denom = int(denom / n)
                n += 1
            
            if self.exponent < 0:
                num *= -1
            
            if num != denom:
                if denom == 1:
                    exp = "^(" + str(num) + ")"
                else:
                    exp = "^(" + str(num) +  "/" + str(denom) + ")"
        
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
            tmp = self.Copy()
            tmp.TimesOnlyTerm(term)
            
            answer.Add(tmp)
            
        self.terms = answer.terms
        
    def ToString(self, var, denominator = 1):
        text = ""
        for term in self.terms:
            text += term.ToString(var, denominator)
        if text[0] == "+":
            text = text[1:]
            
        return text