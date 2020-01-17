import os
import cv2
import numpy

for item in os.listdir():
    im = cv2.imread(item)
    try:
        print(item)
        cv2.imwrite(os.path.join("blur", item), cv2.medianBlur(im[:,:,2],31))
    except:
        print("Not an image file.")