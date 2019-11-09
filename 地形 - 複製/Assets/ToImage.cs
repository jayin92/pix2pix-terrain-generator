using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.Image.BMP;

public class ToImage : MonoBehaviour {

    // Use this for initialization
    public Terrain terrain;
	void Start () {
		int w = terrain.terrainData.heightmapWidth;
		int h = terrain.terrainData.heightmapHeight;
		float[,] hm = terrain.terrainData.GetHeights(0, 0, w, h);
		Color[] img = new Color[w*h];
		float min = hm[0, 0];
		float max = hm[0, 0];
		float d;
		for(int i=0;i<w;i++){
			for(int j=0;j<h;j++){
				if(hm[i, j] > max) max = hm[i, j]; 
				if(hm[i, j] < min) min = hm[i, j];
			}
		}
		for(int i=0;i<w;i++){
			for(int j=0;j<h;j++){
				img[i*w+j] = new Color (hm[i, j] * 255 / max ,hm[i, j] * 255 / max, hm[i, j] * 255 / max);
			}
		}
		BMPImage im;
		// im.ImageData = img;
       
		
	}
	
}
