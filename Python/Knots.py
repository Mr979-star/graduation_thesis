from Definition import *     
    
class Trivial(Knot):
    def __init__(self):
        self.SetCross([])
    
    
class Three_1(Knot):
    def __init__(self):
        cross0 = Cross([[1, 1], [2, 2], [2, 1], [1, 2]])
        cross1 = Cross([[2, 3], [0, 0], [0, 3], [2, 0]])
        cross2 = Cross([[1, 3], [0, 2], [0, 1], [1, 0]])
        self.SetCross([cross0, cross1, cross2])
        
        
class Four_1(Knot):
    def __init__(self):
        cross0 = Cross([[2, 1], [1, 0], [1, 3], [3, 2]])
        cross1 = Cross([[0, 1], [2, 0], [3, 3], [0, 2]])
        cross2 = Cross([[1, 1], [0, 0], [3, 1], [3, 0]])
        cross3 = Cross([[2, 3], [2, 2], [0, 3], [1, 2]])
        self.SetCross([cross0, cross1, cross2, cross3])
        
class Five_1(Knot):
    def __init__(self):
        cross0 = Cross([[1, 1], [4, 0], [4, 3], [1, 2]])
        cross1 = Cross([[2, 1], [0, 0], [0, 3], [2, 2]])
        cross2 = Cross([[3, 1], [1, 0], [1, 3], [3, 2]])
        cross3 = Cross([[4, 1], [2, 0], [2, 3], [4, 2]])
        cross4 = Cross([[0, 1], [3, 0], [3, 3], [0, 2]])
        self.SetCross([cross0, cross1, cross2, cross3, cross4])
        
class Eight_14(Knot):
    def __init__(self):
        cross0 = Cross([[2, 1], [1, 0], [1, 3], [6, 0]])
        cross1 = Cross([[0, 1], [2, 0], [4, 1], [0, 2]])
        cross2 = Cross([[1, 1], [0, 0], [3, 1], [3, 0]])
        cross3 = Cross([[2, 3], [2, 2], [7, 1], [4, 2]])
        cross4 = Cross([[5, 1], [1, 2], [3, 3], [7, 0]])
        cross5 = Cross([[6, 1], [4, 0], [7, 3], [6, 2]])
        cross6 = Cross([[0, 3], [5, 0], [5, 3], [7, 2]])
        cross7 = Cross([[4, 3], [3, 2], [6, 3], [5, 2]])
        self.SetCross([cross0, cross1, cross2, cross3, cross4, cross5, cross6, cross7])
        
class Two_1_2(Knot):
    def __init__(self, plus):
        cross0 = Cross([[1, 1], [1, 0], [1, 3], [1, 2]])
        cross1 = Cross([[0, 1], [0, 0], [0, 3], [0, 2]])
        if plus:
            self.orientStartPosses = [[0, 0], [0, 1]]
        else:
            self.orientStartPosses = [[0, 0], [0, 3]]
        self.SetCross([cross0, cross1])
        
trivial = Trivial()
three_1 = Three_1()
four_1 = Four_1()
five_1 = Five_1()
eight_14 = Eight_14()
two_1_2_plus = Two_1_2(True)
two_1_2_minus = Two_1_2(False)