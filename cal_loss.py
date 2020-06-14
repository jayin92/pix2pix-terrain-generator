import os
import torch
import torch.nn as nn
import argparse
from tqdm import tqdm
from PIL import Image
from statistics import *
from torchvision import transforms
from matplotlib import pyplot as plt



device = torch.device("cuda:0" if torch.cuda.is_available() else "cpu")

loader = transforms.ToTensor()

unloader = transforms.ToPILImage()

def image_loader(img_path):
    img = Image.open(img_path).convert("RGB")
    img = loader(img).unsqueeze(0)

    return img.to(device, torch.float)

def cal_loss(path1, path2):
    t1 = image_loader(path1)
    t2 = image_loader(path2)
    # print(t1.min(), t1.max())

    loss = nn.MSELoss()   
    
    return loss(t1, t2).item()


def cal_one_dir_loss():
    parser = argparse.ArgumentParser()
    parser.add_argument("--dataroot", help="path to result directory.")

    args = parser.parse_args()

    dataroot = args.dataroot
    imgs = sorted(os.listdir(dataroot))
    results = []
    cnt = 0
    for i in tqdm.tqdm(range(0, len(imgs), 3)):
        results.append(cal_loss(os.path.join(dataroot, imgs[i]), os.path.join(dataroot, imgs[i+2])))
    
    print("Average L1 Loss:", mean(results))
    print("std:", pstdev(results))

def cal_two_dir_each_size():
    parser = argparse.ArgumentParser()
    parser.add_argument("--dirA")
    parser.add_argument("--dirB")

    args = parser.parse_args()

    dirA = args.dirA
    dirB = args.dirB

    x = [i for i in range(1, 31, 2)]
    print(x)

    lossA = [[] for i in range(15)]
    lossB = [[] for i in range(15)]

    imgA = sorted(os.listdir(dirA))
    imgB = sorted(os.listdir(dirB))
    print(len(imgA), len(imgB))
    for i in tqdm(range(0, len(imgA), 3)):
        idx = int((int(imgA[i].split("_")[0]) - 1) / 2)
        lossA[idx].append(cal_loss(os.path.join(dirA, imgA[i]), os.path.join(dirA, imgA[i + 2])) * 100)
        lossB[idx].append(cal_loss(os.path.join(dirB, imgB[i]), os.path.join(dirB, imgB[i + 2])) * 100)

    for i in range(len(lossA)):
        lossA[i] = mean(lossA[i])
        lossB[i] = mean(lossB[i])
    
    plt.plot(x, lossA, label="modelA")
    plt.plot(x, lossB, label="modelB")

    plt.legend()
    # plt.show()
    plt.savefig("loss.png")

def cal_each_size(path):
    loss = [[] for i in range(15)]

    img = sorted(os.listdir(path))

    for i in range(0, len(img), 3):
        idx = int((int(img[i].split("_")[0]) - 1) / 2)
        loss[idx].append(cal_loss(os.path.join(path, img[i]), os.path.join(path, img[i + 2])))
    
    for i in range(len(loss)):
        loss[i] = mean(loss[i])

    return loss


def cal_multi():
    dir_name = [
        "test0606_5",
        "test0606_3",
        "test0606_4",
        "test0606_2",
        "test0605",
        "test0605_2",
        "test0607",
    ]

    prefix = "/home/host/pytorch-CycleGAN-and-pix2pix/results/"
    suffix = "test_latest/images"
    
    x = [i for i in range(1, 31, 2)]

    for item in dir_name:
        print(item)
        loss = cal_each_size(os.path.join(prefix, item, suffix))
        plt.plot(x, loss, marker="o", label=item)

    plt.legend(bbox_to_anchor=(1.05, 1.0), loc='upper left')
    plt.tight_layout()
    plt.savefig("loss2.png")


if __name__ == "__main__":
    cal_multi()