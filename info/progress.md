---
title: 專題進度
tags: 專題
---

# 專題進度

## 第一周 (9/3 ~ 9/10)
- 發想專題題目
    - [專題題目](/d3ECGjwoTH6pGSBxUQD-Gg)
## 第二周 (9/10 ~ 9/17)
- 確定題目為利用GAN生成山脈地形
- 在網路上搜尋是否有前人做過類似的專題
- 找到由Phillip Isola, Jun-Yan Zhu, Tinghui Zhou, Alexei A. Efros研究的[Image-to-Image Translation with Conditional Adversarial Networks](https://arxiv.org/pdf/1611.07004.pdf)及其利用PyTorch寫成的實做模型[junyanz/pytorch-CycleGAN-and-pix2pix](https://github.com/junyanz/pytorch-CycleGAN-and-pix2pix)
- 利用Google Maps API抓下來的高度圖訓練上述之模型
    -  台灣模糊高度圖轉清晰高度圖 (利用`cv2.medianBlur()`模糊圖片)

    | real_A | fake_B | real_B |
    | -------- | -------- | -------- |
    | ![](https://i.imgur.com/H7Mk3LS.png) | ![](https://i.imgur.com/WYPfmVv.png)| ![](https://i.imgur.com/GqYxPrt.png)|
    
    - 中國模糊高度圖轉清晰高度圖 (利用`cv2.medianBlur()`模糊圖片)
    
    | real_A | fake_B | real_B |
    | -------- | -------- | -------- |
    |![](https://i.imgur.com/FcbWoCk.png)|![](https://i.imgur.com/t9kQzG8.png)|![](https://i.imgur.com/VKm0jPY.png)|

- 因為我們較熟悉keras的語法，故我們也找了根據上面pix2pix paper的keras implemention [eriklindernoren/Keras-GAN](https://github.com/eriklindernoren/Keras-GAN/tree/master/pix2pix)
## 第三周 (9/17 ~ 9/24)
- 討論第二周生成圖像的成果 (pix2pix模型對此問題效果相當好)
- 學長提議可以試著訓練可以將清晰高度圖轉空照圖的模型
    - 利用Google API將空照圖抓下來，並確認與高度圖有對齊
    - 將空照圖及清晰高度圖配成一組
    - 清晰高度圖轉空照圖

    | real_A | fake_B | real_B |
    | -------- | -------- | -------- |
    |![](https://i.imgur.com/wi8zRv8.png)|![](https://i.imgur.com/GAzx0ub.png)|![](https://i.imgur.com/vi3dSHh.png)|
    
    - 空照圖轉清晰高度圖
    
    | real_A | fake_B | real_B |
    | -------- | -------- | -------- |
    |![](https://i.imgur.com/VwyrWfj.png)|![](https://i.imgur.com/aDvkeiN.png)|![](https://i.imgur.com/0BlKzPB.png)|
- Perlin Noise 轉具山脈特徵的Perlin Noise圖
    - 2D Perlin Noise 和模糊過的真實高度圖相似，所以用 Perlin Noise 當第二周訓練好的模糊轉清晰模型的輸入，能達到強化山脈的特徵的效果
    

    | real_A | fake_B |
    | -------- | -------- |
    |![](https://i.imgur.com/t0UDYNu.png)|![](https://i.imgur.com/eQb8GTb.png)|
    - 清晰化的結果成功加強山脈和河谷的線條，但和真實高度圖的特徵還是有差距，例如河谷不連續
    - 可能的解決方案是產生更複雜(高頻)的 Perlin Noise，再套用`cv2.medianBlur()`將其模糊，作為輸入
- 具山脈特徵的Perlin Noise圖轉空照圖
    - 因為具山脈特徵的 Perlin Noise 與清晰高度圖相似，所以我們嘗試使用剛剛提到的清晰高度圖轉空照圖的model來將具山脈特徵的Perlin Noise圖轉成空照圖

    | real_A | fake_B |
    | -------- | -------- |
    |![](https://i.imgur.com/BdPEyOd.png)|![](https://i.imgur.com/cYcIglN.png)|
    
- 在這周我們將一張隨機產生的 Perlin Noise 透過兩種 model 轉成空照圖
    

## 第四周 (9/24 ~ 10/1)
- 測試同樣在[junyanz/pytorch-CycleGAN-and-pix2pix](https://github.com/junyanz/pytorch-CycleGAN-and-pix2pix)的cycle GAN模型 (將中國清晰高度圖轉空照圖)
    - 可能因為discriminator判斷有問題，所以圖像並不能成功生成
    

    | real_A | fake_B | rec_A | 
    | -------- | -------- | -------- | 
    | ![](https://i.imgur.com/JHaHtxg.png) | ![](https://i.imgur.com/pxu1xdf.png) | ![](https://i.imgur.com/7RdIzYe.png) 
    | **real_B** | **fake_A** | **rec_B** |
    | ![](https://i.imgur.com/1ADtyNj.png) | ![](https://i.imgur.com/Iv3s7FZ.png) | ![](https://i.imgur.com/mIXpSKh.png) |
## 第五周 (10/1 ~ 10/8)
- Unity
    - 用Unity把pix2pix模型生成出來的height map及color map顯示成立體的地形，方便觀察
    

    | height map| color map| 3d |
    | -------- | -------- | -------- |
    | ![](https://i.imgur.com/nTqGYZI.png =x150)     | ![](https://i.imgur.com/KXTfQLr.png =x150)| ![](https://i.imgur.com/WXE5jki.png =x160)     |

- 利用簡單圖形訓練cycle GAN
    - 因為上周使用 cycle GAN 並不能使圖像正確的生成（圖像的黑白相反），故這周嘗試使用簡單的圖形進行測試(生成100*100的隨機二值圖，模型應該要能生成黑白相反的圖片)，但圖像仍舊無法成功生成
    
    | real_A | fake_B | rec_A | 
    | -------- | -------- | -------- | 
    | ![](https://i.imgur.com/jdcwxk3.png) | ![](https://i.imgur.com/aCPmw6n.png) | ![](https://i.imgur.com/JN3Zufj.png) 
    | **real_B** | **fake_A** | **rec_B** |
    | ![](https://i.imgur.com/8qS8D5r.png) |![](https://i.imgur.com/7TXJ7Vh.png) |![](https://i.imgur.com/CJU3ox4.png) |

    - 接下來可能會使用更簡單的幾何圖形進行測試（正方形、長方形、三角形等等）

## 第六周 (10/8 ~ 10/15)
第一次段考前一週，暫停一次

## 第七周 (10/15 ~ 10/22)
第一次段考周

## 第八周 (10/22 ~ 10/29)
- cycleGAN
    - 利用小畫家所畫出來圖片測試，發現可以正確反轉顏色

## 第九周 (10/29 ~ 11/5)
- 利用Unity模擬水流侵蝕
    - 可以成功使地形侵蝕及堆積
    - 目前尚未解決為何河流下仍會繼續侵蝕造成相當深的河谷
- 利用**擴張 + 侵蝕 + 細線化**延伸河流
    - 目前經過以上處理的所延伸的河流過於直線，不像現實中的河流


## 第十周 (11/5 ~ 11/12)
- Unity
    - 利用Compute Shader進行GPU的平行運算
- 河流
    - 效果依舊沒有很好
