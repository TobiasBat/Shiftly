from machine import Pin
import time
import network

class ESP32Utilities: 
    onBoardLedPinNumber: 2

    @staticmethod
    def turnOnOnBoardLED(): 
        print('Turning on Onboard LED')
        pin = Pin(2, Pin.OUT)
        pin.on()

    @staticmethod
    def blinkOnBoardLed(): 
        pin = Pin(2, Pin.OUT)
        for i in range(0,10):
            pin.on()
            time.sleep_ms(300)
            pin.off()
            time.sleep_ms(300)

    @staticmethod
    def turnOffOnBoardLed():
        pin = Pin(2, Pin.OUT)
        pin.off()
    
    @staticmethod
    def turnOffAndOn(ms): 
        pin = Pin(2, Pin.OUT)
        pin.off()
        time.sleep_ms(ms)
        pin.on()
    
    @staticmethod
    def turnOffAndBlinkAndOn(): 
        pin = Pin(2, Pin.OUT)
        for i in range(6):
            pin.off()
            time.sleep_ms(100)
            pin.on()
            time.sleep_ms(100)


    @staticmethod
    def connectToNetwork(network_name, network_password): 
        wlan = network.WLAN(network.STA_IF)
        wlan.active(True)
        if not wlan.isconnected():
            print('connecting to network...')
            wlan.connect(network_name, network_password)

            start = time.ticks_ms()
            while not wlan.isconnected() and time.ticks_diff(time.ticks_ms(), start) < 5000:
                pass

            if wlan.isconnected():
                ESP32Utilities.turnOnOnBoardLED()
                print('network config:', wlan.ifconfig())
                return True
            else:
                ESP32Utilities.blinkOnBoardLed()
                print('Failed to Connect to Network', network_name)
                return False

