from PolynomialDefinition import *

class Cross:
    connections = []
    
    def __init__(self, edge0, edge1, edge2, edge3):
        self.connections = [edge0, edge1, edge2, edge3]
    
    def Copy(self):
        copy = self.connections.copy()
        return Cross(copy[0], copy[1], copy[2], copy[3])
    
    track_0to2 = None
    track_1to3 = None
    
    def SetTrack(self, edgeIndex):
        if edgeIndex % 2 == 0:
            self.track_0to2 = (edgeIndex == 0)
        else:
            self.track_1to3 = (edgeIndex == 1)
    
    def Sign(self):
        if self.track_0to2 == None or self.track_1to3 == None:
            return 0
            
        if self.track_0to2 == self.track_1to3:
            return 1
        else:
            return -1

class Knot:
    crosses = []
    
    def __init__(self, *crosses, startEdges = [(0, 0)]):
        self.crosses = crosses
        self.SetWrithe(startEdges)
        
    def Copy(self):
        knot = Knot()
        knot.crosses = [cross.Copy() for cross in self.crosses]
        return knot
            
    writhe = 0
                      
    def ConnectedEdge(self, crossIndex, edgeIndex):
            return self.crosses[crossIndex].connections[edgeIndex]
                      
    def SetWrithe(self, startEdges):
        if len(self.crosses) == 0:
            return
        
        componentIndex = 0
        currentEdge = startEdges[componentIndex]
        
        while True:
            nextEdge = self.ConnectedEdge(currentEdge[0], currentEdge[1])
            self.crosses[nextEdge[0]].SetTrack(nextEdge[1])
            
            if sum(cross.Sign() == 0 for cross in self.crosses) == 0:
                break
            
            currentEdge = (nextEdge[0], (nextEdge[1] + 2) % 4)
            
            if currentEdge == startEdges[componentIndex]:
                componentIndex += 1
                currentEdge = startEdges[componentIndex]
        
        self.writhe = sum(cross.Sign() for cross in self.crosses)
            
    def ConnectEdges(self, edge1, edge2):
        self.crosses[edge1[0]].connections[edge1[1]] = edge2
        self.crosses[edge2[0]].connections[edge2[1]] = edge1


class Calculator:
    def A_Separate(self, knot):
        crossIndex = len(knot.crosses) - 1
        trivialCount = 0
        connect0 = knot.ConnectedEdge(crossIndex, 0)
        connect1 = knot.ConnectedEdge(crossIndex, 1)
        connect2 = knot.ConnectedEdge(crossIndex, 2)
        connect3 = knot.ConnectedEdge(crossIndex, 3)
                    
        if connect0 == (crossIndex, 1) and connect2 == (crossIndex, 3):
            trivialCount = 1
            
        elif connect0 == (crossIndex, 3) and connect1 == (crossIndex, 2):
            trivialCount = 2
            
        elif connect0 == (crossIndex, 1):
            knot.ConnectEdges(connect2, connect3)

        elif connect2 == (crossIndex, 3):
            knot.ConnectEdges(connect0, connect1)

        elif connect0 == (crossIndex, 3):
            trivialCount = 1
            knot.ConnectEdges(connect1, connect2)
            
        elif connect1 == (crossIndex, 2):
            trivialCount = 1
            knot.ConnectEdges(connect0, connect3)

        else:
            knot.ConnectEdges(connect0, connect3)
            knot.ConnectEdges(connect1, connect2)

        knot.crosses.pop(crossIndex)

        return knot, trivialCount

    def B_Separate(self, knot):
        crossIndex = len(knot.crosses) - 1
        trivialCount = 0
        connect0 = knot.ConnectedEdge(crossIndex, 0)
        connect1 = knot.ConnectedEdge(crossIndex, 1)
        connect2 = knot.ConnectedEdge(crossIndex, 2)
        connect3 = knot.ConnectedEdge(crossIndex, 3)

        if connect0 == (crossIndex, 1) and connect2 == (crossIndex, 3):
            trivialCount = 2
            
        elif connect0 == (crossIndex, 3) and connect1 == (crossIndex, 2):
            trivialCount = 1
            
        elif connect0 == (crossIndex, 1):
            trivialCount = 1
            knot.ConnectEdges(connect2, connect3)

        elif connect2 == (crossIndex, 3):
            trivialCount = 1
            knot.ConnectEdges(connect0, connect1)

        elif connect0 == (crossIndex, 3):
            knot.ConnectEdges(connect1, connect2)
            
        elif connect1 == (crossIndex, 2):
            knot.ConnectEdges(connect0, connect3)

        else:
            knot.ConnectEdges(connect0, connect1)
            knot.ConnectEdges(connect2, connect3)

        knot.crosses.pop(crossIndex)

        return knot, trivialCount

    def SeparatedPolynomial(self, knot, separates):
        polynomial = Polynomial(Term(1, Fraction(0)))
        A = Polynomial(Term(1, Fraction(1)))
        B = Polynomial(Term(1, Fraction(-1)))
        trivialPolynomial = Polynomial(Term(-1, Fraction(-2)), Term(-1, Fraction(2)))

        totalTrivialCount = 0

        for ab in separates:

            if ab == "a":
                polynomial.Times(A)
                knot, trivialCount = self.A_Separate(knot.Copy())

            if ab == "b":
                polynomial.Times(B)
                knot, trivialCount = self.B_Separate(knot.Copy())

            totalTrivialCount += trivialCount

        for _ in range(totalTrivialCount - 1):
            polynomial.Times(trivialPolynomial)

        return polynomial

    def Bracket_Polynomial(self, knot): 
        polynomial = Polynomial()

        def Func(abList, n):
            if len(abList) == n:          
                add = self.SeparatedPolynomial(knot, abList)
                print("".join(abList) + " : " + add.ToString("A"))
                polynomial.Add(add)

            else:
                Func(abList + ["a"], n)
                Func(abList + ["b"], n)

        Func([], len(knot.crosses))
        print("\n---Bracket---")
        print(polynomial.ToString("A"))
        return polynomial

    def X_Polynomial(self, knot):
        polynomial = self.Bracket_Polynomial(knot)
        factor = Polynomial(Term(-1, Fraction(3)))
        writhe = -knot.writhe

        if writhe < 0:
            writhe *= -1
            factor = Polynomial(Term(-1, Fraction(-3)))

        for _ in range(writhe):
            polynomial.Times(factor)

        print("\n---X---")
        print(polynomial.ToString("A"))
        return polynomial

    def Jones_Polynomial(self, knot):
        polynomial = self.X_Polynomial(knot)

        for term in polynomial.terms:
            term.exponent /= -4

        print("\n---Jones---")
        print(polynomial.ToString("t"))