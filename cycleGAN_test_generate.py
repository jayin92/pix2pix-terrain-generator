import numpy as np
import cv2

width = 100

# Train Data
for i in range(1, 201):
    a = np.random.randint(2, size=width ** 2)
    a_ = np.array([255 if i == 1 else 0 for i in a])
    b = np.array([0 if i == 1 else 255 for i in a])
    a_.resize(width, width)
    b.resize(width, width)
    cv2.imwrite("./cycleGAN_test/trainA/{}.png".format(i), a_)
    cv2.imwrite("./cycleGAN_test/trainB/{}.png".format(i), b)

# Test Data
for i in range(1, 101):
    a = np.random.randint(2, size=width ** 2)
    a_ = np.array([255 if i == 1 else 0 for i in a])
    b = np.array([0 if i == 1 else 255 for i in a])
    a_.resize(width, width)
    b.resize(width, width)
    cv2.imwrite("./cycleGAN_test/testA/{}.png".format(i), a_)
    cv2.imwrite("./cycleGAN_test/testB/{}.png".format(i), b)