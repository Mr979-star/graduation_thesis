from Definition import *     
    
class Hopf(Knot):
    def __init__(self):
        cross0 = Cross((1, 3), (1, 2), (1, 1), (1, 0))
        cross1 = Cross((0, 3), (0, 2), (0, 1), (0, 0))
        super().__init__(cross0, cross1, startEdges = [(0, 0), (0, 1)])
    
class Trivial(Knot):
    def __init__(self):
        super().__init__()
    
class Trefoil(Knot):
    def __init__(self):
        cross0 = Cross((1, 1), (2, 2), (2, 1), (1, 2))
        cross1 = Cross((2, 3), (0, 0), (0, 3), (2, 0))
        cross2 = Cross((1, 3), (0, 2), (0, 1), (1, 0))
        super().__init__(cross0, cross1, cross2)
        
class EightFigure(Knot):
    def __init__(self):
        cross0 = Cross((2, 1), (1, 0), (1, 3), (3, 2))
        cross1 = Cross((0, 1), (2, 0), (3, 3), (0, 2))
        cross2 = Cross((1, 1), (0, 0), (3, 1), (3, 0))
        cross3 = Cross((2, 3), (2, 2), (0, 3), (1, 2))
        super().__init__(cross0, cross1, cross2, cross3)
        
class Five_1(Knot):
    def __init__(self):
        cross0 = Cross((1, 1), (4, 0), (4, 3), (1, 2))
        cross1 = Cross((2, 1), (0, 0), (0, 3), (2, 2))
        cross2 = Cross((3, 1), (1, 0), (1, 3), (3, 2))
        cross3 = Cross((4, 1), (2, 0), (2, 3), (4, 2))
        cross4 = Cross((0, 1), (3, 0), (3, 3), (0, 2))
        super().__init__(cross0, cross1, cross2, cross3, cross4)
        
class Seven_3(Knot):
    def __init__(self):
        cross0 = Cross((3, 1), (1, 0), (1, 3), (6, 0))
        cross1 = Cross((0, 1), (2, 0), (2, 3), (0, 2))
        cross2 = Cross((1, 1), (3, 0), (6, 1), (1, 2))
        cross3 = Cross((2, 1), (0, 0), (4, 1), (4, 0))
        cross4 = Cross((3, 3), (3, 2), (5, 1), (5, 0))
        cross5 = Cross((4, 3), (4, 2), (6, 3), (6, 2))
        cross6 = Cross((0, 3), (2, 2), (5, 3), (5, 2))
        super().__init__(cross0, cross1, cross2, cross3, cross4, cross5, cross6)

class Hopf_minus(Knot):
    def __init__(self):
        cross0 = Cross((1, 3), (1, 2), (1, 1), (1, 0))
        cross1 = Cross((0, 3), (0, 2), (0, 1), (0, 0))
        super().__init__(cross0, cross1, startEdges = [(0, 0), (1, 2)])

class Borromean(Knot):
    def __init__(self):
        cross0 = Cross((1, 1), (2, 0), (5, 1), (4, 2))
        cross1 = Cross((2, 1), (0, 0), (4, 1), (3, 0))
        cross2 = Cross((0, 1), (1, 0), (3, 3), (5, 2))
        cross3 = Cross((1, 3), (4, 0), (5, 3), (2, 2))
        cross4 = Cross((3, 1), (1, 2), (0, 3), (5, 0))
        cross5 = Cross((4, 3), (0, 2), (2, 3), (3, 2))
        super().__init__(cross0, cross1, cross2, cross3, cross4, cross5, startEdges = [(0, 1), (2, 3), (5, 1)])

class ThistlethwaiteUnknot(Knot):
    def __init__(self):
        cross0 = Cross((3, 0), (9, 0), (1, 1), (1, 0))
        cross1 = Cross((0, 3), (0, 2), (7, 1), (2, 1))
        cross2 = Cross((3, 1), (1, 3), (5, 1), (4, 0))
        cross3 = Cross((0, 0), (2, 0), (4, 3), (13, 0))
        cross4 = Cross((2, 3), (5, 0), (6, 0), (3, 2))
        cross5 = Cross((4, 1), (2, 2), (7, 0), (6, 1))
        cross6 = Cross((4, 2), (5, 3), (8, 0), (12, 3))
        cross7 = Cross((5, 2), (1, 2), (9, 3), (8, 1))
        cross8 = Cross((6, 2), (7, 3), (10, 3), (12, 0))
        cross9 = Cross((0, 1), (11, 1), (10, 0), (7, 2))
        cross10 = Cross((9, 2), (11, 0), (12, 1), (8, 2))
        cross11 = Cross((10, 1), (9, 1), (14, 3), (14, 2))
        cross12 = Cross((8, 3), (10, 2), (13, 1), (6, 3))
        cross13 = Cross((3, 3), (12, 2), (14, 1), (14, 0))
        cross14 = Cross((13, 3), (13, 2), (11, 3), (11, 2))       
        super().__init__(cross0, cross1, cross2, cross3, cross4, cross5, cross6, cross7, cross8, cross9, cross10, cross11, cross12, cross13, cross14)