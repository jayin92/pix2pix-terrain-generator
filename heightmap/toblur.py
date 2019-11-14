import cv2

im = cv2.imread("Heightmap/Unity_test.png")
print(im)
cv2.imwrite("gray_blur.png", cv2.medianBlur(im[:,:,2], 15))
