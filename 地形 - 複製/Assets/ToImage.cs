using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.IO;

public class ToImage : MonoBehaviour {

    // Use this for initialization
    public Terrain terrain;
    void Start () {        
		int w = terrain.terrainData.heightmapWidth;
		int h = terrain.terrainData.heightmapHeight;
		Texture2D heightMap = new Texture2D(w, h);
		float[,] hm = terrain.terrainData.GetHeights(0, 0, w, h);		
		Color[] img = new Color[w*h];
		float min = hm[0, 0];
		float max = hm[0, 0];
		for(int i=0;i<w;i++){
			for(int j=0;j<h;j++){
				if(hm[i, j] > max) max = hm[i, j]; 
				if(hm[i, j] < min) min = hm[i, j];
			}
		}
		// print(min);
		// print(max);
		for(int i=0;i<w;i++){
			for(int j=0;j<h;j++){
				img[i+j*h] = new Color(hm[i, j] / max, hm[i, j] / max, hm[i, j] / max);
				// print(hm[i, j] * 255 / max);
			}
		}
		heightMap.SetPixels(img);
        heightMap.Apply();

		FileStream fs = new FileStream(@"/home/jayinnn/scifair/test.png", FileMode.CreateNew);
		fs.Write(heightMap.EncodeToPNG(), 0, heightMap.EncodeToPNG().Length);
       
		
	}
	
}
