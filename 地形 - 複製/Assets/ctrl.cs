using UnityEngine;
using UnityEngine.UI;

public class ctrl : MonoBehaviour {
    public Display2d d2;
    public Display3d d3;
    public Rain2 rain;
    public GameObject chunkPrefab;
    public Terrain terrain;

    public float[,] heightMap;
    public Color[,] colorMap;
    public Texture2D texure_h,texure_c;
    public InputField inputField;
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
                if(water)
                    for (int x = 0; x < ChunkHeight.GetLength(0); x++)
                    {
                        for (int y = 0; y < ChunkHeight.GetLength(1); y++)
                        {
                            ChunkHeight[x, y] = heightMap[x + i * chunkSize, y + j * chunkSize]+ rain.d[x + i * chunkSize, y + j * chunkSize]*(water?1:0);
                            ChunkColor[x, y] = colorMap[x + i * chunkSize, y + j * chunkSize];
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
}

