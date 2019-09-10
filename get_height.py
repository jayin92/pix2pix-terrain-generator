import requests
import json

request_url = "https://maps.googleapis.com/maps/api/elevation/json"

params = {
    "locations": "23.4699995,120.9487002",
    "key": "AIzaSyD57zm-VEPud6YTbl6XKpu7kZIdlHxHZIQ"
}

r = requests.get(request_url, params)

data = r.json()

data = data["results"][0]
print(data["location"])
print(data["elevation"])