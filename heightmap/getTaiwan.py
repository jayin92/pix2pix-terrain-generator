import requests
import json
import numpy as np
import matplotlib.pyplot as plt
import cv2
from PIL import Image
from tqdm import tqdm
url = "https://maps.googleapis.com/maps/api/elevation/json"
p1=np.array([24.663513, 121.182540])
p2=np.array([24.534208, 121.655832])
p3=np.array([23.188591, 120.532673])
p4=np.array([23.012812, 121.095572])


data=[]
locations=''
lat = 23.8
lon = 121.048414
size1 = 2000
size2 = 4000
n = 0
ppbar= tqdm(total=size1*size2)
for i in range(-size1//2,size1-size1//2):
    for j in range(-size2//2,size2-size2//2):
        ppbar.update(1)
        locations+="{:.3f}".format(lat-i*0.001)+","+"{:.3f}".format(lon+j*0.001)
        ppbar.set_description_str("{},{}".format("{:.3f}".format(lat-i*0.001),"{:.3f}".format(lon+j*0.001)))
        n += 1
        if n == 510:
            param = {"locations":locations,"key": "AIzaSyCgFZ_bPFLzrCOQQxRE9wwxqGDHCRNyM3Q" }     
            r = requests.get(url, param)
            for k in range(n):
                data.append(r.json()['results'][k]['elevation'] )
            locations=''
            n=0
        else: locations+='|'

if n!=0:
    locations=locations[:-1]
    param = {"locations":locations,"key": "AIzaSyCgFZ_bPFLzrCOQQxRE9wwxqGDHCRNyM3Q" } 
    r = requests.get(url, param)
    for i in range(n):
        data.append(r.json()['results'][i]['elevation'] )


total = float(255 ** 3)
max_ = float(max(data))
min_ = float(min(data))
data = [float(((num - min_) / ((max_ - min_)/total))) for num in data]

data=np.array(data,dtype='int64')
output = np.zeros([size1*size2, 3])
for j in range(len(data)):
    value = [0, 0, 0]
    num = data[j]
    i = 0
    while num > 255:
        value[i] = num % 256
        num /= 256
        i += 1

    value[i] = num
    for k in range(3):
        output[j][k] = value[k]
output.resize(size1,size2,3)
cv2.imwrite('Heightmap/Taiwan/'+"{:.3f}".format(lat) +','+"{:.3f}".format(lon)+'.png', output)
