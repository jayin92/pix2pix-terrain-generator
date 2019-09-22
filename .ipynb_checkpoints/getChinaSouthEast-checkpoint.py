import requests
import json
import numpy as np
import matplotlib.pyplot as plt
import cv2
import os
from PIL import Image
from tqdm import tqdm
url = "https://maps.googleapis.com/maps/api/elevation/json"

ppbar = tqdm(total=2000*2000)

        
data=[]
locations=''    
lat = 26.9
lon = 100
size = 2500
n = 0
for i in range(-size//2,size-size//2):
    for j in range(-size//2,size-size//2):
        locations+="{:.3f}".format(lat-i*0.001)+","+"{:.3f}".format(lon+j*0.001)
        ppbar.set_description_str("{},{}".format("{:.3f}".format(lat-i*0.001),"{:.3f}".format(lon+j*0.001)))
        ppbar.update(1)
        n += 1
        if n == 510:
            param = {"locations":locations,"key": "AIzaSyCgFZ_bPFLzrCOQQxRE9wwxqGDHCRNyM3Q" }     
            r = requests.get(url, param)
            for k in range(n):
                data.append(r.json()['results'][k]['elevation'])
            locations=''
            n=0
        else: locations+='|'
if n != 0:
    locations=locations[:-1]
    param = {"locations":locations,"key": "AIzaSyCgFZ_bPFLzrCOQQxRE9wwxqGDHCRNyM3Q" } 
    r = requests.get(url, param)
    for i in range(n):
        data.append(r.json()['results'][i]['elevation'])

total = float(255 ** 3)        
min_=min(data)
max_=max(data)
data = [float(((num - min_) / ((max_ - min_)/total))) for num in data]
data = np.array(data,dtype='int64')
output = np.zeros([size*size, 3])
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
output.resize(size,size,3)
cv2.imwrite('Heightmap/china-southeast/'+"{:.3f}".format(lat) +','+"{:.3f}".format(lon)+'.png', output)
ppbar.update(1)
