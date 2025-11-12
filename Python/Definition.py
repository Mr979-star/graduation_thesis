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
    def A_Separate(self, target):
        knot = target.Copy()
        crossIndex = len(knot.crosses) - 1

        with0 = knot.ConnectedEdge(crossIndex, 0)
        with1 = knot.ConnectedEdge(crossIndex, 1)
        with2 = knot.ConnectedEdge(crossIndex, 2)
        with3 = knot.ConnectedEdge(crossIndex, 3)

        trivialCount = 0

        if with0 == (crossIndex, 3):
            trivialCount += 1
        if with1 == (crossIndex, 2):
            trivialCount += 1
        if with0 == (crossIndex, 1) and with2 == (crossIndex, 3):
            trivialCount += 1

        if with0 == (crossIndex, 1):
            knot.ConnectEdges(with2, with3)

        elif with2 == (crossIndex, 3):
            knot.ConnectEdges(with0, with1)

        else:
            knot.ConnectEdges(with0, with3)
            knot.ConnectEdges(with1, with2)

        knot.crosses.pop(crossIndex)

        return knot, trivialCount

    def B_Separate(self, target):
        knot = target.Copy()
        crossIndex = len(knot.crosses) - 1

        with0 = knot.ConnectedEdge(crossIndex, 0)
        with1 = knot.ConnectedEdge(crossIndex, 1)
        with2 = knot.ConnectedEdge(crossIndex, 2)
        with3 = knot.ConnectedEdge(crossIndex, 3)

        trivialCount = 0

        if with0 == (crossIndex, 1):
            trivialCount += 1
        if with2 == (crossIndex, 3):
            trivialCount += 1
        if with1 == (crossIndex, 2) and with0 == (crossIndex, 3):
            trivialCount += 1

        if with0 == (crossIndex, 3):
            knot.ConnectEdges(with1, with2)

        elif with1 == (crossIndex, 2):
            knot.ConnectEdges(with0, with3)

        else:
            knot.ConnectEdges(with0, with1)
            knot.ConnectEdges(with2, with3)

        knot.crosses.pop(crossIndex)

        return knot, trivialCount

    def SeparatedPolynomial(self, knot, separates):
        tmp = knot
        polynomial = Polynomial(Term(1, 0))
        A = Polynomial(Term(1, 1))
        B = Polynomial(Term(1, -1))
        trivialPolynomial = Polynomial(Term(-1, -2), Term(-1, 2))

        totalTrivialCount = 0

        for ab in separates:

            if ab == "A":
                polynomial.Times(A)
                tmp, trivialCount = self.A_Separate(tmp)

            if ab == "B":
                polynomial.Times(B)
                tmp, trivialCount = self.B_Separate(tmp)

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
                Func(abList + ["A"], n)
                Func(abList + ["B"], n)

        Func([], len(knot.crosses))
        print("\n---Bracket---")
        print(polynomial.ToString("A"))
        return polynomial

    def X_Polynomial(self, knot):
        polynomial = self.Bracket_Polynomial(knot)
        factor = Polynomial(Term(-1, 3))
        writhe = -knot.writhe

        if writhe < 0:
            writhe *= -1
            factor = Polynomial(Term(-1, -3))

        for _ in range(writhe):
            polynomial.Times(factor)

        print("\n---X---")
        print(polynomial.ToString("A"))
        return polynomial

    def Jones_Polynomial(self, knot):
        polynomial = self.X_Polynomial(knot)

        for term in polynomial.terms:
            term.exponent *= -1

        print("\n---Jones---")
        print(polynomial.ToString("t", 4))