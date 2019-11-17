using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ctrl : MonoBehaviour {
    public Display2d d2;
    public Rain2 rain;
    public GameObject chunkPrefab;
    public Terrain terrain;

    public float[,] heightMap;
    public float[,] waterMap;
    public Color[,] colorMap;
    public Texture2D texure_h,texure_c;
    public int chunkSize=100;
    
    [System.Serializable]
    public class PerlinNoiseInfo
    {
        public int w, h;
        public float yScale;
        public Vector2 pos;
        public float scale = 0.2f;
        public AnimationCurve curve;
        public Gradient colorOfHeight;
    }
    public PerlinNoiseInfo perlin;
    public void GeneratePerlinNoise()
    {
        heightMap = HeightMapGenerator.Perlin(perlin.w, perlin.h,perlin.pos,perlin.scale);
        waterMap = new float[perlin.w, perlin.h];
        colorMap = new Color[heightMap.GetLength(0),heightMap.GetLength(1)];
        for (int x = 0; x < perlin.w; x++)
        {
            for (int y = 0; y < perlin.h; y++)
            {
                heightMap[x, y] = Mathf.Max(0, perlin.curve.Evaluate(heightMap[x, y]) * perlin.yScale/perlin.scale) ;
                colorMap[x, y] = perlin.colorOfHeight.Evaluate(heightMap[x, y]);
            }
        }
        rain.Init(heightMap);
        Display3D();
    }

    public void Terrain()
    {
        float [,] heightMap_ = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
        
        perlin.w = heightMap_.GetLength(0);
        perlin.h = heightMap_.GetLength(1);
        heightMap = new float[perlin.w,perlin.h];
        waterMap = new float[perlin.w, perlin.h];  
        colorMap = new Color[heightMap_.GetLength(0), heightMap_.GetLength(1)];
        for (int x = 0; x < perlin.w; x++)
        {
            for (int y = 0; y < perlin.h; y++)
            {
                heightMap[x, y] =heightMap_[y,x] *perlin.yScale;
                colorMap[x, y] = perlin.colorOfHeight.Evaluate(heightMap[x, y]);
            }
        }
        rain.Init(heightMap);
        Display3D();

        Color[] colormap_ = new Color[perlin.w * perlin.h];
        for (int x = 0; x < perlin.w; x++)
        {
            for (int y = 0; y < perlin.h; y++)
            {
                colormap_[x + y * perlin.w] =new Color(heightMap[x,y]*256, heightMap[x, y] * 256, heightMap[x, y] * 256);
            }
        }
 
    }

    public void RainErosionAt()
    {
        colorMap = new Color[heightMap.GetLength(0), heightMap.GetLength(1)];
        for (int x = 0; x < perlin.w; x++)
        {
            for (int y = 0; y < perlin.h; y++)
            {
                colorMap[x, y] = perlin.colorOfHeight.Evaluate(heightMap[x, y]);
            }
        }
        Display3D();
    }

    public void display2d()
    {     
        d2.Display(heightMap);
    }

    public int res = 4;
    GameObject chunkParent;
    public void ReadPng()
    {
        heightMap = new float[texure_h.width / res, texure_h.height / res];
        colorMap = new Color[texure_h.width / res, texure_h.height / res];
        for (int i = 0; i < texure_h.width / res; i++)
        {
            for (int j = 0; j < texure_h.height / res; j++)
            {
                heightMap[i, j] = texure_h.GetPixel(i * res, j * res).r;
                colorMap[i, j] = texure_c.GetPixel(i * res, j * res);
            }
        }
        Display3D();
    }
    public bool water;
    public void RainErosion()
    {
        
        rain.Rain();
        colorMap = new Color[heightMap.GetLength(0), heightMap.GetLength(1)];
        for (int x = 0; x < perlin.w; x++)
        {
            for (int y = 0; y < perlin.h; y++)
            {
                colorMap[x, y] =Color.Lerp( perlin.colorOfHeight.Evaluate(heightMap[x, y]), Color.cyan, rain.d[x,y]*100.0f*(water?1:0));
            }
        }
        waterMap = rain.d;
        Display3D();
    }
    
    public void Display3D()
    {
        if (chunkParent != null)
            DestroyImmediate(chunkParent);
        chunkParent = new GameObject();
        int w, h;
        w = heightMap.GetLength(0);
        h = heightMap.GetLength(1);
        int[] chunk_n = { Mathf.CeilToInt(w / (float)chunkSize), Mathf.CeilToInt(h / (float)chunkSize) };
        for (int i = 0; i < chunk_n[0]; i++)
        {
            for (int j = 0; j < chunk_n[1]; j++)
            {
                float[,] ChunkHeight = new float[Mathf.Min(chunkSize + 1, w - i * chunkSize), Mathf.Min(chunkSize + 1, h - j * chunkSize)];
                Color[,] ChunkColor = new Color[Mathf.Min(chunkSize + 1, w - i * chunkSize), Mathf.Min(chunkSize + 1, h - j * chunkSize)];
                if (water)
                    for (int x = 0; x < ChunkHeight.GetLength(0); x++)
                    {
                        for (int y = 0; y < ChunkHeight.GetLength(1); y++)
                        {
                            ChunkHeight[x, y] = heightMap[x + i * chunkSize, y + j * chunkSize] + waterMap[x + i * chunkSize, y + j * chunkSize];
                            ChunkColor[x, y] =Color.Lerp( colorMap[x + i * chunkSize, y + j * chunkSize],Color.cyan, waterMap[x + i * chunkSize, y + j * chunkSize]*100.0f);
                        }
                    }
                else
                    for (int x = 0; x < ChunkHeight.GetLength(0); x++)
                    {
                        for (int y = 0; y < ChunkHeight.GetLength(1); y++)
                        {
                            ChunkHeight[x, y] = heightMap[x + i * chunkSize, y + j * chunkSize];
                            ChunkColor[x, y] = colorMap[x + i * chunkSize, y + j * chunkSize];
                        }
                    }
                GameObject newChunk = Instantiate(chunkPrefab, new Vector3(i * chunkSize, 0, j * chunkSize), new Quaternion(0, 0, 0, 1), chunkParent.transform);
                newChunk.GetComponent<Display3d>().Display(ChunkHeight, ChunkColor);
            }
        }
    }
    public void ToImage()
    {
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

		for(int i=0;i<w;i++){
			for(int j=0;j<h;j++){
				img[i+j*h] = new Color(hm[i, j] / max, hm[i, j] / max, hm[i, j] / max);
			}
		}
		heightMap.SetPixels(img);
        heightMap.Apply();
        string name = terrain.name;
		FileStream fs = new FileStream(@"/home/jayinnn/scifair/heightmap/unity/" + name + ".png", FileMode.Create);
		fs.Write(heightMap.EncodeToPNG(), 0, heightMap.EncodeToPNG().Length);
        fs.Close();
		
	}
	
}
