from Knots import *

class Hopf(Knot):

    def __init__(self):
        cross0 = Cross((1, 3), (1, 2), (1, 1), (1, 0))
        cross1 = Cross((0, 3), (0, 2), (0, 1), (0, 0))
        super().__init__(cross0, cross1, startEdges = [(0, 0), (0, 1)])
        
# class Hopf_minus(Knot):

#     def __init__(self):
#         cross0 = Cross((1, 3), (1, 2), (1, 1), (1, 0))
#         cross1 = Cross((0, 3), (0, 2), (0, 1), (0, 0))
#         super().__init__(cross0, cross1, startEdges = [(0, 0), (1, 2)])


calculator = Calculator()
hopf_plus = Hopf()
calculator.BracketPolynomial(hopf_plus)
#calculator.Jones_Polynomial(three_1)