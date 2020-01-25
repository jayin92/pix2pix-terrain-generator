import os
import torch
import json
import random
import string
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


opt = TestOptions().parse()  # get test options
# hard-code some parameters for test
opt.num_threads = 0   # test code only supports num_threads = 1
opt.batch_size = 1    # test code only supports batch_size = 1
opt.serial_batches = True  # disable data shuffling; comment this line if results on randomly chosen images are needed.
opt.no_flip = True    # no flip; comment this line if results on flipped images are needed.
opt.display_id = -1   # no visdom display; the test code saves the results to a HTML file.

opt.name = "China256"
opt.gpu_ids = "-1"
opt.model = "test"
input_nc = opt.output_nc if opt.direction == 'BtoA' else opt.input_nc
model = create_model(opt)      # create a model given opt.model and other options
model.setup(opt)               # regular setup: load and print networks; create schedulers
model.eval()
transform = get_transform(opt, grayscale=(input_nc == 1))

def delete_img():
    folder = os.path.join("static", "gen")
    images = os.listdir(folder)
    if len(images) >= 20:
        for image in images[:10]:
            os.remove(os.path.join(folder, image))

class Dataset(torch.utils.data.Dataset):
    def __init__(self, file):
        super(Dataset, self).__init__()
        self._data = [file]
    
    def __getitem__(self, index):
        image = self._data[index]
        A = transform(image)

        return {'A': A, 'A_paths': ""}

    def __len__(self):
            return len(self._data)
    
@app.route("/", methods=["GET"])
def index():
    delete_img()
    return render_template("index.html")
 
@app.route("/generate", methods=["POST"])
def generate():
    print(request)
    delete_img()
    if request.method == "POST":
        file = request.files["file"]
        A_img = Image.open(file).convert("RGB")
        dataset = Dataset(A_img)
        data_loader = torch.utils.data.DataLoader(
            dataset,
            batch_size=opt.batch_size,
            shuffle=not opt.serial_batches,
            num_workers=int(opt.num_threads))
        
        for index, data in enumerate(data_loader):
            model.set_input(data)
            model.test()
            visuals = model.get_current_visuals()
            for label, im_data in visuals.items():
                im = util.tensor2im(im_data)
                img_name = ''.join(random.choice(string.ascii_lowercase) for i in range(6))
                util.save_image(im, "static/gen/{}.png".format(img_name))
        rep = {
            'file_name': img_name
        }
        return Response(json.dumps(rep), mimetype="application/json")


if __name__ == "__main__":
    app.run(debug=True, port=8888)