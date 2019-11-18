# 武陵高中科學班專題：擬真山脈地形生成與驗證

## 摘要
使用Google Maps擷取真實的地形，並利用一個二維的矩陣儲存特定位置的高度。後使用Perlin Noise生成類似山脈的地形，但因為此地形未經過河流的侵蝕，所以離真正的山脈地形仍有段距離，故我們可以訓練一個神經網路優化下雨的大小及水對山脈的影響，盡可能的模擬出接近真實山脈的地形。我們還可以利用一個神經網路判斷一個山脈是否是由人工生成出來的。所以這邊就有兩個神經網路，互相優化，有點類似GAN的概念。


## 資料夾
- `heightmap`: 台灣及中國東南之山地圖及空照圖
- `info`: 內有進度及資料集說明檔 (`progress.md`, `name.md`)
- `perlin_image`: Perlin noise之灰階圖
- `perlin_for_test`: 經合併之Perlin noise
- `river`: 研究支流生成
- `simple_poly`: 測試cycle_gan的簡單多邊形
- `Unity_Hydraulic_Erosion`: Unity專案 (水流侵蝕, Unity Terrain轉灰階高度圖...)

## 進度
[專題進度](https://github.com/jayin92/scifair/blob/master/docs/progress.md)

## 資料集

[專題各Datasets及Results名稱](https://github.com/jayin92/scifair/blob/master/docs/name.md)
