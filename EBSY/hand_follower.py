from arduino_alvik import ArduinoAlvik
from time import sleep_ms
import sys

alvik = ArduinoAlvik()
alvik.begin()

reference = 10.0

try:
    while True:
        L, CL, C, CR, R = alvik.get_distance()
        print(f'L:{L:.1f} CL:{CL:.1f} C:{C:.1f} CR:{CR:.1f} R:{R:.1f}')

        error_forward = C - reference
        error_turn = (CR - CL)

        forward_speed = error_forward * 8
        turn_speed    = error_turn * 5

        left_speed  = forward_speed - turn_speed
        right_speed = forward_speed + turn_speed

        alvik.set_wheels_speed(left_speed, right_speed)

        sleep_ms(100)

except KeyboardInterrupt:
    print('over')
    alvik.stop()
    sys.exit()

