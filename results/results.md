---
title: 專題各Datasets及Results名稱
tags: 專題
---
# 專題各Datasets及Results名稱

## 中國
- 中國模糊高度圖轉清晰高度圖
    - Datasets: ChinaForTrain
    - Results: China
- 中國空照圖轉清晰高度圖
    - Datasets: ChinaAerialTrain
    - Results: china_aerial
- 中國清晰高度圖轉空照圖
    - Datasets: ChinaAerialTrain
    - Results: china_aerial_BtoA
- 中國模糊高度圖轉空照圖
    - Datasets: ChinaAerialTrainBlur
    - Results: china_aerial_blur
- 中國清晰高度圖轉空照圖（Cycle GAN）
    - Datasets: china_aerial_cyclegan
    - Results: china_aerial_cyclegan

## 台灣
- 台灣模糊高度圖轉清晰高度圖
    - Datasets: TaiwanForTrain
    - Results: Taiwan
- 台灣清晰高度圖轉空照圖
    - Dataset: aerialTrain
    - Results: taiwan_aerial

## Perlin Noise
- Perlin Noise轉符合山脈特徵的Perlin Noise (使用中國模糊高度圖轉清晰高度圖模型)
    - Datasets: PerlinForTrain
    - Results: perlin_blur_to_clear
- 符合山脈特徵的Perlin Noise轉空照圖（使用中國清晰高度圖轉空照圖模型）
    - Datasets: PerlinForTrainHigh
    - Results: perlin_clear_to_aerial
- 模糊Perlin Noise轉空照圖（使用中國模糊高度圖轉空照圖模型）
    - Datasets: PerlinForTrain
    - Results: perlin_blur_to_aerial
