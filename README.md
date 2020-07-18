# jayin92/pix2pix-terrain-generator：利用生成對抗網路生成擬真的山脈地形 

## 摘要 Abstract
本研究中，我們利用Google所提供的API收集來自台灣及中國的地形高度圖(heightmap)和衛星空照圖。我們利用收集的圖像訓練pix2pix cGAN (conditional Generative Adversarial Network) 模型，並將人工繪製的高度圖加上真實山脈應有的細節(包含尖銳的山脊、山壁上的紋路、連續的河流網路……等)，透過訓練的pix2pix模型生成更接近真實山脈的效果。為了提升擬真地形的生成效果，我們在原先pix2pix模型的基礎下，額外加入「動態權重層」來達成。最後利用這些經訓練的pix2pix模型，開發了API、Unity客戶端及網頁客戶端，使我們訓練的pix2pix模型更加實用。本研究除了能應用於遊戲開發，使生成擬真山脈的流程更為簡化，也可將其應用於將低解析度之高度圖轉換為高解析度之高度圖，同時保持高度圖的真實性，幫助地形資料的收集作業。

In this research, we collected topographic altitude maps from Taiwan and China using the API provided by Google and satellite sky map. We use the collected images to train pix2pix cGAN (conditional Generative Adversarial Network) model, and will manually draw the Altimeter with all the details expected of a real mountain range (including sharp ridges, lines on walls, continuous river networks. By using the trained pix2pix model, we can generate a more realistic mountain range effect. In order to improve the realistic terrain generation, we added "dynamic weight layer" to the original pix2pix model. We have also developed a new version of the Pix2pix model. Finally, using these trained pix2pix models, we developed the API, Unity client and web client to enable the We have trained pix2pix models to be more practical. In addition to simplifying the process of generating realistic mountain ranges for game development, this research can also be applied to the creation of low-resolution, high-resolution mountain ranges. Altimeter maps are converted to high resolution altimeter maps, while maintaining the authenticity of the altimeter maps to facilitate the collection of topographic data.

## 論文 Paper (Chinese)
- [報告書](https://github.com/jayin92/pix2pix-terrain-generator/paper/paper_final.pdf)


## 資料集 Datasets
[https://github.com/jayin92/pixpix-terrain-generator-datasets](https://github.com/jayin92/pix2pix-terrain-generator/blob/master/paper/paper_final.pdf)


## 獎項 Awards
- 桃園市立武陵高中校內科展 電腦與資訊學科 第一名
- 桃園市第六十屆科展 電腦與資訊學科 第一名
- 旺宏科學獎 入圍決賽


## 作者 Authors
- 程品奕 [@a931eric](https://github.com/a931eric)
- 李杰穎 [@jayin92](https://github.com/jayinnn)


