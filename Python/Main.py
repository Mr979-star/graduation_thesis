from Knots import *

class Hopf_plus(Knot):

    def __init__(self):
        cross0 = Cross((1, 3), (1, 2), (1, 1), (1, 0))
        cross1 = Cross((0, 3), (0, 2), (0, 1), (0, 0))
        super().__init__(cross0, cross1, startEdges = [(0, 0), (0, 1)])
        
class Hopf_minus(Knot):

    def __init__(self):
        cross0 = Cross((1, 3), (1, 2), (1, 1), (1, 0))
        cross1 = Cross((0, 3), (0, 2), (0, 1), (0, 0))
        super().__init__(cross0, cross1, startEdges = [(0, 0), (1, 2)])

print(Hopf_plus().writhe)
print(Hopf_minus().writhe)

# calculator = Calculator()
# calculator.Jones_Polynomial(two_1_2_plus)