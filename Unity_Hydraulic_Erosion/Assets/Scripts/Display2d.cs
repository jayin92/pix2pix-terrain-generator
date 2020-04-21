using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Display2d : MonoBehaviour {
    public Renderer textureRenderer;
    public RawImage image;

    
    public void Display(float[,] noiseMap)
    {
        int w = noiseMap.GetLength(0), h = noiseMap.GetLength(1);
        Color[] colorMap = new Color[w*h];
        float min=10000, max=-10000;


        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (noiseMap[x, y] < min) min = noiseMap[x, y];
                if (noiseMap[x, y] >max) max = noiseMap[x, y];
            }
        }
                for (int x = 0; x < w; x++)
        {
            for(int y = 0; y < h; y++)
            {
                colorMap[x + y * w] = Color.Lerp(Color.black, Color.white, (noiseMap[x, y]-min)/(max-min));
            }
        }
        Texture2D texture = new Texture2D(w, h);
        texture.SetPixels(colorMap);
        texture.Apply();
        textureRenderer.material.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(w*0.1f, 1, h*0.1f);
        image.texture = texture;
    }
}
