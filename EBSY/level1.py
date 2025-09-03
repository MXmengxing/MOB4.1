from arduino_alvik import ArduinoAlvik
from time import sleep_ms
import sys

alvik = ArduinoAlvik()
alvik.begin()

FORWARD_BASE = 60
MAX_SPEED    = 100
STOP_DIST    = 12.0
WARN_DIST    = 22.0
BACK_TIME_MS = 300
TURN_TIME_MS = 350
SMOOTH_ALPHA = 0.4

def clamp(v, lo, hi):
    return lo if v < lo else hi if v > hi else v

prev_left = 0.0
prev_right = 0.0
def smooth_set_speed(l, r):
    global prev_left, prev_right
    l = clamp(l, -MAX_SPEED, MAX_SPEED)
    r = clamp(r, -MAX_SPEED, MAX_SPEED)
    l_out = prev_left  + SMOOTH_ALPHA * (l - prev_left)
    r_out = prev_right + SMOOTH_ALPHA * (r - prev_right)
    alvik.set_wheels_speed(l_out, r_out)
    prev_left, prev_right = l_out, r_out

def stop(ms=0):
    smooth_set_speed(0, 0)
    if ms > 0:
        sleep_ms(ms)

def forward(speed=FORWARD_BASE):
    smooth_set_speed(speed, speed)

def back(speed=FORWARD_BASE):
    smooth_set_speed(-speed, -speed)

def turn_left(speed=FORWARD_BASE):
    smooth_set_speed(-speed * 0.6, speed)

def turn_right(speed=FORWARD_BASE):
    smooth_set_speed(speed, -speed * 0.6)

try:
    while True:
        L, CL, C, CR, R = alvik.get_distance()
        print(f'L:{L:.1f} CL:{CL:.1f} C:{C:.1f} CR:{CR:.1f} R:{R:.1f}')

        near_front = (C <= STOP_DIST) or (CL <= STOP_DIST) or (CR <= STOP_DIST)
        if near_front:
            back(FORWARD_BASE)
            sleep_ms(BACK_TIME_MS)
            stop(80)
            if CL > CR:
                turn_left(FORWARD_BASE)
            else:
                turn_right(FORWARD_BASE)
            sleep_ms(TURN_TIME_MS)
            stop(40)
            sleep_ms(60)
            continue

        warn_front = (C <= WARN_DIST) or (CL <= WARN_DIST) or (CR <= WARN_DIST)
        if warn_front:
            if CL > CR:
                left = FORWARD_BASE * 0.6
                right = FORWARD_BASE * 1.1
                smooth_set_speed(left, right)
            else:
                left = FORWARD_BASE * 1.1
                right = FORWARD_BASE * 0.6
                smooth_set_speed(left, right)
            sleep_ms(120)
            continue

        corridor_bias = clamp((CR - CL) * 1.0, -20, 20)
        left_speed  = FORWARD_BASE - corridor_bias
        right_speed = FORWARD_BASE + corridor_bias
        smooth_set_speed(left_speed, right_speed)

        sleep_ms(50)

except KeyboardInterrupt:
    print('over')
    stop(0)
    alvik.stop()
    sys.exit()
