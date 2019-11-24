using UnityEngine;

public class Display3d : MonoBehaviour {
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Material forRealColor, forGeneratedColor;


	public void Display(float [,] heightmap,Color[,] colormap)
    {

        Mesh mesh = new Mesh();
        int w = heightmap.GetLength(0), h = heightmap.GetLength(1);
        Vector3[] vertices = new Vector3[w*h];
      
        for (int x = 0; x < w; x++)
        {
            for(int y = 0; y < h; y++)
            {
                vertices[x + y * w] = new Vector3(x, heightmap[x, y], y);
            }
        }
        mesh.vertices = vertices;
        int[] triangles = new int[(w-1) * (h-1) * 6];
        int i = 0,j=0;
        for (int y = 0; y < h; y++) 
        {
            for (int x = 0; x < w; x++)
            {
                if (x < w - 1 && y < h - 1)
                {
                    triangles[6 * j] = i;
                    triangles[6 * j+1] = i+w;
                    triangles[6 * j + 2] = i+1+w;
                    triangles[6 * j+3] = i;
                    triangles[6 * j + 4] = i + 1+w;
                    triangles[6 * j + 5] = i + 1 ;
                    j++;
                }
                i++;
            }
        }
        mesh.triangles = triangles;
        Vector2[] uvs = new Vector2[w * h];
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                uvs[x + y * w] = new Vector2(x / (float)w, y / (float)h);
            }
        }
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        if (colormap != null)
        {
            Color[] colormap_ = new Color[w * h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    colormap_[x + y * w] = colormap[x, y];
                }
            }

            Texture2D texture = new Texture2D(w, h);
            texture.SetPixels(colormap_);
            texture.Apply();
            meshRenderer = GetComponent<MeshRenderer>();
            var tempMaterial =new Material( forRealColor);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            tempMaterial.SetTexture("_Texture", texture);
            meshRenderer.material = tempMaterial;
        }
        else
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = forGeneratedColor;
        }
    }
}


