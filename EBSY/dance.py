from arduino import *
from arduino_alvik import ArduinoAlvik

alvik = ArduinoAlvik()

SLOW = 80
FAST = 120

OFF     = (0, 0, 0)
RED     = (1, 0, 0)
GREEN   = (0, 1, 0)
BLUE    = (0, 0, 1)
YELLOW  = (1, 1, 0)
CYAN    = (0, 1, 1)
MAGENTA = (1, 0, 1)
WHITE   = (1, 1, 1)

def set_leds(color):
    r, g, b = color
    alvik.left_led.set_color(r, g, b)
    alvik.right_led.set_color(r, g, b)

def blink(color, times=3, on=150, off=120):
    for _ in range(times):
        set_leds(color); delay(on)
        set_leds(OFF);  delay(off)

def move(l_rpm, r_rpm, ms, color=WHITE):
    set_leds(color)
    alvik.set_wheels_speed(l_rpm, r_rpm)
    delay(ms)

def setup():
    alvik.begin()
    delay(300)
    blink(WHITE, 2)
    
def loop():
    move(SLOW, SLOW, 1200, GREEN)
    move(0,   FAST,  800,  YELLOW)
    move(FAST, 0,    800,  BLUE)
    move(-SLOW, -SLOW, 1000, MAGENTA)

    move(-FAST, FAST, 800,  RED)
    move(-SLOW, -SLOW, 1000, MAGENTA)
    move(FAST, -FAST, 800,  CYAN)
    move(-SLOW, -SLOW, 900, MAGENTA)
    move(-FAST, FAST, 200,  RED)

def cleanup():
    alvik.stop()
    set_leds(OFF)

start(setup, loop, cleanup)
