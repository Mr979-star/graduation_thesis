class Term:
    def __init__(self, coefficient, exponent):
        self.coefficient = coefficient
        self.exponent = exponent
        
    def Times(self, term):
        self.coefficient *= term.coefficient
        self.exponent += term.exponent
    
    def ToString(self, var):
        cof = f"{self.coefficient:+}"
        if self.exponent == 0:
            return cof
        else:
            if self.exponent == 1:
                exp = ""
            else:
                exp = f"({self.exponent})"
            if self.coefficient == 1:
                cof = "+"
            if self.coefficient == -1:
                cof = "-"
            return cof + var + exp
    
    def Copy(self):
        return Term(self.coefficient, self.exponent)

class Polynomial:
    def __init__(self, *terms):
        self.terms = list(terms)
    
    def Copy(self):
        return Polynomial(*[term.Copy() for term in self.terms])
    
    def Add(self, add):
        i = 0
        while i < len(add.terms):
            j = 0
            while j < len(self.terms):
                if self.terms[j].exponent == add.terms[i].exponent:
                    self.terms[j].coefficient += add.terms[i].coefficient
                    del add.terms[i]
                    i -= 1
                    if self.terms[j].coefficient == 0:
                        del self.terms[j]
                    break
                j += 1
            i += 1
        self.terms += add.terms                
    
    def TimesOnlyTerm(self, term):
        for t in self.terms:
            t.Times(term)
            
    def Times(self, times):
        answer = Polynomial()  
        for term in times.terms:
            polynomial = self.Copy()
            polynomial.TimesOnlyTerm(term)            
            answer.Add(polynomial)
        self.terms = answer.terms
    
    def ToString(self, var):
        self.terms.sort(key = lambda x: x.exponent)
        text = "".join(term.ToString(var) for term in self.terms)
        if text[0] == "+":
            text = text[1:]
        return text