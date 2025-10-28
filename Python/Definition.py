from PolynomialDefinition import *
        
class Cross:
    edges = []
    sgn = 0
    from_0or2 = -1
    from_1or3 = -1
    
    def __init__(self, edges):
        self.edges = edges
    
    def Copy(self):
        return Cross(self.edges.copy())
    
    def Print(self):
        print(str(self.edges) + ", " + str(self.sgn))
        
    def SetSgn(self, n):
        if n % 2 == 0:
            self.from_0or2 = n
        else:
            self.from_1or3 = n
            
        if self.from_0or2 == -1 or self.from_1or3 == -1:
            return
        
        if self.from_0or2 == 0 and self.from_1or3 == 1:
            self.sgn = 1
        if self.from_0or2 == 0 and self.from_1or3 == 3:
            self.sgn = -1
        if self.from_0or2 == 2 and self.from_1or3 == 1:
            self.sgn = -1
        if self.from_0or2 == 2 and self.from_1or3 == 3:
            self.sgn = 1
            
class Knot:
    crosses = []
    orientStartPosses = [[0, 0]]
    writhe = 0
                
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
            
    def ConnectedEdge(self, crossIndex, edgeIndex):
        return self.crosses[crossIndex].edges[edgeIndex]
            
    def SetWrithe(self):
        if self.Count() == 0:
            return
        
        startIndex = 0
        currentCrossIndex = 0
        currentEdgeIndex = 0
        
        while True:
            [nextCrossIndex, nextEdgeIndex] = self.ConnectedEdge(currentCrossIndex, currentEdgeIndex)
            self.crosses[nextCrossIndex].SetSgn(nextEdgeIndex)
            
            if sum(cross.sgn == 0 for cross in self.crosses) == 0:
                break
                        
            currentCrossIndex = nextCrossIndex
            currentEdgeIndex = (nextEdgeIndex + 2) % 4
                           
            if [currentCrossIndex, currentEdgeIndex] == self.orientStartPosses[startIndex]:
                startIndex += 1
                [currentCrossIndex, currentEdgeIndex] = self.orientStartPosses[startIndex]
        
        for cross in self.crosses:
            self.writhe += cross.sgn
            
    def ConnectEdges(self, edge1, edge2):
        self.crosses[edge1[0]].edges[edge1[1]] = edge2
        self.crosses[edge2[0]].edges[edge2[1]] = edge1
        

class Calculator:
    def A_Separate(self, target):
        knot = target.Copy()
        crossIndex = len(knot.crosses) - 1

        with0 = knot.ConnectedEdge(crossIndex, 0)
        with1 = knot.ConnectedEdge(crossIndex, 1)
        with2 = knot.ConnectedEdge(crossIndex, 2)
        with3 = knot.ConnectedEdge(crossIndex, 3)

        trivialCount = 0

        if with0 == [crossIndex, 3]:
            trivialCount += 1
        if with1 == [crossIndex, 2]:
            trivialCount += 1
        if with0 == [crossIndex, 1] and with2 == [crossIndex, 3]:
            trivialCount += 1

        if with0 == [crossIndex, 1]:
            knot.ConnectEdges(with2, with3)

        elif with2 == [crossIndex, 3]:
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

        if with0 == [crossIndex, 1]:
            trivialCount += 1
        if with2 == [crossIndex, 3]:
            trivialCount += 1
        if with1 == [crossIndex, 2] and with0 == [crossIndex, 3]:
            trivialCount += 1

        if with0 == [crossIndex, 3]:
            knot.ConnectEdges(with1, with2)

        elif with1 == [crossIndex, 2]:
            knot.ConnectEdges(with0, with3)

        else:
            knot.ConnectEdges(with0, with1)
            knot.ConnectEdges(with2, with3)

        knot.crosses.pop(crossIndex)

        return knot, trivialCount

    def SeparatedPolynomial(self, knot, separates):
        tmp = knot
        polynomial = Polynomial([Variable(1, 0)])
        A = Polynomial([Variable(1, 1)])
        B = Polynomial([Variable(1, -1)])
        trivialPolynomial = Polynomial([Variable(-1, -2), Variable(-1, 2)])

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
        polynomial = Polynomial([])

        def Func(abList, n):
            if len(abList) == n:          
                add = self.SeparatedPolynomial(knot, abList)
                print("".join(abList) + " : " + add.ToString("A"))
                polynomial.Add(add)

            else:
                Func(abList + ["A"], n)
                Func(abList + ["B"], n)

        Func([], knot.Count())
        print("\n---Bracket---")
        polynomial.Print("A")
        return polynomial


    def X_Polynomial(self, knot):
        polynomial = self.Bracket_Polynomial(knot)
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


    def Jones_Polynomial(self, knot):
        polynomial = self.X_Polynomial(knot)

        for variable in polynomial.terms:
            variable.exponent *= -1
            variable.denominator = 4

        print("\n---Jones---")
        polynomial.Print("t")