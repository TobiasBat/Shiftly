import time
import math
from machine import Pin

class StepperController: 
    # PINS
    STEP_PIN = Pin(16, Pin.OUT)
    DIR_PIN = Pin(17, Pin.OUT)
    MS3_PIN = Pin(0, Pin.OUT)
    MS2_PIN = Pin(4, Pin.OUT)
    MS1_PIN = Pin(16, Pin.OUT)

    numberOfStepsPerRevolution = 200 * 16 
    micro_steps = 16
    ratio = 1
    id = 0
    currentStep = 0
    turning = False
    direction = True # True if clockwise
    

    def __init__(self, id ,directionPinNumber, stepPinNumber, ms1_pinNumber = 16, ms2_pinNumber = 4, ms3_pinNumber = 0, micro_steps = 16, ratio = 1):
        print('Initiating new Stepper', id, directionPinNumber, stepPinNumber)
        print('MS PIN NUMBERS: ', ms1_pinNumber, ms2_pinNumber, ms3_pinNumber, micro_steps, ratio)

        self.id = id
        #Init STEP and DIR pin
        self.STEP_PIN = Pin(stepPinNumber, Pin.OUT)
        self.STEP_PIN.off()
        self.DIR_PIN = Pin(directionPinNumber, Pin.OUT)
        self.setDirectionClockwise()

        # Set Speed Control Pins
        self.MS1_PIN = Pin(ms1_pinNumber, Pin.OUT)
        self.MS2_PIN = Pin(ms2_pinNumber, Pin.OUT)
        self.MS3_PIN = Pin(ms3_pinNumber, Pin.OUT)

        self.ratio = ratio
        self.micro_steps = micro_steps
        self.updateSpeed()


    def updateSpeed(self): 
        self.numberOfStepsPerRevolution = 200 * self.micro_steps * self.ratio

        if self.micro_steps == 16:
            self.MS1_PIN.on()
            self.MS2_PIN.on()
            self.MS3_PIN.on()
            
        elif self.micro_steps == 8: 
            self.MS1_PIN.on()
            self.MS2_PIN.on()
            self.MS3_PIN.off()

        elif self.micro_steps == 4: 
            self.MS1_PIN.off()
            self.MS2_PIN.on()
            self.MS3_PIN.off()
        
        elif self.micro_steps == 2: 
            self.MS1_PIN.on()
            self.MS2_PIN.off()
            self.MS3_PIN.off()
        
        elif self.micro_steps == 1: 
            self.MS1_PIN.off()
            self.MS2_PIN.off()
            self.MS3_PIN.off()

        else: 
            self.MS1_PIN.on()
            self.MS2_PIN.on()
            self.MS3_PIN.on()


    def setDegree(self, degree):
        self.currentStep = self.numberOfStepsPerRevolution / 360.0 * degree
        print('set stepper to ', self.currentStep, ' or ', degree , ' degree.')
        self.printStateOfStepper()

    def setDirectionClockwise(self): 
        self.DIR_PIN.on()
        self.direction = True

    def setDirectionCounterClockWise(self): 
        self.DIR_PIN.off()
        self.direction = False

    def turn360degree(self, breakMilliSeconds): 
        print('Turning ', self.id)
        
        update = 1
        if not self.direction:
            update = -1
        
        for i in range(0, self.numberOfStepsPerRevolution):
            self.STEP_PIN.on()
            time.sleep_ms(breakMilliSeconds)
            self.STEP_PIN.off()
            self.currentStep = (self.currentStep + update) % self.numberOfStepsPerRevolution
            time.sleep_ms(breakMilliSeconds)
        
        print('Motor', self.id, ':', self.currentStep )
    
    def turnDegree(self, degree, breakMilliSeconds): 
        numberOfSteps = (degree / 360.0) * self.numberOfStepsPerRevolution
        self.turnSteeps(numberOfSteps, breakMilliSeconds)

    def turnDegreesWithoutCounting(self, degree, micro_steps, breakMilliSeconds): 
        if self.micro_steps != micro_steps: 
            self.updateMicroSteps(micro_steps)
        numberOfSteps = (degree / 360.0) * self.numberOfStepsPerRevolution
        self.turnStepsWithoutCounting(numberOfSteps, breakMilliSeconds)

    def turnSteeps(self, steps, breakMilliSeconds):
        update = 1
        if (steps < 0):
            self.setDirectionCounterClockWise()
            update = -1
        else: 
            self.setDirectionClockwise()
        
        for i in range(0, math.fabs(steps)):
            self.STEP_PIN.on()
            time.sleep_ms(breakMilliSeconds)
            self.STEP_PIN.off()
            self.currentStep = (self.currentStep + update) % self.numberOfStepsPerRevolution
            time.sleep_ms(breakMilliSeconds)
        self.printStateOfStepper()

    def turnStepsWithoutCounting(self, steps, breakMilliSeconds):
        update = 1
        if (steps < 0):
            self.setDirectionCounterClockWise()
            update = -1
        else: 
            self.setDirectionClockwise()
        
        for i in range(0, math.fabs(steps)):
            self.STEP_PIN.on()
            time.sleep_ms(breakMilliSeconds)
            self.STEP_PIN.off()
            # self.currentStep = (self.currentStep + update) % self.numberOfStepsPerRevolution
            time.sleep_ms(breakMilliSeconds)
        self.printStateOfStepper()

    def turnToDegree(self, degree, micro_steps, breakMilliSeconds): 
        if self.micro_steps != micro_steps: 
            self.updateMicroSteps(micro_steps)
        
        directionStep = degree / 360 * self.numberOfStepsPerRevolution
        deltaSteps = 0
        if (self.currentStep == directionStep): 
            print('Stepper', self.id, 'No motion required')
            return
        if (self.currentStep > directionStep): 
            # means negativ change 
            deltaSteps = (self.currentStep - directionStep) * (-1.0)
        else: 
            deltaSteps = (directionStep - self.currentStep)

        print('Delta Steps', deltaSteps)
        self.turnSteeps(deltaSteps, breakMilliSeconds)

    def updateMicroSteps(self, micro_steps): 
        if micro_steps == 16 or micro_steps == 8 or micro_steps == 4 or micro_steps == 2 or micro_steps == 1: 
            self.micro_steps = micro_steps
            self.updateSpeed()
        else: 
            print('No valid Speed', micro_steps)

    def getTurning(self):
        return self.turning
    
    def printStateOfStepper(self):
        print('Motor', 
            self.id, '-', 
            'steps: ', self.currentStep, 
            ', norm: ' , self.currentStep / self.numberOfStepsPerRevolution, 
            ', degree:',  self.currentStep / self.numberOfStepsPerRevolution * 360.0, '\n')