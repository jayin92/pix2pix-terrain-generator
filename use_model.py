import os
import cv2
import numpy as np
import argparse

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--dataroot", help="path to test images")
    parser.add_argument("--name", type=str, help="name of the experiment")
    args = parser.parse_args()

    name = args.name
    dataroot = args.dataroot
    print("---Remove all images in /tmp/test---")
    os.system("rm -rf {}".format(os.path.join("heightmap", "tmp", "test")))
    os.system("mkdir {}".format(os.path.join("heightmap", "tmp", "test")))
    print("---Images stick with a black image---")
    for file in os.listdir(dataroot):
        try:
            img = cv2.imread(os.path.join(dataroot, file))
            size = img.shape[0]
            tmp = [[0, 0, 0] for i in range(size)]
            img_com = []
            for i in range(size):
                img_com.append(tmp)
            img_com = np.array(img_com)
            img = np.concatenate((img, img_com), axis=1)
            cv2.imwrite(os.path.join("heightmap", "tmp", "test", "{}.png".format(file)), img)
        except AttributeError:
            print(file, "is not an image file")
    print("---Complete & Running test.py---")
    os.chdir("../pytorch-CycleGAN-and-pix2pix")
    os.system("python test.py --dataroot ../scifair/heightmap/tmp --name {} --direction AtoB --model pix2pix".format(name))
    print("---Copying results---")
    os.system("cp -r results/{}/test_latest/ ../scifair/heightmap/tmp".format(name))