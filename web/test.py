import requests
import base64
import json

url = "http://localhost:5001/generate"

with open("E:/scifair/heightmap/China256/0,50_China.png", "rb") as img:
    encode = base64.b64encode(img.read())

d = {"file": str(encode)[2:], "overlay": "100"}
print(encode)
x = requests.post(url, json=d)

print(x.text)