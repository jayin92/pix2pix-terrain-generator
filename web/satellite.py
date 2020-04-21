import os
import torch
import json
import random
import string
import pathlib
import base64
import re
import cv2
import configparser

from io import BytesIO
from options.test_options import TestOptions
from data import create_dataset
from data.base_dataset import get_transform, BaseDataset, get_params
from data.image_folder import make_dataset
from models import create_model
from util.visualizer import save_images
from util import html, util
from PIL import Image

from flask import Flask, request, send_file, redirect, render_template, Response
app = Flask(__name__)

import util.split_merge as spl
import numpy as np

config = configparser.ConfigParser()
config.read("config.ini")

opt = TestOptions().parse()  # get test options
# hard-code some parameters for test
opt.name = "china_aerial_BtoA"
opt.model = "test"
opt.direction = "BtoA"
opt.num_threads = 0   # test code only supports num_threads = 1
opt.batch_size = 1    # test code only supports batch_size = 1
opt.serial_batches = True  # disable data shuffling; comment this line if results on randomly chosen images are needed.
opt.no_flip = True    # no flip; comment this line if results on flipped images are needed.
opt.display_id = -1   # no visdom display; the test code saves the results to a HTML file.


input_nc = opt.output_nc if opt.direction == 'BtoA' else opt.input_nc
model = create_model(opt)      # create a model given opt.model and other options
model.setup(opt)               # regular setup: load and print networks; create schedulers
model.eval()
transform_s = get_transform(opt, grayscale=(input_nc == 1))

def delete_img():
    folder = os.path.join("static", "gen")
    images = os.listdir(folder)
    if len(images) >= 20:
        for image in images[:10]:
            os.remove(os.path.join(folder, image))

class Dataset(torch.utils.data.Dataset):
    def __init__(self, file):
        super(Dataset, self).__init__()
        self._data = file
    
    def __getitem__(self, index):
        image = self._data[index]
        A = transform_s(image)

        return {'A': A, 'A_paths': ""}

    def __len__(self):
            return len(self._data)
    
@app.route("/", methods=["GET"])
def index():
    path = os.path.join("static", "gen")
    try:
        os.makedirs(path)
    except:
        print("folder exists")
    delete_img()
    return render_template("index.html")
 
@app.route("/generate", methods=["POST"])
def generate():
    delete_img()
    path = os.path.join("static", "gen")
    pathlib.Path('static/gen').mkdir(parents=True, exist_ok=True)
    if request.method == "POST":
        res = request.json
        print(res)
        spl.size = 256
        spl.overlay = int(res["overlay"])
        file = res["file"]
        file = re.sub('^data:image/.+;base64,', '', file)
        file = base64.b64decode(file)
        file = BytesIO(file)
        A_img = Image.open(file).convert("RGB")
        tmp = []
        tmp.append(A_img)
        dataset = Dataset(tmp)
        data_loader = torch.utils.data.DataLoader(
            dataset,
            batch_size=opt.batch_size,
            shuffle=not opt.serial_batches,
            num_workers=int(opt.num_threads))

        im=[]
        for index, data in enumerate(data_loader):  
            model.set_input(data)
            model.test()
            visuals = model.get_current_visuals()
            for label, im_data in visuals.items():
                if label=='fake':
                    im.append(util.tensor2im(im_data))
        
        print(type(im[0]))
        result_array=im[0]
        result_image=Image.fromarray(result_array,"RGB")
        img_name = ''.join(random.choice(string.ascii_lowercase) for i in range(6))                
        result_image.save(os.path.join(path, "{}.png".format(img_name)))
        #util.save_image(np.array([result_image,result_image,result_image]), os.path.join(path, "{}.png".format(img_name)))
        rep = {
            'file_name': img_name
        }

        return Response(json.dumps(rep), mimetype="application/json")

@app.route("/satellite", methods=["POST"])
def satellite():
    delete_img()
    path = os.path.join("static", "gen")
    pathlib.Path('static/gen').mkdir(parents=True, exist_ok=True)
    if request.method == "POST":
        res = request.json
        print(res)
        spl.size = 256
        spl.overlay = int(res["overlay"])
        file = res["file"]
        file = re.sub('^data:image/.+;base64,', '', file)
        file = base64.b64decode(file)
        file = BytesIO(file)
        A_img = Image.open(file).convert("RGB")
        tmp = []
        tmp.append(A_img)
        dataset = Dataset(tmp)
        data_loader = torch.utils.data.DataLoader(
            dataset,
            batch_size=opt.batch_size,
            shuffle=not opt.serial_batches,
            num_workers=int(opt.num_threads))

        im=[]
        for index, data in enumerate(data_loader):  
            model.set_input(data)
            model.test()
            visuals = model.get_current_visuals()
            for label, im_data in visuals.items():
                if label=='fake':
                    im.append(util.tensor2im(im_data))
        
        print(type(im[0]))
        result_array=im[0]
        result_image=Image.fromarray(result_array,"RGB")
        img_name = ''.join(random.choice(string.ascii_lowercase) for i in range(6))                
        result_image.save(os.path.join(path, "{}.png".format(img_name)))
        #util.save_image(np.array([result_image,result_image,result_image]), os.path.join(path, "{}.png".format(img_name)))
        rep = {
            'file_name': img_name
        }

        return Response(json.dumps(rep), mimetype="application/json")


if __name__ == "__main__":
    app.run(debug=True, port=5002)
