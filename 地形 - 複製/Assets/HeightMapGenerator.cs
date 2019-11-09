using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator{
    public static float[,] Perlin(int w,int h,Vector2 pos,float scale)
    {
        float[,] map = new float[w, h];
        for (float f = 1; f <=4.0f/scale ; f *=2)
        {
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    map[x, y] += 0.5f/f*Mathf.PerlinNoise(x * scale*f + pos.x, y * scale*f + pos.y);
                }
            }
        }
        return map;
    }
}
