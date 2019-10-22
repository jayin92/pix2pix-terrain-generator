import cv2
import os
import numpy

for item in os.listdir():
    if item == "trainA" or item == "trainB" or item == "invert.py":
        continue

    print(item)
    im = cv2.imread(item)
    im = cv2.bitwise_not(im)
    cv2.imwrite(os.path.join("trainB", item), im)