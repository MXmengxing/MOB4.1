from time import sleep_ms, ticks_ms, ticks_diff
from math import atan2, sqrt, degrees
from arduino_alvik import ArduinoAlvik

alvik = ArduinoAlvik()
alvik.begin()

BASE = 70
TURN = 60

PITCH_UP = 8.0
PITCH_FLAT = 3.0
UP_COUNT = 4
FLAT_COUNT = 6

SPIN_MS = 3200

def read_pitch_deg():
    if hasattr(alvik, "get_euler"):
        r, p, y = alvik.get_euler()
        return float(p)
    if hasattr(alvik, "get_imu_euler"):
        r, p, y = alvik.get_imu_euler()
        return float(p)
    if hasattr(alvik, "get_accel"):
        ax, ay, az = alvik.get_accel()
        return float(degrees(atan2(-ax, sqrt(ay*ay + az*az))))
    return 0.0

def spin_360():
    if hasattr(alvik, "get_gyro"):
        yaw = 0.0
        last = ticks_ms()
        alvik.set_wheels_speed(TURN, -TURN)
        while abs(yaw) < 360.0:
            sleep_ms(15)
            gx, gy, gz = alvik.get_gyro()
            now = ticks_ms()
            dt = ticks_diff(now, last) / 1000.0
            last = now
            yaw += float(gz) * dt
        alvik.set_wheels_speed(0, 0)
    else:
        alvik.set_wheels_speed(TURN, -TURN)
        sleep_ms(SPIN_MS)
        alvik.set_wheels_speed(0, 0)

mode = "drive"
up_cnt = 0
flat_cnt = 0

try:
    while True:
        pitch = read_pitch_deg()

        if mode == "drive":
            alvik.set_wheels_speed(BASE, BASE)
            up_cnt = up_cnt + 1 if pitch >= PITCH_UP else 0
            if up_cnt >= UP_COUNT:
                mode = "wait_flat"
                flat_cnt = 0

        elif mode == "wait_flat":
            alvik.set_wheels_speed(BASE, BASE)
            flat_cnt = flat_cnt + 1 if abs(pitch) <= PITCH_FLAT else 0
            if flat_cnt >= FLAT_COUNT:
                mode = "spin"

        elif mode == "spin":
            spin_360()
            mode = "drive"

        sleep_ms(30)

except KeyboardInterrupt:
    alvik.set_wheels_speed(0, 0)
    alvik.stop()
