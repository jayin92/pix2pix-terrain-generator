using UnityEngine;
using System.IO;
using UnityEditor;
[ExecuteInEditMode]
public class ctrl : MonoBehaviour {


    public Display2d d2;
    public GameObject chunkPrefab;
    public Terrain terrain;

    public float[,] heightMap;
    public float[,] waterMap;
    public Color[,] colorMap=null;
    public Texture2D texure_h,texure_c;
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
        Display3D();
    }

    public void Terrain()
    {
        float [,] heightMap_ = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapWidth, terrain.terrainData.heightmapHeight);
        
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
        Display3D();

        Color[] colormap_ = new Color[w * h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                colormap_[x + y * w] =new Color(heightMap[x,y]*256, heightMap[x, y] * 256, heightMap[x, y] * 256);
            }
        }
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
        w = heightMap.GetLength(0);
        h = heightMap.GetLength(1);
        
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                heightMap[i, j] = texure_h.GetPixel(i * res, j * res).r*perlin.yScale;
                colorMap[i, j] = texure_c.GetPixel(i * res, j * res);
            }
        }
        waterMap = new float[w, h];
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
                    GameObject newChunk = Instantiate(chunkPrefab, transform.position + new Vector3(i * chunkSize, .01f, j * chunkSize), new Quaternion(0, 0, 0, 1), chunkParent.transform);
                    newChunk.GetComponent<Display3d>().DrawWater(ChunkWater);
                }
        display2d();
    }

    public int t = 0;
    public void ToImage()
    {
        w = heightMap.GetLength(0);
        h = heightMap.GetLength(1);
        Texture2D o = new Texture2D(w, h);	
		Color[] img = new Color[w*h];
		float min = heightMap[0, 0];
		float max = heightMap[0, 0];
		for(int i=0;i<w;i++){
			for(int j=0;j<h;j++){
				if(heightMap[i, j] > max) max = heightMap[i, j]; 
				if(heightMap[i, j] < min) min = heightMap[i, j];
			}
		}
		for(int i=0;i<w;i++){
			for(int j=0;j<h;j++){
				img[i+j*h] = new Color(heightMap[i, j] / max, heightMap[i, j] / max, heightMap[i, j] / max);
			}
		}
		o.SetPixels(img);
        o.Apply();
		FileStream fs = new FileStream(@".\toGAN\fromCS"+t+++".png", FileMode.Create);
        byte[] png = o.EncodeToPNG();
        fs.Write(png, 0, png.Length);
        fs.Close();
    }
    public void runGAN()
    {
        System.Diagnostics.Process.Start("test.bat");
    }

    public void GenerateRamdomHeightMap()
    {
        for (rain.KS = 0.005f; rain.KS < 0.015f; rain.KS += 0.002f)
        {
            for (rain.drag = 0f; rain.drag < 0.005f; rain.drag += 0.001f)
            {
                for (rain.A = 5; rain.A < 30; rain.A += 3)
                {
                    perlin.pos = new Vector2(Random.Range(0, 1000), Random.Range(0, 1000));
                    GeneratePerlinNoise();
                    rain.Int();
                    rain.Rain();
                    ToImage(); 
                }
            }
        }
    }


    public void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            GeneratePerlinNoise(); 
        }
    }
}
