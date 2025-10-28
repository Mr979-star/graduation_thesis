class Variable:
    
    def __init__(self, coefficient, exponent):
        self.coefficient = coefficient
        self.exponent = exponent
        self.denominator = 1
        
    def Times(self, variable):
        self.coefficient *= variable.coefficient
        self.exponent += variable.exponent
        
    def ToString(self, var):
        cof = str(self.coefficient) if self.coefficient < 0 else "+" + str(self.coefficient)
        exp = ""
        
        if self.exponent == 0:
            var = ""
            exp = ""
        elif self.exponent == self.denominator:
            exp = ""
        elif self.exponent % self.denominator == 0:
            exp = "(" + str((int)(self.exponent / self.denominator)) + ")"
        else:
            exp = "(" + str(self.exponent) +  "/" + str(self.denominator) + ")"
        
        if var != "":
            if self.coefficient == 1:
                cof = "+"
            if self.coefficient == -1:
                cof = "-"
        
        return cof + var + exp
    
    def Copy(self):
        return Variable(self.coefficient, self.exponent)
        
        
class Polynomial:
    terms = []
    
    def __init__(self, terms):
        self.terms = terms
    
    def Copy(self):
        return Polynomial([v.Copy() for v in self.terms])
    
    def Update(self):
        self.terms = [v for v in self.terms if v.coefficient != 0]
        self.terms.sort(key=lambda x: -x.exponent)
    
    def Add(self, p):
        polynomial = p.Copy()
        for i in range(len(self.terms)):
            for j in range(len(polynomial.terms)):
                if self.terms[i].exponent == polynomial.terms[j].exponent:
                    self.terms[i].coefficient += polynomial.terms[j].coefficient
                    polynomial.terms[j].coefficient = 0
                    
        polynomial.Update()
        self.terms += polynomial.terms
        self.Update()
        
    def TimesVariable(self, variable):
        for v in self.terms:
            v.Times(variable)
            
    def Times(self, polynomial):
        answer = Polynomial([])  
        origin = self.Copy()
        
        for v in polynomial.terms:
            tmp = origin.Copy()
            tmp.TimesVariable(v)
            
            answer.Add(tmp)
            
        self.terms = answer.terms
        
    def ToString(self, var):
        text = ""
        for v in self.terms:
            text += v.ToString(var)
        if text[0] == "+":
            text = text[1:len(text)]
        return text
        
    def Print(self, var):
        print(self.ToString(var))