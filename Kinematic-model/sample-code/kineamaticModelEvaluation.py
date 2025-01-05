import math
import numpy as np
import cairo 
from scipy.optimize import fsolve
import matplotlib.pyplot as plt

# Parameters of the module 
c = 67.5 # hinge
r = 28 # radiusWheel
w_max = 149 # corresponds to length of paper.
d = w_max - c - r
d1 = d
d2 = 0

def prRed(skk): print("\033[91;1m {}\033[00m" .format(skk))

def computeTouchPointsFromTheta(theta): 
    w = computeWidthFromTheta(theta)
    xi = computeXi(w)
    h = computeSagiate(xi, w)
    q = computeQi(
        np.array([0, 1.0]),
        h,
        np.array([0, 0]),
        np.array([w, 0])
    )
    print('width', w, ' xi', xi, 'h', h, 'q', q)
    createShiftlyProfile(w, q)

    return w, q

# Eqatuin 1
def computeWidthFromTheta(theta):
    beta = math.asin(
            (r * math.sin(math.pi - theta)) / c
    )
    aSquared = r**2 + c**2 - 2 * r * c * math.cos(theta - beta)
    return d1 + d2  + math.sqrt(aSquared)

# Equation 5
def computeQi(n, h, p, v):
    return n * h + p + v/2.0

# Equation 4
def computeSagiate(xi, w):
    s = w_max / xi
    if 0 < xi < math.pi: 
        return s - math.sqrt(s ** 2 - (w ** 2 / 4) )
    if math.pi < xi < 2 * math.pi: 
        print('xi is larger than pi', xi)
        return s + math.sqrt(s ** 2 - (w ** 2 / 4) )
    return 0

# Equation 3
def computeXi(w):
    def func2(xi_): 
        return 2.0 * np.sin(xi_/2.0) / xi_ - w / w_max
    xi = fsolve(func2, 0.001)[0]
    return xi


# Eq 6
# s is the radius of the circle approximating a curvature 1/s
def computeWidthFromRadius(s_i): 
    w_i = 2 * s_i * math.sin(w_max / (2 * s_i))
    print('width' ,'w_i', w_i)
    return w_i

def computeThetaFromWidth(w_i):
    theta_i = math.pi - math.acos(
        (
            (w_i - d1 - d2)**2 + r**2 - c**2
        ) / 
        (
            2 * (w_i - d1 - d2 ) * r
        )
    )
    print ('gearRotation','theta_i', theta_i, theta_i * 180 / math.pi)
    return theta_i
    

# Eq 7
# s is the radius of the circle approximating curvature 1/s
def computeThetaFromRadius(s_i): 
    w_i = computeWidthFromRadius(s_i)
    theta_i = computeThetaFromWidth(w_i)
    return theta_i

# Computes the evaluation sequence for five different motor rotations
def computeEvaluationSequences():
    sequence = [0.00,0.79,1.57,2.36,3.14]
    print('\n')
    print('θ', '  ', 'predicted w', 'predicted h' )
    print('----------------')
    for theta in sequence:
        w = computeWidthFromTheta(theta)
        xi = computeXi(w)
        h = computeSagiate(xi, w)
        print(round(theta,2), '  ', round(w), round(h))

def computeMinS():
    w_min = d1 + d2 + c - r + 0.0000000001
    print('w_min', w_min)
    xi = computeXi(w_min)
    h = computeSagiate(xi, w_min)
    minS = h / 2 + w_min**2 / (8 * h)
    print('minRadius', 's_min', minS)
    return minS

def computeSFor3():
    u = 3 * w_max
    s_i = u / (2 * math.pi)
    w_i = computeWidthFromRadius(s_i)
    print('radiusThreeOrigami', 's_i', s_i)
    print('width_i', w_i)
    return s_i

def computeMinCircle(): 
    minS = computeMinS()
    uCircle = minS * 2 * math.pi
    percentOfCircle = w_max / uCircle
    print()
    print('u Max Circle', uCircle)
    print('percent covered by Origami', percentOfCircle)
    return percentOfCircle

# drawing Classes 
def createShiftlyProfile(w, q):
    plt.close('all')    
    plt.plot(0, 0, 'bo')
    plt.plot(w, 0, 'bo')
    plt.plot(w_max, 0, 'co')
    plt.plot(q[0], q[1], 'ro')

    plt.xlim(0
             -0.1, w_max)
    plt.ylim(-0.1, w_max)
    plt.show()

def help():
    print('\nCommands:')
    print('eval', '', 'to run a motor rotation sequence for evaluation')
    print('t', '<number>', 'to compute the modules touch point position from motor largest gear angle')
    print('s', '<number>', 'to compute motor largest gear angle from radius of circle')
    print('w', '<number>', 'to compute module width from largest gear angle')
    print('minS', '', 'minimum radius of shiftly module')
    print('minCircle', '', 'share of a full circle covered by the minium radius of shiftly module')
    print('sThree', '', 'computes curvature for shiftly forming 360deg cylinder')
    print('exit')
    print('\n')


def main():
    exit = False

    while (not exit): 
        argument = input()
        if argument == 'exit':
            exit = True
        
        argumentSplit = argument.split(' ')
        variable = argumentSplit[0]
        value = 0
        if (len(argumentSplit) > 1):
            value = float(argumentSplit[1])
        if (variable == 'help'):
            help()
        elif (variable == 'eval'):
            computeEvaluationSequences()
        elif variable == 't':
            computeTouchPointsFromTheta(value)
        elif variable == 's': 
            computeThetaFromRadius(value)
        elif variable == 'w': 
            computeThetaFromWidth(value)
        elif variable == 'minS': 
            computeMinS()
        elif variable == 'sThree':
            computeSFor3()
        elif variable == 'minCircle': 
            computeMinCircle()
        elif variable == 'exit': 
            exit = True
        else:
            print('Comment not implemented. Run help to see available commands')

if __name__ == "__main__":
    main()
