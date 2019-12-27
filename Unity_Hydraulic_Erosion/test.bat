cd ..
call conda activate pytorch
python use_model.py --name China --dataroot .\Unity_Hydraulic_Erosion\toGan
copy ..\pytorch-CycleGAN-and-pix2pix\results\China\test_latest\images\fromCS.png_fake_B.png .\Unity_Hydraulic_Erosion\fromGAN\fromGAN.png
pause