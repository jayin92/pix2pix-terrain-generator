import requests
import json
import numpy as np
import matplotlib.pyplot as plt
import cv2
from PIL import Image
from tqdm import tqdm
url = "https://maps.googleapis.com/maps/api/elevation/json"
p1=np.array([27.162822, 117.929151])
p2=np.array([27.494665, 113.754346])
p3=np.array([24.952233, 117.819288])
p4=np.array([24.772807, 113.820264])

ppbar = tqdm(total=34*30)
for t1 in np.linspace(0,1,34):
    for t2 in np.linspace(0,1,30):
        data=[]
        locations=''
        p=(p1*t1+p2*(1.0-t1))*t2+(p3*t1+p4*(1.0-t1))*(1.0-t2)
        lat=p[0]
        lon=p[1]
        size=100
        n=0
        n_sum=0;
        ppbar.set_description_str("{},{}".format(lat,lon))
        for i in range(-size//2,size-size//2):
            for j in range(-size//2,size-size//2):
                locations+="{:.3f}".format(lat-i*0.001)+","+"{:.3f}".format(lon+j*0.001)
                n+=1;n_sum+=1;
                if n == 300:
                    param = {"locations":locations,"key": "AIzaSyD57zm-VEPud6YTbl6XKpu7kZIdlHxHZIQ" }     
                    r = requests.get(url, param)
                    for k in range(n):
                        data.append(r.json()['results'][k]['elevation'])
                    locations=''
                    n=0
                    # print((str)(n_sum)+'/'+(str)(size**2))
                else: locations+='|'
        if n!=0:
            locations=locations[:-1]
            param = {"locations":locations,"key": "AIzaSyD57zm-VEPud6YTbl6XKpu7kZIdlHxHZIQ" } 
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
