using UnityEngine;
using System.IO;
using System.Net.Http;
using System;

public class ctrl : MonoBehaviour {
    GameObject chunkParent;
    public Display2d d2;
    public GameObject chunkPrefab;
    public Terrain terrain;

    public float[,] heightMap;
    public float[,] waterMap;
    public Color[,] colorMap=null;
    public Texture2D texture_h,texture_c;
    public int chunkSize=100;
    public bool water;
    public Rain3 rain;
    
    public int w, h;
    [System.Serializable]
    public class PerlinNoiseInfo
    {
        public float yScale,rot;
        public Vector2 pos;
        public float scale = 0.2f;
        public AnimationCurve heightCurve;
    }
    public PerlinNoiseInfo perlin;

    public enum DisplayMode
    {
        RealColor,TextureColor
    };
    public DisplayMode displayMode;

    public void Scale()
    {

    }

    public void GeneratePerlinNoise()
    {
        heightMap = HeightMapGenerator.Perlin(w, h,perlin.pos,perlin.rot,perlin.scale);
        waterMap = new float[w, h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y <h; y++)
            {
                heightMap[x, y] = Mathf.Max(0, perlin.heightCurve.Evaluate(heightMap[x, y]) * perlin.yScale/perlin.scale) ;
            }
        }
        Display();
    }
    public void Terrain()
    {
        
        float [,] heightMap_ = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
        
        w = heightMap_.GetLength(0);
        h = heightMap_.GetLength(1);
        heightMap = new float[w,h];
        waterMap = new float[w, h];  
        colorMap = new Color[heightMap_.GetLength(0), heightMap_.GetLength(1)];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                heightMap[x, y] =heightMap_[y,x] *perlin.yScale;
            }
        }
        Display();

        Color[] colormap_ = new Color[w * h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                colormap_[x + y * w] =new Color(heightMap[x,y]*256, heightMap[x, y] * 256, heightMap[x, y] * 256);
            }
        }
    }
    public void ReadPng()
    {
        heightMap = new float[texture_h.width , texture_h.height ];
        colorMap = new Color[texture_h.width , texture_h.height ];
        w = heightMap.GetLength(0);
        h = heightMap.GetLength(1);
        
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                heightMap[i, j] = texture_h.GetPixel(i , j ).r*perlin.yScale;
                colorMap[i, j] = texture_c.GetPixel(i , j );
            }
        }
        waterMap = new float[w, h];
        Display();
    }
    public void ReadPngFromDisk()
    {
        FileStream fs = new FileStream(@".\fromGAN\fromGAN.png",FileMode.Open);
        byte[] png = new byte[fs.Length];
        fs.Read(png, 0, (int)fs.Length);
        fs.Dispose();
        heightMap = PNGToMap(png,0,perlin.yScale);
        waterMap = new float[w, h];
        Display();
    }
    public void Display()
    {
        if (chunkParent != null)
            DestroyImmediate(chunkParent);
        chunkParent = new GameObject();
        int w, h;
        w = heightMap.GetLength(0);
        h = heightMap.GetLength(1);
        int[] chunk_n = { Mathf.CeilToInt(w / (float)chunkSize), Mathf.CeilToInt(h / (float)chunkSize) };
        if (displayMode == DisplayMode.TextureColor)
            for (int i = 0; i < chunk_n[0]; i++)
                for (int j = 0; j < chunk_n[1]; j++)
                {
                    float[,] ChunkHeight = new float[Mathf.Min(chunkSize + 1, w - i * chunkSize), Mathf.Min(chunkSize + 1, h - j * chunkSize)];
                    for (int x = 0; x < ChunkHeight.GetLength(0); x++)
                    {
                        for (int y = 0; y < ChunkHeight.GetLength(1); y++)
                        {
                            ChunkHeight[x, y] = heightMap[x + i * chunkSize, y + j * chunkSize];
                        }
                    }
                    GameObject newChunk = Instantiate(chunkPrefab, transform.position + new Vector3(i * chunkSize, 0, j * chunkSize), new Quaternion(0, 0, 0, 1), chunkParent.transform);
                    newChunk.GetComponent<Display3d>().DrawTerrain(ChunkHeight);
                }
        else
            for (int i = 0; i < chunk_n[0]; i++)
                for (int j = 0; j < chunk_n[1]; j++)
                {
                    float[,] ChunkHeight = new float[Mathf.Min(chunkSize + 1, w - i * chunkSize), Mathf.Min(chunkSize + 1, h - j * chunkSize)];
                    Color[,] ChunkColor = new Color[Mathf.Min(chunkSize + 1, w - i * chunkSize), Mathf.Min(chunkSize + 1, h - j * chunkSize)];
                    for (int x = 0; x < ChunkHeight.GetLength(0); x++)
                    {
                        for (int y = 0; y < ChunkHeight.GetLength(1); y++)
                        {
                            ChunkHeight[x, y] = heightMap[x + i * chunkSize, y + j * chunkSize];
                            ChunkColor[x, y] = colorMap[x + i * chunkSize, y + j * chunkSize];
                        }
                    }
                    GameObject newChunk = Instantiate(chunkPrefab, transform.position + new Vector3(i * chunkSize, 0, j * chunkSize), new Quaternion(0, 0, 0, 1), chunkParent.transform);
                    newChunk.GetComponent<Display3d>().DrawTerrainWithRealColor(ChunkHeight,ChunkColor);
                }
        if (water)
            for (int i = 0; i < chunk_n[0]; i++)
                for (int j = 0; j < chunk_n[1]; j++)
                {
                    float[,] ChunkWater = new float[Mathf.Min(chunkSize + 1, w - i * chunkSize), Mathf.Min(chunkSize + 1, h - j * chunkSize)];
                    for (int x = 0; x < ChunkWater.GetLength(0); x++)
                    {
                        for (int y = 0; y < ChunkWater.GetLength(1); y++)
                        {
                            ChunkWater[x, y] = waterMap[x + i * chunkSize, y + j * chunkSize]+ heightMap[x + i * chunkSize, y + j * chunkSize];
                        }
                    }
                    GameObject newChunk = Instantiate(chunkPrefab, transform.position + new Vector3(i * chunkSize,0, j * chunkSize), new Quaternion(0, 0, 0, 1), chunkParent.transform);
                    newChunk.GetComponent<Display3d>().DrawWater(ChunkWater);
                }
        d2.Display(heightMap);
    }

    public int t = 0;
    public void SaveAsPNG(float[,] map)
    {
        
		FileStream fs = new FileStream(@".\toGAN\fromCS"+t+++".png", FileMode.Create);
        float[]minmax=MinMax(map);
        byte[] png = MapToPng(map,minmax[0],minmax[1]);
        fs.Write(png, 0, png.Length);
        fs.Close();
    }
    float[] MinMax(float[,] map)
    {
        w = map.GetLength(0);
        h = map.GetLength(1);
        float min = map[0, 0];
        float max = map[0, 0];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (map[i, j] > max) max = map[i, j];
                if (map[i, j] < min) min = map[i, j];
            }
        }
        return new float[2] { min, max };
    }
    byte[] MapToPng(float[,] map,float min,float max,string mode="GRAY")
    {
        w = map.GetLength(0);
        h = map.GetLength(1);
        Texture2D o = new Texture2D(w, h);
        Color[] img = new Color[w * h];
        if(mode=="GRAY")
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float v =( map[i, j] - min) / (max - min);
                    img[i + j * h] = new Color(v,v,v);
                }
            }
        else if (mode == "RGB")
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float v=(map[i, j] - min) / (max - min);
                    img[i + j * h] = new Color(v,(v*256)%1,(v*65536)%1);
                }
            }
        o.SetPixels(img);
        o.Apply();
        return o.EncodeToPNG();
    }
    float[,] PNGToMap(byte[] png,float min, float max,string mode = "GRAY")//uses global variables w and h
    {
        float[,] o = new float[w, h];
        Texture2D texture = new Texture2D(w, h);
        texture.LoadImage(png);
        if (mode == "GRAY")
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color p = texture.GetPixel(i, j);
                    o[i, j] = p.r*(max-min)+min;
                }
            }
        else if (mode == "RGB")
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color p = texture.GetPixel(i, j);
                    o[i, j] =( p.r + p.g / 256 + p.b / 65536) * (max - min) + min;
                }
            }
        return o;
    }
    Color[,] PNGToColorMap(byte[] png)//uses global variables w and h
    {
        Color[,] o = new Color[w, h];
        Texture2D texture = new Texture2D(w, h);
        texture.LoadImage(png);
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                o[i, j] = texture_h.GetPixel(i, j);
            }
        }
        return o;
    }

    public string postUrl;
    public string getUrl;
    [System.Serializable]
    class ResponseData
    {
        public string file_name = "";
    }
    class PostData
    {
        public int overlay = 128;
        public string file;
        public PostData(int overlay, string file)
        {
            this.overlay = overlay;
            this.file = file;
        }
    }
    public async void RunGANOnServer()
    {
        float[] minmax = MinMax(heightMap);
        string pngBase64 = Convert.ToBase64String(MapToPng(heightMap, minmax[0], minmax[1],mode:"RGB"));
        HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(10    );
        string response;
        using (var content = new StringContent(JsonUtility.ToJson(new PostData(128,pngBase64)), System.Text.Encoding.UTF8, "application/json"))
        {
            var postResult =await client.PostAsync(postUrl, content);
            response = await postResult.Content.ReadAsStringAsync();
        }
        print("response:"+response);
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(response);
        byte[] getData = await client.GetByteArrayAsync(getUrl+responseData.file_name+".png");
        heightMap = PNGToMap(getData,minmax[0],minmax[1],mode:"RGB");
        waterMap = new float[w, h];
        Display();
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            GeneratePerlinNoise(); 
        }
    }
}
