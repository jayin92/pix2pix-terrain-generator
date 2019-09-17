import os
import cv2
import numpy

for item in os.listdir():
    im = cv2.imread(item)
    if item == "blur" or item == "gray" or item == "togray.py":
        continue;
    print(item)
    cv2.imwrite(os.path.join("gray", item), im[:,:,2])