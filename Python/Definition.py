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
        
class Cross:
    edges = []
    sgn = 0
    from1 = -1
    from2 = -1
    
    def __init__(self, edges):
        self.edges = edges
        
    def Destination(self, edgeIndex):
        return self.edges[edgeIndex]
    
    def Copy(self):
        return Cross(self.edges.copy())
    
    def Print(self):
        print(str(self.edges) + ", " + str(self.sgn))
        
    def SetSgn(self, n):
        if n % 2 == 0:
            self.from1 = n
        else:
            self.from2 = n
            
        if self.from1 == -1 or self.from2 == -1:
            return
        
        if self.from1 == 0 and self.from2 == 1:
            self.sgn = 1
        if self.from1 == 0 and self.from2 == 3:
            self.sgn = -1
        if self.from1 == 2 and self.from2 == 1:
            self.sgn = -1
        if self.from1 == 2 and self.from2 == 3:
            self.sgn = 1
        
        
class Knot:
    crosses = []
    orientStartPosses = [[0, 0]]
    writhe = 0
    componentCount = 1
                
    def Count(self):
        return len(self.crosses)
    
    def Copy(self):
        knot = Knot()
        knot.crosses = [cross.Copy() for cross in self.crosses]
        return knot
        
    def Print(self):
        for cross in self.crosses:
            cross.Print()
            
    def SetCross(self, crosses):
        self.crosses = crosses
        self.SetWrithe()
            
    def SetWrithe(self):
        if self.Count() == 0:
            return
        
        startIndex = 0
        currentCrossIndex = 0
        currentEdgeIndex = 0
        
        while True:
            dest = self.crosses[currentCrossIndex].Destination(currentEdgeIndex)
            cross = self.crosses[dest[0]]
            cross.SetSgn(dest[1])
            
            if len([cross for cross in self.crosses if cross.sgn == 0]) == 0:
                break
                        
            currentCrossIndex = dest[0]
            currentEdgeIndex = (dest[1] + 2) % 4
                           
            if [currentCrossIndex, currentEdgeIndex] == self.orientStartPosses[startIndex]:
                startIndex += 1
                self.componentCount += 1
                [currentCrossIndex, currentEdgeIndex] = self.orientStartPosses[startIndex]
                break
        
        for cross in self.crosses:
            self.writhe += cross.sgn
            
    def IsTrivial(self):
        if self.Count() == 0:
            return True
        
        currentCrossIndex = 0
        currentEdgeIndex = 0
        heightList = []
        
        while True:
            heightList += [currentEdgeIndex % 2 == 0]
            
            if len(heightList) == 2 * self.Count():
                break
            
            dest = self.crosses[currentCrossIndex].Destination(currentEdgeIndex)            
            currentCrossIndex = dest[0]
            currentEdgeIndex = (dest[1] + 2) % 4  
        
        while heightList[0] == heightList[-1]:
            heightList = heightList[-1:] + heightList[:-1]
            
        return heightList[0] == heightList[1] and heightList[1] == heightList[2]

def A_Separate(target):
    knot = target.Copy()
    crossIndex = len(knot.crosses) - 1
       
    dest0 = knot.crosses[crossIndex].Destination(0)
    dest1 = knot.crosses[crossIndex].Destination(1)
    dest2 = knot.crosses[crossIndex].Destination(2)   
    dest3 = knot.crosses[crossIndex].Destination(3)
    
    hasTrivial = dest0 == [crossIndex, 3] or dest1 == [crossIndex, 2]
    
    if dest0 == [crossIndex, 1]:
        knot.crosses[dest2[0]].edges[dest2[1]] = dest3
        knot.crosses[dest3[0]].edges[dest3[1]] = dest2
        
    elif dest2 == [crossIndex, 3]:
        knot.crosses[dest0[0]].edges[dest0[1]] = dest1
        knot.crosses[dest1[0]].edges[dest1[1]] = dest0
    
    else:
        knot.crosses[dest0[0]].edges[dest0[1]] = dest3
        knot.crosses[dest3[0]].edges[dest3[1]] = dest0
        knot.crosses[dest1[0]].edges[dest1[1]] = dest2
        knot.crosses[dest2[0]].edges[dest2[1]] = dest1
    
    knot.crosses.pop(crossIndex)
    
    return knot, hasTrivial

def B_Separate(target):
    knot = target.Copy()
    crossIndex = len(knot.crosses) - 1
       
    dest0 = knot.crosses[crossIndex].Destination(0)
    dest1 = knot.crosses[crossIndex].Destination(1)
    dest2 = knot.crosses[crossIndex].Destination(2)   
    dest3 = knot.crosses[crossIndex].Destination(3)
    
    hasTrivial = dest0 == [crossIndex, 1] or dest2 == [crossIndex, 3]
    
    if dest0 == [crossIndex, 3]:
        knot.crosses[dest1[0]].edges[dest1[1]] = dest2
        knot.crosses[dest2[0]].edges[dest2[1]] = dest1
    
    elif dest1 == [crossIndex, 2]:
        knot.crosses[dest0[0]].edges[dest0[1]] = dest3
        knot.crosses[dest3[0]].edges[dest3[1]] = dest0
    
    else:
        knot.crosses[dest0[0]].edges[dest0[1]] = dest1
        knot.crosses[dest1[0]].edges[dest1[1]] = dest0
        knot.crosses[dest2[0]].edges[dest2[1]] = dest3
        knot.crosses[dest3[0]].edges[dest3[1]] = dest2
    
    knot.crosses.pop(crossIndex)
            
    return knot, hasTrivial

def SeparatedPolynomial(knot, separates):
    tmp = knot
    polynomial = Polynomial([Variable(1, 0)])
    A = Polynomial([Variable(1, 1)])
    B = Polynomial([Variable(1, -1)])
    trivialPolynomial = Polynomial([Variable(-1, -2), Variable(-1, 2)])
    
    for ab in separates:
        
        if ab == "A":
            polynomial.Times(A)
            tmp, hasTrivial = A_Separate(tmp)
            if hasTrivial:
                polynomial.Times(trivialPolynomial)
                           
        if ab == "B":
            polynomial.Times(B)
            tmp, hasTrivial = B_Separate(tmp)
            if hasTrivial:
                polynomial.Times(trivialPolynomial)
                
    return polynomial


def Bracket_Polynomial(knot): 
    polynomial = Polynomial([])
    
    def Func(abList, n):
        if len(abList) == n:          
            add = SeparatedPolynomial(knot, abList)
            print("".join(abList) + " : " + add.ToString("A"))
            polynomial.Add(add)
            
        else:
            Func(abList + ["A"], n)
            Func(abList + ["B"], n)
            
    Func([], knot.Count())
    print("\n---Bracket---")
    polynomial.Print("A")
    return polynomial
    
    
def X_Polynomial(knot):
    polynomial = Bracket_Polynomial(knot)
    factor = Polynomial([Variable(-1, 3)])
    writhe = -knot.writhe
    
    if writhe < 0:
        writhe *= -1
        factor = Polynomial([Variable(-1, -3)])
    
    for i in range(writhe):
        polynomial.Times(factor)
        
    print("\n---X---")
    polynomial.Print("A")
    return polynomial


def Jones_Polynomial(knot):
    polynomial = X_Polynomial(knot)
    
    for variable in polynomial.terms:
        variable.exponent *= -1
        variable.denominator = 4
        
    print("\n---Jones---")
    polynomial.Print("t")