# 武陵高中科學班專題：擬真山脈地形生成與驗證

## 摘要
本研究中，我們利用 Google 所提供的 API 服務，收集來自台灣及中國的灰階高度圖和衛星空照圖。我們利用以上圖像訓練 pix2pix 生成對抗網路模型，使其能將輸入之灰階高度圖加上山脈細節，使其更為接近真實的山脈。利用經訓練的 pix2pix 模型，我們開發了 API、Unity
客戶端及網頁客戶端，使我們訓練的 pix2pix 模型更加實用。本研究能應用於遊戲開發，使生成擬真山脈的流程更為簡化，也可將其應用於將低解析度之地形高度圖轉換為高解析度之地形高度圖，同時保持高度圖的真實性。

## 資料夾
- `heightmap`: 台灣及中國東南之山地圖及空照圖
- `info`: 內有進度及資料集說明檔 (`progress.md`, `name.md`)
- `perlin_image`: Perlin noise之灰階圖
- `perlin_for_test`: 經合併之Perlin noise
- `river`: 研究支流生成
- `simple_poly`: 測試cycle_gan的簡單多邊形
- `Unity_Hydraulic_Erosion`: Unity專案 (水流侵蝕, Unity Terrain轉灰階高度圖...)
- `low_to_high_exp`：低解析度轉高解析度

## 進度
[專題進度](https://github.com/jayin92/scifair/blob/master/docs/progress.md)

## 資料集

[專題各Datasets及Results名稱](https://github.com/jayin92/scifair/blob/master/docs/name.md)
