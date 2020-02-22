from PIL import Image
import numpy as np
def rgb2array(img):
    rgb=img.split()
    return np.array(rgb[0])*65536+np.array(rgb[1])*256+np.array(rgb[2])

def interpolation(t):
    return 3*t*t-2*t*t*t
    

size=128
overlay=64
min_value=[]
max_value=[]
num_imgs=(0,0)
def split(a):
    global min_value,max_value,num_imgs
    stride=size-overlay
    num_imgs=(int((a.size[0]-overlay)/stride),int((a.size[1]-overlay)/stride))
    b=[
    rgb2array(a.crop((i*stride,j*stride,i*stride+size,j*stride+size)))
   for j in range(num_imgs[1])
  for i in range(num_imgs[0])
]
    min_value=[np.min(b[i])for i in range(num_imgs[0]*num_imgs[1])]
    max_value=[np.max(b[i])for i in range(num_imgs[0]*num_imgs[1])]
    return[Image.fromarray(((b[i]-min_value[i])*256.0/(max_value[i]-min_value[i])-0.5).astype(np.uint8)).convert('RGB')for i in range(num_imgs[0]*num_imgs[1])]
def merge(a):
    stride=size-overlay
    a=[a[i]*((max_value[i]-min_value[i])/256.0)+min_value[i] for i in range(num_imgs[0]*num_imgs[1])]
    result=np.zeros((num_imgs[0]*stride+overlay,num_imgs[1]*stride+overlay,3))
    for j in range(num_imgs[1]):
        for i in range(num_imgs[0]):
            x0=i*stride
            y0=j*stride
            for x in range(size):
                for y in range(size):
                    factor=1.0
                    if x<overlay and i>0:
                        factor*=interpolation(x/overlay)
                    if x>stride and i<num_imgs[0]-1:
                        factor*=interpolation((size-x)/overlay)
                    if y<overlay and j>0:
                        factor*=interpolation(y/overlay)
                    if y>stride and j<num_imgs[1]-1:
                        factor*=interpolation((size-y)/overlay)
                    value=a[i+j*num_imgs[0]][y][x][0]*factor
                    result[y0+y][x0+x][0]+=value/65536
                    result[y0+y][x0+x][1]+=(value%65536)/256
                    result[y0+y][x0+x][2]+=(value%256)
    return result.astype(np.uint8)


