from PolynomialDefinition import *
from fractions import Fraction
import time

class Cross:
    def __init__(self, edge0, edge1, edge2, edge3):
        self.connections = [edge0, edge1, edge2, edge3]
    
    def Copy(self):
        edges = self.connections
        return Cross(edges[0], edges[1], edges[2], edges[3])
    
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
        elif self.track_0to2 == self.track_1to3:
            return 1
        else:
            return -1

class Knot:
    def __init__(self, *crosses, startEdges = [(0, 0)]):
        self.crosses = crosses
        self.SetWrithe(startEdges)
        
    def Copy(self):
        knot = Knot()
        knot.crosses = [cross.Copy() for cross in self.crosses]
        return knot
                      
    def ConnectedEdge(self, crossIndex, edgeIndex):
        return self.crosses[crossIndex].connections[edgeIndex]
                      
    def SetWrithe(self, startEdges):
        if len(self.crosses) == 0:
            self.writhe = 0
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

        del knot.crosses[crossIndex]

        return trivialCount

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

        del knot.crosses[crossIndex]

        return trivialCount

    def SeparatedPolynomial(self, knot, separate):
        knot = knot.Copy()
        polynomial = Polynomial(Term(1, Fraction(0)))
        a = Polynomial(Term(1, Fraction(1)))
        b = Polynomial(Term(1, Fraction(-1)))
        trivial = Polynomial(Term(-1, Fraction(-2)), Term(-1, Fraction(2)))
        trivialCount = 0

        for ab in separate:
            if ab == "a":
                polynomial.Times(a)
                trivialCount += self.A_Separate(knot)
            elif ab == "b":
                polynomial.Times(b)
                trivialCount += self.B_Separate(knot)

        for _ in range(trivialCount - 1):
            polynomial.Times(trivial)

        return polynomial

    def Bracket_Polynomial(self, knot): 
        polynomial = Polynomial()
        
        def AddAllSeparates(separate):
            if len(separate) == len(knot.crosses):          
                add = self.SeparatedPolynomial(knot, separate)
                polynomial.Add(add)
            else:
                AddAllSeparates(separate + "a")
                AddAllSeparates(separate + "b")
                
        AddAllSeparates("")
        print(f"bracket: {polynomial.ToString("A")}")
        return polynomial

    def X_Polynomial(self, knot):
        term = Term(1 - 2 * (knot.writhe % 2), Fraction(3 * -knot.writhe))
        polynomial = self.Bracket_Polynomial(knot)
        polynomial.Times(Polynomial(term))
        print(f"X      : {polynomial.ToString("A")}")
        return polynomial

    def Jones_Polynomial(self, knot):       
        polynomial = self.X_Polynomial(knot)
        for term in polynomial.terms:
            term.exponent /= -4
        print(f"Jones  : {polynomial.ToString("t")}")
        return polynomial
        
    def Calculate(self, knot):
        print(f"\n---{knot.__class__.__name__}---")       
        startTime = time.perf_counter() 
        self.Jones_Polynomial(knot)        
        endTime = time.perf_counter()
        print(f"time   : {endTime - startTime}s")