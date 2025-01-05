import time
import utime
import uasyncio
import _thread
import socket
import sys
from machine import Pin
from steppercontroller import StepperController
from esp32_utilities import ESP32Utilities


# Turns a Stepper to a certain degree
def turnStepperToDegree(stepper, degree, micro_steps,breakMilliSeconds, counting):
    while stepper.getTurning():
        print('waiting line')
        time.sleep_ms(100)
    
    stepper.turning = True
    print('Startin a new Stepper Motion', stepper.id, 'to degree', degree, ' with thread id', _thread.get_ident(), 'counting: ', counting)
    if counting: 
        stepper.turnToDegree(degree, micro_steps, breakMilliSeconds)
    else: 
        stepper.turnDegreesWithoutCounting(degree, micro_steps, breakMilliSeconds)
    stepper.turning = False
    _thread.exit()

# Serial Communication
# Gets the serial command lines 
def handleSerialCommunication():
    print('Starting to Handle Serial Communication')

    while True:
        data = sys.stdin.readline()
        parseASerialCommand(data)

# Parses the first instruction of the serial command line
def parseASerialCommand(serialLine):
    try: 
        serialLine = serialLine.strip("\n")
        arguments = serialLine.split(' ')
        firstArgument = arguments[0]
        if firstArgument == 'step':
            handleStepSerialCommand(arguments)
        elif firstArgument == 'set':
            handleSetSerialCommand(arguments)
        else: 
            handleDegreeSerialCommand(arguments)
    except:
        print('Could not parse instruction line', serialLine)

# Handles a serial command line of the form: 
#   set stepper=3 degree=0
def handleSetSerialCommand(arguments):
    try:
        stepper = ''
        degree = '' 
        for argument in arguments:
            parts = argument.split("=")
            if len(parts) > 1: 
                if parts[0] == 'stepper':
                    name = parts[1]
                    if name == '1': 
                        stepper = stepper_1
                    elif name == '2':
                        stepper = stepper_2
                    elif name == '3':
                        stepper = stepper_3
                elif parts[0] == 'degree':
                    degree = int(parts[1])
        if stepper !=  '' and degree != '':
            stepper.setDegree(degree)
        else: 
            print('stepper:', stepper, ' degree: ', degree)
    except:
        print('Could not completely execute set command')
    
# Handles a serial command line of the form:
#   step stepper=3 degree=100 microsteps=8
def handleStepSerialCommand(arguments):
    handleDegreeSerialCommand(arguments, counting = False)

# Handles a serial command of the form: 
#   stepper=3 degree=360 microsteps=8
def handleDegreeSerialCommand(arguments, counting = True):
    stepper = ''
    degree = ''
    microSteps = ''
    for argument in arguments: 
        parts = argument.split('=')
        if len(parts) > 1: 
            if parts[0] == 'stepper': 
                # Parse stepper name 
                name = parts[1]
                if name == '1': 
                    stepper = stepper_1
                elif name == '2':
                    stepper = stepper_2
                elif name == '3':
                    stepper = stepper_3
            elif parts[0] == 'degree': 
                degree = int(parts[1])
            elif parts[0] == 'microsteps':
                microSteps = int(parts[1])
    if stepper !=  '' and degree != '' and microSteps != '': 
        if counting:
            _thread.start_new_thread(turnStepperToDegree, (stepper, degree, microSteps, 1, True))
        else: 
            _thread.start_new_thread(turnStepperToDegree, (stepper, degree, microSteps, 1, False))

def parseUrlArguments(arguments): 
    a = 0
    url_parameters = arguments.split('&')
    stepper_name = ''
    stepper_degree = ''
    stepper_microsteps = ''

    for a in url_parameters:
        if 'stepper' in a:
            stepper_name = a.split('=')[1]
        elif 'degree' in a:
            stepper_degree = int(a.split('=')[1])
        elif 'microsteps' in a: 
            stepper_microsteps = int(a.split("=")[1])
    return stepper_name, stepper_degree, stepper_microsteps

def parseStepperNameToStepper(stepper_name): 
    stepper = stepper_1
    if '2' in stepper_name: 
        stepper = stepper_2
    elif '3' in stepper_name: 
        stepper = stepper_3
    return stepper
    
