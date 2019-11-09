using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Display2d : MonoBehaviour {
    public Renderer textureRenderer;
    

    
    public void Display(float[,] noiseMap)
    {
        int w = noiseMap.GetLength(0), h = noiseMap.GetLength(1);
        Color[] colorMap = new Color[w*h];
        for(int x = 0; x < w; x++)
        {
            for(int y = 0; y < h; y++)
            {
                colorMap[x + y * w] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        Texture2D texture = new Texture2D(w, h);
        texture.SetPixels(colorMap);
        texture.Apply();
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(w, 1, h);
    }
}