## Handle Server Communication
def handlingServerCommunication(): 
    print('Startin the Server\n')
    socket_ = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    socket_.bind(('', 80))
    socket_.listen(5)

    print('Server is Listening')

    while True:
        conn, addr = socket_.accept()
        try: 
            request = conn.recv(1024)
            request = request.decode().strip().split('\n', 1)
            firstLine = request[0].split(' ')
            request_type = firstLine[0]

            print(request_type, str(addr), firstLine[1])
            
            if 'PUT' in request_type:
                arguments = firstLine[1].split('?', 1)[1]
                commandPart = firstLine[1].split('?', 1)[0]
                
                command = ''
                if len(commandPart) > 1: 
                    command = commandPart.split('/', 1)[1]
                
                stepper_name, stepper_degree, stepper_microsteps = parseUrlArguments(arguments)

                # If request contains name and degree and speed
                if command == '' and stepper_name != '' and stepper_degree != '' and stepper_microsteps != '':
                    stepper = parseStepperNameToStepper(stepper_name)
                    _thread.start_new_thread(turnStepperToDegree, (stepper, stepper_degree, stepper_microsteps, 1, True))
                    conn.send('HTTP/1.1 200 OK\n')
                elif command == 'step' and stepper_name != '' and stepper_degree != '' and stepper_microsteps != '': 
                    stepper = parseStepperNameToStepper(stepper_name)
                    _thread.start_new_thread(turnStepperToDegree, (stepper, stepper_degree, stepper_microsteps, 1, False))
                    conn.send('HTTP/1.1 200 OK\n')
                elif command == 'set' and stepper_name != '' and stepper_degree != '':
                    stepper = parseStepperNameToStepper(stepper_name)
                    stepper.setDegree(stepper_degree)
                    conn.send('HTTP/1.1 200 OK\n')
                else:
                    print(' Something went wrong and argument Missing', stepper_name, stepper_degree, stepper_microsteps)
                    conn.send('HTTP/1.1 400 Bad Request\n')

            else: 
                print(' Request not supported')
                conn.send('HTTP/1.1 400 Bad Request\n')
                conn.send('Request not supported')

        except:
            print('Something went wrong with processing request')
            conn.send('HTTP/1.1 400 Bad Request\n')
        
        conn.send('Connection: close\n\n')
        print('closing connection\n')
        conn.close()



##################
#
# Main Program
#
##################

# PARAMETERS
# Stepper 1 Pin Numbers
stepper_1_directionPinNumber = 15 # on the old board is 2
stepper_1_stepPinNumber = 2 # on the old board is 15
stepper_1_ms1_pinNumber = 16
stepper_1_ms2_pinNumber = 4
stepper_1_ms3_pinNumber = 0

# Stepper 2 Pin Number
stepper_2_directionPinNumber = 17
stepper_2_stepPinNumber = 5
stepper_2_ms1_pinNumber = 21
stepper_2_ms2_pinNumber = 19
stepper_2_ms3_pinNumber = 18

# Stepper 3 Pin Number
stepper_3_directionPinNumber = 22
stepper_3_stepPinNumber = 23
stepper_3_ms1_pinNumber = 32
stepper_3_ms2_pinNumber = 33
stepper_3_ms3_pinNumber = 25

useWirelessCommunication = True
network_name =  'some network name'
network_password = 'some network password'


print('\nHello Welcome to Shiftly\n')

stepper_1 = StepperController(1, stepper_1_directionPinNumber, stepper_1_stepPinNumber, 
                                    ms1_pinNumber=stepper_1_ms1_pinNumber,
                                    ms2_pinNumber=stepper_1_ms2_pinNumber,
                                    ms3_pinNumber=stepper_1_ms3_pinNumber,
                                    micro_steps=8,
                                    ratio=(32/12)
                                    )

stepper_2 = StepperController(2, stepper_2_directionPinNumber, stepper_2_stepPinNumber,
                                    ms1_pinNumber=stepper_2_ms1_pinNumber,
                                    ms2_pinNumber=stepper_2_ms2_pinNumber,
                                    ms3_pinNumber=stepper_2_ms3_pinNumber,
                                    micro_steps=8,
                                    ratio=(32/12)
                                    )
stepper_3 = StepperController(3, stepper_3_directionPinNumber, stepper_3_stepPinNumber,
                                    ms1_pinNumber=stepper_3_ms1_pinNumber,
                                    ms2_pinNumber=stepper_3_ms2_pinNumber,
                                    ms3_pinNumber=stepper_3_ms3_pinNumber,
                                    micro_steps=8,
                                    ratio=(32/12)
                            )


if useWirelessCommunication: 
    connectedToNetwork = ESP32Utilities().connectToNetwork(network_name, network_password)

    time.sleep(1)

    if not connectedToNetwork:
        print('Not connected to Wifi')
        handleSerialCommunication()
    else:
        handlingServerCommunication()
else: 
    handleSerialCommunication()